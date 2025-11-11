using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DanteAPI;
using DanteAPI.Entities;
using Microsoft.Extensions.Configuration;
using ClosedXML.Excel;

namespace DanteAPI.Backup
{
    class Program
    {
        private static string _logFilePath;
        private static int _maxCollectionDepth;

        static async Task Main(string[] args)
        {
            await MainAsync();
        }

        static async Task MainAsync()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            string backupStorageFolder = configuration["BackupStorageFolder"];
            string entities = configuration["Entities"];
            _maxCollectionDepth = int.Parse(configuration["MaxCollectionDepth"] ?? "1");

            if (string.IsNullOrEmpty(backupStorageFolder))
            {
                Console.WriteLine("BackupStorageFolder not configured in appsettings.json");
                return;
            }

            if (string.IsNullOrEmpty(entities))
            {
                Console.WriteLine("Entities not configured in appsettings.json");
                return;
            }

            // Create dated backup folder
            string datestamp = DateTime.Now.ToString("yyyy-MM-dd");
            string backupFolder = Path.Combine(backupStorageFolder, datestamp);
            Directory.CreateDirectory(backupFolder);

            // Initialize log file
            _logFilePath = Path.Combine(backupFolder, "backup.log");
            LogMessage("=== Dante Backup Started ===");
            LogMessage($"Backup Folder: {backupFolder}");
            LogMessage($"Max Collection Depth: {_maxCollectionDepth}");

            using var api = new DanteAPI.Main(configuration["DanteURL"], configuration["DanteAPIKey"]);

            // Validate API key
            LogMessage("Validating API key...");
            var validateKeyResponse = await api.ValidateApiKey();
            if (!validateKeyResponse.Data.Valid)
            {
                LogMessage($"ERROR: Invalid API key{string.Join(", ", validateKeyResponse.Data.Messages)}");
                Console.ReadLine();
                return;
            }
            LogMessage("API key validated successfully");

            // Parse entities to backup
            string[] entitiesToDownload = entities.Split('|');
            LogMessage($"Entities to backup: {string.Join(", ", entitiesToDownload)}");

            // Process each entity
            foreach (string entityName in entitiesToDownload)
            {
                try
                {
                    LogMessage($"\n--- Processing Entity: {entityName} ---");
                    await ProcessEntityAsync(api, entityName.Trim(), backupFolder);
                }
                catch (Exception ex)
                {
                    LogMessage($"ERROR processing entity {entityName}: {ex.Message}");
                    LogMessage($"Stack Trace: {ex.StackTrace}");

                    // Log additional context if available
                    if (ex.InnerException != null)
                    {
                        LogMessage($"Inner Exception: {ex.InnerException.Message}");
                    }
                }
            }

            LogMessage("\n=== Dante Backup Completed ===");
            Console.WriteLine($"\nBackup completed. Check log file: {_logFilePath}");
        }

        static async Task ProcessEntityAsync(DanteAPI.Main api, string entityName, string backupFolder)
        {
            LogMessage($"Processing entity: {entityName}");

            try
            {
                switch (entityName)
                {
                    case "Booking":
                        await ProcessEntityTypedAsync<DanteAPI.Entities.Booking>(api, entityName, backupFolder);
                        break;
                    case "BookingItem":
                        await ProcessEntityTypedAsync<DanteAPI.Entities.References.BookingItem>(api, entityName, backupFolder);
                        break;
                    case "Company":
                        await ProcessEntityTypedAsync<DanteAPI.Entities.Company>(api, entityName, backupFolder);
                        break;
                    case "Delegate":
                        await ProcessEntityTypedAsync<DanteAPI.Entities.Delegate>(api, entityName, backupFolder);
                        break;
                    case "Schedule":
                        await ProcessEntityTypedAsync<DanteAPI.Entities.Schedule>(api, entityName, backupFolder);
                        break;
                    case "ScheduleDelegate":
                        await ProcessEntityTypedAsync<DanteAPI.Entities.ScheduleDelegate>(api, entityName, backupFolder);
                        break;
                    case "User":
                        await ProcessEntityTypedAsync<DanteAPI.Entities.References.User>(api, entityName, backupFolder);
                        break;
                    case "Status":
                        await ProcessEntityTypedAsync<DanteAPI.Entities.References.Status>(api, entityName, backupFolder);
                        break;
                    case "Country":
                        await ProcessEntityTypedAsync<DanteAPI.Entities.References.Country>(api, entityName, backupFolder);
                        break;
                    default:
                        LogMessage($"ERROR: Unknown entity type '{entityName}'.");
                        break;
                }
            }
            catch (Exception ex)
            {
                LogMessage($"ERROR processing entity {entityName}: {ex.Message}");
                LogMessage($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    LogMessage($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

        static async Task ProcessEntityTypedAsync<T>(DanteAPI.Main api, string entityName, string backupFolder)
        {
            // Get all data using pagination
            List<object> allData = await GetAllDataAsync<T>(api);

            if (allData == null || allData.Count == 0)
            {
                LogMessage($"No data retrieved for {entityName}");
                return;
            }

            LogMessage($"Retrieved {allData.Count} records");

            // Create Excel file
            string excelFilePath = Path.Combine(backupFolder, $"{entityName}.xlsx");
            CreateExcelFile(allData, typeof(T), excelFilePath, entityName);

            LogMessage($"Excel file created: {excelFilePath}");
        }

        static async Task<List<object>> GetAllDataAsync<T>(DanteAPI.Main api)
        {
            List<object> allData = new List<object>();
            int pageIndex = 0;
            int pageSize = 1000;
            bool hasMoreData = true;

            while (hasMoreData)
            {
                try
                {
                    LogMessage($"Fetching page {pageIndex + 1} (pagesize: {pageSize})...");

                    var response = await api.Select<T>(
                        new List<string> { "*" },
                        new List<DanteAPI.Main.Filter>(),
                        pageSize,
                        pageIndex
                    );

                    if (!response.IsSuccess)
                    {
                        LogMessage($"ERROR fetching data: {response.ErrorMessage}");
                        break;
                    }

                    if (response.Data == null || response.Data.Count == 0)
                    {
                        hasMoreData = false;
                    }
                    else
                    {
                        foreach (T item in response.Data)
                        {
                            allData.Add(item);
                        }

                        LogMessage($"Retrieved {response.Data.Count} records from page {pageIndex + 1}");

                        // If we got fewer records than pageSize, we've reached the end
                        if (response.Data.Count < pageSize)
                        {
                            hasMoreData = false;
                        }
                        else
                        {
                            pageIndex++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"ERROR during pagination: {ex.Message}");
                    LogMessage($"Stack Trace: {ex.StackTrace}");
                    hasMoreData = false;
                }
            }

            return allData;
        }

        static void CreateExcelFile(List<object> data, Type entityType, string filePath, string entityName)
        {
            using (var workbook = new XLWorkbook())
            {
                // Create main sheet
                var mainSheet = workbook.Worksheets.Add(entityName);

                // Get properties, excluding those ending in "Lookup"
                PropertyInfo[] properties = entityType.GetProperties()
                    .Where(p => !p.Name.EndsWith("Lookup"))
                    .ToArray();

                // Track collections for separate sheets
                Dictionary<string, List<CollectionItem>> collections = new Dictionary<string, List<CollectionItem>>();

                // Write headers
                int col = 1;
                Dictionary<int, PropertyInfo> columnPropertyMap = new Dictionary<int, PropertyInfo>();

                foreach (PropertyInfo prop in properties)
                {
                    if (IsCollectionProperty(prop))
                    {
                        // Skip collections in main sheet
                        continue;
                    }
                    else if (IsComplexType(prop.PropertyType))
                    {
                        // Complex type - flatten properties
                        PropertyInfo[] nestedProps = prop.PropertyType.GetProperties()
                            .Where(p => !p.Name.EndsWith("Lookup") && !IsCollectionProperty(p))
                            .ToArray();

                        foreach (PropertyInfo nestedProp in nestedProps)
                        {
                            // Skip nested complex types (only go one level deep)
                            if (IsComplexType(nestedProp.PropertyType))
                            {
                                continue;
                            }

                            mainSheet.Cell(1, col).Value = $"{prop.Name}.{nestedProp.Name}";
                            columnPropertyMap[col] = prop; // Store parent property
                            col++;
                        }
                    }
                    else
                    {
                        mainSheet.Cell(1, col).Value = prop.Name;
                        columnPropertyMap[col] = prop;
                        col++;
                    }
                }

                // Write data rows
                int row = 2;
                foreach (object item in data)
                {
                    col = 1;
                    object parentId = GetIdValue(item);

                    foreach (PropertyInfo prop in properties)
                    {
                        if (IsCollectionProperty(prop))
                        {
                            // Process collection for separate sheet
                            IEnumerable collection = prop.GetValue(item) as IEnumerable;
                            if (collection != null)
                            {
                                string collectionKey = $"{entityName}_{prop.Name}";
                                if (!collections.ContainsKey(collectionKey))
                                {
                                    collections[collectionKey] = new List<CollectionItem>();
                                }

                                foreach (object collectionItem in collection)
                                {
                                    collections[collectionKey].Add(new CollectionItem
                                    {
                                        ParentId = parentId,
                                        Item = collectionItem
                                    });
                                }
                            }
                        }
                        else if (IsComplexType(prop.PropertyType))
                        {
                            // Complex type - write flattened properties
                            object complexValue = prop.GetValue(item);
                            PropertyInfo[] nestedProps = prop.PropertyType.GetProperties()
                                .Where(p => !p.Name.EndsWith("Lookup") && !IsCollectionProperty(p))
                                .ToArray();

                            foreach (PropertyInfo nestedProp in nestedProps)
                            {
                                // Skip nested complex types (only go one level deep)
                                if (IsComplexType(nestedProp.PropertyType))
                                {
                                    continue; // Don't increment col - we didn't create a column for this
                                }

                                if (complexValue != null)
                                {
                                    object nestedValue = nestedProp.GetValue(complexValue);
                                    SetCellValue(mainSheet.Cell(row, col), nestedValue);
                                }
                                col++;
                            }
                        }
                        else
                        {
                            object value = prop.GetValue(item);
                            SetCellValue(mainSheet.Cell(row, col), value);
                            col++;
                        }
                    }
                    row++;
                }

                // Format main sheet
                if (row > 1 && col > 1)
                {
                    mainSheet.Columns(1, col - 1).AdjustToContents();

                    // Make headers bold
                    mainSheet.Row(1).Style.Font.Bold = true;
                }

                // Create collection sheets
                foreach (var collectionKvp in collections)
                {
                    CreateCollectionSheet(workbook, collectionKvp.Key, collectionKvp.Value, entityName);
                }

                // Save file
                workbook.SaveAs(filePath);
            }
        }

        static void CreateCollectionSheet(XLWorkbook workbook, string sheetName, List<CollectionItem> items, string parentEntityName)
        {
            if (items.Count == 0) return;

            var sheet = workbook.Worksheets.Add(sheetName);

            // Get the type of items in the collection
            Type itemType = items[0].Item.GetType();
            PropertyInfo[] properties = itemType.GetProperties()
                .Where(p => !p.Name.EndsWith("Lookup"))
                .ToArray();

            // Write header - start with ParentID column
            int col = 1;
            sheet.Cell(1, col).Value = $"{parentEntityName}ID";
            col++;

            foreach (PropertyInfo prop in properties)
            {
                if (!IsCollectionProperty(prop))
                {
                    sheet.Cell(1, col).Value = prop.Name;
                    col++;
                }
            }

            // Write data rows
            int row = 2;
            foreach (var collectionItem in items)
            {
                col = 1;

                // Write parent ID
                SetCellValue(sheet.Cell(row, col), collectionItem.ParentId);
                col++;

                // Write item properties
                foreach (PropertyInfo prop in properties)
                {
                    if (!IsCollectionProperty(prop))
                    {
                        object value = prop.GetValue(collectionItem.Item);
                        SetCellValue(sheet.Cell(row, col), value);
                        col++;
                    }
                }
                row++;
            }

            // Format sheet
            sheet.Columns(1, col - 1).AdjustToContents();

            // Make headers bold
            sheet.Row(1).Style.Font.Bold = true;
        }

        static object GetIdValue(object entity)
        {
            PropertyInfo idProperty = entity.GetType().GetProperty("ID");
            return idProperty?.GetValue(entity);
        }

        static object FormatValueForClosedXML(object value)
        {
            if (value == null) return null;

            // ClosedXML handles DateTime, decimal, int, etc. natively
            // Just return the value as-is
            return value;
        }

        static void SetCellValue(IXLCell cell, object value)
        {
            if (value == null)
            {
                cell.Value = "";
                return;
            }

            // Handle different types explicitly for ClosedXML
            switch (value)
            {
                case string s:
                    // Excel has a limit of 32,767 characters per cell
                    if (s.Length > 32767)
                    {
                        cell.Value = s.Substring(0, 32767);
                        cell.Style.Fill.BackgroundColor = XLColor.LightYellow; // Highlight truncated cells
                    }
                    else
                    {
                        cell.Value = s;
                    }
                    break;
                case int i:
                    cell.Value = i;
                    break;
                case long l:
                    cell.Value = l;
                    break;
                case double d:
                    cell.Value = d;
                    break;
                case decimal dec:
                    cell.Value = (double)dec;
                    break;
                case bool b:
                    cell.Value = b;
                    break;
                case DateTime dt:
                    cell.Value = dt;
                    break;
                default:
                    string strValue = value.ToString();
                    if (strValue.Length > 32767)
                    {
                        cell.Value = strValue.Substring(0, 32767);
                        cell.Style.Fill.BackgroundColor = XLColor.LightYellow; // Highlight truncated cells
                    }
                    else
                    {
                        cell.Value = strValue;
                    }
                    break;
            }
        }

        static bool IsCollectionProperty(PropertyInfo prop)
        {
            if (prop.PropertyType == typeof(string)) return false;

            return typeof(IEnumerable).IsAssignableFrom(prop.PropertyType);
        }

        static bool IsComplexType(Type type)
        {
            return !type.IsPrimitive
                && type != typeof(string)
                && type != typeof(DateTime)
                && type != typeof(decimal)
                && !type.IsEnum
                && Nullable.GetUnderlyingType(type) == null;
        }


        static void LogMessage(string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            Console.WriteLine(logEntry);

            if (!string.IsNullOrEmpty(_logFilePath))
            {
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            }
        }

        class CollectionItem
        {
            public object ParentId { get; set; }
            public object Item { get; set; }
        }
    }
}
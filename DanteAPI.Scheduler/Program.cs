using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using DanteAPI;
using DanteAPI.Entities;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using Microsoft.Extensions.Configuration;

class Program
{
    private static string csvFilePath;
    private static bool fileSelected;
    private static string logFilePath;

    private static readonly HashSet<string> PredefinedFields = new HashSet<string>
    {
        "Dates", "StartTime", "EndTime", "Resource", "CourseID", "CourseReference",
        "Venue_Resource", "Tutor_Internal_Resource", "Tutor_External_Resource"
    };

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Console.WriteLine("Initializing Console App...");

        // Initialize log file with current date
        logFilePath = $"import_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        Log($"Import started at {DateTime.Now}");

        // Perform pre-launch validation checks
        if (!PerformPreLaunchChecks())
        {
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            return;
        }

        try
        {
            // Create a form that will handle the file selection
            var form = new FileSelectionForm();
            Application.Run(form);

            csvFilePath = form.SelectedFilePath;
            fileSelected = !string.IsNullOrEmpty(csvFilePath);

            if (!fileSelected)
            {
                Console.WriteLine("No file selected.");
                return;
            }

            // Now run the async operations
            ProcessDataAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            var errorMsg = $"An error occurred: {ex.Message}";
            Console.WriteLine(errorMsg);
            LogError(errorMsg);
            LogError($"Stack trace: {ex.StackTrace}");
        }

        Log($"Import completed at {DateTime.Now}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    // Form class to handle file selection
    private class FileSelectionForm : Form
    {
        public string SelectedFilePath { get; private set; }

        public FileSelectionForm()
        {
            // Make the form invisible
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Load += FileSelectionForm_Load;
        }

        private void FileSelectionForm_Load(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select CSV File";
                openFileDialog.Filter = "CSV files (*.csv)|*.csv";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SelectedFilePath = openFileDialog.FileName;
                }
            }

            // Close the form after file selection
            this.Close();
        }
    }

    static bool PerformPreLaunchChecks()
    {
        Console.WriteLine("\n=== Pre-Launch Validation ===");
        bool allChecksPass = true;

        // Check 1: Verify appsettings.json exists
        Console.Write("Checking for appsettings.json... ");
        string appSettingsPath = "appsettings.json";
        if (!File.Exists(appSettingsPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("FAILED");
            Console.ResetColor();
            Console.WriteLine($"  Error: {appSettingsPath} not found.");
            LogError($"{appSettingsPath} not found.");
            allChecksPass = false;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("OK");
            Console.ResetColor();

            // Check 2: Verify required configuration variables
            try
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile(appSettingsPath, optional: false, reloadOnChange: true)
                    .Build();

                Console.Write("Checking DanteAPIKey configuration... ");
                string apiKey = configuration["DanteAPIKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAILED");
                    Console.ResetColor();
                    Console.WriteLine("  Error: DanteAPIKey is not set or is empty.");
                    LogError("DanteAPIKey is not set in appsettings.json.");
                    allChecksPass = false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK");
                    Console.ResetColor();
                }

                Console.Write("Checking DanteURL configuration... ");
                string apiUrl = configuration["DanteURL"];
                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAILED");
                    Console.ResetColor();
                    Console.WriteLine("  Error: DanteURL is not set or is empty.");
                    LogError("DanteURL is not set in appsettings.json.");
                    allChecksPass = false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK");
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("FAILED");
                Console.ResetColor();
                Console.WriteLine($"  Error reading configuration: {ex.Message}");
                LogError($"Error reading configuration: {ex.Message}");
                allChecksPass = false;
            }
        }

        // Check 3: Verify mappings.json exists
        Console.Write("Checking for mappings.json... ");
        string mappingsPath = "mappings.json";
        if (!File.Exists(mappingsPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("FAILED");
            Console.ResetColor();
            Console.WriteLine($"  Error: {mappingsPath} not found.");
            LogError($"{mappingsPath} not found.");
            allChecksPass = false;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("OK");
            Console.ResetColor();
        }

        Console.WriteLine("=============================\n");

        if (!allChecksPass)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Pre-launch validation failed. Please fix the errors above before continuing.");
            Console.ResetColor();
            LogError("Pre-launch validation failed.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("All pre-launch checks passed successfully!");
            Console.ResetColor();
        }

        return allChecksPass;
    }

    static async Task ProcessDataAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        // Validate API Key
        string apiKey = configuration["DanteAPIKey"];
        string apiUrl = configuration["DanteURL"];

        int maxThreads = 1;
        if (int.TryParse(configuration["MaxThreads"], out int configThreads))
        {
            maxThreads = Math.Min(Math.Max(1, configThreads), 3);//Max 3 threads
        }
        Console.WriteLine($"Using {maxThreads} thread(s) for processing.");
        Log($"Using {maxThreads} thread(s) for processing.");

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiUrl))
        {
            var errorMsg = "API Key or API URL not set in environment variables.";
            Console.WriteLine(errorMsg);
            LogError(errorMsg);
            return;
        }

        using var apiClient = new Main(apiUrl, apiKey);

        // Validate API key
        var validationResult = await apiClient.ValidateApiKey();
        if (!validationResult.IsSuccess)
        {
            var errorMsg = "Invalid API Key: " + validationResult.ErrorMessage;
            Console.WriteLine(errorMsg);
            LogError(errorMsg);
            return;
        }
        Console.WriteLine("API Key validated successfully.");

        Console.WriteLine($"Selected file: {csvFilePath}");

        List<Dictionary<string, string>> csvData = LoadCsv(csvFilePath);
        if (csvData.Count == 0)
        {
            var errorMsg = "CSV file is empty.";
            Console.WriteLine(errorMsg);
            LogError(errorMsg);
            return;
        }

        // Load and validate mappings
        string mappingsPath = "mappings.json";
        if (!File.Exists(mappingsPath))
        {
            var errorMsg = "Mappings file not found.";
            Console.WriteLine(errorMsg);
            LogError(errorMsg);
            return;
        }

        Mappings mappings = LoadMappings(mappingsPath);
        if (!ValidateMappings(mappings, csvData[0]))
        {
            var errorMsg = "Invalid mappings file.";
            Console.WriteLine(errorMsg);
            LogError(errorMsg);
            return;
        }

        var lookup = mappings.Fields.FirstOrDefault(f => f.Lookup);
        if (lookup == null)
        {
            var errorMsg = "Mappings must have exactly one lookup field.";
            Console.WriteLine(errorMsg);
            LogError(errorMsg);
            return;
        }

        Console.WriteLine($"Using {lookup.Source}/{lookup.Target} as the lookup field.");

        // Process each row
        int totalRows = csvData.Count;
        int processedRows = 0;
        int insertedRows = 0;
        int updatedRows = 0;
        int skippedRows = 0;
        int failedRows = 0;
        object progressLock = new object();
        var startTime = DateTime.Now;

        // Use SemaphoreSlim to limit concurrent operations
        using var semaphore = new SemaphoreSlim(maxThreads, maxThreads);

        var tasks = csvData.Select(async (row, index) =>
        {
            await semaphore.WaitAsync();
            try
            {
                string lookupValue = row.ContainsKey(lookup.Source) ? row[lookup.Source] : null;
                if (string.IsNullOrEmpty(lookupValue))
                {
                    var warnMsg = $"Skipping row {index + 1}: Lookup field is empty.";
                    Console.WriteLine(warnMsg);
                    LogWarning(warnMsg);
                    lock (progressLock)
                    {
                        processedRows++;
                        skippedRows++;
                    }
                    return;
                }

                try
                {
                    // Create a separate API client for this thread
                    using var threadApiClient = new Main(apiUrl, apiKey);
                    
                    var schedule = await GetScheduleByLookup(threadApiClient, lookup.Target, lookupValue);
                    bool isInsert = schedule == null;
                    
                    if (isInsert)
                    {
                        schedule = await InsertSchedule(threadApiClient, mappings, row);
                    }

                    await UpdateSchedule(threadApiClient, schedule.ID, mappings, row);

                    lock (progressLock)
                    {
                        processedRows++;
                        if (isInsert)
                            insertedRows++;
                        else
                            updatedRows++;
                            
                        if (processedRows % 10 == 0 || processedRows == totalRows)
                        {
                            var elapsed = DateTime.Now - startTime;
                            var avgTimePerRow = elapsed.TotalSeconds / processedRows;
                            var remainingRows = totalRows - processedRows;
                            var estimatedRemainingTime = TimeSpan.FromSeconds(avgTimePerRow * remainingRows);
                            var estimatedCompletion = DateTime.Now.Add(estimatedRemainingTime);
                            
                            Console.WriteLine($"Progress: {processedRows}/{totalRows} rows | " +
                                            $"Inserted: {insertedRows} | Updated: {updatedRows} | Skipped: {skippedRows} | Failed: {failedRows} | " +
                                            $"Elapsed: {FormatTimeSpan(elapsed)} | " +
                                            $"ETA: {estimatedCompletion:HH:mm:ss} ({FormatTimeSpan(estimatedRemainingTime)} remaining)");
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorMsg = $"Error processing row {index + 1} (lookup: {lookupValue}): {ex.Message}";
                    Console.WriteLine(errorMsg);
                    LogError(errorMsg);
                    lock (progressLock)
                    {
                        processedRows++;
                        failedRows++;
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        var totalElapsed = DateTime.Now - startTime;
        var successfulRows = insertedRows + updatedRows;
        
        Console.WriteLine("\n=== Import Complete ===");
        Console.WriteLine($"Total Rows Processed: {processedRows}/{totalRows}");
        Console.WriteLine($"  - Inserted: {insertedRows}");
        Console.WriteLine($"  - Updated: {updatedRows}");
        Console.WriteLine($"  - Skipped: {skippedRows}");
        Console.WriteLine($"  - Failed: {failedRows}");
        Console.WriteLine($"Total Time: {FormatTimeSpan(totalElapsed)}");
        Console.WriteLine($"Average Time per Row: {(totalElapsed.TotalSeconds / processedRows):F2}s");
        
        Log("\n=== Import Statistics ===");
        Log($"Total Rows Processed: {processedRows}/{totalRows}");
        Log($"  - Inserted: {insertedRows}");
        Log($"  - Updated: {updatedRows}");
        Log($"  - Skipped: {skippedRows}");
        Log($"  - Failed: {failedRows}");
        Log($"Total Time: {FormatTimeSpan(totalElapsed)}");
        Log($"Average Time per Row: {(totalElapsed.TotalSeconds / processedRows):F2}s");
        Log($"Successful Rows: {successfulRows} ({(successfulRows * 100.0 / totalRows):F1}%)");
    }

    static List<Dictionary<string, string>> LoadCsv(string filePath)
    {
        var records = new List<Dictionary<string, string>>();

        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var reader = new StreamReader(stream))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HeaderValidated = null,
            MissingFieldFound = null
        }))
        {
            var rows = csv.GetRecords<dynamic>();
            foreach (var row in rows)
            {
                var dict = ((IDictionary<string, object>)row)
                    .ToDictionary(k => k.Key, v => v.Value?.ToString() ?? "");
                records.Add(dict);
            }
        }

        return records;
    }

    static Mappings LoadMappings(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<Mappings>(json);
    }

    static bool ValidateMappings(Mappings mappings, Dictionary<string, string> csvHeaders)
    {
        HashSet<string> scheduleProperties = typeof(Schedule)
            .GetProperties()
            .Select(p => p.Name)
            .ToHashSet();

        int lookupCount = mappings.Fields.Count(f => f.Lookup);
        if (lookupCount != 1)
        {
            var errorMsg = $"Error: Mappings must have exactly one lookup field. Found {lookupCount} lookup fields.";
            Console.WriteLine(errorMsg);
            LogError(errorMsg);
            return false;
        }

        // Check for missing CSV headers
        var missingHeaders = mappings.Fields
            .Where(f => !csvHeaders.ContainsKey(f.Source))
            .Select(f => f.Source)
            .ToList();

        if (missingHeaders.Any())
        {
            Console.WriteLine("Error: The following mapped fields are missing from the CSV headers:");
            LogError("Error: The following mapped fields are missing from the CSV headers:");
            foreach (var header in missingHeaders)
            {
                Console.WriteLine($"  - {header}");
                LogError($"  - {header}");
            }
            return false;
        }

        // Check for invalid target properties (excluding predefined fields)
        var invalidTargets = mappings.Fields
            .Where(f => !PredefinedFields.Contains(f.Target))
            .Where(f => !scheduleProperties.Contains(f.Target))
            .Select(f => f.Target)
            .ToList();

        if (invalidTargets.Any())
        {
            Console.WriteLine("Error: The following target fields are not valid Schedule properties:");
            LogError("Error: The following target fields are not valid Schedule properties:");
            foreach (var target in invalidTargets)
            {
                Console.WriteLine($"  - {target}");
                LogError($"  - {target}");
            }
            return false;
        }

        return true;
    }

    static async Task<Schedule> GetScheduleByLookup(Main apiClient, string lookupField, string lookupValue)
    {
        var filters = new List<Main.Filter>
        {
            new Main.Filter { FieldName = lookupField, Operator = "=", Value = lookupValue }
        };

        var response = await apiClient.Select<Schedule>(null, filters);
        return response.IsSuccess && response.Data.Any() ? response.Data.First() : null;
    }

    static async Task UpdateSchedule(Main apiClient, int scheduleId, Mappings mappings, Dictionary<string, string> row)
    {
        var data = mappings.Fields
            .Where(f => !PredefinedFields.Contains(f.Target))
            .ToDictionary(m => m.Target, m => row[m.Source]);
        
        var response = await apiClient.Update<Schedule>(scheduleId, data);
        if (!response.IsSuccess)
        {
            throw new Exception($"Failed to update schedule: {response.ErrorMessage}");
        }
    }

    static async Task<Schedule> InsertSchedule(Main apiClient, Mappings mappings, Dictionary<string, string> row)
    {
        int courseId = await GetCourseId(apiClient, mappings, row);
        var scheduleDates = await GetScheduleDates(mappings, row);
        var scheduleResources = await GetScheduleResources(apiClient, mappings, row);

        var requestData = new Dictionary<string, string>
        {
            ["CourseID"] = courseId.ToString(),
            ["Dates"] = JsonSerializer.Serialize(scheduleDates)
        };

        if (scheduleResources.Any())
        {
            requestData["Resources"] = JsonSerializer.Serialize(scheduleResources);
        }

        var response = await apiClient.CustomAction<Schedule>("ScheduleCourse", requestData);
        if (!response.IsSuccess)
        {
            throw new Exception($"Failed to schedule course: {response.ErrorMessage}");
        }

        return response.Data;
    }

    private static async Task<int> GetCourseId(Main apiClient, Mappings mappings, Dictionary<string, string> row)
    {
        var courseIdMapping = mappings.Fields.FirstOrDefault(m => m.Target == "CourseID");
        
        if (courseIdMapping != null && int.TryParse(row[courseIdMapping.Source], out int courseId))
        {
            return courseId;
        }

        var courseRefMapping = mappings.Fields.FirstOrDefault(m => m.Target == "CourseReference");
        if (courseRefMapping == null)
        {
            throw new Exception("Neither CourseID nor CourseReference found in mappings");
        }

        var courseRef = row[courseRefMapping.Source];
        var courseFilters = new List<Main.Filter>
        {
            new Main.Filter { FieldName = "Reference", Operator = "=", Value = courseRef }
        };

        var courseResponse = await apiClient.Select<Course>(null, courseFilters);
        if (!courseResponse.IsSuccess)
        {
            throw new Exception($"Failed to get course: {courseResponse.ErrorMessage}");
        }
        if (!courseResponse.Data.Any())
        {
            throw new Exception($"Course not found with Reference: {courseRef}");
        }
        return courseResponse.Data.First().ID;
    }

    private static async Task<List<ScheduleDate>> GetScheduleDates(Mappings mappings, Dictionary<string, string> row)
    {
        var dateMapping = mappings.Fields.FirstOrDefault(m => m.Target == "Dates");
        var startTimeMapping = mappings.Fields.FirstOrDefault(m => m.Target == "StartTime");
        var endTimeMapping = mappings.Fields.FirstOrDefault(m => m.Target == "EndTime");

        if (dateMapping == null)
        {
            throw new Exception("'Dates' mapping is missing");
        }

        var dates = row[dateMapping.Source]
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(d => d.Trim())
            .Where(d => !string.IsNullOrEmpty(d))
            .ToList();

        if (!dates.Any())
        {
            throw new Exception("No valid dates provided");
        }

        TimeSpan? startTime = null;
        TimeSpan? endTime = null;

        if (startTimeMapping != null && !string.IsNullOrWhiteSpace(row[startTimeMapping.Source]))
        {
            startTime = TimeSpan.Parse(row[startTimeMapping.Source]);
        }

        if (endTimeMapping != null && !string.IsNullOrWhiteSpace(row[endTimeMapping.Source]))
        {
            endTime = TimeSpan.Parse(row[endTimeMapping.Source]);
        }

        return dates.Select(dateStr => new ScheduleDate
        {
            Date = DateTime.Parse(dateStr),
            StartTime = startTime,
            EndTime = endTime
        }).ToList();
    }

    private static async Task<List<ScheduleResource>> GetScheduleResources(Main apiClient, Mappings mappings, Dictionary<string, string> row)
    {
        var allResources = new List<(string value, int typeId)>();

        // Process the general Resource column (pipe-separated)
        var resourceMapping = mappings.Fields.FirstOrDefault(m => m.Target == "Resource");
        if (resourceMapping != null && !string.IsNullOrWhiteSpace(row[resourceMapping.Source]))
        {
            var resources = row[resourceMapping.Source]
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrEmpty(r));

            foreach (var resource in resources)
            {
                string resourceValue;
                int typeId = 4; // Default type ID

                // Check if resource contains type ID
                var parts = resource.Split(':');
                if (parts.Length > 1)
                {
                    resourceValue = parts[0].Trim();
                    if (!int.TryParse(parts[1].Trim(), out typeId))
                    {
                        throw new Exception($"Invalid resource type ID format for resource: {resource}");
                    }
                }
                else
                {
                    resourceValue = resource;
                }

                allResources.Add((resourceValue, typeId));
            }
        }

        // Process Venue_Resource column (TypeID = 1)
        var venueMapping = mappings.Fields.FirstOrDefault(m => m.Target == "Venue_Resource");
        if (venueMapping != null && !string.IsNullOrWhiteSpace(row[venueMapping.Source]))
        {
            var venues = row[venueMapping.Source]
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrEmpty(r));

            foreach (var venue in venues)
            {
                allResources.Add((venue, 1));
            }
        }

        // Process Tutor_Internal_Resource column (TypeID = 2)
        var tutorInternalMapping = mappings.Fields.FirstOrDefault(m => m.Target == "Tutor_Internal_Resource");
        if (tutorInternalMapping != null && !string.IsNullOrWhiteSpace(row[tutorInternalMapping.Source]))
        {
            var tutorsInternal = row[tutorInternalMapping.Source]
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrEmpty(r));

            foreach (var tutor in tutorsInternal)
            {
                allResources.Add((tutor, 2));
            }
        }

        // Process Tutor_External_Resource column (TypeID = 4)
        var tutorExternalMapping = mappings.Fields.FirstOrDefault(m => m.Target == "Tutor_External_Resource");
        if (tutorExternalMapping != null && !string.IsNullOrWhiteSpace(row[tutorExternalMapping.Source]))
        {
            var tutorsExternal = row[tutorExternalMapping.Source]
                .Split('|', StringSplitOptions.RemoveEmptyEntries)
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrEmpty(r));

            foreach (var tutor in tutorsExternal)
            {
                allResources.Add((tutor, 4));
            }
        }

        if (!allResources.Any())
        {
            return new List<ScheduleResource>();
        }

        // Process all resources and remove duplicates
        var scheduleResources = new List<ScheduleResource>();
        var processedResourceKeys = new HashSet<(int resourceId, int typeId)>();

        foreach (var (resourceValue, typeId) in allResources)
        {
            int resourceId;

            if (int.TryParse(resourceValue, out resourceId))
            {
                // Resource value is an ID
                var key = (resourceId, typeId);
                if (!processedResourceKeys.Contains(key))
                {
                    scheduleResources.Add(new ScheduleResource { ResourceID = resourceId, TypeID = typeId });
                    processedResourceKeys.Add(key);
                }
            }
            else
            {
                // Resource value is a Reference
                var resourceFilters = new List<Main.Filter>
                {
                    new Main.Filter { FieldName = "Reference", Operator = "=", Value = resourceValue }
                };

                var resourceResponse = await apiClient.Select<Resource>(null, resourceFilters);
                if (!resourceResponse.IsSuccess)
                {
                    throw new Exception($"Failed to get resource: {resourceResponse.ErrorMessage}");
                }
                if (!resourceResponse.Data.Any())
                {
                    throw new Exception($"Resource not found with Reference: {resourceValue}");
                }

                resourceId = resourceResponse.Data.First().ID;
                var key = (resourceId, typeId);
                if (!processedResourceKeys.Contains(key))
                {
                    scheduleResources.Add(new ScheduleResource 
                    { 
                        ResourceID = resourceId,
                        TypeID = typeId
                    });
                    processedResourceKeys.Add(key);
                }
            }
        }

        return scheduleResources;
    }

    private static readonly object logLock = new object();

    private static void Log(string message)
    {
        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
        lock (logLock)
        {
            try
            {
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch
            {
                // If we can't write to log file, just continue
            }
        }
    }

    private static void LogError(string message)
    {
        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {message}";
        lock (logLock)
        {
            try
            {
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch
            {
                // If we can't write to log file, just continue
            }
        }
    }

    private static void LogWarning(string message)
    {
        var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] WARNING: {message}";
        lock (logLock)
        {
            try
            {
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            }
            catch
            {
                // If we can't write to log file, just continue
            }
        }
    }

    private static string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalHours >= 1)
        {
            return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m {timeSpan.Seconds}s";
        }
        else if (timeSpan.TotalMinutes >= 1)
        {
            return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
        }
        else
        {
            return $"{timeSpan.Seconds}s";
        }
    }

    public class Resource
    {
        public int ID { get; set; }
        public string Reference { get; set; }
    }

    public class ScheduleResource
    {
        public int ResourceID { get; set; }
        public int TypeID { get; set; }
    }

    public class ScheduleDate
    {
        public DateTime Date { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}

// JSON Mapping Classes
public class Mappings
{
    public List<MappingField> Fields { get; set; }
}

public class MappingField
{
    public string Source { get; set; }
    public string Target { get; set; }
    public bool Lookup { get; set; }
}
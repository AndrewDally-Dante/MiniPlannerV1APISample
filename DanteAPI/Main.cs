using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DanteAPI
{
    public class Main : IDisposable
    {
        private readonly HttpClient _client;
        private readonly string DanteURL;
        private readonly string APIKey;

        public Main(string danteurl, string apikey)
        {
            if (string.IsNullOrEmpty(danteurl) || string.IsNullOrEmpty(apikey))
                throw new ArgumentNullException("App Setting Dante URL or API Key not set");
            DanteURL = danteurl;
            APIKey = apikey;

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("x-apikey", APIKey);
        }

        public async Task<bool> ValidateApiKey()
        {
            var response = await CustomAction<object>("ValidateKey", new Dictionary<string, string>());
            return response.IsSuccess;
        }

        public async Task<ApiResponse<List<T>>> Select<T>(List<string> fields, List<Filter> filters, int? pagesize = null, int? pageindex = null)
        {
            string url = $"{DanteURL}/API/V1/{typeof(T).Name}/Select";

            var queryParams = new List<string>();

            if (fields != null && fields.Count > 0)
                queryParams.Add("fields=" + string.Join(",", fields));

            if (filters != null && filters.Count > 0)
            {
                var filtersJson = JsonSerializer.Serialize(filters);
                var filtersEncoded = Uri.EscapeDataString(filtersJson);
                queryParams.Add("filters=" + filtersEncoded);
            }
            if (pagesize != null)
                queryParams.Add("pagesize=" + pagesize);
            if (pageindex != null)
                queryParams.Add("pageindex=" + pageindex);

            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            var response = new ApiResponse<List<T>>();

            try
            {
                HttpResponseMessage httpResponse = await _client.GetAsync(url);

                response.StatusCode = httpResponse.StatusCode;

                string responseBody = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    response.Data = JsonSerializer.Deserialize<List<T>>(responseBody, options);
                    response.IsSuccess = true;
                }
                else
                {
                    response.ErrorMessage = responseBody;
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }

            return response;
        }

        /// <summary>
        /// Inserts an entity using a dictionary of field names and values
        /// </summary>
        public async Task<ApiResponse<T>> Insert<T>(Dictionary<string, string> data)
        {
            string url = $"{DanteURL}/API/V1/{typeof(T).Name}/Insert";

            var response = new ApiResponse<T>();

            try
            {
                var formData = new MultipartFormDataContent();
                foreach (var kvp in data)
                {
                    formData.Add(new StringContent(kvp.Value ?? ""), kvp.Key);
                }

                HttpResponseMessage httpResponse = await _client.PostAsync(url, formData);

                response.StatusCode = httpResponse.StatusCode;

                string responseBody = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    response.Data = JsonSerializer.Deserialize<T>(responseBody, options);
                    response.IsSuccess = true;
                }
                else
                {
                    response.ErrorMessage = responseBody;
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }

            return response;
        }

        /// <summary>
        /// Updates an entity using a dictionary of field names and values
        /// </summary>
        public async Task<ApiResponse<T>> Update<T>(int id, Dictionary<string, string> data)
        {
            string url = $"{DanteURL}/API/V1/{typeof(T).Name}/Update?id={id}";

            var response = new ApiResponse<T>();

            try
            {
                var formData = new MultipartFormDataContent();
                foreach (var kvp in data)
                {
                    formData.Add(new StringContent(kvp.Value ?? ""), kvp.Key);
                }

                HttpResponseMessage httpResponse = await _client.PostAsync(url, formData);

                response.StatusCode = httpResponse.StatusCode;

                string responseBody = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    response.Data = JsonSerializer.Deserialize<T>(responseBody, options);
                    response.IsSuccess = true;
                }
                else
                {
                    response.ErrorMessage = responseBody;
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }

            return response;
        }

        /// <summary>
        /// Converts an entity object to a dictionary using reflection
        /// </summary>
        private Dictionary<string, string> ConvertEntityToDictionary<T>(T entity)
        {
            var dictionary = new Dictionary<string, string>();

            if (entity == null)
                return dictionary;

            // Get all properties of the entity
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                // Skip properties that can't be read
                if (!property.CanRead)
                    continue;

                // Get the property value
                object value = property.GetValue(entity);

                // Convert value to string (handling null values)
                string stringValue = value?.ToString();

                // Add to dictionary
                dictionary.Add(property.Name, stringValue);
            }

            return dictionary;
        }

        public async Task<ApiResponse<bool>> Delete<T>(int id)
        {
            string url = $"{DanteURL}/API/V1/{typeof(T).Name}/Delete?id={id}";

            var response = new ApiResponse<bool>();

            try
            {
                HttpResponseMessage httpResponse = await _client.GetAsync(url);

                response.StatusCode = httpResponse.StatusCode;

                string responseBody = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.IsSuccessStatusCode)
                {
                    response.Data = true;
                    response.IsSuccess = true;
                }
                else
                {
                    response.Data = false;
                    response.ErrorMessage = responseBody;
                    response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
            }

            return response;
        }

        public async Task<ApiResponse<T>> CustomAction<T>(string action, Dictionary<string, string> data)
        {
            string url = $"{DanteURL}/API/V1/{typeof(T).Name}/{action}";

            var response = new ApiResponse<T>();

            try
            {
                var formData = new MultipartFormDataContent();
                foreach (var kvp in data)
                {
                    formData.Add(new StringContent(kvp.Value ?? ""), kvp.Key);
                }

                HttpResponseMessage httpResponse = await _client.PostAsync(url, formData);

                response.StatusCode = httpResponse.StatusCode;

                string responseBody = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.IsSuccessStatusCode)
                {
                    if (typeof(T) == typeof(bool))
                    {
                        response.Data = (T)(object)true;
                    }
                    else
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        response.Data = JsonSerializer.Deserialize<T>(responseBody, options);
                    }
                    response.IsSuccess = true;
                }
                else
                {
                    response.ErrorMessage = responseBody;
                    response.IsSuccess = false;
                    if (typeof(T) == typeof(bool))
                    {
                        response.Data = (T)(object)false;
                    }
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.IsSuccess = false;
                if (typeof(T) == typeof(bool))
                {
                    response.Data = (T)(object)false;
                }
            }

            return response;
        }

        /// <summary>
        /// Performs a custom action using an entity object instead of a dictionary
        /// </summary>
        public async Task<ApiResponse<T>> CustomActionEntity<T, E>(string action, E entity)
        {
            // Convert entity to dictionary
            Dictionary<string, string> data = ConvertEntityToDictionary(entity);

            // Call the existing CustomAction method
            return await CustomAction<T>(action, data);
        }

        public class Filter
        {
            public string FieldName { get; set; }
            public string Operator { get; set; }
            public string Value { get; set; }
            public int? DecryptValue { get; set; }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }

    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }           // Indicates if the request was successful
        public T Data { get; set; }                   // The data returned from the API
        public string ErrorMessage { get; set; }      // The error message, if any
        public HttpStatusCode StatusCode { get; set; } // The HTTP status code
    }
}
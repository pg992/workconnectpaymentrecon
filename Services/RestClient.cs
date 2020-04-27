using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace PaymentReconciliation.Services
{
    public class RestClient : IRestClient
    {
        private HttpClient _httpClient;

        private readonly List<string> _sensitiveParameters;
        public RestClient()
        {
            _httpClient = new HttpClient();
            _sensitiveParameters = new List<string>
            {
                "password"
            };
            _httpClient.DefaultRequestHeaders.Accept.Clear();
        }

        public void AcceptHeaders(params string[] acceptTypes)
        {
            foreach (var param in acceptTypes)
            {
                _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(param));
            }
        }

        public void AddHeaders(string name, string value)
        {
            if (_httpClient.DefaultRequestHeaders.Contains(name))
            {
                _httpClient.DefaultRequestHeaders.Remove(name);

            }
            _httpClient.DefaultRequestHeaders.Add(name, value);
        }

        public void RemoveHeaders(string name)
        {
            _httpClient.DefaultRequestHeaders.Remove(name);
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return _httpClient.GetAsync(url);
        }

        public async Task<Stream> GetRawStreamAsync(string url)
        {
            LogInput(url, "GET");
            return await _httpClient.GetStreamAsync(url).ConfigureAwait(false);
        }

        public async Task<T> GetAsync<T>(string url)
        {
            LogInput(url, "GET");
            var res = await GetAsync(url).ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<T>(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            LogOutput(response, url, "GET");
            return response;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            LogInput(url, "DELETE");
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            var result = await _httpClient.SendAsync(request);
            LogOutput(result.StatusCode, string.Empty, result.IsSuccessStatusCode);
            return result;
        }

        public async Task<(HttpResponseMessage response, TOutput output)> ExecuteGetAsync<TOutput>(string url)
        {
            LogInput(url, "GET");
            var response = await GetAsync(url).ConfigureAwait(false);
            var res = response.IsSuccessStatusCode ?
               JsonConvert.DeserializeObject<TOutput>(await response.Content.ReadAsStringAsync()
               .ConfigureAwait(false)) : default(TOutput);
            LogOutput(res, url, "GET");
            return (response, res);
        }

        public async Task<(HttpResponseMessage response, TOutput output)> ExecutePostAsync<TInput, TOutput>(string url, TInput input, string contentType = "application/json")
        {
            LogInput(url, "POST", input);
            var response = await PostAsync(url, input, contentType).ConfigureAwait(false);
            var res = response.IsSuccessStatusCode ?
               JsonConvert.DeserializeObject<TOutput>(await response.Content.ReadAsStringAsync()
               .ConfigureAwait(false)) : default(TOutput);
            LogOutput(res, url, "GET");
            return (response, res);
        }

        public async Task<(HttpResponseMessage response, TOutput output)> ExecutePutAsync<TInput, TOutput>(string url, TInput input, string contentType = "application/json")
        {
            LogInput(url, "PUT", input);
            var response = await PutAsync(url, input, contentType).ConfigureAwait(false);
            var res = response.IsSuccessStatusCode ?
               JsonConvert.DeserializeObject<TOutput>(await response.Content.ReadAsStringAsync()
               .ConfigureAwait(false)) : default(TOutput);
            LogOutput(res, url, "GET");
            return (response, res);
        }

        public async Task<(HttpResponseMessage response, string output)> ExecutePostAsync<TInput>(string url, TInput input, string contentType = "application/json", string resolver = "lowercamel")
        {
            LogInput(url, "POST", input);
            var response = await PostAsync(url, input, contentType, resolver).ConfigureAwait(false);
            var res = response.IsSuccessStatusCode ?
             await response.Content.ReadAsStringAsync()
               .ConfigureAwait(false) : string.Empty;
            //Log.Information($"{GetRequestIdentifier()} RESPONSE: {res}");
            return (response, res);
        }

        public async Task<(HttpResponseMessage httpResponse, string output, TOutput response)> ExecuteGetAsync<TOutput>(string url, HttpStatusCode successCode = HttpStatusCode.OK)
        {
            LogInput(url, "GET");
            var response = await GetAsync(url).ConfigureAwait(false);
            var res = await response.Content.ReadAsStringAsync();
            //Log.Information($"{GetRequestIdentifier()} RESPONSE: {res}");

            if (response.StatusCode == successCode)
            {
                return (response, res, JsonConvert.DeserializeObject<TOutput>(res));
            }
            else
            {
                return (response, res, default(TOutput));
            }
        }

        public async Task<TOutput> PostAsync<TInput, TOutput>(string url, TInput content, string contentType = "application/json")
        {
            LogInput(url, "POST", content);
            var res = await PostAsync(url, content, contentType).ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<TOutput>(await res.Content.ReadAsStringAsync()
                .ConfigureAwait(false));
            LogOutput(response, url, "POST");
            return response;
        }

        public async Task<(HttpResponseMessage httpResponseMessage, TOutput response)> PostAsyncWithInfo<TInput, TOutput>(string url, TInput content, string contentType = "application/json")
        {
            var res = await PostAsync(url, content, contentType).ConfigureAwait(false);

            if (!res.IsSuccessStatusCode)
            {
                return (res, default(TOutput));
            }

            var objectResponse = JsonConvert.DeserializeObject<TOutput>(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            LogOutput(objectResponse, url, "POST");
            return (res, objectResponse);
        }

        public async Task<TOutput> PutAsync<TInput, TOutput>(string url, TInput content, string contentType = "application/json")
        {
            LogInput(url, "PUT", content);
            var res = await PutAsync(url, content, contentType).ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<TOutput>(await res.Content.ReadAsStringAsync()
                .ConfigureAwait(false));
            LogOutput(response, url, "PUT");
            return response;
        }

        public Task<HttpResponseMessage> DeleteAsync<T>(string url, T content)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
            };

            return _httpClient.SendAsync(request);
        }

        public async Task<TOutput> DeleteAsync<TInput, TOutput>(string url, TInput content)
        {
            LogInput(url, "DELETE");
            var res = await DeleteAsync(url, content).ConfigureAwait(false);
            var response = JsonConvert.DeserializeObject<TOutput>(await res.Content.ReadAsStringAsync()
                .ConfigureAwait(false));
            LogOutput(response, url, "DELETE");
            return response;
        }

        private IContractResolver GetContractResolver(string resolver)
        {
            switch (resolver.ToLower())
            {
                case "lowercamel":
                    return new CamelCasePropertyNamesContractResolver();
                default:
                    return new DefaultContractResolver();
            }
        }

        private Task<HttpResponseMessage> PostAsync<T>(string url, T content, string contentType, string resolver = "lowercamel")
        {
            return _httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                ContractResolver = GetContractResolver(resolver)
            }), Encoding.UTF8, contentType));
        }

        private Task<HttpResponseMessage> PostAsyncUrlEncodedXml(string url, IDictionary<string, string> content)
        {
            return _httpClient.PostAsync(url, new FormUrlEncodedContent(content.ToList()));
        }

        public async Task<(HttpResponseMessage httpResponse, string content)> PostUrlEncodedXmlAsync(string url, IDictionary<string, string> content)
        {
            LogInput(url, "POST", content, "URL-ENCODED");
            var res = await PostAsyncUrlEncodedXml(url, content);
            (HttpResponseMessage message, string resultContent) = (res, string.Empty);
            if (res.IsSuccessStatusCode)
            {
                resultContent = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            LogOutput(res.StatusCode, resultContent, res.IsSuccessStatusCode);

            return (message, resultContent);
        }

        private Task<HttpResponseMessage> PutAsync<T>(string url, T content, string contentType)
        {
            return _httpClient.PutAsync(url, new StringContent(JsonConvert.SerializeObject(content, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }), Encoding.UTF8, contentType));
        }

        private void LogInput(string url, string method)
        {
            //Log.Information($"{GetRequestIdentifier()} REQUEST HTTP METHOD: {method} EXTERNAL URL: {url}");
        }

        private void LogOutput<T>(T output, string url = "", string method = "")
        {
            //Log.Information($"{GetRequestIdentifier()} RESPONSE HTTP METHOD: {method} EXTERNAL URL: {url} RESPONSE BODY: {GetSerializedObject(output)}");
        }

        private void LogOutput(System.Net.HttpStatusCode httpStatusCode, string content, bool isSuccessfulStatusCode)
        {
            //Log.Information($"{GetRequestIdentifier()} RESPONSE: HTTP STATUS CODE {httpStatusCode.ToString()}");
            //if (isSuccessfulStatusCode)
            //{
            //    Log.Information($"{GetRequestIdentifier()} CONTENT: {content}");
            //}
        }

        public void LogInput<T>(string url, string method, T content, string bodyType = "BODY")
        {
            LogInput(url, method);
            //Log.Information($"{GetRequestIdentifier()} {bodyType}: {GetSerializedObject(content)}");
        }

        public string GetSerializedObject<T>(T input)
        {
            var jsonContent = input != null ? JsonConvert.SerializeObject(input, formatting: Formatting.Indented, settings: new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }) : "{}";
            return LogSensitiveInput(jsonContent);
        }

        public void ProcessJson(JToken token)
        {
            if (token is JObject)
            {
                var dat = token as JObject;
                foreach (var item in dat.Values())
                {
                    if (item.Type == JTokenType.String)
                    {
                        if (_sensitiveParameters.Contains(item.Path))
                        {
                            var s = (JToken)(new string(item.ToString().Select(c => '*').ToArray()));
                            dat[item.Path] = s;
                        }
                    }
                    if (item.Type == JTokenType.Object)
                    {
                        CalculateSensitiveInfo(item as JObject);
                    }
                    if (item.Type == JTokenType.Array)
                    {
                        LogSensitiveInput(item.ToString());
                    }
                }
            }
            if (token is JArray)
            {
                var dat = token as JArray;
                foreach (var item in dat.Values())
                {
                    if (item.Type == JTokenType.String)
                    {
                        if (_sensitiveParameters.Contains(item.Path))
                        {
                            var s = (JToken)(new string(item.ToString().Select(c => '*').ToArray()));
                            dat[item.Path] = s;
                        }
                    }
                    if (item.Type == JTokenType.Object)
                    {
                        CalculateSensitiveInfo(item as JObject);
                    }
                    if (item.Type == JTokenType.Array)
                    {
                        LogSensitiveInput(item.ToString());
                    }
                    if (item.Type == JTokenType.Property)
                    {
                        CalculateSensitiveInfo(item as JProperty);
                    }
                }
            }
        }

        public string LogSensitiveInput(string input)
        {
            var content = JToken.Parse(input);
            ProcessJson(content);
            return content.ToString();
        }

        private void CalculateSensitiveInfo(JObject inputToken)
        {
            foreach (var val in inputToken.Values())
            {
                if (val.Type == JTokenType.Object)
                {
                    CalculateSensitiveInfo(val as JObject);
                }
                if (val.Type == JTokenType.String)
                {
                    var parts = val.Path.Split('.');
                    if (_sensitiveParameters.Contains(parts[parts.Count() - 1]))
                    {
                        var s = (JToken)(new string(val.ToString().Select(c => '*').ToArray()));
                        inputToken[parts[parts.Count() - 1]] = s;
                    }
                }
                if (val.Type == JTokenType.Array)
                {
                    ProcessJson(val);
                }
            }
        }

        private void CalculateSensitiveInfo(JProperty inputToken)
        {
            foreach (var val in inputToken.Values())
            {
                if (val.Type == JTokenType.Object)
                {
                    CalculateSensitiveInfo(val as JObject);
                }
                if (val.Type == JTokenType.String)
                {
                    var parts = val.Path.Split('.');
                    if (_sensitiveParameters.Contains(parts[parts.Count() - 1]))
                    {
                        var s = (JToken)(new string(val.ToString().Select(c => '*').ToArray()));
                        var key = parts[parts.Count() - 1];
                        inputToken.Value = s;
                    }
                }
            }
        }
    }
}

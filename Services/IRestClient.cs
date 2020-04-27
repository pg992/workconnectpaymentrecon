using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PaymentReconciliation.Services
{
    public interface IRestClient
    {
        void AddHeaders(string name, string value);
        void RemoveHeaders(string name);
        Task<T> GetAsync<T>(string url);
        Task<TOutput> PostAsync<TInput, TOutput>(string url, TInput content, string contentType = "application/json");
        Task<TOutput> PutAsync<TInput, TOutput>(string url, TInput content, string contentType = "application/json");
        Task<TOutput> DeleteAsync<TInput, TOutput>(string url, TInput content);
        Task<(HttpResponseMessage httpResponseMessage, TOutput response)> PostAsyncWithInfo<TInput, TOutput>(string url, TInput content, string contentType = "application/json");
        Task<Stream> GetRawStreamAsync(string url);
        Task<(HttpResponseMessage response, TOutput output)> ExecutePostAsync<TInput, TOutput>(string url, TInput input, string contentType = "application/json");
        Task<(HttpResponseMessage response, TOutput output)> ExecuteGetAsync<TOutput>(string url);
        Task<(HttpResponseMessage response, string output)> ExecutePostAsync<TInput>(string url, TInput input, string contentType = "application/json", string resolver = "lowercamel");
        Task<(HttpResponseMessage response, TOutput output)> ExecutePutAsync<TInput, TOutput>(string url, TInput input, string contentType = "application/json");
        Task<(HttpResponseMessage httpResponse, string content)> PostUrlEncodedXmlAsync(string url, IDictionary<string, string> content);
        Task<HttpResponseMessage> DeleteAsync(string url);
        Task<(HttpResponseMessage httpResponse, string output, TOutput response)> ExecuteGetAsync<TOutput>(string url, HttpStatusCode successCode = HttpStatusCode.OK);
    }
}

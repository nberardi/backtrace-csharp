using System;
using Backtrace.Model;
using System.Collections.Generic;
using Backtrace.Interfaces;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Security.Authentication;
using Backtrace.Common;
using System.Threading.Tasks;
using System.Net.Http;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Backtrace.Tests")]
namespace Backtrace.Services
{
    /// <summary>
    /// Backtrace Api class that allows to send a diagnostic data to server
    /// </summary>
    internal class BacktraceApi<T> : IBacktraceApi<T>
    {
        /// <summary>
        /// Url to server
        /// </summary>
        private readonly string _serverurl;

        /// <summary>
        /// The http client.
        /// </summary>
        private readonly HttpClient _http;

        /// <summary>
        /// Create a new instance of Backtrace API
        /// </summary>
        /// <param name="credentials">API credentials</param>
        public BacktraceApi(BacktraceCredentials credentials) 
        {
            _http = new HttpClient();
            _serverurl = $"{credentials.BacktraceHostUri.AbsoluteUri}post?format=json&token={credentials.Token}";
        }

        /// <summary>
        /// Get serialization settings
        /// </summary>
        /// <returns></returns>
        private JsonSerializerSettings SerializerSettings { get; } = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore
        };

        /// <summary>
        /// Sending a diagnostic report data to server API. 
        /// </summary>
        /// <param name="data">Diagnostic data</param>
        /// <returns>False if operation fail or true if API return OK</returns>
        public async Task<BacktraceServerResponse> Send(BacktraceData<T> data)
        {
            var json = JsonConvert.SerializeObject(data, SerializerSettings);
            var attachments = data.Attachments;

            var requestId = Guid.NewGuid().ToString("N");
            var request = new HttpRequestMessage(HttpMethod.Post, _serverurl);
            var content = new MultipartFormDataContent(requestId);

            const string jsonName = "upload_file";
            content.Add(new StringContent(json), jsonName, jsonName);

            foreach (var attachmentPath in attachments)
            {
                if (!File.Exists(attachmentPath))
                    continue;

                var name = "attachment_" + Path.GetFileName(attachmentPath);
                content.Add(new StreamContent(File.OpenRead(attachmentPath)), name, name);
            }

            request.Content = content;

            var response = await _http.SendAsync(request);
            var fullResponse = await response.Content.ReadAsStringAsync();
            var serverResponse = JsonConvert.DeserializeObject<BacktraceServerResponse>(fullResponse, SerializerSettings);
            return serverResponse;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _http?.Dispose();
                }

                disposedValue = true;
            }
        }

        ~BacktraceApi() 
        {
           Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

﻿using HttpRequestHandler.Responses;
using HttpRequestHandler.Utils;
using System.Net.Http.Headers;
using System.Text;

namespace Tipi.Tools
{
    /// <summary>
    /// Class <c>HttpRequestHandler</c> serves as a wrapper for the class <c>HttpClient</c>.
    /// </summary>
    /// <remarks>
    /// Removes unessesary code when working with Http Requests.
    /// </remarks>
    public class HttpRequestHandler : IDisposable
    {
        #region Private Properties
        private readonly HttpClient _client;
        private HttpResponseMessage? _response;
        private StringContent? _content;
        #endregion
        #region Constructors
        /// <summary>This constructor initializes the new Default <c>HttpRequestHandler</c>.</summary>
        public HttpRequestHandler()
        {
            _client = new HttpClient();
        }
        /// <summary>This constructor initializes a new <c>HttpRequestHandler</c> with a Bearer Token auth flow.</summary>
        /// <param name="bearerToken">Bearer Token.</param>
        public HttpRequestHandler(string bearerToken) : this()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
        /// <summary>This constructor initializes a new <c>HttpRequestHandler</c> with custom headers.</summary>
        /// <param name="headers">Dictionary representing Key and Value Headers.</param>
        public HttpRequestHandler(Dictionary<string, string> headers) : this()
        {
            foreach (var header in headers)
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
        /// <summary>This constructor initializes a new <c>HttpRequestHandler</c> with a Bearer Token auth flow and custom headers.</summary>
        /// <param name="bearerToken">Bearer Token.</param>
        /// <param name="headers">Dictionary representing Key and Value Headers.</param>
        public HttpRequestHandler(string bearerToken, Dictionary<string, string> headers) : this()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            foreach (var header in headers)
                _client.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// This method executes an HTTP Request.
        /// </summary>
        /// <remarks>
        /// Executed the provided HTTP Verb to the provided Endpoint.
        /// </remarks>
        /// <param name="method">HTTP Verb in All caps EX: GET.</param>
        /// <param name="endpoint">Endpoint URL.</param>
        /// <param name="body">String Json object, this field is not required.</param>
        /// <returns>
        /// Returns an object containg the HTTP Response and a string containing the Body of the response
        /// </returns>
        public async Task<HttpResponse> ExecuteAsync(string method, string endpoint, string? body = null)
        {
            _response = new HttpResponseMessage(); //Initialize Response object
            if (body != null) //Check if the execute request need a base model for the body parameter
                SerializeBody(body);

            if (method == HttpMethods.Post)
                _response = await _client.PostAsync(endpoint, _content);

            else if (method == HttpMethods.Patch)
                _response = await _client.PatchAsync(endpoint, _content);

            else if (method == HttpMethods.Put)
                _response = await _client.PutAsync(endpoint, _content);

            else if (method == HttpMethods.Get)
                _response = await _client.GetAsync(endpoint);

            else if (method == HttpMethods.Delete)
                _response = await _client.DeleteAsync(endpoint);

            return new HttpResponse
            {
                StatusCode = _response.StatusCode,
                Body = await _response.Content.ReadAsStringAsync()
            };
        }
        /// <summary>
        /// Diposes the <c>HttpRequestHandler</c>.
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
            if (_response != null)
                _response.Dispose();
            if (_content != null)
                _content.Dispose();
        }
        #endregion
        #region Private Methods
        private void SerializeBody(string jsonObject)
        {
            _content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
        }
        #endregion
    }
}
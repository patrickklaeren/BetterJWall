using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;

namespace Atlassian.Jira.Remote
{
    internal class JiraRestClient : IJiraRestClient
    {
        private readonly RestClient _restClient;
        private readonly JiraRestClientSettings _clientSettings;
        private readonly ServiceLocator _services;

        internal JiraRestClient(ServiceLocator services, string url, string username = null, string password = null, JiraRestClientSettings settings = null)
        {
            url = url.EndsWith("/") ? url : url += "/";

            _clientSettings = settings ?? new JiraRestClientSettings();
            _restClient = new RestClient(url);
            _services = services;

            if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
            {
                this._restClient.Authenticator = new HttpBasicAuthenticator(username, password);
            }
        }

        public RestClient RestSharpClient
        {
            get
            {
                return _restClient;
            }
        }

        public string Url
        {
            get
            {
                return _restClient.BaseUrl.ToString();
            }
        }

        public JiraRestClientSettings Settings
        {
            get
            {
                return _clientSettings;
            }
        }

        public async Task<T> ExecuteRequestAsync<T>(Method method, string resource, object requestBody = null, CancellationToken token = default(CancellationToken))
        {
            var result = await ExecuteRequestAsync(method, resource, requestBody, token).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(result.ToString(), Settings.JsonSerializerSettings);
        }

        public async Task<JToken> ExecuteRequestAsync(Method method, string resource, object requestBody = null, CancellationToken token = default(CancellationToken))
        {
            if (method == Method.GET && requestBody != null)
            {
                throw new InvalidOperationException($"GET requests are not allowed to have a request body. Resource: {resource}. Body: {requestBody}");
            }

            var request = new RestRequest();
            request.Method = method;
            request.Resource = resource;
            request.RequestFormat = DataFormat.Json;

            if (requestBody is string)
            {
                request.AddParameter(new Parameter
                {
                    Name = "application/json",
                    Type = ParameterType.RequestBody,
                    Value = requestBody
                });
            }
            else if (requestBody != null)
            {
                request.JsonSerializer = new RestSharpJsonSerializer(JsonSerializer.Create(Settings.JsonSerializerSettings));
                request.AddJsonBody(requestBody);
            }

            LogRequest(request, requestBody);
            var response = await this._restClient.ExecuteTaskAsync(request, token).ConfigureAwait(false);
            return GetValidJsonFromResponse(request, response);
        }

        public async Task<IRestResponse> ExecuteRequestAsync(IRestRequest request, CancellationToken token = default(CancellationToken))
        {
            var response = await this._restClient.ExecuteTaskAsync(request, token).ConfigureAwait(false);
            GetValidJsonFromResponse(request, response);
            return response;
        }

        private void LogRequest(RestRequest request, object body = null)
        {
            if (this._clientSettings.EnableRequestTrace)
            {
                Trace.WriteLine(String.Format("[{0}] Request Url: {1}",
                    request.Method,
                    request.Resource));

                if (body != null)
                {
                    Trace.WriteLine(String.Format("[{0}] Request Data: {1}",
                        request.Method,
                        JsonConvert.SerializeObject(body, new JsonSerializerSettings()
                        {
                            Formatting = Formatting.Indented,
                            NullValueHandling = NullValueHandling.Ignore
                        })));
                }
            }
        }

        private JToken GetValidJsonFromResponse(IRestRequest request, IRestResponse response)
        {
            var content = response.Content != null ? response.Content.Trim() : string.Empty;

            if (this._clientSettings.EnableRequestTrace)
            {
                Trace.WriteLine(String.Format("[{0}] Response for Url: {1}\n{2}",
                    request.Method,
                    request.Resource,
                    content));
            }

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                throw new InvalidOperationException(response.ErrorMessage);
            }
            else if (response.StatusCode == HttpStatusCode.InternalServerError
                || response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new InvalidOperationException(String.Format("Response Content: {0}", content));
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden
                || response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new System.Security.Authentication.AuthenticationException(string.Format("Response Content: {0}", content));
            }
            else if (string.IsNullOrWhiteSpace(content))
            {
                return new JObject();
            }
            else if (!content.StartsWith("{") && !content.StartsWith("["))
            {
                throw new InvalidOperationException(String.Format("Response was not recognized as JSON. Content: {0}", content));
            }
            else
            {
                JToken parsedContent;

                try
                {
                    parsedContent = JToken.Parse(content);
                }
                catch (JsonReaderException ex)
                {
                    throw new InvalidOperationException(String.Format("Failed to parse response as JSON. Content: {0}", content), ex);
                }

                if (parsedContent != null && parsedContent.Type == JTokenType.Object && parsedContent["errorMessages"] != null)
                {
                    throw new InvalidOperationException(string.Format("Response reported error(s) from JIRA: {0}", parsedContent["errorMessages"].ToString()));
                }

                return parsedContent;
            }
        }
    }
}

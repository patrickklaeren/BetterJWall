using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Atlassian.Jira.Remote
{
    /// <summary>
    /// Contract for a client that interacts with JIRA via rest.
    /// </summary>
    public interface IJiraRestClient
    {
        /// <summary>
        /// Base url of the Jira server.
        /// </summary>
        string Url { get; }

        /// <summary>
        /// RestSharp client used to issue requests.
        /// </summary>
        RestClient RestSharpClient { get; }

        /// <summary>
        /// Settings to configure the rest client.
        /// </summary>
        JiraRestClientSettings Settings { get; }

        /// <summary>
        /// Executes a request.
        /// </summary>
        /// <param name="request">Request object.</param>
        /// <param name="token">Cancellation token for the operation.</param>
        Task<IRestResponse> ExecuteRequestAsync(IRestRequest request, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Executes an async request and returns the response as JSON.
        /// </summary>
        /// <param name="method">Request method.</param>
        /// <param name="resource">Request resource url.</param>
        /// <param name="requestBody">Request body to be serialized.</param>
        /// <param name="token">Cancellation token for the operation.</param>
        Task<JToken> ExecuteRequestAsync(Method method, string resource, object requestBody = null, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Executes an async request and serializes the response to an object.
        /// </summary>
        /// <typeparam name="T">Type to serialize the response.</typeparam>
        /// <param name="method">Request method.</param>
        /// <param name="resource">Request resource url.</param>
        /// <param name="requestBody">Request body to be serialized.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<T> ExecuteRequestAsync<T>(Method method, string resource, object requestBody = null, CancellationToken token = default(CancellationToken));
    }
}

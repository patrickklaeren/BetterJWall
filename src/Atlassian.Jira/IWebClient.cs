using System;
using System.Threading.Tasks;

namespace Atlassian.Jira
{
    /// <summary>
    /// Abstracts a web client.
    /// </summary>
    public interface IWebClient
    {
        /// <summary>
        /// Downloads a file from the server.
        /// </summary>
        Task DownloadAsync(string url, string fileName);

        /// <summary>
        /// Downloads a file from the server including authentication header.
        /// </summary>
        Task DownloadWithAuthenticationAsync(string url, string fileName);
    }
}

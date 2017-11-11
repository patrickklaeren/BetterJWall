using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Atlassian.Jira
{
    internal class WebClientWrapper : IWebClient
    {
        private readonly WebClient _webClient;
        private readonly Jira _jira;

        public WebClientWrapper(Jira jira)
        {
            _jira = jira;
            _webClient = new WebClient();
            _webClient.DownloadFileCompleted += _webClient_DownloadFileCompleted;
        }

        void _webClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            var completionSource = e.UserState as TaskCompletionSource<object>;

            if (completionSource != null)
            {
                if (e.Cancelled)
                {
                    completionSource.TrySetCanceled();
                }
                else if (e.Error != null)
                {
                    completionSource.TrySetException(e.Error);
                }
                else
                {
                    completionSource.TrySetResult(null);
                }
            }
        }

        public Task DownloadAsync(string url, string fileName)
        {
            _webClient.CancelAsync();

            var completionSource = new TaskCompletionSource<object>();
            _webClient.Headers.Remove(HttpRequestHeader.Authorization);
            _webClient.DownloadFileAsync(new Uri(url), fileName, completionSource);

            return completionSource.Task;
        }

        public Task DownloadWithAuthenticationAsync(string url, string fileName)
        {
            var credentials = _jira.GetCredentials();

            if (String.IsNullOrEmpty(credentials.UserName) || String.IsNullOrEmpty(credentials.Password))
            {
                throw new InvalidOperationException("Unable to download file, user and/or password are missing. You can specify a provider for credentials when constructing the Jira instance.");
            }

            _webClient.CancelAsync();

            var completionSource = new TaskCompletionSource<object>();
            string encodedUserNameAndPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials.UserName + ":" + credentials.Password));

            _webClient.Headers.Remove(HttpRequestHeader.Authorization);
            _webClient.Headers.Add(HttpRequestHeader.Authorization, "Basic " + encodedUserNameAndPassword);
            _webClient.DownloadFileAsync(new Uri(url), fileName, completionSource);

            return completionSource.Task;
        }
    }
}

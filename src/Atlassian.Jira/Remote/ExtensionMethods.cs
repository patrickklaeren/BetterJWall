using System;
using System.Threading;
using System.Threading.Tasks;

namespace Atlassian.Jira.Remote
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Create a new RemoteIssue based on the information in a given issue.
        /// </summary>
        public static RemoteIssue ToRemote(this Issue issue)
        {
            return issue.ToRemoteAsync(CancellationToken.None).Result;
        }

        /// <summary>
        /// Create a new RemoteIssue based on the information in a given issue.
        /// </summary>
        public static Task<RemoteIssue> ToRemoteAsync(this Issue issue, CancellationToken token = default(CancellationToken))
        {
            return issue.ToRemoteAsync(token);
        }

        /// <summary>
        /// Create a new Issue from a RemoteIssue
        /// </summary>
        public static Issue ToLocal(this RemoteIssue remoteIssue, Jira jira = null)
        {
            return new Issue(jira, remoteIssue);
        }

        /// <summary>
        /// Create a new Attachment from a RemoteAttachment
        /// </summary>
        public static Attachment ToLocal(this RemoteAttachment remoteAttachment, Jira jira, IWebClient webClient)
        {
            return new Attachment(jira, webClient, remoteAttachment);
        }

        /// <summary>
        /// Creates a new Version from RemoteVersion
        /// </summary>
        public static ProjectVersion ToLocal(this RemoteVersion remoteVersion, Jira jira)
        {
            return new ProjectVersion(jira, remoteVersion);
        }

        /// <summary>
        /// Creates a new Component from RemoteComponent
        /// </summary>
        public static ProjectComponent ToLocal(this RemoteComponent remoteComponent)
        {
            return new ProjectComponent(remoteComponent);
        }
    }
}

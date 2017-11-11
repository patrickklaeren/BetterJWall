using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents the operations on the issue link of jira.
    /// </summary>
    public interface IIssueLinkService
    {
        /// <summary>
        /// Returns all available issue link types.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IEnumerable<IssueLinkType>> GetLinkTypesAsync(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Creates an issue link between two issues.
        /// </summary>
        /// <param name="outwardIssueKey">Key of the outward issue.</param>
        /// <param name="inwardIssueKey">Key of the inward issue.</param>
        /// <param name="linkName">Name of the issue link.</param>
        /// <param name="comment">Comment to add to the outward issue.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task CreateLinkAsync(string outwardIssueKey, string inwardIssueKey, string linkName, string comment, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns all issue links associated with a given issue.
        /// </summary>
        /// <param name="issueKey">The issue to retrieve links for.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IEnumerable<IssueLink>> GetLinksForIssueAsync(string issueKey, CancellationToken token);
    }
}

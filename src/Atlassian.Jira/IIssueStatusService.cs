using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents the operations on the issue statuses of jira.
    /// Maps to https://docs.atlassian.com/jira/REST/latest/#api/2/status.
    /// </summary>
    public interface IIssueStatusService
    {
        /// <summary>
        /// Returns all the issue statuses within JIRA.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IEnumerable<IssueStatus>> GetStatusesAsync(CancellationToken token = default(CancellationToken));
    }
}
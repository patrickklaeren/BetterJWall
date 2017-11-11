using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents the operations on the issue link types of jira.
    /// </summary>
    public interface IIssueFieldService
    {
        /// <summary>
        /// Returns all custom fields within JIRA.
        /// </summary>
        Task<IEnumerable<CustomField>> GetCustomFieldsAsync(CancellationToken token = default(CancellationToken));
    }
}

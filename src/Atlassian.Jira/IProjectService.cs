using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents the operations on the projects of jira.
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// Returns all projects defined in JIRA.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IEnumerable<Project>> GetProjectsAsync(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns a single project in JIRA.
        /// </summary>
        /// <param name="projectKey">Project key for the single project to load</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<Project> GetProjectAsync(string projectKey, CancellationToken token = default(CancellationToken));
    }
}

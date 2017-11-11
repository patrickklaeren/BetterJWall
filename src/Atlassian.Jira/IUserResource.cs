using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atlassian.Jira
{
    public interface IUserResource
    {
        /// <summary>
        /// Get a known user
        /// </summary>
        JiraUser Get(string username);

        /// <summary>
        /// Get a known user
        /// </summary>
        Task<JiraUser> GetAsync(string username, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Create a user
        /// </summary>
        JiraUser Create(JiraUserCreationInfo user);

        /// <summary>
        /// Create a user
        /// </summary>
        Task<JiraUser> CreateAsync(JiraUserCreationInfo user, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Delete a user
        /// </summary>
        void Delete(string username);

        /// <summary>
        /// Delete a user
        /// </summary>
        Task DeleteAsync(string username, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Search for a user
        /// </summary>
        IEnumerable<JiraUser> Search(string username, bool includeActive = true, bool includeInactive = false, int startAt = 0, int maxResults = 50);

        /// <summary>
        /// Search for a user
        /// </summary>
        Task<IEnumerable<JiraUser>> SearchAsync(string username, bool includeActive = true, bool includeInactive = false, int startAt = 0, int maxResults = 50, CancellationToken token = default(CancellationToken));
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Atlassian.Jira
{
    public interface IGroupResource
    {
        /// <summary>
        /// Return members of a group
        /// </summary>
        IPagedQueryResult<JiraUser> GetGroupMembers(string groupname, bool includeInactiveUsers = false, int startAt = 0, int maxResults = 50);

        /// <summary>
        /// Return members of a group
        /// </summary>
        Task<IPagedQueryResult<JiraUser>> GetGroupMembersAsync(string groupname, bool includeInactiveUsers = false, int startAt = 0, int maxResults = 50, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Add user to a group
        /// </summary>
        void AddUser(string groupname, string username);


        /// <summary>
        /// Add user to a group
        /// </summary>
        Task AddUserAsync(string groupname, string username, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Remove user from a group
        /// </summary>
        void RemoveUser(string groupname, string username);

        /// <summary>
        /// Remove user from a group
        /// </summary>
        Task RemoveUserAsync(string groupname, string username, CancellationToken token = default(CancellationToken));
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents the operations on the filters of jira.
    /// </summary>
    public interface IIssueFilterService
    {
        /// <summary>
        /// Returns the favourite filters for the user.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IEnumerable<JiraFilter>> GetFavouritesAsync(CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns issues that match the specified favorite filter.
        /// </summary>
        /// <param name="filterName">The name of the filter used for the search</param>
        /// <param name="maxIssues">Maximum number of issues to return (defaults to 50). The maximum allowable value is dictated by the JIRA property 'jira.search.views.default.max'. If you specify a value that is higher than this number, your search results will be truncated.</param>
        /// <param name="startAt">Index of the first issue to return (0-based)</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IPagedQueryResult<Issue>> GetIssuesFromFavoriteAsync(string filterName, int? maxIssues = null, int startAt = 0, CancellationToken token = default(CancellationToken));
    }
}

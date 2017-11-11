using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace Atlassian.Jira.Remote
{
    internal class IssueFilterService : IIssueFilterService
    {
        private readonly Jira _jira;

        public IssueFilterService(Jira jira)
        {
            _jira = jira;
        }

        public Task<IEnumerable<JiraFilter>> GetFavouritesAsync(CancellationToken token = default(CancellationToken))
        {
            return _jira.RestClient.ExecuteRequestAsync<IEnumerable<JiraFilter>>(Method.GET, "rest/api/2/filter/favourite", null, token);
        }

        public async Task<IPagedQueryResult<Issue>> GetIssuesFromFavoriteAsync(string filterName, int? maxIssues = default(int?), int startAt = 0, CancellationToken token = default(CancellationToken))
        {
            var filters = await this.GetFavouritesAsync(token).ConfigureAwait(false);
            var filter = filters.FirstOrDefault(f => f.Name.Equals(filterName, StringComparison.OrdinalIgnoreCase));

            if (filter == null)
            {
                throw new InvalidOperationException(String.Format("Filter with name '{0}' was not found.", filterName));
            }

            return await _jira.Issues.GetIssuesFromJqlAsync(filter.Jql, maxIssues, startAt, token).ConfigureAwait(false);
        }
    }
}

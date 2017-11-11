using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace Atlassian.Jira.Remote
{
    internal class IssueStatusService : IIssueStatusService
    {
        private readonly Jira _jira;

        public IssueStatusService(Jira jira)
        {
            _jira = jira;
        }
        public async Task<IEnumerable<IssueStatus>> GetStatusesAsync(CancellationToken token = default(CancellationToken))
        {
            var cache = _jira.Cache;

            if (!cache.Statuses.Any())
            {
                var results = await _jira.RestClient.ExecuteRequestAsync<RemoteStatus[]>(Method.GET, "rest/api/2/status", null, token).ConfigureAwait(false);
                cache.Statuses.TryAdd(results.Select(s => new IssueStatus(s)));
            }

            return cache.Statuses.Values;
        }
    }
}

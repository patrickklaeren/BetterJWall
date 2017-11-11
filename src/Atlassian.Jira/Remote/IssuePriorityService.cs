using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace Atlassian.Jira.Remote
{
    internal class IssuePriorityService : IIssuePriorityService
    {
        private readonly Jira _jira;

        public IssuePriorityService(Jira jira)
        {
            _jira = jira;
        }

        public async Task<IEnumerable<IssuePriority>> GetPrioritiesAsync(CancellationToken token = default(CancellationToken))
        {
            var cache = _jira.Cache;

            if (!cache.Priorities.Any())
            {
                var priorities = await _jira.RestClient.ExecuteRequestAsync<RemotePriority[]>(Method.GET, "rest/api/2/priority", null, token).ConfigureAwait(false);
                cache.Priorities.TryAdd(priorities.Select(p => new IssuePriority(p)));
            }

            return cache.Priorities.Values;
        }
    }
}

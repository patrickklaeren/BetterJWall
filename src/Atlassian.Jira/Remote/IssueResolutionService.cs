using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace Atlassian.Jira.Remote
{
    internal class IssueResolutionService : IIssueResolutionService
    {
        private readonly Jira _jira;

        public IssueResolutionService(Jira jira)
        {
            _jira = jira;
        }

        public async Task<IEnumerable<IssueResolution>> GetResolutionsAsync(CancellationToken token)
        {
            var cache = _jira.Cache;

            if (!cache.Resolutions.Any())
            {
                var resolutions = await _jira.RestClient.ExecuteRequestAsync<RemoteResolution[]>(Method.GET, "rest/api/2/resolution", null, token).ConfigureAwait(false);
                cache.Resolutions.TryAdd(resolutions.Select(r => new IssueResolution(r)));
            }

            return cache.Resolutions.Values;
        }
    }
}

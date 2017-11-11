using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace Atlassian.Jira.Remote
{
    internal class IssueFieldService : IIssueFieldService
    {
        private readonly Jira _jira;

        public IssueFieldService(Jira jira)
        {
            _jira = jira;
        }

        public async Task<IEnumerable<CustomField>> GetCustomFieldsAsync(CancellationToken token = default(CancellationToken))
        {
            var cache = _jira.Cache;

            if (!cache.CustomFields.Any())
            {
                var remoteFields = await _jira.RestClient.ExecuteRequestAsync<RemoteField[]>(Method.GET, "rest/api/2/field", null, token).ConfigureAwait(false);
                var results = remoteFields.Where(f => f.IsCustomField).Select(f => new CustomField(f));
                cache.CustomFields.TryAdd(results);
            }

            return cache.CustomFields.Values;
        }
    }
}

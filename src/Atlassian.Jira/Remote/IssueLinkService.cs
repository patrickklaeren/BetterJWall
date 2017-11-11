using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Atlassian.Jira.Remote
{
    internal class IssueLinkService : IIssueLinkService
    {
        private readonly Jira _jira;

        public IssueLinkService(Jira jira)
        {
            _jira = jira;
        }

        public Task CreateLinkAsync(string outwardIssueKey, string inwardIssueKey, string linkName, string comment, CancellationToken token = default(CancellationToken))
        {
            var bodyObject = new JObject();
            bodyObject.Add("type", new JObject(new JProperty("name", linkName)));
            bodyObject.Add("inwardIssue", new JObject(new JProperty("key", inwardIssueKey)));
            bodyObject.Add("outwardIssue", new JObject(new JProperty("key", outwardIssueKey)));

            if (!String.IsNullOrEmpty(comment))
            {
                bodyObject.Add("comment", new JObject(new JProperty("body", comment)));
            }

            return _jira.RestClient.ExecuteRequestAsync(Method.POST, "rest/api/2/issueLink", bodyObject, token);
        }

        public async Task<IEnumerable<IssueLink>> GetLinksForIssueAsync(string issueKey, CancellationToken token)
        {
            var serializerSettings = _jira.RestClient.Settings.JsonSerializerSettings;
            var resource = String.Format("rest/api/2/issue/{0}?fields=issuelinks,created", issueKey);
            var issueLinksResult = await _jira.RestClient.ExecuteRequestAsync(Method.GET, resource, null, token).ConfigureAwait(false);
            var issueLinksJson = issueLinksResult["fields"]["issuelinks"];

            if (issueLinksJson == null)
            {
                throw new InvalidOperationException("There is no 'issueLinks' field on the issue data, make sure issue linking is turned on in JIRA.");
            }

            var issueLinks = issueLinksJson.Cast<JObject>();
            var issuesToGet = issueLinks.Select(issueLink =>
            {
                var issueJson = issueLink["outwardIssue"] ?? issueLink["inwardIssue"];
                return issueJson["key"].Value<string>();
            }).ToList();
            issuesToGet.Add(issueKey);

            var issuesMap = await _jira.Issues.GetIssuesAsync(issuesToGet, token).ConfigureAwait(false);
            var issue = issuesMap[issueKey];
            return issueLinks.Select(issueLink =>
            {
                var linkType = JsonConvert.DeserializeObject<IssueLinkType>(issueLink["type"].ToString(), serializerSettings);
                var outwardIssue = issueLink["outwardIssue"];
                var inwardIssue = issueLink["inwardIssue"];
                var outwardIssueKey = outwardIssue != null ? (string)outwardIssue["key"] : null;
                var inwardIssueKey = inwardIssue != null ? (string)inwardIssue["key"] : null;
                return new IssueLink(
                    linkType,
                    outwardIssueKey == null ? issue : issuesMap[outwardIssueKey],
                    inwardIssueKey == null ? issue : issuesMap[inwardIssueKey]);
            });
        }

        public async Task<IEnumerable<IssueLinkType>> GetLinkTypesAsync(CancellationToken token = default(CancellationToken))
        {
            var cache = _jira.Cache;
            var serializerSettings = _jira.RestClient.Settings.JsonSerializerSettings;

            if (!cache.LinkTypes.Any())
            {
                var results = await _jira.RestClient.ExecuteRequestAsync(Method.GET, "rest/api/2/issueLinkType", null, token).ConfigureAwait(false);
                var linkTypes = results["issueLinkTypes"]
                    .Cast<JObject>()
                    .Select(issueLinkJson => JsonConvert.DeserializeObject<IssueLinkType>(issueLinkJson.ToString(), serializerSettings));

                cache.LinkTypes.TryAdd(linkTypes);
            }

            return cache.LinkTypes.Values;
        }
    }
}

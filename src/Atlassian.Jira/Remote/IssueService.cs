using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Atlassian.Jira.Remote
{
    internal class IssueService : IIssueService
    {
        private const int DEFAULT_MAX_ISSUES_PER_REQUEST = 20;

        private readonly Jira _jira;
        private readonly JiraRestClientSettings _restSettings;

        private JsonSerializerSettings _serializerSettings;

        private class JqlOptions
        {
            public JqlOptions(string jql, CancellationToken token)
            {
                this.Jql = jql;
                this.Token = token;
            }

            public string Jql { get; private set; }
            public CancellationToken Token { get; private set; }
            public int MaxIssuesPerRequest { get; set; } = DEFAULT_MAX_ISSUES_PER_REQUEST;
            public int StartAt { get; set; } = 0;
            public bool ValidateQuery { get; set; } = true;
        }

        public IssueService(Jira jira, JiraRestClientSettings restSettings)
        {
            _jira = jira;
            _restSettings = restSettings;
        }

        public JiraQueryable<Issue> Queryable
        {
            get
            {
                var translator = _jira.Services.Get<IJqlExpressionVisitor>();
                var provider = new JiraQueryProvider(translator, this);
                return new JiraQueryable<Issue>(provider);
            }
        }

        public bool ValidateQuery { get; set; } = true;

        public int MaxIssuesPerRequest { get; set; } = DEFAULT_MAX_ISSUES_PER_REQUEST;

        private async Task<JsonSerializerSettings> GetIssueSerializerSettingsAsync(CancellationToken token)
        {
            if (this._serializerSettings == null)
            {
                var fieldService = _jira.Services.Get<IIssueFieldService>();
                var customFields = await fieldService.GetCustomFieldsAsync(token).ConfigureAwait(false);
                var remoteFields = customFields.Select(f => f.RemoteField);

                var serializers = new Dictionary<string, ICustomFieldValueSerializer>(this._restSettings.CustomFieldSerializers, StringComparer.InvariantCultureIgnoreCase);

                this._serializerSettings = new JsonSerializerSettings();
                this._serializerSettings.NullValueHandling = NullValueHandling.Ignore;
                this._serializerSettings.Converters.Add(new RemoteIssueJsonConverter(remoteFields, serializers));
            }

            return this._serializerSettings;
        }

        public async Task<Issue> GetIssueAsync(string issueKey, CancellationToken token = default(CancellationToken))
        {
            var resource = String.Format("rest/api/2/issue/{0}", issueKey);
            var response = await _jira.RestClient.ExecuteRequestAsync(Method.GET, resource, null, token).ConfigureAwait(false);
            var serializerSettings = await GetIssueSerializerSettingsAsync(token).ConfigureAwait(false);
            var issue = JsonConvert.DeserializeObject<RemoteIssueWrapper>(response.ToString(), serializerSettings);

            return new Issue(_jira, issue.RemoteIssue);
        }

        public Task<IPagedQueryResult<Issue>> GetIssuesFromJqlAsync(string jql, int? maxIssues = default(int?), int startAt = 0, CancellationToken token = default(CancellationToken))
        {
            return GetIssuesFromJqlAsync(new JqlOptions(jql, token)
            {
                MaxIssuesPerRequest = maxIssues ?? this.MaxIssuesPerRequest,
                StartAt = startAt,
                ValidateQuery = this.ValidateQuery
            });
        }

        private async Task<IPagedQueryResult<Issue>> GetIssuesFromJqlAsync(JqlOptions options)
        {
            if (_jira.Debug)
            {
                Trace.WriteLine("[GetFromJqlAsync] JQL: " + options.Jql);
            }

            var parameters = new
            {
                jql = options.Jql,
                startAt = options.StartAt,
                maxResults = options.MaxIssuesPerRequest,
                validateQuery = options.ValidateQuery,
            };

            var result = await _jira.RestClient.ExecuteRequestAsync(Method.POST, "rest/api/2/search", parameters, options.Token).ConfigureAwait(false);
            var serializerSettings = await this.GetIssueSerializerSettingsAsync(options.Token).ConfigureAwait(false);
            var issues = result["issues"]
                .Cast<JObject>()
                .Select(issueJson =>
                {
                    var remoteIssue = JsonConvert.DeserializeObject<RemoteIssueWrapper>(issueJson.ToString(), serializerSettings).RemoteIssue;
                    return new Issue(_jira, remoteIssue);
                });

            return PagedQueryResult<Issue>.FromJson((JObject)result, issues);
        }

        public async Task UpdateIssueAsync(Issue issue, IssueUpdateOptions options, CancellationToken token = default(CancellationToken))
        {
            var resource = String.Format("rest/api/2/issue/{0}", issue.Key.Value);
            if (options.SuppressEmailNotification)
            {
                resource += "?notifyUsers=false";
            }
            var fieldProvider = issue as IRemoteIssueFieldProvider;
            var remoteFields = await fieldProvider.GetRemoteFieldValuesAsync(token).ConfigureAwait(false);
            var remoteIssue = await issue.ToRemoteAsync(token).ConfigureAwait(false);
            var fields = await this.BuildFieldsObjectFromIssueAsync(remoteIssue, remoteFields, token).ConfigureAwait(false);

            await _jira.RestClient.ExecuteRequestAsync(Method.PUT, resource, new { fields = fields }, token).ConfigureAwait(false);
        }

        public Task UpdateIssueAsync(Issue issue, CancellationToken token = default(CancellationToken))
        {
            var options = new IssueUpdateOptions();
            return UpdateIssueAsync(issue, options, token);
        }

        public async Task<string> CreateIssueAsync(Issue issue, CancellationToken token = default(CancellationToken))
        {
            var remoteIssue = await issue.ToRemoteAsync(token).ConfigureAwait(false);
            var remoteIssueWrapper = new RemoteIssueWrapper(remoteIssue, issue.ParentIssueKey);
            var serializerSettings = await this.GetIssueSerializerSettingsAsync(token).ConfigureAwait(false);
            var requestBody = JsonConvert.SerializeObject(remoteIssueWrapper, serializerSettings);

            var result = await _jira.RestClient.ExecuteRequestAsync(Method.POST, "rest/api/2/issue", requestBody, token).ConfigureAwait(false);
            return (string)result["key"];
        }

        private async Task<JObject> BuildFieldsObjectFromIssueAsync(RemoteIssue remoteIssue, RemoteFieldValue[] remoteFields, CancellationToken token)
        {
            var issueWrapper = new RemoteIssueWrapper(remoteIssue);
            var serializerSettings = await this.GetIssueSerializerSettingsAsync(token).ConfigureAwait(false);
            var issueJson = JsonConvert.SerializeObject(issueWrapper, serializerSettings);

            var fieldsJsonSerializerSettings = new JsonSerializerSettings()
            {
                DateParseHandling = DateParseHandling.None
            };

            var issueFields = JsonConvert.DeserializeObject<JObject>(issueJson, fieldsJsonSerializerSettings)["fields"] as JObject;
            var updateFields = new JObject();

            foreach (var field in remoteFields)
            {
                var issueFieldName = field.id;
                var issueFieldValue = issueFields[issueFieldName];

                if (issueFieldValue == null && issueFieldName.Equals("components", StringComparison.OrdinalIgnoreCase))
                {
                    // JIRA does not accept 'null' as a valid value for the 'components' field.
                    //   So if the components field has been cleared it must be set to empty array instead.
                    issueFieldValue = new JArray();
                }

                updateFields.Add(issueFieldName, issueFieldValue);
            }

            return updateFields;
        }

        public async Task ExecuteWorkflowActionAsync(Issue issue, string actionName, WorkflowTransitionUpdates updates, CancellationToken token = default(CancellationToken))
        {
            var actions = await this.GetActionsAsync(issue.Key.Value, token).ConfigureAwait(false);
            var action = actions.FirstOrDefault(a => a.Name.Equals(actionName, StringComparison.OrdinalIgnoreCase));

            if (action == null)
            {
                throw new InvalidOperationException(String.Format("Workflow action with name '{0}' not found.", actionName));
            }

            updates = updates ?? new WorkflowTransitionUpdates();

            var resource = String.Format("rest/api/2/issue/{0}/transitions", issue.Key.Value);
            var fieldProvider = issue as IRemoteIssueFieldProvider;
            var remoteFields = await fieldProvider.GetRemoteFieldValuesAsync(token).ConfigureAwait(false);
            var remoteIssue = await issue.ToRemoteAsync(token).ConfigureAwait(false);
            var fields = await BuildFieldsObjectFromIssueAsync(remoteIssue, remoteFields, token).ConfigureAwait(false);
            var updatesObject = new JObject();

            if (!String.IsNullOrEmpty(updates.Comment))
            {
                updatesObject.Add("comment", new JArray(new JObject[]
                {
                    new JObject(new JProperty("add",
                        new JObject(new JProperty("body", updates.Comment))))
                }));
            }

            var requestBody = new
            {
                transition = new
                {
                    id = action.Id
                },
                update = updatesObject,
                fields = fields
            };

            await _jira.RestClient.ExecuteRequestAsync(Method.POST, resource, requestBody, token).ConfigureAwait(false);
        }

        public async Task<IssueTimeTrackingData> GetTimeTrackingDataAsync(string issueKey, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(issueKey))
            {
                throw new InvalidOperationException("Unable to retrieve time tracking data, make sure the issue has been created.");
            }

            var resource = String.Format("rest/api/2/issue/{0}?fields=timetracking", issueKey);
            var response = await _jira.RestClient.ExecuteRequestAsync(Method.GET, resource, null, token).ConfigureAwait(false);

            var serializerSettings = _jira.RestClient.Settings.JsonSerializerSettings;
            var timeTrackingJson = response["fields"]["timetracking"];
            return JsonConvert.DeserializeObject<IssueTimeTrackingData>(timeTrackingJson.ToString(), serializerSettings);
        }

        public async Task<IDictionary<string, IssueFieldEditMetadata>> GetFieldsEditMetadataAsync(string issueKey, CancellationToken token = default(CancellationToken))
        {
            var dict = new Dictionary<string, IssueFieldEditMetadata>();
            var resource = String.Format("rest/api/2/issue/{0}/editmeta", issueKey);
            var serializer = JsonSerializer.Create(_jira.RestClient.Settings.JsonSerializerSettings);
            var result = await _jira.RestClient.ExecuteRequestAsync(Method.GET, resource, null, token).ConfigureAwait(false);
            JObject fields = result["fields"].Value<JObject>();

            foreach (var prop in fields.Properties())
            {
                var fieldName = (prop.Value["name"] ?? prop.Name).ToString();
                dict.Add(fieldName, prop.Value.ToObject<IssueFieldEditMetadata>(serializer));
            }

            return dict;
        }

        public async Task<Comment> AddCommentAsync(string issueKey, Comment comment, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(comment.Author))
            {
                throw new InvalidOperationException("Unable to add comment due to missing author field.");
            }

            var resource = String.Format("rest/api/2/issue/{0}/comment", issueKey);
            var remoteComment = await _jira.RestClient.ExecuteRequestAsync<RemoteComment>(Method.POST, resource, comment.toRemote(), token).ConfigureAwait(false);
            return new Comment(remoteComment);
        }

        public async Task<IPagedQueryResult<Comment>> GetPagedCommentsAsync(string issueKey, int? maxComments = default(int?), int startAt = 0, CancellationToken token = default(CancellationToken))
        {
            var resource = $"rest/api/2/issue/{issueKey}/comment?startAt={startAt}";

            if (maxComments.HasValue)
            {
                resource += $"&maxResults={maxComments.Value}";
            }

            var result = await _jira.RestClient.ExecuteRequestAsync(Method.GET, resource).ConfigureAwait(false);
            var serializerSettings = _jira.RestClient.Settings.JsonSerializerSettings;
            var comments = result["comments"]
                .Cast<JObject>()
                .Select(commentJson =>
                {
                    var remoteComment = JsonConvert.DeserializeObject<RemoteComment>(commentJson.ToString(), serializerSettings);
                    return new Comment(remoteComment);
                });

            return PagedQueryResult<Comment>.FromJson((JObject)result, comments);
        }

        public async Task<IEnumerable<JiraNamedEntity>> GetActionsAsync(string issueKey, CancellationToken token = default(CancellationToken))
        {
            var resource = String.Format("rest/api/2/issue/{0}/transitions", issueKey);
            var serializerSettings = _jira.RestClient.Settings.JsonSerializerSettings;
            var result = await _jira.RestClient.ExecuteRequestAsync(Method.GET, resource, null, token).ConfigureAwait(false);
            var remoteTransitions = JsonConvert.DeserializeObject<RemoteNamedObject[]>(result["transitions"].ToString(), serializerSettings);

            return remoteTransitions.Select(transition => new JiraNamedEntity(transition));
        }

        public async Task<IEnumerable<Attachment>> GetAttachmentsAsync(string issueKey, CancellationToken token = default(CancellationToken))
        {
            var resource = String.Format("rest/api/2/issue/{0}?fields=attachment", issueKey);
            var serializerSettings = _jira.RestClient.Settings.JsonSerializerSettings;
            var result = await _jira.RestClient.ExecuteRequestAsync(Method.GET, resource, null, token).ConfigureAwait(false);
            var attachmentsJson = result["fields"]["attachment"];
            var attachments = JsonConvert.DeserializeObject<RemoteAttachment[]>(attachmentsJson.ToString(), serializerSettings);

            return attachments.Select(remoteAttachment => new Attachment(_jira, new WebClientWrapper(_jira), remoteAttachment));
        }

        public async Task<string[]> GetLabelsAsync(string issueKey, CancellationToken token = default(CancellationToken))
        {
            var resource = String.Format("rest/api/2/issue/{0}?fields=labels", issueKey);
            var serializerSettings = await this.GetIssueSerializerSettingsAsync(token).ConfigureAwait(false);
            var response = await _jira.RestClient.ExecuteRequestAsync(Method.GET, resource).ConfigureAwait(false);
            var issue = JsonConvert.DeserializeObject<RemoteIssueWrapper>(response.ToString(), serializerSettings);
            return issue.RemoteIssue.labels ?? new string[0];
        }

        public Task SetLabelsAsync(string issueKey, string[] labels, CancellationToken token = default(CancellationToken))
        {
            var resource = String.Format("rest/api/2/issue/{0}", issueKey);
            return _jira.RestClient.ExecuteRequestAsync(Method.PUT, resource, new
            {
                fields = new
                {
                    labels = labels
                }

            }, token);
        }

        public async Task<IEnumerable<JiraUser>> GetWatchersAsync(string issueKey, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(issueKey))
            {
                throw new InvalidOperationException("Unable to interact with the watchers resource, make sure the issue has been created.");
            }

            var resourceUrl = String.Format("rest/api/2/issue/{0}/watchers", issueKey);
            var serializerSettings = _jira.RestClient.Settings.JsonSerializerSettings;
            var result = await _jira.RestClient.ExecuteRequestAsync(Method.GET, resourceUrl, null, token).ConfigureAwait(false);
            var watchersJson = result["watchers"];
            return watchersJson.Select(watcherJson => JsonConvert.DeserializeObject<JiraUser>(watcherJson.ToString(), serializerSettings));
        }

        public async Task<IEnumerable<IssueChangeLog>> GetChangeLogsAsync(string issueKey, CancellationToken token = default(CancellationToken))
        {
            var resourceUrl = String.Format("rest/api/2/issue/{0}?fields=created&expand=changelog", issueKey);
            var serializerSettings = _jira.RestClient.Settings.JsonSerializerSettings;
            var response = await _jira.RestClient.ExecuteRequestAsync(Method.GET, resourceUrl, null, token).ConfigureAwait(false);
            var result = Enumerable.Empty<IssueChangeLog>();
            var changeLogs = response["changelog"];
            if (changeLogs != null)
            {
                var histories = changeLogs["histories"];
                if (histories != null)
                {
                    result = histories.Select(history => JsonConvert.DeserializeObject<IssueChangeLog>(history.ToString(), serializerSettings));
                }
            }

            return result;
        }

        public Task DeleteWatcherAsync(string issueKey, string username, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(issueKey))
            {
                throw new InvalidOperationException("Unable to interact with the watchers resource, make sure the issue has been created.");
            }

            var resourceUrl = String.Format("rest/api/2/issue/{0}/watchers?username={1}", issueKey, System.Uri.EscapeDataString(username));
            return _jira.RestClient.ExecuteRequestAsync(Method.DELETE, resourceUrl, null, token);
        }

        public Task AddWatcherAsync(string issueKey, string username, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(issueKey))
            {
                throw new InvalidOperationException("Unable to interact with the watchers resource, make sure the issue has been created.");
            }

            var requestBody = String.Format("\"{0}\"", username);
            var resourceUrl = String.Format("rest/api/2/issue/{0}/watchers", issueKey);
            return _jira.RestClient.ExecuteRequestAsync(Method.POST, resourceUrl, requestBody, token);
        }

        public Task<IPagedQueryResult<Issue>> GetSubTasksAsync(string issueKey, int? maxIssues = default(int?), int startAt = 0, CancellationToken token = default(CancellationToken))
        {
            var jql = String.Format("parent = {0}", issueKey);
            return GetIssuesFromJqlAsync(jql, maxIssues, startAt, token);
        }

        public Task AddAttachmentsAsync(string issueKey, UploadAttachmentInfo[] attachments, CancellationToken token = default(CancellationToken))
        {
            var resource = String.Format("rest/api/2/issue/{0}/attachments", issueKey);
            var request = new RestRequest();
            request.Method = Method.POST;
            request.Resource = resource;
            request.AddHeader("X-Atlassian-Token", "nocheck");
            request.AlwaysMultipartFormData = true;

            foreach (var attachment in attachments)
            {
                request.AddFile("file", attachment.Data, attachment.Name);
            }

            return _jira.RestClient.ExecuteRequestAsync(request, token);
        }

        public Task DeleteAttachmentAsync(string issueKey, string attachmentId, CancellationToken token = default(CancellationToken))
        {
            var resource = String.Format("rest/api/2/attachment/{0}", attachmentId);

            return _jira.RestClient.ExecuteRequestAsync(Method.DELETE, resource, null, token);
        }

        public async Task<IDictionary<string, Issue>> GetIssuesAsync(IEnumerable<string> issueKeys, CancellationToken token = default(CancellationToken))
        {
            if (issueKeys.Any())
            {
                var distinctKeys = issueKeys.Distinct();
                var jql = String.Format("key in ({0})", String.Join(",", distinctKeys));
                var result = await this.GetIssuesFromJqlAsync(new JqlOptions(jql, token)
                {
                    MaxIssuesPerRequest = distinctKeys.Count(),
                    ValidateQuery = false
                }).ConfigureAwait(false);
                return result.ToDictionary<Issue, string>(i => i.Key.Value);
            }
            else
            {
                return new Dictionary<string, Issue>();
            }
        }

        public Task<IDictionary<string, Issue>> GetIssuesAsync(params string[] issueKeys)
        {
            return this.GetIssuesAsync(issueKeys, default(CancellationToken));
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync(string issueKey, CancellationToken token = default(CancellationToken))
        {
            var resource = String.Format("rest/api/2/issue/{0}/comment?expand=properties", issueKey);
            var serializerSettings = _jira.RestClient.Settings.JsonSerializerSettings;
            var issueJson = await _jira.RestClient.ExecuteRequestAsync(Method.GET, resource, null, token).ConfigureAwait(false);
            var commentJson = issueJson["comments"];

            var remoteComments = JsonConvert.DeserializeObject<RemoteComment[]>(commentJson.ToString(), serializerSettings);

            return remoteComments.Select(c => new Comment(c));
        }

        public Task DeleteCommentAsync(string issueKey, string commentId, CancellationToken token = default(CancellationToken))
        {
            var resource = String.Format("rest/api/2/issue/{0}/comment/{1}", issueKey, commentId);

            return _jira.RestClient.ExecuteRequestAsync(Method.DELETE, resource, null, token);
        }

        public async Task<Worklog> AddWorklogAsync(string issueKey, Worklog worklog, WorklogStrategy worklogStrategy = WorklogStrategy.AutoAdjustRemainingEstimate, string newEstimate = null, CancellationToken token = default(CancellationToken))
        {
            var remoteWorklog = worklog.ToRemote();
            string queryString = null;

            if (worklogStrategy == WorklogStrategy.RetainRemainingEstimate)
            {
                queryString = "adjustEstimate=leave";
            }
            else if (worklogStrategy == WorklogStrategy.NewRemainingEstimate)
            {
                queryString = "adjustEstimate=new&newEstimate=" + Uri.EscapeDataString(newEstimate);
            }

            var resource = String.Format("rest/api/2/issue/{0}/worklog?{1}", issueKey, queryString);
            var serverWorklog = await _jira.RestClient.ExecuteRequestAsync<RemoteWorklog>(Method.POST, resource, remoteWorklog, token).ConfigureAwait(false);
            return new Worklog(serverWorklog);
        }

        public Task DeleteWorklogAsync(string issueKey, string worklogId, WorklogStrategy worklogStrategy = WorklogStrategy.AutoAdjustRemainingEstimate, string newEstimate = null, CancellationToken token = default(CancellationToken))
        {
            string queryString = null;

            if (worklogStrategy == WorklogStrategy.RetainRemainingEstimate)
            {
                queryString = "adjustEstimate=leave";
            }
            else if (worklogStrategy == WorklogStrategy.NewRemainingEstimate)
            {
                queryString = "adjustEstimate=new&newEstimate=" + Uri.EscapeDataString(newEstimate);
            }

            var resource = String.Format("rest/api/2/issue/{0}/worklog/{1}?{2}", issueKey, worklogId, queryString);
            return _jira.RestClient.ExecuteRequestAsync(Method.DELETE, resource, null, token);
        }

        public async Task<IEnumerable<Worklog>> GetWorklogsAsync(string issueKey, CancellationToken token = default(CancellationToken))
        {
            var resource = String.Format("rest/api/2/issue/{0}/worklog", issueKey);
            var serializerSettings = _jira.RestClient.Settings.JsonSerializerSettings;
            var response = await _jira.RestClient.ExecuteRequestAsync(Method.GET, resource, null, token).ConfigureAwait(false);
            var worklogsJson = response["worklogs"];
            var remoteWorklogs = JsonConvert.DeserializeObject<RemoteWorklog[]>(worklogsJson.ToString(), serializerSettings);

            return remoteWorklogs.Select(w => new Worklog(w));
        }

        public async Task<Worklog> GetWorklogAsync(string issueKey, string worklogId, CancellationToken token = default(CancellationToken))
        {
            var resource = String.Format("rest/api/2/issue/{0}/worklog/{1}", issueKey, worklogId);
            var remoteWorklog = await _jira.RestClient.ExecuteRequestAsync<RemoteWorklog>(Method.GET, resource, null, token).ConfigureAwait(false);
            return new Worklog(remoteWorklog);
        }

        public Task DeleteIssueAsync(string issueKey, CancellationToken token = default(CancellationToken))
        {
            var resource = String.Format("rest/api/2/issue/{0}", issueKey);
            return _jira.RestClient.ExecuteRequestAsync(Method.DELETE, resource, null, token);
        }
    }
}

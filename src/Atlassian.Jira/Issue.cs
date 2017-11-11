using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira.Linq;
using Atlassian.Jira.Remote;

namespace Atlassian.Jira
{
    /// <summary>
    /// A JIRA issue
    /// </summary>
    public class Issue : IRemoteIssueFieldProvider
    {
        private readonly Jira _jira;

        private ComparableString _key;
        private string _project;
        private RemoteIssue _originalIssue;
        private DateTime? _createDate;
        private DateTime? _updateDate;
        private DateTime? _dueDate;
        private DateTime? _resolutionDate;
        private IssueSecurityLevel _securityLevel;
        private ProjectVersionCollection _affectsVersions = null;
        private ProjectVersionCollection _fixVersions = null;
        private ProjectComponentCollection _components = null;
        private CustomFieldValueCollection _customFields = null;
        private IssueLabelCollection _labels = null;
        private IssueStatus _status;
        private string _parentIssueKey;

        /// <summary>
        /// Creates a new Issue.
        /// </summary>
        /// <param name="jira">Jira instance that owns this issue.</param>
        /// <param name="projectKey">Project key that owns this issue.</param>
        /// <param name="parentIssueKey">If provided, marks this issue as a subtask of the given parent issue.</param>
        public Issue(Jira jira, string projectKey, string parentIssueKey = null)
            : this(jira, new RemoteIssue() { project = projectKey }, parentIssueKey)
        {
        }

        /// <summary>
        /// Creates a new Issue from a remote issue.
        /// </summary>
        /// <param name="jira">The Jira instance that owns this issue.</param>
        /// <param name="remoteIssue">The remote issue object.</param>
        /// <param name="parentIssueKey">If provided, marks this issue as a subtask of the given parent issue.</param>
        public Issue(Jira jira, RemoteIssue remoteIssue, string parentIssueKey = null)
        {
            _jira = jira;
            _parentIssueKey = parentIssueKey;
            Initialize(remoteIssue);
        }

        private void Initialize(RemoteIssue remoteIssue)
        {
            _originalIssue = remoteIssue;

            _project = remoteIssue.project;
            _key = remoteIssue.key;
            _createDate = remoteIssue.created;
            _dueDate = remoteIssue.duedate;
            _updateDate = remoteIssue.updated;
            _resolutionDate = remoteIssue.resolutionDateReadOnly;
            _securityLevel = remoteIssue.securityLevelReadOnly;

            Assignee = remoteIssue.assignee;
            Description = remoteIssue.description;
            Environment = remoteIssue.environment;
            Reporter = remoteIssue.reporter;
            Summary = remoteIssue.summary;
            Votes = remoteIssue.votesData?.votes;
            HasUserVoted = remoteIssue.votesData != null ? remoteIssue.votesData.hasVoted : false;

            if (!String.IsNullOrEmpty(remoteIssue.parentKey))
            {
                _parentIssueKey = remoteIssue.parentKey;
            }

            // named entities
            _status = remoteIssue.status == null ? null : new IssueStatus(remoteIssue.status);
            Priority = remoteIssue.priority == null ? null : new IssuePriority(remoteIssue.priority);
            Resolution = remoteIssue.resolution == null ? null : new IssueResolution(remoteIssue.resolution);
            Type = remoteIssue.type == null ? null : new IssueType(remoteIssue.type);

            // collections
            _customFields = _originalIssue.customFieldValues == null ? new CustomFieldValueCollection(this)
                : new CustomFieldValueCollection(this, _originalIssue.customFieldValues.Select(f => new CustomFieldValue(f.customfieldId, this) { Values = f.values }).ToList());

            var affectsVersions = _originalIssue.affectsVersions ?? Enumerable.Empty<RemoteVersion>();
            _affectsVersions = new ProjectVersionCollection("versions", _jira, Project, affectsVersions.Select(v =>
            {
                v.ProjectKey = _originalIssue.project;
                return new ProjectVersion(_jira, v);
            }).ToList());

            var fixVersions = _originalIssue.fixVersions ?? Enumerable.Empty<RemoteVersion>();
            _fixVersions = new ProjectVersionCollection("fixVersions", _jira, Project, fixVersions.Select(v =>
            {
                v.ProjectKey = _originalIssue.project;
                return new ProjectVersion(_jira, v);
            }).ToList());

            var labels = _originalIssue.labels ?? new string[0];
            _labels = new IssueLabelCollection(labels.ToList());

            var components = _originalIssue.components ?? Enumerable.Empty<RemoteComponent>();
            _components = new ProjectComponentCollection("components", _jira, Project, components.Select(c =>
            {
                c.ProjectKey = _originalIssue.project;
                return new ProjectComponent(c);
            }).ToList());
        }

        internal RemoteIssue OriginalRemoteIssue
        {
            get
            {
                return this._originalIssue;
            }
        }

        /// <summary>
        /// The parent key if this issue is a subtask.
        /// </summary>
        /// <remarks>
        /// Only available if issue was retrieved using REST API.
        /// </remarks>
        public string ParentIssueKey
        {
            get { return _parentIssueKey; }
        }

        /// <summary>
        /// The JIRA server that created this issue
        /// </summary>
        public Jira Jira
        {
            get
            {
                return _jira;
            }
        }

        /// <summary>
        /// Gets the security level set on the issue.
        /// </summary>
        public IssueSecurityLevel SecurityLevel
        {
            get
            {
                return _securityLevel;
            }
        }

        /// <summary>
        /// Brief one-line summary of the issue
        /// </summary>
        [JqlContainsEquality]
        public string Summary { get; set; }

        /// <summary>
        /// Detailed description of the issue
        /// </summary>
        [JqlContainsEquality]
        public string Description { get; set; }

        /// <summary>
        /// Hardware or software environment to which the issue relates
        /// </summary>
        [JqlContainsEquality]
        public string Environment { get; set; }

        /// <summary>
        /// Person to whom the issue is currently assigned
        /// </summary>
        public string Assignee { get; set; }

        /// <summary>
        /// Gets the internal identifier assigned by JIRA.
        /// </summary>
        public string JiraIdentifier
        {
            get
            {
                if (String.IsNullOrEmpty(this._originalIssue.key))
                {
                    throw new InvalidOperationException("Unable to retrieve JIRA id, issue has not been created.");
                }

                return this._originalIssue.id;
            }
        }

        /// <summary>
        /// Unique identifier for this issue
        /// </summary>
        public ComparableString Key
        {
            get
            {
                return _key;
            }
        }

        /// <summary>
        /// Importance of the issue in relation to other issues
        /// </summary>
        public IssuePriority Priority { get; set; }

        /// <summary>
        /// Parent project to which the issue belongs
        /// </summary>
        public string Project
        {
            get
            {
                return _project;
            }
        }

        /// <summary>
        /// Person who entered the issue into the system
        /// </summary>
        public string Reporter { get; set; }

        /// <summary>
        /// Record of the issue's resolution, if the issue has been resolved or closed
        /// </summary>
        public IssueResolution Resolution { get; set; }

        /// <summary>
        /// The stage the issue is currently at in its lifecycle.
        /// </summary>
        public IssueStatus Status
        {
            get
            {
                return _status;
            }
        }

        /// <summary>
        /// The type of the issue
        /// </summary>
        [RemoteFieldName("issuetype")]
        public IssueType Type { get; set; }

        /// <summary>
        /// Number of votes the issue has
        /// </summary>
        public long? Votes { get; private set; }

        /// <summary>
        /// Whether the user that retrieved this issue has voted on it.
        /// </summary>
        public bool HasUserVoted { get; private set; }

        /// <summary>
        /// Time and date on which this issue was entered into JIRA
        /// </summary>
        public DateTime? Created
        {
            get
            {
                return _createDate;
            }
        }

        /// <summary>
        /// Date by which this issue is scheduled to be completed
        /// </summary>
        public DateTime? DueDate
        {
            get
            {
                return _dueDate;
            }
            set
            {
                _dueDate = value;
            }
        }

        /// <summary>
        /// Time and date on which this issue was last edited
        /// </summary>
        public DateTime? Updated
        {
            get
            {
                return _updateDate;
            }
        }

        /// <summary>
        /// Time and date on which this issue was resolved.
        /// </summary>
        /// <remarks>
        /// Only available if issue was retrieved using REST API, use GetResolutionDate
        /// method for SOAP clients.
        /// </remarks>
        public DateTime? ResolutionDate
        {
            get
            {
                return _resolutionDate;
            }
        }

        /// <summary>
        /// The components associated with this issue
        /// </summary>
        [JqlFieldName("component")]
        public ProjectComponentCollection Components
        {
            get
            {
                return _components;
            }
        }

        /// <summary>
        /// The versions that are affected by this issue
        /// </summary>
        [JqlFieldName("AffectedVersion")]
        public ProjectVersionCollection AffectsVersions
        {
            get
            {
                return _affectsVersions;
            }
        }

        /// <summary>
        /// The versions in which this issue is fixed
        /// </summary>
        [JqlFieldName("FixVersion")]
        public ProjectVersionCollection FixVersions
        {
            get
            {
                return _fixVersions;
            }
        }

        /// <summary>
        /// The labels assigned to this issue.
        /// </summary>
        public IssueLabelCollection Labels
        {
            get
            {
                return _labels;
            }
        }

        /// <summary>
        /// The custom fields associated with this issue
        /// </summary>
        public CustomFieldValueCollection CustomFields
        {
            get
            {
                return _customFields;
            }
        }

        /// <summary>
        /// Gets or sets the value of a custom field
        /// </summary>
        /// <param name="customFieldName">Custom field name</param>
        /// <returns>Value of the custom field</returns>
        public ComparableString this[string customFieldName]
        {
            get
            {
                var customField = _customFields[customFieldName];

                if (customField != null && customField.Values != null && customField.Values.Count() > 0)
                {
                    return customField.Values[0];
                }
                return null;
            }
            set
            {
                var customField = _customFields[customFieldName];
                string[] customFieldValue = value == null ? null : new string[] { value.Value };

                if (customField != null)
                {
                    customField.Values = customFieldValue;
                }
                else
                {
                    _customFields.Add(customFieldName, customFieldValue);
                }
            }
        }

        /// <summary>
        /// Saves field changes to server.
        /// </summary>
        public void SaveChanges()
        {
            Issue serverIssue = null;
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                var newKey = _jira.Issues.CreateIssueAsync(this).Result;
                serverIssue = _jira.Issues.GetIssueAsync(newKey).Result;
            }
            else
            {
                _jira.Issues.UpdateIssueAsync(this).Wait();
                serverIssue = _jira.Issues.GetIssueAsync(_originalIssue.key).Result;
            }

            Initialize(serverIssue.OriginalRemoteIssue);
        }

        /// <summary>
        /// Saves field changes to server.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public async Task<Issue> SaveChangesAsync(CancellationToken token = default(CancellationToken))
        {
            Issue serverIssue;
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                var newKey = await _jira.Issues.CreateIssueAsync(this, token).ConfigureAwait(false);
                serverIssue = await _jira.Issues.GetIssueAsync(newKey, token).ConfigureAwait(false);
            }
            else
            {
                await _jira.Issues.UpdateIssueAsync(this, token).ConfigureAwait(false);
                serverIssue = await _jira.Issues.GetIssueAsync(_originalIssue.key, token).ConfigureAwait(false);
            }

            Initialize(serverIssue.OriginalRemoteIssue);
            return serverIssue;
        }

        /// <summary>
        /// Creates a link between this issue and the issue specified.
        /// </summary>
        /// <param name="inwardIssueKey">Key of the issue to link.</param>
        /// <param name="linkName">Name of the issue link type.</param>
        /// <param name="comment">Comment to add to this issue.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task LinkToIssueAsync(string inwardIssueKey, string linkName, string comment = null, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to link issue, issue has not been created.");
            }

            return this.Jira.Links.CreateLinkAsync(this.Key.Value, inwardIssueKey, linkName, comment, token);
        }

        /// <summary>
        /// Gets the issue links associated with this issue.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IEnumerable<IssueLink>> GetIssueLinksAsync(CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to get issue links issues, issue has not been created.");
            }

            return this.Jira.Links.GetLinksForIssueAsync(_originalIssue.key, token);
        }

        /// <summary>
        /// Creates an remote link for an issue.
        /// </summary>
        /// <param name="remoteUrl">Remote url to link to.</param>
        /// <param name="title">Title of the remote link.</param>
        /// <param name="summary">Summary of the remote link.</param>
        public Task AddRemoteLinkAsync(string remoteUrl, string title, string summary = null)
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to add remote link, issue has not been created.");
            }

            return this.Jira.RemoteLinks.CreateRemoteLinkAsync(this.Key.Value, remoteUrl, title, summary);
        }

        /// <summary>
        /// Gets the remote links associated with this issue.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IEnumerable<IssueRemoteLink>> GetRemoteLinksAsync(CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to get remote links, issue has not been created.");
            }

            return this.Jira.RemoteLinks.GetRemoteLinksForIssueAsync(_originalIssue.key, token);
        }

        /// <summary>
        /// Transition an issue through a workflow action.
        /// </summary>
        /// <param name="actionName">The workflow action to transition to.</param>
        /// <param name="additionalUpdates">Additional updates to perform when transitioning the issue.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public async Task WorkflowTransitionAsync(string actionName, WorkflowTransitionUpdates additionalUpdates = null, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to execute workflow transition, issue has not been created.");
            }

            await _jira.Issues.ExecuteWorkflowActionAsync(this, actionName, additionalUpdates, token).ConfigureAwait(false);
            var issue = await _jira.Issues.GetIssueAsync(_originalIssue.key, token).ConfigureAwait(false);
            Initialize(issue.OriginalRemoteIssue);
        }

        /// <summary>
        /// Returns the issues that are marked as sub tasks of this issue.
        /// </summary>
        /// <param name="maxIssues">Maximum number of issues to retrieve.</param>
        /// <param name="startAt">Index of the first issue to return (0-based).</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IPagedQueryResult<Issue>> GetSubTasksAsync(int? maxIssues = null, int startAt = 0, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to retrieve subtasks from server, issue has not been created.");
            }

            return _jira.Issues.GetSubTasksAsync(_originalIssue.key, maxIssues, startAt, token);
        }

        /// <summary>
        /// Retrieve attachment metadata from server for this issue
        /// </summary>
        public Task<IEnumerable<Attachment>> GetAttachmentsAsync(CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to retrieve attachments from server, issue has not been created.");
            }

            return this.Jira.Issues.GetAttachmentsAsync(this.Key.Value, token);
        }

        /// <summary>
        /// Add one or more attachments to this issue
        /// </summary>
        /// <param name="filePaths">Full paths of files to upload</param>
        public void AddAttachment(params string[] filePaths)
        {
            var attachments = filePaths.Select(f => new UploadAttachmentInfo(Path.GetFileName(f), _jira.FileSystem.FileReadAllBytes(f))).ToArray();

            AddAttachment(attachments);
        }

        /// <summary>
        /// Add an attachment to this issue
        /// </summary>
        /// <param name="name">Attachment name with extension</param>
        /// <param name="data">Attachment data</param>
        public void AddAttachment(string name, byte[] data)
        {
            AddAttachment(new UploadAttachmentInfo(name, data));
        }

        /// <summary>
        /// Add one or more attachments to this issue.
        /// </summary>
        /// <param name="attachments">Attachment objects that describe the files to upload.</param>
        public void AddAttachment(params UploadAttachmentInfo[] attachments)
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to upload attachments to server, issue has not been created.");
            }

            AddAttachmentAsync(attachments).Wait();
        }

        /// <summary>
        /// Add one or more attachments to this issue.
        /// </summary>
        /// <param name="attachments">Attachment objects that describe the files to upload.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task AddAttachmentAsync(UploadAttachmentInfo[] attachments, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to upload attachments to server, issue has not been created.");
            }

            return _jira.Issues.AddAttachmentsAsync(_originalIssue.key, attachments, token);
        }

        /// <summary>
        /// Removes an attachment from this issue.
        /// </summary>
        /// <param name="attachment">Attachment to remove.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task DeleteAttachmentAsync(Attachment attachment, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to delete attachment from server, issue has not been created.");
            }

            return _jira.Issues.DeleteAttachmentAsync(_originalIssue.key, attachment.Id, token);
        }

        /// <summary>
        /// Gets a dictionary with issue field names as keys and their metadata as values.
        /// </summary>
        public Task<IDictionary<String, IssueFieldEditMetadata>> GetIssueFieldsEditMetadataAsync(CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to retrieve issue fields from server, make sure the issue has been created.");
            }

            return _jira.Issues.GetFieldsEditMetadataAsync(_originalIssue.key, token);
        }

        /// <summary>
        /// Retrieve change logs from server for this issue.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IEnumerable<IssueChangeLog>> GetChangeLogsAsync(CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to retrieve change logs from server, issue has not been created.");
            }

            return _jira.Issues.GetChangeLogsAsync(_originalIssue.key, token);
        }

        /// <summary>
        /// Get the comments for this issue.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IEnumerable<Comment>> GetCommentsAsync(CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to retrieve comments from server, issue has not been created.");
            }

            return _jira.Issues.GetCommentsAsync(_originalIssue.key, token);
        }

        /// <summary>
        /// Get the comments for this issue.
        /// </summary>
        /// <param name="maxComments">Maximum number of comments to retrieve.</param>
        /// <param name="startAt">Index of the first comment to return (0-based).</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IPagedQueryResult<Comment>> GetPagedCommentsAsync(int? maxComments = null, int startAt = 0, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to retrieve comments from server, issue has not been created.");
            }

            return this.Jira.Issues.GetPagedCommentsAsync(this.Key.Value, maxComments, startAt, token);
        }

        /// <summary>
        /// Add a comment to this issue.
        /// </summary>
        /// <param name="comment">Comment text to add.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task AddCommentAsync(string comment, CancellationToken token = default(CancellationToken))
        {
            var credentials = _jira.GetCredentials();
            return this.AddCommentAsync(new Comment() { Author = credentials.UserName, Body = comment }, token);
        }

        /// <summary>
        /// Removes a comment from this issue.
        /// </summary>
        /// <param name="comment">Comment to remove.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task DeleteCommentAsync(Comment comment, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to delete comment from server, issue has not been created.");
            }

            return _jira.Issues.DeleteCommentAsync(_originalIssue.key, comment.Id, token);
        }

        /// <summary>
        /// Add a comment to this issue.
        /// </summary>
        /// <param name="comment">Comment object to add.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task AddCommentAsync(Comment comment, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to add comment to issue, issue has not been created.");
            }

            return this.Jira.Issues.AddCommentAsync(this.Key.Value, comment, token);
        }

        /// <summary>
        /// Retrieve the labels from server for this issue.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        [Obsolete("Use Issue.Labels instead.")]
        public Task<string[]> GetLabelsAsync(CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to get labels from issue, issue has not been created.");
            }

            return Jira.Issues.GetLabelsAsync(_originalIssue.key, token);
        }

        /// <summary>
        /// Sets the labels of this issue.
        /// </summary>
        /// <param name="labels">The list of labels to set on the issue</param>
        [Obsolete("Modify the Issue.Labels collection and call Issue.SaveChanges to update the labels field.")]
        public Task SetLabelsAsync(params string[] labels)
        {
            return SetLabelsAsync(labels, CancellationToken.None);
        }

        /// <summary>
        /// Sets the labels of this issue.
        /// </summary>
        /// <param name="labels">The list of labels to set on the issue</param>
        /// <param name="token">Cancellation token for this operation.</param>
        [Obsolete("Modify the Issue.Labels collection and call Issue.SaveChanges to update the labels field.")]
        public Task SetLabelsAsync(string[] labels, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to add label to issue, issue has not been created.");
            }

            return _jira.Issues.SetLabelsAsync(_originalIssue.key, labels, token);
        }

        /// <summary>
        ///  Adds a worklog to this issue.
        /// </summary>
        /// <param name="timespent">Specifies a time duration in JIRA duration format, representing the time spent working on the worklog</param>
        /// <param name="worklogStrategy">How to handle the remaining estimate, defaults to AutoAdjustRemainingEstimate</param>
        /// <param name="newEstimate">New estimate (only used if worklogStrategy set to NewRemainingEstimate)</param>
        /// <returns>Worklog as constructed by server</returns>
        public Task<Worklog> AddWorklogAsync(string timespent,
                                 WorklogStrategy worklogStrategy = WorklogStrategy.AutoAdjustRemainingEstimate,
                                 string newEstimate = null,
                                 CancellationToken token = default(CancellationToken))
        {
            // todo: Use the CancellationToken Parameter
            return AddWorklogAsync(new Worklog(timespent, DateTime.Now), worklogStrategy, newEstimate);
        }

        /// <summary>
        ///  Adds a worklog to this issue.
        /// </summary>
        /// <param name="worklog">The worklog instance to add</param>
        /// <param name="worklogStrategy">How to handle the remaining estimate, defaults to AutoAdjustRemainingEstimate</param>
        /// <param name="newEstimate">New estimate (only used if worklogStrategy set to NewRemainingEstimate)</param>
        /// <param name="token">Cancellation token for this operation.</param>
        /// <returns>Worklog as constructed by server</returns>
        public Task<Worklog> AddWorklogAsync(Worklog worklog,
                                  WorklogStrategy worklogStrategy = WorklogStrategy.AutoAdjustRemainingEstimate,
                                  string newEstimate = null,
                                  CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to add worklog to issue, issue has not been saved to server.");
            }

            return _jira.Issues.AddWorklogAsync(_originalIssue.key, worklog, worklogStrategy, newEstimate, token);
        }

        /// <summary>
        /// Deletes the given worklog from the issue and updates the remaining estimate field.
        /// </summary>
        /// <param name="worklog">The worklog to remove.</param>
        /// <param name="worklogStrategy">How to handle the remaining estimate, defaults to AutoAdjustRemainingEstimate.</param>
        /// <param name="newEstimate">New estimate (only used if worklogStrategy set to NewRemainingEstimate)</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task DeleteWorklogAsync(Worklog worklog, WorklogStrategy worklogStrategy = WorklogStrategy.AutoAdjustRemainingEstimate, string newEstimate = null, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to delete worklog from issue, issue has not been saved to server.");
            }

            return _jira.Issues.DeleteWorklogAsync(_originalIssue.key, worklog.Id, worklogStrategy, newEstimate, token);
        }

        /// <summary>
        /// Retrieve worklogs for this issue.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IEnumerable<Worklog>> GetWorklogsAsync(CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to retrieve worklog, issue has not been saved to server.");
            }

            return _jira.Issues.GetWorklogsAsync(_originalIssue.key, token);
        }

        /// <summary>
        /// Updates all fields from server.
        /// </summary>
        public void Refresh()
        {
            this.RefreshAsync().Wait();
        }

        /// <summary>
        /// Updates all fields from server.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public async Task RefreshAsync(CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to refresh, issue has not been saved to server.");
            }

            // todo: Use the CancellationToken Parameter
            var serverIssue = await _jira.Issues.GetIssueAsync(_originalIssue.key).ConfigureAwait(false);
            Initialize(serverIssue.OriginalRemoteIssue);
        }

        /// <summary>
        /// Gets the workflow actions that the issue can be transitioned to.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IEnumerable<JiraNamedEntity>> GetAvailableActionsAsync(CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to retrieve actions, issue has not been saved to server.");
            }

            return this._jira.Issues.GetActionsAsync(_originalIssue.key, token);
        }

        /// <summary>
        /// Gets time tracking information for this issue.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IssueTimeTrackingData> GetTimeTrackingDataAsync(CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to retrieve time tracking data, issue has not been saved to server.");
            }

            return _jira.Issues.GetTimeTrackingDataAsync(_originalIssue.key, token);
        }

        /// <summary>
        /// Adds a user to the watchers of the issue.
        /// </summary>
        /// <param name="username">Username of the user to add.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task AddWatcherAsync(string username, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to add watcher, issue has not been saved to server.");
            }

            return _jira.Issues.AddWatcherAsync(_originalIssue.key, username, token);
        }

        /// <summary>
        /// Gets the users that are watching the issue.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IEnumerable<JiraUser>> GetWatchersAsync(CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to get watchers, issue has not been saved to server.");
            }

            return _jira.Issues.GetWatchersAsync(_originalIssue.key, token);
        }

        /// <summary>
        /// Removes a user from the watchers of the issue.
        /// </summary>
        /// <param name="username">Username of the user to add.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task DeleteWatcherAsync(string username, CancellationToken token = default(CancellationToken))
        {
            if (String.IsNullOrEmpty(_originalIssue.key))
            {
                throw new InvalidOperationException("Unable to remove watcher, issue has not been saved to server.");
            }

            return _jira.Issues.DeleteWatcherAsync(_originalIssue.key, username, token);
        }

        /// <summary>
        /// Gets the RemoteFields representing the fields that were updated
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        async Task<RemoteFieldValue[]> IRemoteIssueFieldProvider.GetRemoteFieldValuesAsync(CancellationToken token)
        {
            var fields = new List<RemoteFieldValue>();

            var remoteFields = typeof(RemoteIssue).GetProperties();
            foreach (var localProperty in typeof(Issue).GetProperties())
            {
                if (typeof(IRemoteIssueFieldProvider).IsAssignableFrom(localProperty.PropertyType))
                {
                    var fieldsProvider = localProperty.GetValue(this, null) as IRemoteIssueFieldProvider;

                    if (fieldsProvider != null)
                    {
                        var remoteFieldValues = await fieldsProvider.GetRemoteFieldValuesAsync(token).ConfigureAwait(false);
                        fields.AddRange(remoteFieldValues);
                    }
                }
                else
                {
                    var remoteProperty = remoteFields.FirstOrDefault(i => i.Name.Equals(localProperty.Name, StringComparison.OrdinalIgnoreCase));
                    if (remoteProperty == null)
                    {
                        continue;
                    }

                    var localStringValue = await GetStringValueForPropertyAsync(this, localProperty, token).ConfigureAwait(false);
                    var remoteStringValue = await GetStringValueForPropertyAsync(_originalIssue, remoteProperty, token).ConfigureAwait(false);

                    if (remoteStringValue != localStringValue)
                    {
                        var remoteFieldName = remoteProperty.Name;

                        var remoteFieldNameAttr = localProperty.GetCustomAttributes(typeof(RemoteFieldNameAttribute), true).OfType<RemoteFieldNameAttribute>().FirstOrDefault();
                        if (remoteFieldNameAttr != null)
                        {
                            remoteFieldName = remoteFieldNameAttr.Name;
                        }

                        fields.Add(new RemoteFieldValue()
                        {
                            id = remoteFieldName,
                            values = new string[1] { localStringValue }
                        });
                    }
                }
            }

            return fields.ToArray();
        }

        internal async Task<RemoteIssue> ToRemoteAsync(CancellationToken token)
        {
            var remote = new RemoteIssue()
            {
                assignee = this.Assignee,
                description = this.Description,
                environment = this.Environment,
                project = this.Project,
                reporter = this.Reporter,
                summary = this.Summary,
                votesData = this.Votes != null ? new RemoteVotes() { hasVoted = this.HasUserVoted == true, votes = this.Votes.Value } : null,
                duedate = this.DueDate
            };

            remote.key = this.Key != null ? this.Key.Value : null;

            if (Status != null)
            {
                await Status.LoadIdAndNameAsync(_jira, token).ConfigureAwait(false);
                remote.status = new RemoteStatus() { id = Status.Id, name = Status.Name };
            }

            if (Resolution != null)
            {
                await Resolution.LoadIdAndNameAsync(_jira, token).ConfigureAwait(false);
                remote.resolution = new RemoteResolution() { id = Resolution.Id, name = Resolution.Name };
            }

            if (Priority != null)
            {
                await Priority.LoadIdAndNameAsync(_jira, token).ConfigureAwait(false);
                remote.priority = new RemotePriority() { id = Priority.Id, name = Priority.Name };
            }

            if (Type != null)
            {
                await Type.LoadIdAndNameAsync(_jira, token).ConfigureAwait(false);
                remote.type = new RemoteIssueType() { id = Type.Id, name = Type.Name };
            }

            if (this.AffectsVersions.Count > 0)
            {
                remote.affectsVersions = this.AffectsVersions.Select(v => v.RemoteVersion).ToArray();
            }

            if (this.FixVersions.Count > 0)
            {
                remote.fixVersions = this.FixVersions.Select(v => v.RemoteVersion).ToArray();
            }

            if (this.Components.Count > 0)
            {
                remote.components = this.Components.Select(c => c.RemoteComponent).ToArray();
            }

            if (this.CustomFields.Count > 0)
            {
                remote.customFieldValues = this.CustomFields.Select(f => new RemoteCustomFieldValue()
                {
                    customfieldId = f.Id,
                    values = f.Values
                }).ToArray();
            }

            if (this.Labels.Count > 0)
            {
                remote.labels = this.Labels.ToArray();
            }

            return remote;
        }

        private async Task<string> GetStringValueForPropertyAsync(object container, PropertyInfo property, CancellationToken token)
        {
            var value = property.GetValue(container, null);

            if (property.PropertyType == typeof(DateTime?))
            {
                var dateValue = (DateTime?)value;
                return dateValue.HasValue ? dateValue.Value.ToString("d/MMM/yy") : null;
            }
            else if (typeof(JiraNamedEntity).IsAssignableFrom(property.PropertyType))
            {
                var jiraNamedEntity = property.GetValue(container, null) as JiraNamedEntity;
                if (jiraNamedEntity != null)
                {
                    await jiraNamedEntity.LoadIdAndNameAsync(_jira, token).ConfigureAwait(false);
                    return jiraNamedEntity.Id;
                }
                return null;
            }
            else if (typeof(AbstractNamedRemoteEntity).IsAssignableFrom(property.PropertyType))
            {
                var remoteEntity = property.GetValue(container, null) as AbstractNamedRemoteEntity;
                if (remoteEntity != null)
                {
                    return remoteEntity.id;
                }
                return null;
            }
            else
            {
                return value != null ? value.ToString() : null;
            }
        }
    }
}

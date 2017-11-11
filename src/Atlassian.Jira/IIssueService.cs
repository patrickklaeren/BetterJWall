using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira.Linq;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents the operations on the issues of jira.
    /// </summary>
    public interface IIssueService
    {
        /// <summary>
        /// Query builder for issues in jira.
        /// </summary>
        JiraQueryable<Issue> Queryable { get; }

        /// <summary>
        /// Whether to validate a JQL query
        /// </summary>
        bool ValidateQuery { get; set; }

        /// <summary>
        /// Maximum number of issues to retrieve per request.
        /// </summary>
        int MaxIssuesPerRequest { get; set; }

        /// <summary>
        /// Retrieves an issue by its key.
        /// </summary>
        /// <param name="issueKey">The issue key to retrieve</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<Issue> GetIssueAsync(string issueKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Retrieves a list of issues by their keys.
        /// </summary>
        /// <param name="issueKeys">List of issue keys to retrieve.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IDictionary<string, Issue>> GetIssuesAsync(IEnumerable<string> issueKeys, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Retrieves a list of issues by their keys.
        /// </summary>
        /// <param name="issueKeys">List of issue keys to retrieve.</param>
        Task<IDictionary<string, Issue>> GetIssuesAsync(params string[] issueKeys);

        /// <summary>
        /// Updates all fields of an issue.
        /// </summary>
        /// <param name="issue">Issue to update.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task UpdateIssueAsync(Issue issue, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Updates all fields of an issue.
        /// </summary>
        /// <param name="issue">Issue to update.</param>
        /// <param name="options">Options for the update</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task UpdateIssueAsync(Issue issue, IssueUpdateOptions options, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Creates an issue and returns a new instance populated from server.
        /// </summary>
        /// <param name="issue">Issue to create.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        /// <returns>Promise that contains the new issue key when resolved.</returns>
        Task<string> CreateIssueAsync(Issue issue, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Deletes the specified issue.
        /// </summary>
        /// <param name="issueKey">Key of issue to delete.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task DeleteIssueAsync(string issueKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Execute a specific JQL query and return the resulting issues.
        /// </summary>
        /// <param name="jql">JQL search query</param>
        /// <param name="maxIssues">Maximum number of issues to return (defaults to 50). The maximum allowable value is dictated by the JIRA property 'jira.search.views.default.max'. If you specify a value that is higher than this number, your search results will be truncated.</param>
        /// <param name="startAt">Index of the first issue to return (0-based)</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IPagedQueryResult<Issue>> GetIssuesFromJqlAsync(string jql, int? maxIssues = null, int startAt = 0, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Transition an issue through a workflow action.
        /// </summary>
        /// <param name="issue">Issue to transition.</param>
        /// <param name="actionName">The workflow action name to transition to.</param>
        /// <param name="updates">Additional updates to perform when transitioning the issue.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task ExecuteWorkflowActionAsync(Issue issue, string actionName, WorkflowTransitionUpdates updates, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Gets time tracking information for an issue.
        /// </summary>
        /// <param name="issueKey">The issue key.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IssueTimeTrackingData> GetTimeTrackingDataAsync(string issueKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Gets metadata object containing dictionary with issuefields identifiers as keys and their metadata as values
        /// </summary>
        /// <param name="issueKey">The issue key.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IDictionary<String, IssueFieldEditMetadata>> GetFieldsEditMetadataAsync(string issueKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Adds a comment to an issue.
        /// </summary>
        /// <param name="issueKey">Issue key to add the comment to.</param>
        /// <param name="comment">Comment object to add.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<Comment> AddCommentAsync(string issueKey, Comment comment, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns all comments of an issue.
        /// </summary>
        /// <param name="issueKey">Issue key to retrieve comments from.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IEnumerable<Comment>> GetCommentsAsync(string issueKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Removes a comment from an issue.
        /// </summary>
        /// <param name="issueKey">Issue key to remove the comment from.</param>
        /// <param name="commentId">Identifier of the comment to remove.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task DeleteCommentAsync(string issueKey, string commentId, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns the comments of an issue with paging.
        /// </summary>
        /// <param name="issueKey">Issue key to retrieve comments from.</param>
        /// <param name="maxComments">Maximum number of comments to retrieve.</param>
        /// <param name="startAt">Index of the first comment to return (0-based).</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IPagedQueryResult<Comment>> GetPagedCommentsAsync(string issueKey, int? maxComments = null, int startAt = 0, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns the workflow actions that an issue can be transitioned to.
        /// </summary>
        /// <param name="issueKey">The issue key</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IEnumerable<JiraNamedEntity>> GetActionsAsync(string issueKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Retrieve attachment metadata from server for this issue
        /// </summary>
        /// <param name="issueKey">The issue key to get attachments from.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IEnumerable<Attachment>> GetAttachmentsAsync(string issueKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Retrieve the labels from server for the issue specified.
        /// </summary>
        /// <param name="issueKey">The issue key to get labels from.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        [Obsolete("Use Issue.Labels instead.")]
        Task<string[]> GetLabelsAsync(string issueKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Sets the labels for the issue specified.
        /// </summary>
        /// <param name="issueKey">The issue key to set the labels.</param>
        /// <param name="labels">The list of labels to set on the issue.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        [Obsolete("Modify the Issue.Labels collection and call Issue.SaveChanges to update the labels field.")]
        Task SetLabelsAsync(string issueKey, string[] labels, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Retrieve the watchers from server for the issue specified.
        /// </summary>
        /// <param name="issueKey">The issue key to get watchers from.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IEnumerable<JiraUser>> GetWatchersAsync(string issueKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Removes a user from the watcher list of an issue.
        /// </summary>
        /// <param name="issueKey">The issue key to remove the watcher from.</param>
        /// <param name="username">User name of user to remove.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task DeleteWatcherAsync(string issueKey, string username, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Adds a user to the watcher list of an issue.
        /// </summary>
        /// <param name="issueKey">The issue key to add the watcher to.</param>
        /// <param name="username">User name of user to add.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task AddWatcherAsync(string issueKey, string username, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Retrieve the change logs from server for the issue specified.
        /// </summary>
        /// <param name="issueKey">The issue key to get watchers from.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IEnumerable<IssueChangeLog>> GetChangeLogsAsync(string issueKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Returns the issues that are marked as sub tasks of this issue.
        /// </summary>
        /// <param name="issueKey">The issue key to get sub tasks from.</param>
        /// <param name="maxIssues">Maximum number of issues to retrieve.</param>
        /// <param name="startAt">Index of the first issue to return (0-based).</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IPagedQueryResult<Issue>> GetSubTasksAsync(string issueKey, int? maxIssues = null, int startAt = 0, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Add one or more attachments to an issue.
        /// </summary>
        /// <param name="issueKey">Issue key to add attachments to.</param>
        /// <param name="attachments">Attachments to add.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task AddAttachmentsAsync(string issueKey, UploadAttachmentInfo[] attachments, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Removes an attachment from an issue.
        /// </summary>
        /// <param name="issueKey">Issue key to remove the attachment from.</param>
        /// <param name="attachmentId">Identifier of the attachment to remove.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task DeleteAttachmentAsync(string issueKey, string attachmentId, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Gets the worklog with the given identifier from an issue.
        /// </summary>
        /// <param name="issueKey">The issue key to retrieve the worklog from.</param>
        /// <param name="worklogId">The worklog identifier.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        /// <returns></returns>
        Task<Worklog> GetWorklogAsync(string issueKey, string worklogId, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Gets the worklogs for an issue.
        /// </summary>
        /// <param name="issueKey">Issue key to retrieve the worklogs from.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<IEnumerable<Worklog>> GetWorklogsAsync(string issueKey, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Adds a work log to an issue.
        /// </summary>
        /// <param name="issueKey">Issue key to add the worklog to.</param>
        /// <param name="worklog">The worklog instance to add.</param>
        /// <param name="worklogStrategy">How to handle the remaining estimate, defaults to AutoAdjustRemainingEstimate.</param>
        /// <param name="newEstimate">New estimate (only used if worklogStrategy set to NewRemainingEstimate)</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task<Worklog> AddWorklogAsync(string issueKey, Worklog worklog, WorklogStrategy worklogStrategy = WorklogStrategy.AutoAdjustRemainingEstimate, string newEstimate = null, CancellationToken token = default(CancellationToken));

        /// <summary>
        /// Removes a work log from an issue.
        /// </summary>
        /// <param name="issueKey">Issue key to remove the work log from.</param>
        /// <param name="worklogId">The identifier of the work log to remove.</param>
        /// <param name="worklogStrategy">How to handle the remaining estimate, defaults to AutoAdjustRemainingEstimate.</param>
        /// <param name="newEstimate">New estimate (only used if worklogStrategy set to NewRemainingEstimate)</param>
        /// <param name="token">Cancellation token for this operation.</param>
        Task DeleteWorklogAsync(string issueKey, string worklogId, WorklogStrategy worklogStrategy = WorklogStrategy.AutoAdjustRemainingEstimate, string newEstimate = null, CancellationToken token = default(CancellationToken));
    }
}

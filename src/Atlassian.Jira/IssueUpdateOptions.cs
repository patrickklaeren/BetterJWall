namespace Atlassian.Jira
{
    /// <summary>
    /// Settings to configure update options for issues.
    /// </summary>
    public class IssueUpdateOptions
    {
        /// <summary>
        /// Suppresses email notification (supported on Jira server 7.1+).
        /// </summary>
        public bool SuppressEmailNotification { get; set; } = false;
    }
}

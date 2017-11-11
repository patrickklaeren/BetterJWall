using Atlassian.Jira;
using IssueStatus = BetterJWall.Common.IssueStatus;

namespace BetterJWall.JIRABridge
{
    public static class Extensions
    {
        public static IssueStatus GetIssueStatus(this Issue issue)
        {
            switch (issue.Status.Name)
            {
                case Constants.JIRA_IN_PROGRESS_STATUS_KEY:
                    return IssueStatus.InProgress;
                case Constants.JIRA_IN_REVIEW_STATUS_KEY:
                    return IssueStatus.InReview;
                case Constants.JIRA_DONE_STATUS_KEY:
                    return IssueStatus.Done;
                default:
                    return IssueStatus.Unknown;
            }
        }
    }
}

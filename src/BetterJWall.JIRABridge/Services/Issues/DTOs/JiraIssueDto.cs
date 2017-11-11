using Atlassian.Jira;
using IssueStatus = BetterJWall.Common.IssueStatus;

namespace BetterJWall.JIRABridge.Services.Issues.DTOs
{
    public class JiraCaseDto
    {
        public JiraCaseDto(Issue issue)
        {
            IssueKey = issue.Key.ToString();
            Assignee = issue.Assignee;
            Summary = issue.Summary;
            IssueTypeIconUrl = issue.Type.IconUrl;
            Status = issue.GetIssueStatus();
        }

        public string IssueKey { get; }
        public string Assignee { get; }
        public string Summary { get; }
        public string IssueTypeIconUrl { get; }
        public IssueStatus Status { get; }
    }
}
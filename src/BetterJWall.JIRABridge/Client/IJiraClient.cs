using Atlassian.Jira;

namespace BetterJWall.JIRABridge.Client
{
    public interface IJiraClient
    {
        IIssueService Issues { get; }
    }
}
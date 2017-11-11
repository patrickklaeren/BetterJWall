using Atlassian.Jira;
using BetterJWall.Common;

namespace BetterJWall.JIRABridge.Client
{
    public class JiraClient : IJiraClient
    {
        private readonly IConfigurationHelper _configurationHelper;

        public JiraClient(IConfigurationHelper configurationHelper)
        {
            _configurationHelper = configurationHelper;
        }

        public Jira Client => Jira.CreateRestClient(_configurationHelper.JiraServerEndpoint, _configurationHelper.JiraUsername, _configurationHelper.JiraPassword);

        public IIssueService Issues => Client.Issues;
    }
}

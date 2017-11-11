using System;
using System.Collections.Generic;
using System.Text;
using BetterJWall.Common.Configurations;
using Microsoft.Extensions.Options;

namespace BetterJWall.Common
{
    public interface IConfigurationHelper
    {
        string JiraServerEndpoint { get; }
        string JiraUsername { get; }
        string JiraPassword { get; }
    }

    public class ConfigurationHelper : IConfigurationHelper
    {
        private readonly IOptions<JiraClientConfiguration> _jiraClientConfigurationOptions;

        public ConfigurationHelper(IOptions<JiraClientConfiguration> jiraClientConfigurationOptions)
        {
            _jiraClientConfigurationOptions = jiraClientConfigurationOptions;
        }

        public string JiraServerEndpoint => _jiraClientConfigurationOptions.Value.ServerEndpoint;
        public string JiraUsername => _jiraClientConfigurationOptions.Value.Username;
        public string JiraPassword => _jiraClientConfigurationOptions.Value.Password;
    }
}

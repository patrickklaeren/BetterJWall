using System;

namespace Atlassian.Jira
{
    /// <summary>
    /// Holds user and password information for user that connects to JIRA.
    /// </summary>
    public class JiraCredentials
    {
        private readonly string _username;
        private readonly string _password;

        public JiraCredentials(string username, string password = null)
        {
            _username = username;
            _password = password;
        }

        public string UserName
        {
            get { return _username; }
        }

        public string Password
        {
            get { return _password; }
        }
    }
}

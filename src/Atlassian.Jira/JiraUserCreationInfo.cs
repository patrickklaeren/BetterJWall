using System;
using Newtonsoft.Json;

namespace Atlassian.Jira
{
    /// <remarks>
    /// Class that encapsulates the necessary information to create a new jira user.
    /// </remarks>
    public class JiraUserCreationInfo
    {
        /// <summary>
        /// Set the username
        /// </summary>
        [JsonProperty("name")]
        public string Username { get; set; }

        /// <summary>
        /// Set the DisplayName
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Set the email address
        /// </summary>
        [JsonProperty("emailAddress")]
        public string Email { get; set; }

        /// <summary>
        /// If password field is not set then password will be randomly generated.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Set to true to have the user notified by email upon account creation. False to prevent notification.
        /// </summary>
        [JsonProperty("notification")]
        public bool Notification { get; set; }

        public override string ToString()
        {
            return Username;
        }
    }
}
using System;
using Newtonsoft.Json;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents a JIRA user.
    /// </summary>
    public class JiraUser
    {
        /// <summary>
        /// The 'username' for the user.
        /// </summary>
        [JsonProperty("name")]
        public string Username { get; private set; }

        /// <summary>
        /// The long display name for the user.
        /// </summary>
        [JsonProperty("displayName")]
        public string DisplayName { get; private set; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        [JsonProperty("emailAddress")]
        public string Email { get; private set; }

        /// <summary>
        /// Whether the user is marked as active on the server.
        /// </summary>
        [JsonProperty("active")]
        public bool IsActive { get; private set; }

        /// <summary>
        /// The locale of the User.
        /// </summary>
        [JsonProperty("locale")]
        public string Locale { get; private set; }

        /// <summary>
        /// Url to access this resource.
        /// </summary>
        [JsonProperty("self")]
        public string Self { get; private set; }

        public override string ToString()
        {
            return Username;
        }

        public override bool Equals(object other)
        {
            var otherAsThisType = other as JiraUser;
            return otherAsThisType != null && Username.Equals(otherAsThisType.Username);
        }

        public override int GetHashCode()
        {
            return Username.GetHashCode();
        }
    }
}

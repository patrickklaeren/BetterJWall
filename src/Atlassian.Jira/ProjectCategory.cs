using System;
using Newtonsoft.Json;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents a project category in Jira.
    /// </summary>
    public class ProjectCategory : JiraNamedResource
    {
        /// <summary>
        /// Description of the category.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; private set; }
    }
}

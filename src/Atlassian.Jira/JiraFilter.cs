using System;
using Newtonsoft.Json;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents a JIRA filter.
    /// </summary>
    public class JiraFilter : JiraNamedResource
    {
        /// <summary>
        /// Creates an instance of a JiraFilter.
        /// </summary>
        /// <param name="id">Identifier of the resource.</param>
        /// <param name="name">Name of the resource.</param>
        /// <param name="jql">Jql of the filter.</param>
        /// <param name="self">Url to the resource.</param>
        public JiraFilter(string id, string name, string jql = null, string self = null)
            : base(id, name, self)
        {
            Jql = jql;
        }

        /// <summary>
        /// JQL for this filter.
        /// </summary>
        [JsonProperty("jql")]
        public string Jql { get; private set; }

        /// <summary>
        /// Description for this filter.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; private set; }
    }
}

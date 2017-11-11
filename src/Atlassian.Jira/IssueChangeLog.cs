using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents the log of the change done to an issue as recorded by JIRA.
    /// </summary>
    public class IssueChangeLog : IJiraEntity
    {
        /// <summary>
        /// Identifier of this change log.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; private set; }

        /// <summary>
        /// User that performed the change.
        /// </summary>
        [JsonProperty("author")]
        public JiraUser Author { get; private set; }

        /// <summary>
        /// Date that the change was performed.
        /// </summary>
        [JsonProperty("created")]
        public DateTime CreatedDate { get; private set; }

        /// <summary>
        /// List of items that were changed.
        /// </summary>
        [JsonProperty("items")]
        public IEnumerable<IssueChangeLogItem> Items { get; private set; }
    }
}

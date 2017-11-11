using System;
using Newtonsoft.Json;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents an individual field change within a change log.
    /// </summary>
    public class IssueChangeLogItem
    {
        /// <summary>
        /// Name of the field that was changed.
        /// </summary>
        [JsonProperty("field")]
        public string FieldName { get; private set; }

        /// <summary>
        /// Type of the field that was changed.
        /// </summary>
        [JsonProperty("fieldtype")]
        public string FieldType { get; private set; }

        /// <summary>
        /// Identifier of the original value of the field.
        /// </summary>
        [JsonProperty("from")]
        public string FromId { get; private set; }

        /// <summary>
        /// Original value of the field.
        /// </summary>
        [JsonProperty("fromString")]
        public string FromValue { get; private set; }

        /// <summary>
        /// Identifier of the new value of the field.
        /// </summary>
        [JsonProperty("to")]
        public string ToId { get; private set; }

        /// <summary>
        /// New value of the field.
        /// </summary>
        [JsonProperty("toString")]
        public string ToValue { get; private set; }
    }
}

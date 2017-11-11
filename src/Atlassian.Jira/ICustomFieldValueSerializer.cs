using System;
using Newtonsoft.Json.Linq;

namespace Atlassian.Jira
{
    /// <summary>
    /// Contract to serialize and deserialize a custom field value from JIRA.
    /// </summary>
    public interface ICustomFieldValueSerializer
    {
        /// <summary>
        /// Deserializes values from a custom field.
        /// </summary>
        /// <param name="json">JToken representing the json value(s).</param>
        string[] FromJson(JToken json);

        /// <summary>
        /// Serializes values for a custom field.
        /// </summary>
        /// <param name="values">Values to serialize as JSON.</param>
        JToken ToJson(string[] values);
    }
}

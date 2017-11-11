using System;
using Newtonsoft.Json;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents an issue link type in JIRA.
    /// </summary>
    public class IssueLinkType : JiraNamedResource
    {
        /// <summary>
        /// Creates an instance of IssueLinkType.
        /// </summary>
        public IssueLinkType()
        {
        }

        /// <summary>
        /// Creates an instance of IssueLinkType.
        /// </summary>
        /// <param name="id">Identifier of the issue link type.</param>
        /// <param name="name">Name of the issue link type.</param>
        /// <param name="inward">Description of the 'inward' issue link relationship.</param>
        /// <param name="outward">Description of the 'outward' issue link relationship.</param>
        public IssueLinkType(string id, string name, string inward, string outward)
            : base(id, name)
        {
            Inward = inward;
            Outward = outward;
        }

        /// <summary>
        /// Description of the 'inward' issue link relationship.
        /// </summary>
        [JsonProperty("inward")]
        public string Inward { get; private set; }

        /// <summary>
        /// Description of the 'outward' issue link relationship.
        /// </summary>
        [JsonProperty("outward")]
        public string Outward { get; private set; }
    }
}

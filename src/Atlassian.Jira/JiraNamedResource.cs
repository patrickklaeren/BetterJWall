using System;
using Newtonsoft.Json;

namespace Atlassian.Jira
{
    /// <summary>
    /// Class used to deserialize a generic JIRA resource as returned by the REST API.
    /// </summary>
    public class JiraNamedResource : IJiraEntity
    {
        /// <summary>
        /// Creates an instance of JiraNamedResource.
        /// </summary>
        public JiraNamedResource()
        {
        }

        /// <summary>
        /// Creates an instance of JiraNamedResource.
        /// </summary>
        /// <param name="id">Identifier of the resource.</param>
        /// <param name="name">Name of the resource.</param>
        /// <param name="self">Url to the resource.</param>
        public JiraNamedResource(string id, string name, string self = null)
        {
            Id = id;
            Name = name;
            Self = self;
        }

        /// <summary>
        /// Identifier of this resource.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; private set; }

        /// <summary>
        /// Name of this resource.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; private set; }

        /// <summary>
        /// Url to access this resource.
        /// </summary>
        [JsonProperty("self")]
        public string Self { get; private set; }
    }
}

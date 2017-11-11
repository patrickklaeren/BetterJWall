using System;
using Newtonsoft.Json;

namespace Atlassian.Jira
{
    /// <summary>
    /// Class that encapsulates the necessary information to create a new project component.
    /// </summary>
    public class ProjectComponentCreationInfo
    {
        /// <summary>
        /// Creates a new instance of ProjectComponentCreationInfo.
        /// </summary>
        /// <param name="name">The name of the project component.</param>
        public ProjectComponentCreationInfo(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Name of the project component.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Description of the project component.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Key of the project to associate with this component.
        /// </summary>
        [JsonProperty("project")]
        public string ProjectKey { get; set; }
    }
}
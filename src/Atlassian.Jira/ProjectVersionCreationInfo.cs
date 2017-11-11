using System;
using Newtonsoft.Json;

namespace Atlassian.Jira
{
    /// <summary>
    /// Class that encapsulates the necessary information to create a new project version.
    /// </summary>
    public class ProjectVersionCreationInfo
    {
        /// <summary>
        /// Creates a new instance of ProjectVersionCreationInfo.
        /// </summary>
        /// <param name="name">The name of the project version.</param>
        public ProjectVersionCreationInfo(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Key of the project to associate with this version.
        /// </summary>
        [JsonProperty("project")]
        public string ProjectKey { get; set; }

        /// <summary>
        /// Name of the project version.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Description of the project version.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Whether this version is archived.
        /// </summary>
        [JsonProperty("archived")]
        public bool IsArchived { get; set; }

        /// <summary>
        /// Whether this version has been released.
        /// </summary>
        [JsonProperty("released")]
        public bool IsReleased { get; set; }

        /// <summary>
        /// The release date, null if the version has not been released yet.
        /// </summary>
        [JsonProperty("releaseDate")]
        public DateTime? ReleaseDate { get; set; }

        /// <summary>
        /// The start date, null if version has not been started yet.
        /// </summary>
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlassian.Jira
{
    /// <summary>
    /// Collection of project versions
    /// </summary>
    public class ProjectVersionCollection : JiraNamedEntityCollection<ProjectVersion>
    {
        internal ProjectVersionCollection(string fieldName, Jira jira, string projectKey)
            : this(fieldName, jira, projectKey, new List<ProjectVersion>())
        {
        }

        internal ProjectVersionCollection(string fieldName, Jira jira, string projectKey, IList<ProjectVersion> list)
            : base(fieldName, jira, projectKey, list)
        {
        }

        /// <summary>
        /// Add a version by name
        /// </summary>
        /// <param name="versionName">Version name</param>
        public void Add(string versionName)
        {
            var version = _jira.Versions.GetVersionsAsync(_projectKey).Result.FirstOrDefault(v => v.Name.Equals(versionName, StringComparison.OrdinalIgnoreCase));

            if (version == null)
            {
                throw new InvalidOperationException(String.Format("Unable to find version with name '{0}'.", versionName));
            }

            this.Add(version);
        }
    }
}
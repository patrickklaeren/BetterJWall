using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlassian.Jira
{
    /// <summary>
    /// Collection of project components
    /// </summary>
    public class ProjectComponentCollection : JiraNamedEntityCollection<ProjectComponent>
    {
        internal ProjectComponentCollection(string fieldName, Jira jira, string projectKey)
            : this(fieldName, jira, projectKey, new List<ProjectComponent>())
        {
        }

        internal ProjectComponentCollection(string fieldName, Jira jira, string projectKey, IList<ProjectComponent> list)
            : base(fieldName, jira, projectKey, list)
        {
        }

        /// <summary>
        /// Add a component by name
        /// </summary>
        /// <param name="componentName">Component name</param>
        public void Add(string componentName)
        {
            var component = _jira.Components.GetComponentsAsync(_projectKey).Result.FirstOrDefault(v => v.Name.Equals(componentName, StringComparison.OrdinalIgnoreCase));

            if (component == null)
            {
                throw new InvalidOperationException(String.Format("Unable to find component with name '{0}'.", componentName));
            }

            this.Add(component);
        }
    }
}
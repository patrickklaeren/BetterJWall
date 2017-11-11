using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira.Remote;

namespace Atlassian.Jira
{
    /// <summary>
    /// A JIRA project
    /// </summary>
    public class Project : JiraNamedEntity
    {
        private readonly Jira _jira;
        private readonly RemoteProject _remoteProject;

        /// <summary>
        /// Creates a new Project instance using a remote project.
        /// </summary>
        /// <param name="jira">Instance of the Jira client.</param>
        /// <param name="remoteProject">Remote project.</param>
        public Project(Jira jira, RemoteProject remoteProject)
            : base(remoteProject)
        {
            _jira = jira;
            _remoteProject = remoteProject;
        }

        internal RemoteProject RemoteProject
        {
            get
            {
                return _remoteProject;
            }
        }

        /// <summary>
        /// The unique identifier of the project.
        /// </summary>
        public string Key
        {
            get
            {
                return _remoteProject.key;
            }
        }

        /// <summary>
        /// The category set on this project.
        /// </summary>
        public ProjectCategory Category
        {
            get
            {
                return _remoteProject.projectCategory;
            }
        }

        /// <summary>
        /// Username of the project lead.
        /// </summary>
        public string Lead
        {
            get
            {
                return _remoteProject.lead;
            }
        }

        /// <summary>
        /// The URL set on the project.
        /// </summary>
        public string Url
        {
            get
            {
                return _remoteProject.url;
            }
        }

        /// <summary>
        /// Gets the issue types for the current project.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IEnumerable<IssueType>> GetIssueTypesAsync(CancellationToken token = default(CancellationToken))
        {
            return _jira.IssueTypes.GetIssueTypesForProjectAsync(Key, token);
        }

        /// <summary>
        /// Creates a new project component.
        /// </summary>
        /// <param name="projectComponent">Information of the new component.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<ProjectComponent> AddComponentAsync(ProjectComponentCreationInfo projectComponent, CancellationToken token = default(CancellationToken))
        {
            projectComponent.ProjectKey = Key;
            return _jira.Components.CreateComponentAsync(projectComponent, token);
        }

        /// <summary>
        /// Gets the components for the current project.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IEnumerable<ProjectComponent>> GetComponetsAsync(CancellationToken token = default(CancellationToken))
        {
            return _jira.Components.GetComponentsAsync(Key, token);
        }

        /// <summary>
        /// Deletes a project component.
        /// </summary>
        /// <param name="componentName">Name of the component to remove.</param>
        /// <param name="moveIssuesTo">The component to set on issues where the deleted component is the component, If null then the component is removed.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public async Task DeleteComponentAsync(string componentName, string moveIssuesTo = null, CancellationToken token = default(CancellationToken))
        {
            var components = await this.GetComponetsAsync(token).ConfigureAwait(false);
            var component = components.First(c => String.Equals(c.Name, componentName));

            if (component == null)
            {
                throw new InvalidOperationException(String.Format("Unable to locate a component with name '{0}'", componentName));
            }

            await _jira.Components.DeleteComponentAsync(component.Id, moveIssuesTo, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new project version.
        /// </summary>
        /// <param name="projectVersion">Information of the new project version.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<ProjectVersion> AddVersionAsync(ProjectVersionCreationInfo projectVersion, CancellationToken token = default(CancellationToken))
        {
            projectVersion.ProjectKey = Key;
            return _jira.Versions.CreateVersionAsync(projectVersion, token);
        }

        /// <summary>
        /// Gets the versions for this project.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IEnumerable<ProjectVersion>> GetVersionsAsync(CancellationToken token = default(CancellationToken))
        {
            return _jira.Versions.GetVersionsAsync(Key, token);
        }

        /// <summary>
        /// Gets the paged versions for this project (not-cached).
        /// </summary>
        /// <param name="startAt">The page offset, if not specified then defaults to 0.</param>
        /// <param name="maxResults">How many results on the page should be included. Defaults to 50.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public Task<IPagedQueryResult<ProjectVersion>> GetPagedVersionsAsync(int startAt = 0, int maxResults = 50, CancellationToken token = default(CancellationToken))
        {
            return _jira.Versions.GetPagedVersionsAsync(Key, startAt, maxResults, token);
        }

        /// <summary>
        /// Deletes a project version.
        /// </summary>
        /// <param name="versionName">Name of the version to delete.</param>
        /// <param name="moveFixIssuesTo">The version to set fixVersion to on issues where the deleted version is the fix version, If null then the fixVersion is removed.</param>
        /// <param name="moveAffectedIssuesTo">The version to set fixVersion to on issues where the deleted version is the fix version, If null then the fixVersion is removed.</param>
        /// <param name="token">Cancellation token for this operation.</param>
        public async Task DeleteVersionAsync(string versionName, string moveFixIssuesTo = null, string moveAffectedIssuesTo = null, CancellationToken token = default(CancellationToken))
        {
            var versions = await this.GetVersionsAsync(token).ConfigureAwait(false);
            var version = versions.FirstOrDefault(v => String.Equals(v.ProjectKey, Key));

            if (version == null)
            {
                throw new InvalidOperationException(String.Format("Unable to locate a version with name '{0}'", versionName));
            }

            await _jira.Versions.DeleteVersionAsync(version.Id, moveFixIssuesTo, moveAffectedIssuesTo, token).ConfigureAwait(false);
        }
    }
}

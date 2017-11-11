using System;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira.Remote;

namespace Atlassian.Jira
{
    /// <summary>
    /// A version associated with a project
    /// </summary>
    public class ProjectVersion : JiraNamedEntity
    {
        private readonly Jira _jira;
        private RemoteVersion _remoteVersion;

        /// <summary>
        /// Creates a new instance of a ProjectVersion.
        /// </summary>
        /// <param name="jira">The jira instance.</param>
        /// <param name="remoteVersion">The remote version.</param>
        public ProjectVersion(Jira jira, RemoteVersion remoteVersion)
            : base(remoteVersion)
        {
            if (jira == null)
            {
                throw new ArgumentNullException("jira");
            }

            _jira = jira;
            _remoteVersion = remoteVersion;
        }

        internal RemoteVersion RemoteVersion
        {
            get
            {
                return _remoteVersion;
            }
        }

        /// <summary>
        /// Gets the project key associated with this version.
        /// </summary>
        public string ProjectKey
        {
            get
            {
                return _remoteVersion.ProjectKey;
            }
        }

        /// <summary>
        /// Whether this version has been archived
        /// </summary>
        public bool IsArchived
        {
            get
            {
                return _remoteVersion.archived;
            }
            set
            {
                _remoteVersion.archived = value;
            }
        }

        /// <summary>
        /// Whether this version has been released
        /// </summary>
        public bool IsReleased
        {
            get
            {
                return _remoteVersion.released;
            }
            set
            {
                _remoteVersion.released = value;
            }
        }

        /// <summary>
        /// The start date for this version
        /// </summary>
        public DateTime? StartDate
        {
            get
            {
                return _remoteVersion.startDate;
            }
            set
            {
                _remoteVersion.startDate = value;
            }
        }

        /// <summary>
        /// The released date for this version (null if not yet released)
        /// </summary>
        public DateTime? ReleasedDate
        {
            get
            {
                return _remoteVersion.releaseDate;
            }
            set
            {
                _remoteVersion.releaseDate = value;
            }
        }

        /// <summary>
        /// The release description for this version (null if not available)
        /// </summary>
        public string Description
        {
            get
            {
                return _remoteVersion.description;
            }
            set
            {
                _remoteVersion.description = value;
            }
        }

        /// <summary>
        /// Save field changes to the server.
        /// </summary>
        public void SaveChanges()
        {
            try
            {
                SaveChangesAsync().Wait();
            }
            catch (AggregateException ex)
            {
                throw ex.Flatten().InnerException;
            }
        }

        /// <summary>
        /// Save field changes to the server.
        /// </summary>
        /// <param name="token">Cancellation token for this operation.</param>
        public async Task SaveChangesAsync(CancellationToken token = default(CancellationToken))
        {
            var version = await _jira.Versions.UpdateVersionAsync(this, token).ConfigureAwait(false);
            _remoteVersion = version.RemoteVersion;
        }
    }
}
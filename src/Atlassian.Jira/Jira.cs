using System;
using System.Globalization;
using Atlassian.Jira.Linq;
using Atlassian.Jira.Remote;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents a JIRA server
    /// </summary>
    public class Jira
    {
        internal const string DEFAULT_DATE_FORMAT = "yyyy/MM/dd";
        internal static CultureInfo DefaultCultureInfo = CultureInfo.GetCultureInfo("en-us");

        private readonly JiraCredentials _credentials;
        private readonly JiraCache _cache;
        private readonly ServiceLocator _services;

        /// <summary>
        /// Create a client that connects with a JIRA server with specified dependencies.
        /// </summary>
        public Jira(ServiceLocator services, JiraCredentials credentials = null, JiraCache cache = null)
        {
            _services = services;
            _credentials = credentials;
            _cache = cache ?? new JiraCache();

            this.Debug = false;
        }

        /// <summary>
        /// Creates a JIRA rest client.
        /// </summary>
        /// <param name="url">Url to the JIRA server.</param>
        /// <param name="username">Username used to authenticate.</param>
        /// <param name="password">Password used to authenticate.</param>
        /// <param name="settings">Settings to configure the rest client.</param>
        /// <returns>Jira object configured to use REST API.</returns>
        public static Jira CreateRestClient(string url, string username = null, string password = null, JiraRestClientSettings settings = null)
        {
            var services = new ServiceLocator();
            settings = settings ?? new JiraRestClientSettings();
            var jira = new Jira(services, new JiraCredentials(username, password), settings.Cache);
            var restClient = new JiraRestClient(services, url, username, password, settings);

            ConfigureDefaultServices(services, jira, restClient);

            return jira;
        }

        /// <summary>
        /// Creates a JIRA client with the given rest client implementation.
        /// </summary>
        /// <param name="jiraClient">Rest client to use.</param>
        /// <param name="credentials">Credentials to use.</param>
        /// <param name="cache">Cache to use.</param>
        public static Jira CreateRestClient(IJiraRestClient jiraClient, JiraCredentials credentials = null, JiraCache cache = null)
        {
            var services = new ServiceLocator();
            var jira = new Jira(services, credentials, cache);
            ConfigureDefaultServices(services, jira, jiraClient);
            return jira;
        }

        /// <summary>
        /// Gets the service locator for this jira instance.
        /// </summary>
        public ServiceLocator Services
        {
            get
            {
                return _services;
            }
        }

        /// <summary>
        /// Gets an object to interact with the projects of jira.
        /// </summary>
        public IProjectService Projects
        {
            get
            {
                return Services.Get<IProjectService>();
            }
        }

        /// <summary>
        /// Gets an object to interact with the users of jira.
        /// </summary>
        public IJiraUserService Users
        {
            get
            {
                return Services.Get<IJiraUserService>();
            }
        }

        /// <summary>
        /// Gets an object to interact with the user groups of jira.
        /// </summary>
        public IJiraGroupService Groups
        {
            get
            {
                return Services.Get<IJiraGroupService>();
            }
        }

        /// <summary>
        /// Gets an object to interact with the issue of jira.
        /// </summary>
        public IIssueService Issues
        {
            get
            {
                return Services.Get<IIssueService>();
            }
        }

        /// <summary>
        /// Gets an object to interact with the issue fields of jira.
        /// </summary>
        public IIssueFieldService Fields
        {
            get
            {
                return Services.Get<IIssueFieldService>();
            }
        }

        /// <summary>
        /// Gets an object to interact with the issue filters of jira.
        /// </summary>
        public IIssueFilterService Filters
        {
            get
            {
                return Services.Get<IIssueFilterService>();
            }
        }

        /// <summary>
        /// Gets an object to interact with the issue priorities of jira.
        /// </summary>
        public IIssuePriorityService Priorities
        {
            get
            {
                return Services.Get<IIssuePriorityService>();
            }
        }

        /// <summary>
        /// Gets an object to interact with the issue resolutions of jira.
        /// </summary>
        public IIssueResolutionService Resolutions
        {
            get
            {
                return Services.Get<IIssueResolutionService>();
            }
        }

        /// <summary>
        /// Gets an object to interact with the issue statuses of jira.
        /// </summary>
        public IIssueStatusService Statuses
        {
            get
            {
                return Services.Get<IIssueStatusService>();
            }
        }

        /// <summary>
        /// Gets an object to interact with the issue link types of jira.
        /// </summary>
        public IIssueLinkService Links
        {
            get
            {
                return Services.Get<IIssueLinkService>();
            }
        }

        /// <summary>
        /// Gets an object to interact with the issue remote links of jira.
        /// </summary>
        public IIssueRemoteLinkService RemoteLinks
        {
            get
            {
                return Services.Get<IIssueRemoteLinkService>();
            }
        }

        /// <summary>
        /// Gets an object to interact with the issue types of jira.
        /// </summary>
        public IIssueTypeService IssueTypes
        {
            get
            {
                return Services.Get<IIssueTypeService>();
            }
        }

        /// <summary>
        /// Gets an object to interact with the project versions of jira.
        /// </summary>
        public IProjectVersionService Versions
        {
            get
            {
                return Services.Get<IProjectVersionService>();
            }
        }

        /// <summary>
        /// Gets an object to interact with the project components of jira.
        /// </summary>
        public IProjectComponentService Components
        {
            get
            {
                return Services.Get<IProjectComponentService>();
            }
        }

        /// <summary>
        /// Gets the cache for frequently retrieved server items from JIRA.
        /// </summary>
        public JiraCache Cache
        {
            get
            {
                return _cache;
            }
        }

        /// <summary>
        /// Gets a client configured to interact with JIRA's REST API.
        /// </summary>
        public IJiraRestClient RestClient
        {
            get
            {
                return Services.Get<IJiraRestClient>();
            }
        }

        /// <summary>
        /// Whether to print the translated JQL to console
        /// </summary>
        public bool Debug { get; set; }

        /// <summary>
        /// Maximum number of issues per request
        /// </summary>
        [Obsolete("Use Jira.Issues.MaxIssuesPerRequest")]
        public int MaxIssuesPerRequest
        {
            get
            {
                return this.Issues.MaxIssuesPerRequest;
            }
            set
            {
                this.Issues.MaxIssuesPerRequest = value;
            }
        }

        /// <summary>
        /// Url to the JIRA server
        /// </summary>
        public string Url
        {
            get { return RestClient.Url; }
        }

        internal JiraCredentials GetCredentials()
        {
            if (this._credentials == null)
            {
                throw new InvalidOperationException("Unable to get user and password, credentials has not been set.");
            }

            return this._credentials;
        }

        internal IFileSystem FileSystem
        {
            get
            {
                return Services.Get<IFileSystem>();
            }
        }

        /// <summary>
        /// Returns a new issue that when saved will be created on the remote JIRA server
        /// </summary>
        public Issue CreateIssue(string project, string parentIssueKey = null)
        {
            return new Issue(this, project, parentIssueKey);
        }

        private static void ConfigureDefaultServices(ServiceLocator services, Jira jira, IJiraRestClient restClient)
        {
            services.Register<IProjectVersionService>(() => new ProjectVersionService(jira));
            services.Register<IProjectComponentService>(() => new ProjectComponentService(jira));
            services.Register<IIssuePriorityService>(() => new IssuePriorityService(jira));
            services.Register<IIssueResolutionService>(() => new IssueResolutionService(jira));
            services.Register<IIssueStatusService>(() => new IssueStatusService(jira));
            services.Register<IIssueLinkService>(() => new IssueLinkService(jira));
            services.Register<IIssueRemoteLinkService>(() => new IssueRemoteLinkService(jira));
            services.Register<IIssueTypeService>(() => new IssueTypeService(jira));
            services.Register<IIssueFilterService>(() => new IssueFilterService(jira));
            services.Register<IIssueFieldService>(() => new IssueFieldService(jira));
            services.Register<IIssueService>(() => new IssueService(jira, restClient.Settings));
            services.Register<IJiraUserService>(() => new JiraUserService(jira));
            services.Register<IJiraGroupService>(() => new JiraGroupService(jira));
            services.Register<IProjectService>(() => new ProjectService(jira));
            services.Register<IJqlExpressionVisitor>(() => new JqlExpressionVisitor());
            services.Register<IFileSystem>(() => new FileSystem());
            services.Register(() => restClient);
        }
    }
}

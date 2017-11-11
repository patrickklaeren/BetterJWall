using System;

namespace Atlassian.Jira
{
    /// <summary>
    /// Cache for frequently retrieved server items from JIRA.
    /// </summary>
    public class JiraCache
    {
        private JiraEntityDictionary<IssueType> _cachedIssueTypes = new JiraEntityDictionary<IssueType>();
        private JiraEntityDictionary<ProjectComponent> _cachedComponents = new JiraEntityDictionary<ProjectComponent>();
        private JiraEntityDictionary<ProjectVersion> _cachedVersions = new JiraEntityDictionary<ProjectVersion>();
        private JiraEntityDictionary<CustomField> _cachedCustomFields = new JiraEntityDictionary<CustomField>();
        private JiraEntityDictionary<IssuePriority> _cachedPriorities = new JiraEntityDictionary<IssuePriority>();
        private JiraEntityDictionary<IssueStatus> _cachedStatuses = new JiraEntityDictionary<IssueStatus>();
        private JiraEntityDictionary<IssueResolution> _cachedResolutions = new JiraEntityDictionary<IssueResolution>();
        private JiraEntityDictionary<Project> _cachedProjects = new JiraEntityDictionary<Project>();
        private JiraEntityDictionary<IssueLinkType> _cachedLinkTypes = new JiraEntityDictionary<IssueLinkType>();

        public JiraEntityDictionary<IssueType> IssueTypes { get { return this._cachedIssueTypes; } }
        public JiraEntityDictionary<ProjectComponent> Components { get { return this._cachedComponents; } }
        public JiraEntityDictionary<ProjectVersion> Versions { get { return this._cachedVersions; } }
        public JiraEntityDictionary<IssuePriority> Priorities { get { return this._cachedPriorities; } }
        public JiraEntityDictionary<IssueStatus> Statuses { get { return this._cachedStatuses; } }
        public JiraEntityDictionary<IssueResolution> Resolutions { get { return this._cachedResolutions; } }
        public JiraEntityDictionary<Project> Projects { get { return this._cachedProjects; } }
        public JiraEntityDictionary<CustomField> CustomFields { get { return this._cachedCustomFields; } }
        public JiraEntityDictionary<IssueLinkType> LinkTypes { get { return this._cachedLinkTypes; } }
    }
}

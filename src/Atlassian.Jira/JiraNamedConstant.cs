using System;
using Atlassian.Jira.Remote;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents a remote constant within JIRA. Abstracts the IssueType, Priority and Status used on issues.
    /// </summary>
    public class JiraNamedConstant : JiraNamedEntity
    {
        private readonly AbstractRemoteConstant _remoteConstant;

        /// <summary>
        /// Creates a new instance of JiraNamedConstant.
        /// </summary>
        /// <param name="remoteConstant"></param>
        public JiraNamedConstant(AbstractRemoteConstant remoteConstant) :
            base(remoteConstant)
        {
            this._remoteConstant = remoteConstant;
        }

        /// <summary>
        /// Creates an instance of the JiraNamedConstant with the given id and name.
        /// </summary>
        public JiraNamedConstant(string id, string name = null)
            : base(id, name)
        {
        }

        /// <summary>
        /// Description of the entity.
        /// </summary>
        public string Description
        {
            get
            {
                return _remoteConstant?.description;
            }
        }

        /// <summary>
        /// Url to the icon of this entity.
        /// </summary>
        public string IconUrl
        {
            get
            {
                return _remoteConstant?.iconUrl;
            }
        }
    }
}

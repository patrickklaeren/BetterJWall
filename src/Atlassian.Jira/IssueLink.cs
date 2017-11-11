using System;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents a link between two issues.
    /// </summary>
    public class IssueLink
    {
        private readonly IssueLinkType _linkType;
        private readonly Issue _outwardIssue;
        private readonly Issue _inwardIssue;

        /// <summary>
        /// Creates a new IssueLink instance.
        /// </summary>
        public IssueLink(IssueLinkType linkType, Issue outwardIssue, Issue inwardIssue)
        {
            this._linkType = linkType;
            this._outwardIssue = outwardIssue;
            this._inwardIssue = inwardIssue;
        }

        /// <summary>
        /// The inward issue of the link relationship.
        /// </summary>
        public Issue InwardIssue
        {
            get { return _inwardIssue; }
        }

        /// <summary>
        /// The type of the link relationship.
        /// </summary>
        public IssueLinkType LinkType
        {
            get { return _linkType; }
        }

        /// <summary>
        /// The outward issue of the link relationship.
        /// </summary>
        public Issue OutwardIssue
        {
            get { return _outwardIssue; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira.Remote;

namespace Atlassian.Jira
{
    /// <summary>
    /// The type of the issue as defined in JIRA
    /// </summary>
    [SuppressMessage("N/A", "CS0660", Justification = "Operator overloads are used for LINQ to JQL provider.")]
    [SuppressMessage("N/A", "CS0661", Justification = "Operator overloads are used for LINQ to JQL provider.")]
    public class IssueType : JiraNamedConstant
    {
        private bool _isSubTask;

        /// <summary>
        /// Creates an instance of the IssuePriority based on a remote entity.
        /// </summary>
        public IssueType(RemoteIssueType remoteIssueType)
            : base(remoteIssueType)
        {
            _isSubTask = remoteIssueType.subTask;
        }

        /// <summary>
        /// Creates an instance of the IssuePriority with given id and name.
        /// </summary>
        /// <param name="id">Identifiers of the issue type.</param>
        /// <param name="name">Name of the issue type.</param>
        /// <param name="isSubTask">Whether the issue type is a sub task.</param>
        public IssueType(string id, string name = null, bool isSubTask = false)
            : base(id, name)
        {
        }

        /// <summary>
        /// Whether this issue type represents a sub-task.
        /// </summary>
        public bool IsSubTask
        {
            get
            {
                return _isSubTask;
            }
        }

        protected override async Task<IEnumerable<JiraNamedEntity>> GetEntitiesAsync(Jira jira, CancellationToken token)
        {
            var results = await jira.IssueTypes.GetIssueTypesAsync(token).ConfigureAwait(false);
            return results as IEnumerable<JiraNamedEntity>;
        }

        /// <summary>
        /// Allows assignation by name
        /// </summary>
        public static implicit operator IssueType(string name)
        {
            if (name != null)
            {
                int id;
                if (int.TryParse(name, out id))
                {
                    return new IssueType(name /*as id*/);
                }
                else
                {
                    return new IssueType(null, name);
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Operator overload to simplify LINQ queries
        /// </summary>
        /// <remarks>
        /// Allows calls in the form of issue.Priority == "High"
        /// </remarks>
        public static bool operator ==(IssueType entity, string name)
        {
            if ((object)entity == null)
            {
                return name == null;
            }
            else if (name == null)
            {
                return false;
            }
            else
            {
                return entity.Name == name;
            }
        }

        /// <summary>
        /// Operator overload to simplify LINQ queries
        /// </summary>
        /// <remarks>
        /// Allows calls in the form of issue.Priority != "High"
        /// </remarks>
        public static bool operator !=(IssueType entity, string name)
        {
            if ((object)entity == null)
            {
                return name != null;
            }
            else if (name == null)
            {
                return true;
            }
            else
            {
                return entity.Name != name;
            }
        }
    }
}

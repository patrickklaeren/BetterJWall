using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira.Remote;

namespace Atlassian.Jira
{
    /// <summary>
    /// The priority of the issue as defined in JIRA
    /// </summary>
    [SuppressMessage("N/A", "CS0660", Justification = "Operator overloads are used for LINQ to JQL provider.")]
    [SuppressMessage("N/A", "CS0661", Justification = "Operator overloads are used for LINQ to JQL provider.")]
    public class IssuePriority : JiraNamedConstant
    {
        /// <summary>
        /// Creates an instance of the IssuePriority based on a remote entity.
        /// </summary>
        public IssuePriority(RemotePriority remoteEntity)
            : base(remoteEntity)
        {
        }

        /// <summary>
        /// Creates an instance of the IssuePriority with the given id and name.
        /// </summary>
        public IssuePriority(string id, string name = null)
            : base(id, name)
        {
        }

        protected override async Task<IEnumerable<JiraNamedEntity>> GetEntitiesAsync(Jira jira, CancellationToken token)
        {
            var priorities = await jira.Priorities.GetPrioritiesAsync(token).ConfigureAwait(false);
            return priorities as IEnumerable<JiraNamedEntity>;
        }

        /// <summary>
        /// Allows assignation by name
        /// </summary>
        public static implicit operator IssuePriority(string name)
        {
            if (name != null)
            {
                int id;
                if (int.TryParse(name, out id))
                {
                    return new IssuePriority(name /*as id*/);
                }
                else
                {
                    return new IssuePriority(null, name);
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
        public static bool operator ==(IssuePriority entity, string name)
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
        public static bool operator !=(IssuePriority entity, string name)
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

        public static bool operator >(IssuePriority field, string value)
        {
            throw new NotImplementedException();
        }

        public static bool operator <(IssuePriority field, string value)
        {
            throw new NotImplementedException();
        }

        public static bool operator <=(IssuePriority field, string value)
        {
            throw new NotImplementedException();
        }

        public static bool operator >=(IssuePriority field, string value)
        {
            throw new NotImplementedException();
        }
    }
}

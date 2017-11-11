using System;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents a Jira entity with a unique identifier.
    /// </summary>
    public interface IJiraEntity
    {
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
        string Id { get; }
    }
}

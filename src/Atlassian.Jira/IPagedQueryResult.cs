using System;
using System.Collections.Generic;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents a query result for a resource that supports pagination.
    /// </summary>
    public interface IPagedQueryResult<T> : IEnumerable<T>
    {
        /// <summary>
        /// The maximum number of items included on each page.
        /// </summary>
        int ItemsPerPage { get; }

        /// <summary>
        /// The index of the first item in the paged result.
        /// </summary>
        int StartAt { get; }

        /// <summary>
        /// The total number of items.
        /// </summary>
        int TotalItems { get; }
    }
}

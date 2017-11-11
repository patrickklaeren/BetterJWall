using System;

namespace Atlassian.Jira
{
    /// <summary>
    /// Additional data to update when executing a workflow transition.
    /// </summary>
    public class WorkflowTransitionUpdates
    {
        /// <summary>
        /// Comment to add to issue when executing a workflow transition.
        /// </summary>
        public string Comment { get; set; }
    }
}

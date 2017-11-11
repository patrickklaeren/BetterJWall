using System;

namespace Atlassian.Jira
{
    /// <summary>
    /// The worklog time remaining strategy
    /// </summary>
    public enum WorklogStrategy
    {
        AutoAdjustRemainingEstimate,
        RetainRemainingEstimate,
        NewRemainingEstimate
    }
}

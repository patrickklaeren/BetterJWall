using System;
using Newtonsoft.Json;

namespace Atlassian.Jira
{
    /// <summary>
    /// Time tracking information for an issue.
    /// </summary>
    public class IssueTimeTrackingData
    {
        [JsonProperty("originalEstimate")]
        public string OriginalEstimate { get; private set; }

        [JsonProperty("originalEstimateSeconds")]
        public int OriginalEstimateInSeconds { get; private set; }

        [JsonProperty("remainingEstimate")]
        public string RemainingEstimate { get; private set; }

        [JsonProperty("remainingEstimateSeconds")]
        public int RemainingEstimateInSeconds { get; private set; }

        [JsonProperty("timeSpent")]
        public string TimeSpent { get; private set; }

        [JsonProperty("timeSpentSeconds")]
        public int TimeSpentInSeconds { get; private set; }
    }
}

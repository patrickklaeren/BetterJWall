namespace BetterJWall.JIRABridge
{
    public static class Constants
    {
        /// <summary>
        /// There is an arbitrary limit within the Atlassian
        /// API that limits all REST calls to a result set of
        /// 20 entities. We need to reflect this when fetching
        /// data from the API.
        /// </summary>
        public const int JIRA_API_QUERY_LIMIT = 20;

        public const string JIRA_IN_PROGRESS_STATUS_KEY = "In Progress";
        public const string JIRA_IN_REVIEW_STATUS_KEY = "Review";
        public const string JIRA_DONE_STATUS_KEY = "Done";
        public const string JIRA_EPIC_TYPE_KEY = "Epic";
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using BetterJWall.Common;
using BetterJWall.JIRABridge.Client;
using BetterJWall.JIRABridge.Services.Issues.DTOs;

namespace BetterJWall.JIRABridge.Services.Issues
{
    public interface IJiraIssueService
    {
        /// <summary>
        /// Get all issues in progress for the given project
        /// </summary>
        /// <param name="projectKey">JIRA Project Key</param>
        /// <returns>List of JIRA issues in progress for a project</returns>
        IEnumerable<JiraCaseDto> GetInProgressForProject([NotNull] string projectKey);

        /// <summary>
        /// Get all issues in review for the given project
        /// </summary>
        /// <param name="projectKey">JIRA Project Key</param>
        /// <returns>List of JIRA issues in progress for a project</returns>
        IEnumerable<JiraCaseDto> GetInReviewForProject([NotNull] string projectKey);

        /// <summary>
        /// Get all issues in review for the given project
        /// </summary>
        /// <param name="projectKey">JIRA Project Key</param>
        /// <returns>List of JIRA issues in progress for a project</returns>
        IEnumerable<JiraCaseDto> GetDoneForProject([NotNull] string projectKey);
    }

    public class JiraIssueService : IJiraIssueService
    {
        private readonly IJiraClient _jiraClient;
        private readonly IDateTimeFunctions _dateTimeFunctions;

        public JiraIssueService(IJiraClient jiraClient, IDateTimeFunctions dateTimeFunctions)
        {
            _jiraClient = jiraClient;
            _dateTimeFunctions = dateTimeFunctions;
        }

        public IEnumerable<JiraCaseDto> GetInProgressForProject([NotNull] string projectKey)
        {
            var existentIssueKeys = new List<string>();
            var returned = new List<Issue>();
            var isOver = true;

            // JIRA's API limit is set to 20 as of this project, therefore
            // iterate until we get all the issues, we can only infer this
            // once a query returns less than 20 issues
            while (isOver)
            {
                // Get the queried issues, where the keys don't match
                // the already obtained issues
                var queryable = _jiraClient
                    .Issues
                    .Queryable
                    .Where(x => x.Project == projectKey)
                    .Where(x => x.Status == Constants.JIRA_IN_PROGRESS_STATUS_KEY);

                queryable = existentIssueKeys
                    .Aggregate(queryable, (current, existentIssueKey) =>
                        current.Where(x => x.JiraIdentifier != existentIssueKey));

                var queriedIssues = queryable.ToList();

                // Add the obtained issues to the existent keys
                existentIssueKeys.AddRange(queriedIssues.Select(x => x.JiraIdentifier));

                returned.AddRange(queriedIssues);

                // Reevaluate if we need to query JIRA again
                isOver = queriedIssues.Count >= Constants.JIRA_API_QUERY_LIMIT;
            }

            return returned
                .Where(x => x.Type.Name != Constants.JIRA_EPIC_TYPE_KEY)
                .Select(x => new JiraCaseDto(x));
        }

        public IEnumerable<JiraCaseDto> GetInReviewForProject([NotNull] string projectKey)
        {
            var existentIssueKeys = new List<string>();
            var returned = new List<Issue>();
            var isOver = true;

            // JIRA's API limit is set to 20 as of this project, therefore
            // iterate until we get all the issues, we can only infer this
            // once a query returns less than 20 issues
            while (isOver)
            {
                // Get the queried issues, where the keys don't match
                // the already obtained issues
                var queryable = _jiraClient
                    .Issues
                    .Queryable
                    .Where(x => x.Project == projectKey)
                    .Where(x => x.Status == Constants.JIRA_IN_REVIEW_STATUS_KEY);

                queryable = existentIssueKeys
                    .Aggregate(queryable, (current, existentIssueKey) =>
                        current.Where(x => x.JiraIdentifier != existentIssueKey));

                var queriedIssues = queryable.ToList();

                // Add the obtained issues to the existent keys
                existentIssueKeys.AddRange(queriedIssues.Select(x => x.JiraIdentifier));

                returned.AddRange(queriedIssues);

                // Reevaluate if we need to query JIRA again
                isOver = queriedIssues.Count >= Constants.JIRA_API_QUERY_LIMIT;
            }

            return returned
                .Where(x => x.Type.Name != Constants.JIRA_EPIC_TYPE_KEY)
                .Select(x => new JiraCaseDto(x));
        }

        public IEnumerable<JiraCaseDto> GetDoneForProject([NotNull] string projectKey)
        {
            const int DEFAULT_DAYS_TO_LOOK_BACK = 1;
            const int DAYS_TO_LOOK_BACK_ON_WEEKEND = 3;

            var daysToLookBack = DEFAULT_DAYS_TO_LOOK_BACK;

            if (_dateTimeFunctions.Date().DayOfWeek == DayOfWeek.Monday)
                daysToLookBack = DAYS_TO_LOOK_BACK_ON_WEEKEND;

            var lookupFrom = _dateTimeFunctions.Date().AddDays(-daysToLookBack);

            var existentIssueKeys = new List<string>();
            var returned = new List<Issue>();
            var isOver = true;

            // JIRA's API limit is set to 20 as of this project, therefore
            // iterate until we get all the issues, we can only infer this
            // once a query returns less than 20 issues
            while (isOver)
            {
                // Get the queried issues, where the keys don't match
                // the already obtained issues
                var queryable = _jiraClient
                    .Issues
                    .Queryable
                    .Where(x => x.Project == projectKey)
                    .Where(x => x.ResolutionDate != null)
                    .Where(x => x.ResolutionDate >= lookupFrom)
                    .Where(x => x.Status == Constants.JIRA_DONE_STATUS_KEY);

                queryable = existentIssueKeys
                    .Aggregate(queryable, (current, existentIssueKey) =>
                        current.Where(x => x.JiraIdentifier != existentIssueKey));

                var queriedIssues = queryable.ToList();

                // Add the obtained issues to the existent keys
                existentIssueKeys.AddRange(queriedIssues.Select(x => x.JiraIdentifier));

                returned.AddRange(queriedIssues);

                // Reevaluate if we need to query JIRA again
                isOver = queriedIssues.Count >= Constants.JIRA_API_QUERY_LIMIT;
            }

            return returned
                .Where(x => x.Type.Name != Constants.JIRA_EPIC_TYPE_KEY)
                .Select(x => new JiraCaseDto(x));
        }
    }
}
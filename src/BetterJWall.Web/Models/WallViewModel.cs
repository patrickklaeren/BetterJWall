using System;
using System.Collections.Generic;
using System.Linq;
using BetterJWall.Common;
using BetterJWall.JIRABridge.Services.Issues.DTOs;

namespace BetterJWall.Web.Models
{
    public class WallViewModel
    {
        private readonly IEnumerable<IssueViewModel> _issues;

        public WallViewModel(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public WallViewModel(string serverEndpoint, string projectKey, IEnumerable<JiraCaseDto> jiraIssues)
        {
            ProjectKey = projectKey;
            LastDataFetch = DateTime.Now;
            _issues = jiraIssues.Select(a => new IssueViewModel(a));
            ServerEndpoint = serverEndpoint;
        }

        public string ServerEndpoint { get; }

        public bool IsInErrorState => string.IsNullOrWhiteSpace(ErrorMessage) == false;

        public string ErrorMessage { get; }

        public string ProjectKey { get; }

        public DateTime LastDataFetch { get; }

        public List<IssueViewModel> InProgressIssues =>
            _issues.Where(x => x.Status == IssueStatus.InProgress).ToList();

        public List<IssueViewModel> InReviewIssues =>
            _issues.Where(x => x.Status == IssueStatus.InReview).ToList();

        public List<IssueViewModel> DoneIssues =>
            _issues.Where(x => x.Status == IssueStatus.Done).ToList();

        public class IssueViewModel
        {
            public IssueViewModel(JiraCaseDto dto)
            {
                IssueKey = dto.IssueKey;
                Assignee = dto.Assignee;
                Summary = dto.Summary;
                Status = dto.Status;
                IssueTypeIconUrl = dto.IssueTypeIconUrl;
            }

            public string IssueKey { get; }
            public string Assignee { get; }
            public string Summary { get; }
            public IssueStatus Status { get; set; }
            public string IssueTypeIconUrl { get; }
        }
    }
}

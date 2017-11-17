using System;
using System.Diagnostics;
using System.Linq;
using BetterJWall.Common;
using BetterJWall.JIRABridge.Services.Issues;
using Microsoft.AspNetCore.Mvc;
using BetterJWall.Web.Models;

namespace BetterJWall.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IJiraIssueService _jiraIssueService;
        private readonly IConfigurationHelper _configurationHelper;

        public HomeController(IJiraIssueService jiraIssueService, IConfigurationHelper configurationHelper)
        {
            _jiraIssueService = jiraIssueService;
            _configurationHelper = configurationHelper;
        }

        public IActionResult Index()
        {
            var serverEndpoint = _configurationHelper.JiraServerEndpoint;

            var model = new HomeViewModel(serverEndpoint);

            return View(model);
        }

        [Route("Wall/{projectKey}")]
        public IActionResult Wall(string projectKey)
        {
            var serverEndpoint = _configurationHelper.JiraServerEndpoint;

            WallViewModel model;

            try
            {
                var inProgressIssues = _jiraIssueService.GetInProgressForProject(projectKey);
                var inReviewIssues = _jiraIssueService.GetInReviewForProject(projectKey);
                var doneIssues = _jiraIssueService.GetDoneForProject(projectKey);

                var allIssues = inProgressIssues.Concat(inReviewIssues).Concat(doneIssues);

                model = new WallViewModel(serverEndpoint, projectKey, allIssues);
            }
            catch (Exception e)
            {
                model = new WallViewModel(e.Message);
            }

            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

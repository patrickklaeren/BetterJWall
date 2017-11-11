namespace BetterJWall.Web.Models
{
    public class HomeViewModel
    {
        public HomeViewModel(string jiraServerEndpoint)
        {
            JiraServerEndpoint = jiraServerEndpoint;
        }

        public string JiraServerEndpoint { get; }
    }
}

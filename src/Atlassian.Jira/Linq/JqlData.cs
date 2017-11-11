using System;

namespace Atlassian.Jira.Linq
{
    public class JqlData
    {
        public string Expression { get; set; }
        public int? NumberOfResults { get; set; }
        public int? SkipResults { get; set; }
    }
}

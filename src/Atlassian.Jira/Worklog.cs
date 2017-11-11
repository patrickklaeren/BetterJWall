using System;
using Atlassian.Jira.Remote;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents the worklog of an issue
    /// </summary>
    public class Worklog
    {
        private readonly DateTime? _created;
        private readonly string _id;
        private readonly long _timeSpentInSeconds;
        private readonly DateTime? _updated;

        public string Author { get; set; }
        public string Comment { get; set; }
        public DateTime? StartDate { get; set; }
        public string TimeSpent { get; set; }

        public string Id
        {
            get { return _id; }
        }

        public long TimeSpentInSeconds
        {
            get { return _timeSpentInSeconds; }
        }

        public DateTime? CreateDate
        {
            get { return _created; }
        }

        public DateTime? UpdateDate
        {
            get { return _updated; }
        }

        /// <summary>
        /// Creates a new worklog instance
        /// </summary>
        /// <param name="timeSpent">Specifies a time duration in JIRA duration format, representing the time spent working</param>
        /// <param name="startDate">When the work was started</param>
        /// <param name="comment">An optional comment to describe the work</param>
        public Worklog(string timeSpent, DateTime startDate, string comment = null)
        {
            this.TimeSpent = timeSpent;
            this.StartDate = startDate;
            this.Comment = comment;
        }

        internal Worklog(RemoteWorklog remoteWorklog)
        {
            if (remoteWorklog != null)
            {
                this.Author = remoteWorklog.author;
                this.Comment = remoteWorklog.comment;
                this.StartDate = remoteWorklog.startDate;
                this.TimeSpent = remoteWorklog.timeSpent;
                _id = remoteWorklog.id;
                _created = remoteWorklog.created;
                _timeSpentInSeconds = remoteWorklog.timeSpentInSeconds;
                _updated = remoteWorklog.updated;
            }
        }

        internal RemoteWorklog ToRemote()
        {
            return new RemoteWorklog()
            {
                author = this.Author,
                comment = this.Comment,
                startDate = this.StartDate,
                timeSpent = this.TimeSpent
            };
        }
    }
}

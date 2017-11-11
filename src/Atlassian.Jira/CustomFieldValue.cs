using System;
using System.Linq;

namespace Atlassian.Jira
{
    /// <summary>
    /// A custom field associated with an issue
    /// </summary>
    public class CustomFieldValue
    {
        private readonly string _id;
        private readonly Issue _issue;
        private string _name;

        internal CustomFieldValue(string id, Issue issue)
        {
            _id = id;
            _issue = issue;
        }

        internal CustomFieldValue(string id, string name, Issue issue)
            : this(id, issue)
        {
            _name = name;
        }

        /// <summary>
        /// The values of the custom field
        /// </summary>
        public string[] Values
        {
            get;
            set;
        }

        /// <summary>
        /// Id of the custom field as defined in JIRA
        /// </summary>
        public string Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Name of the custom field as defined in JIRA
        /// </summary>
        public string Name
        {
            get
            {
                if (String.IsNullOrEmpty(_name))
                {
                    var customField = _issue.Jira.Fields.GetCustomFieldsAsync().Result.FirstOrDefault(f => f.Id == _id);
                    if (customField == null)
                    {
                        throw new InvalidOperationException(String.Format("Custom field with id '{0}' was not found.", _id));
                    }

                    _name = customField.Name;
                }

                return _name;
            }
        }
    }
}

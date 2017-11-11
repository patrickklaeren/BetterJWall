using System;

namespace Atlassian.Jira
{
    /// <summary>
    /// Represents the values of a cascading select list custom field.
    /// </summary>
    public class CascadingSelectCustomField
    {
        private readonly string _name;
        private readonly string _parentOption;
        private readonly string _childOption;

        /// <summary>
        /// Creates a new instance of a CascadingSelectCustomField.
        /// </summary>
        /// <param name="name">The name of the custom field.</param>
        /// <param name="parentOption">The value of the parent option.</param>
        /// <param name="childOption">The value of the child option.</param>
        public CascadingSelectCustomField(string name, string parentOption, string childOption)
        {
            this._name = name;
            this._parentOption = parentOption;
            this._childOption = childOption;
        }

        /// <summary>
        /// The name of this custom field.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The value of the parent option.
        /// </summary>
        public string ParentOption
        {
            get { return _parentOption; }
        }

        /// <summary>
        /// The value of the child option.
        /// </summary>
        public string ChildOption
        {
            get { return _childOption; }
        }
    }
}

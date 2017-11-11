using System;
using System.Diagnostics.CodeAnalysis;

namespace Atlassian.Jira
{
    /// <summary>
    /// Force a CustomField comparison to use the exact match JQL operator.
    /// </summary>
    [SuppressMessage("N/A", "CS0660", Justification = "Operator overloads are used for LINQ to JQL provider.")]
    [SuppressMessage("N/A", "CS0661", Justification = "Operator overloads are used for LINQ to JQL provider.")]
    public class LiteralMatch
    {
        private readonly string _value;

        public LiteralMatch(string value)
        {
            this._value = value;
        }

        public override string ToString()
        {
            return _value;
        }

        public static bool operator ==(ComparableString comparable, LiteralMatch literal)
        {
            if ((object)comparable == null)
            {
                return literal == null;
            }
            else
            {
                return comparable.Value == literal._value;
            }
        }

        public static bool operator !=(ComparableString comparable, LiteralMatch literal)
        {
            if ((object)comparable == null)
            {
                return literal != null;
            }
            else
            {
                return comparable.Value != literal._value;
            }
        }
    }
}

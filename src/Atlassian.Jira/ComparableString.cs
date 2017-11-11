using System;

namespace Atlassian.Jira
{
    /// <summary>
    /// Type that wraps a string and exposes operator overloads for
    /// easier LINQ queries
    /// </summary>
    /// <remarks>
    /// Allows comparisons in the form of issue.Key > "TST-1"
    /// </remarks>
    public class ComparableString
    {
        public string Value { get; set; }

        public ComparableString(string value)
        {
            this.Value = value;
        }

        public static implicit operator ComparableString(string value)
        {
            if (value != null)
            {
                return new ComparableString(value);
            }
            else
            {
                return null;
            }
        }

        public static bool operator ==(ComparableString field, string value)
        {
            if ((object) field == null)
            {
                return value == null;
            }
            else
            {
                return field.Value == value;
            }
        }

        public static bool operator !=(ComparableString field, string value)
        {
            if ((object) field == null)
            {
                return value != null;
            }
            else
            {
                return field.Value != value;
            }
        }

        public static bool operator >(ComparableString field, string value)
        {
            return field.Value.CompareTo(value) > 0;
        }

        public static bool operator <(ComparableString field, string value)
        {
            return field.Value.CompareTo(value) < 0;
        }

        public static bool operator <=(ComparableString field, string value)
        {
            return field.Value.CompareTo(value) <= 0;
        }

        public static bool operator >=(ComparableString field, string value)
        {
            return field.Value.CompareTo(value) >= 0;
        }

        public static bool operator ==(ComparableString field, DateTime value)
        {
            if ((object)field == null)
            {
                return value == null;
            }
            else
            {
                return field.Value == value.ToString(Jira.DEFAULT_DATE_FORMAT, Jira.DefaultCultureInfo);
            }
        }

        public static bool operator !=(ComparableString field, DateTime value)
        {
            if ((object)field == null)
            {
                return value != null;
            }
            else
            {
                return field.Value != value.ToString(Jira.DEFAULT_DATE_FORMAT, Jira.DefaultCultureInfo);
            }
        }

        public static bool operator >(ComparableString field, DateTime value)
        {
            return field.Value.CompareTo(value.ToString(Jira.DEFAULT_DATE_FORMAT, Jira.DefaultCultureInfo)) > 0;
        }

        public static bool operator <(ComparableString field, DateTime value)
        {
            return field.Value.CompareTo(value.ToString(Jira.DEFAULT_DATE_FORMAT, Jira.DefaultCultureInfo)) < 0;
        }

        public static bool operator <=(ComparableString field, DateTime value)
        {
            return field.Value.CompareTo(value.ToString(Jira.DEFAULT_DATE_FORMAT, Jira.DefaultCultureInfo)) <= 0;
        }

        public static bool operator >=(ComparableString field, DateTime value)
        {
            return field.Value.CompareTo(value.ToString(Jira.DEFAULT_DATE_FORMAT, Jira.DefaultCultureInfo)) >= 0;
        }

        public override string ToString()
        {
            return this.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ComparableString)
            {
                return this.Value.Equals(((ComparableString)obj).Value);
            }
            else if (obj is string)
            {
                return this.Value.Equals((string)obj);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            if (this.Value == null)
            {
                return 0;
            }
            return this.Value.GetHashCode();
        }
    }
}

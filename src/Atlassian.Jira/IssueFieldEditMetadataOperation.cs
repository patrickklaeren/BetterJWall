using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Atlassian.Jira
{
    /// <summary>
    /// Possible values of operations property in IssueFieldEditMetadata.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum IssueFieldEditMetadataOperation
    {
        [EnumMember(Value = "SET")]
        SET = 1,
        [EnumMember(Value = "ADD")]
        ADD = 2,
        [EnumMember(Value = "REMOVE")]
        REMOVE = 3,
        [EnumMember(Value = "EDIT")]
        EDIT = 4
    }
}

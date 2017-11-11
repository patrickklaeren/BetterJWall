using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Atlassian.Jira
{
    /// <summary>
    /// This class is used as output of /// http://example.com:8080/jira/rest/api/2/issue/{issueIdOrKey}/editmeta [GET]
    ///  </summary>
    public class IssueFieldEditMetadata
    {
        /// <summary>
        /// Creates a new instance of IssueFieldEditMetadata.
        /// </summary>
        public IssueFieldEditMetadata()
        {
            AllowedValues = new JArray();
        }

        /// <summary>
        /// Whether this is a custom field.
        /// </summary>
        public bool IsCustom
        {
            get
            {
                return Schema.Custom != null;
            }
        }

        /// <summary>
        /// Whether the field is required.
        /// </summary>
        [JsonProperty("required")]
        public bool IsRequired { get; private set; }

        /// <summary>
        /// Schema of this field.
        /// </summary>
        [JsonProperty("schema")]
        public IssueFieldEditMetadataSchema Schema { get; private set; }

        /// <summary>
        /// Name of this field.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; private set; }

        /// <summary>
        /// The url to use in autocompletion.
        /// </summary>
        [JsonProperty("autoCompleteUrl")]
        public string AutoCompleteUrl { get; private set; }

        /// <summary>
        /// Operations that can be done on this field.
        /// </summary>
        [JsonProperty("operations")]
        public IList<IssueFieldEditMetadataOperation> Operations { get; private set; }

        /// <summary>
        /// List of available allowed values that can be set. All objects in this array are of the same type.
        /// However there is multiple possible types it could be.
        /// You should decide what the type it is and convert to custom implemented type by yourself.
        /// </summary>
        [JsonProperty("allowedValues")]
        public JArray AllowedValues { get; private set; }

        /// <summary>
        /// List of field's available allowed values as object of class T which is ought to be implemented by user of this method.
        /// Conversion from serialized JObject to custom class T takes here place.
        /// </summary>
        public IEnumerable<T> AllowedValuesAs<T>()
        {
            return AllowedValues.Values<JObject>().Select(x => x.ToObject<T>());
        }

    }
}

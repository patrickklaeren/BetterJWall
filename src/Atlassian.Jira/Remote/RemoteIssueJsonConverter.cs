using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Atlassian.Jira.Remote
{
    public class RemoteIssueWrapper
    {
        private readonly RemoteIssue _remoteIssue;
        private readonly string _parentIssueKey;

        public RemoteIssueWrapper(RemoteIssue remoteIssue, string parentIssueKey = null)
        {
            _remoteIssue = remoteIssue;
            _parentIssueKey = parentIssueKey;
        }

        public RemoteIssue RemoteIssue
        {
            get
            {
                return _remoteIssue;
            }
        }

        public string ParentIssueKey
        {
            get
            {
                return this._parentIssueKey;
            }
        }
    }

    public class RemoteIssueJsonConverter : JsonConverter
    {
        private readonly IEnumerable<RemoteField> _remoteFields;
        private readonly IDictionary<string, ICustomFieldValueSerializer> _customFieldSerializers;
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public RemoteIssueJsonConverter(IEnumerable<RemoteField> remoteFields, IDictionary<string, ICustomFieldValueSerializer> customFieldSerializers)
        {
            this._remoteFields = remoteFields;
            this._customFieldSerializers = customFieldSerializers;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RemoteIssueWrapper);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var issueObj = JObject.Load(reader);
            var fields = issueObj["fields"] as JObject;

            // deserialize the RemoteIssue from the fields json.
            var remoteIssue = JsonConvert.DeserializeObject<RemoteIssue>(fields.ToString(), this._serializerSettings);

            // set the id and key of the remoteissue.
            remoteIssue.id = (string)issueObj["id"];
            remoteIssue.key = (string)issueObj["key"];

            // load the custom fields
            var customFields = GetCustomFieldValuesFromObject(fields);
            remoteIssue.customFieldValues = customFields.Any() ? customFields.ToArray() : null;

            return new RemoteIssueWrapper(remoteIssue);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var issueWrapper = value as RemoteIssueWrapper;
            if (issueWrapper == null)
            {
                throw new InvalidOperationException(String.Format("value must be of type {0}.", typeof(RemoteIssueWrapper)));
            }

            var issue = issueWrapper.RemoteIssue;

            // Round trip the remote issue to get a JObject that has all the fields in the proper format.
            var issueJson = JsonConvert.SerializeObject(issue, _serializerSettings);
            var fields = JObject.Parse(issueJson);

            // Add the custom fields as additional JProperties.
            AddCustomFieldValuesToObject(issue, fields);

            // Add a field for the parent issue if this is a sub-task
            if (!String.IsNullOrEmpty(issueWrapper.ParentIssueKey))
            {
                fields.Add("parent", JObject.FromObject(new
                {
                    key = issueWrapper.ParentIssueKey
                }));
            }

            var wrapper = new JObject(new JProperty("fields", fields));
            wrapper.WriteTo(writer);
        }

        private string GetCustomFieldType(string customFieldId)
        {
            var remoteField = this._remoteFields.FirstOrDefault(f => f.id.Equals(customFieldId, StringComparison.InvariantCultureIgnoreCase));

            if (remoteField == null)
            {
                throw new InvalidOperationException($"Custom field with id '{customFieldId}' found on issue does not exist on the list of known custom fields returned by Jira.");
            }

            return remoteField.CustomFieldType;
        }

        private void AddCustomFieldValuesToObject(RemoteIssue remoteIssue, JObject jObject)
        {
            if (remoteIssue.customFieldValues != null)
            {
                foreach (var customField in remoteIssue.customFieldValues)
                {
                    if (customField.values != null)
                    {
                        var customFieldType = GetCustomFieldType(customField.customfieldId);
                        JToken jToken;

                        if (this._customFieldSerializers.ContainsKey(customFieldType))
                        {
                            jToken = this._customFieldSerializers[customFieldType].ToJson(customField.values);
                        }
                        else
                        {
                            jToken = JValue.CreateString(customField.values[0]);
                        }

                        jObject.Add(customField.customfieldId, jToken);
                    }
                }
            }
        }

        private IEnumerable<RemoteCustomFieldValue> GetCustomFieldValuesFromObject(JObject jObject)
        {
            return jObject.Values<JProperty>()
                .Where(field => field.Name.StartsWith("customfield", StringComparison.InvariantCulture) && field.Value.Type != JTokenType.Null)
                .Select(field =>
                {
                    var customFieldType = GetCustomFieldType(field.Name);
                    var remoteCustomFieldValue = new RemoteCustomFieldValue()
                    {
                        customfieldId = field.Name
                    };

                    if (this._customFieldSerializers.ContainsKey(customFieldType))
                    {
                        remoteCustomFieldValue.values = this._customFieldSerializers[customFieldType].FromJson(field.Value);
                    }
                    else
                    {
                        remoteCustomFieldValue.values = new string[1] { field.Value.ToString() };
                    }

                    return remoteCustomFieldValue;
                });
        }
    }
}

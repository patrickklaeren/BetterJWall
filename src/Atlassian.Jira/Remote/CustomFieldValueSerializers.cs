using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Atlassian.Jira.Remote
{
    public class SingleObjectCustomFieldValueSerializer : ICustomFieldValueSerializer
    {
        private readonly string _propertyName;

        public SingleObjectCustomFieldValueSerializer(string propertyName)
        {
            this._propertyName = propertyName;
        }

        public string[] FromJson(JToken json)
        {
            return new string[1] { json[this._propertyName].ToString() };
        }

        public JToken ToJson(string[] values)
        {
            return new JObject(new JProperty(this._propertyName, values[0]));
        }
    }

    public class MultiObjectCustomFieldValueSerializer : ICustomFieldValueSerializer
    {
        private readonly string _propertyName;

        public MultiObjectCustomFieldValueSerializer(string propertyName)
        {
            this._propertyName = propertyName;
        }

        public string[] FromJson(JToken json)
        {
            return ((JArray)json).Select(j => j[_propertyName].ToString()).ToArray();
        }

        public JToken ToJson(string[] values)
        {
            return JArray.FromObject(values.Select(v => new JObject(new JProperty(_propertyName, v))).ToArray());
        }
    }

    public class FloatCustomFieldValueSerializer : ICustomFieldValueSerializer
    {
        public string[] FromJson(JToken json)
        {
            return new string[1] { json.ToString() };
        }

        public JToken ToJson(string[] values)
        {
            return float.Parse(values[0]);
        }
    }

    public class MultiStringCustomFieldValueSerializer : ICustomFieldValueSerializer
    {
        public string[] FromJson(JToken json)
        {
            return JsonConvert.DeserializeObject<string[]>(json.ToString());
        }

        public JToken ToJson(string[] values)
        {
            return JArray.FromObject(values);
        }
    }

    public class CascadingSelectCustomFieldValueSerializer : ICustomFieldValueSerializer
    {
        public string[] FromJson(JToken json)
        {
            var parentOption = json["value"];
            var childOption = json["child"];

            if (parentOption == null)
            {
                throw new InvalidOperationException(String.Format(
                    "Unable to deserialize custom field as a cascading select list. The parent value is required. Json: {0}",
                    json.ToString()));
            }
            else if (childOption == null || childOption["value"] == null)
            {
                return new string[] { parentOption.ToString() };
            }
            else
            {
                return new string[2] { parentOption.ToString(), childOption["value"].ToString() };
            }
        }

        public JToken ToJson(string[] values)
        {
            if (values == null || values.Length < 1)
            {
                throw new InvalidOperationException("Unable to serialize the custom field as a cascading select list. At least the parent value is required.");
            }
            else if (values.Length == 1)
            {
                return JToken.FromObject(new { value = values[0] });
            }
            else
            {
                return JToken.FromObject(new
                {
                    value = values[0],
                    child = new
                    {
                        value = values[1]
                    }
                });
            }
        }
    }

    public class GreenhopperSprintCustomFieldValueSerialiser : ICustomFieldValueSerializer
    {
        private readonly string _propertyName;

        public GreenhopperSprintCustomFieldValueSerialiser(string propertyName)
        {
            this._propertyName = propertyName;
        }

        // Sprint field is malformed
        // See https://ecosystem.atlassian.net/browse/ACJIRA-918 for more information
        public string[] FromJson(JToken json)
        {
            return json.ToString()
                .Split(new char[] { '{', '}', '[', ']', ',' })
                .Where(x => x.StartsWith(_propertyName))
                .Select(x => x.Split(new char[] { '=' })[1])
                .ToArray();
        }

        public JToken ToJson(string[] values)
        {
            string val = values != null ? values.FirstOrDefault() : null;
            int id = 0;

            if (int.TryParse(val, out id))
            {
                return id;
            }

            return val;
        }
    }
}

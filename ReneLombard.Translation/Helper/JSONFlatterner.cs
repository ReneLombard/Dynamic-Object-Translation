using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ReneLombard.Translation.Helper
{
    public class JSONFlattener
    {
        private enum JSONType
        {
            OBJECT, ARRAY
        }
        public static Dictionary<(string path, string type), string> Flatten(JObject jsonObject)
        {
            IEnumerable<JToken> jTokens = jsonObject.Descendants().Where(p => p.Count() == 0);

            Dictionary<(string path, string type), string> results = jTokens.Aggregate(new Dictionary<(string path, string type), string>(), (properties, jToken) =>
            {
                properties.Add((jToken.Path, jToken.Type.ToString()), jToken.ToString());
                return properties;
            });
            return results;
        }

        public static JObject Unflatten(IDictionary<(string path, string type), string> keyValues)
        {
            JContainer result = null;
            JsonMergeSettings setting = new JsonMergeSettings();
            setting.MergeArrayHandling = MergeArrayHandling.Merge;
            foreach (var pathValue in keyValues)
            {
                if (result == null)
                {
                    result = UnflatenSingle(pathValue);
                }
                else
                {
                    result.Merge(UnflatenSingle(pathValue), setting);
                }
            }
            return result as JObject;
        }

        private static JContainer UnflatenSingle(KeyValuePair<(string path, string type), string> keyValue)
        {
            string path = keyValue.Key.path;
            string value = keyValue.Value;
            string objType = keyValue.Key.type;
            var pathSegments = SplitPath(path);

            JContainer lastItem = null;
            //build from leaf to root
            foreach (var pathSegment in pathSegments.Reverse())
            {
                var type = GetJSONType(pathSegment);

                switch (type)
                {
                    case JSONType.OBJECT:
                        var obj = new JObject();
                        if (null == lastItem)
                        {
                            obj.Add(NormalizeValues(pathSegment), ParseValue(value, objType));
                        }
                        else
                        {
                            obj.Add(NormalizeValues(pathSegment), lastItem);
                        }
                        lastItem = obj;
                        break;
                    case JSONType.ARRAY:
                        var array = new JArray();
                        int index = GetArrayIndex(NormalizeValues(pathSegment));
                        array = FillEmpty(array, index);
                        if (lastItem == null)
                        {
                            array[index] = value;
                        }
                        else
                        {
                            array[index] = lastItem;
                        }
                        lastItem = array;
                        break;
                }
            }
            return lastItem;
        }
        public static JToken ParseValue(string value, string type)
        {
            if (type.Contains("Object"))
                return JObject.Parse(value);

            if (type.Contains("Integer") && int.TryParse(value, out var integer))
            {
                return integer;
            }
            else if (type.Contains("Guid") && Guid.TryParse(value, out var guid))
            {
                return guid;
            }
            else if (type.Contains("Date") && DateTime.TryParse(value, out var date))
            {
                return date;
            }
            else if (type.Contains("String"))
            {
                return value;
            }
            else if (type.Contains("Float") && decimal.TryParse(value, out var deci))
            {
                return deci;
            }
            else if (type.Contains("Boolean") && Boolean.TryParse(value, out var bit))
            {
                return bit;
            }
            else if (type.Contains("Array") && value.Contains("[]"))
            {
                return new JArray();
            }
            return value;

        }

        public static string NormalizeValues(string value)
        {
            var temp = value;
            if (temp[0] == '\'') temp = temp.Substring(1);
            if (temp[temp.Length - 1] == '\'') temp = temp.Substring(0, temp.Length - 1);
            return temp;
        }
        public static IList<string> SplitPath(string path)
        {
            IList<string> result = new List<string>();
            Regex reg = new Regex(@"(?!\.)([^.^\[\]]+)|(?!\[)(\d+)(?=\])");
            foreach (Match match in reg.Matches(path))
            {
                result.Add(match.Value);
            }
            return result;
        }

        private static JArray FillEmpty(JArray array, int index)
        {
            for (int i = 0; i <= index; i++)
            {
                array.Add(null);
            }
            return array;
        }

        private static JSONType GetJSONType(string pathSegment)
        {
            int x;
            return int.TryParse(pathSegment, out x) ? JSONType.ARRAY : JSONType.OBJECT;
        }

        private static int GetArrayIndex(string pathSegment)
        {
            int result;
            if (int.TryParse(pathSegment, out result))
            {
                return result;
            }
            throw new Exception("Unable to parse array index: " + pathSegment);
        }

    }
}

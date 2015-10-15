using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONScrubber
{
    public class Scrubber 
    {
        private IDictionary<string, int> _keyLevels;

        public Scrubber()
        {
            _keyLevels = new Dictionary<string, int>();
        }

        public dynamic Scrub(string _samplePayload)
        {
            var converter = new ExpandoObjectConverter();
            var expando = JsonConvert.DeserializeObject<ExpandoObject>(_samplePayload, converter);
            var expandoDict = (IDictionary<string, object>)expando;

            PopulateLevels(expandoDict, 1);
            RemoveNestedRepeatedFields(expandoDict, 1);

            return expando;
        }

        private void RemoveNestedRepeatedFields(IDictionary<string, object> dict, int level)
        {
            foreach (var property in dict.Keys.ToList())
            {
                var value = dict[property];
                if (value is IEnumerable<dynamic>)
                {
                    foreach (var entry in (IEnumerable<dynamic>)value)
                    {
                        RemoveNestedRepeatedFields((IDictionary<string, object>)entry, level + 1);
                    }
                }
                if (_keyLevels[property] < level)
                {
                    dict.Remove(property);
                }
            }
        }

        private void PopulateLevels(IDictionary<string,object> dict, int level)
        {
            foreach (var property in dict.Keys.ToList())
            {
                var value = dict[property];
                if (value is IEnumerable<dynamic>)
                { 
                    foreach(var entry in (IEnumerable<dynamic>) value) {                       
                        PopulateLevels(((IDictionary<string, object>) entry), level + 1);
                    }
                }

                if (!_keyLevels.ContainsKey(property))
                {
                    _keyLevels.Add(property, level);
                } else if (_keyLevels[property] > level)
                {
                    _keyLevels[property] = level;
                }
            }
        }
    }
}

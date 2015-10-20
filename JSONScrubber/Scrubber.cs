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
        public Scrubber()
        {
        }

        public JObject ScrubToJObject(string _samplePayload)
        {
            JObject jobject = JObject.Parse(_samplePayload);
            
            RemoveNestedRepeatedFields(jobject.Root);

            return jobject;
        }

        public string ScrubToString(string _samplePayload)
        {
            var expando = ScrubToJObject(_samplePayload);                        
            return expando.ToString(Formatting.None);
        }

        private void RemoveNestedRepeatedFields(JToken jobject)
        {
            foreach(var jtoken in jobject.Children().ToList())
            {
                RemoveNestedRepeatedFields(jtoken);
                
                var boo = DoesParentHaveSaidKey(jtoken.Parent, jtoken.Path);
                if (boo)
                    jobject.Remove();
            }
        }

        private bool DoesParentHaveSaidKey(JContainer container, string tokenPath)
        {
            if (container == null)
                return false;

            var result = DoesParentHaveSaidKey(container.Parent, tokenPath);

            var key = tokenPath.Split('.').LastOrDefault();

            foreach (var desc in container.Values())
            {
                var descPath = desc.Path.Split('.');
                if (descPath.Length < tokenPath.Split('.').Length && descPath.Last() == key) { 
                    result = result || true;
                    break;  
                }
            }
            return result;
        }
    }
}

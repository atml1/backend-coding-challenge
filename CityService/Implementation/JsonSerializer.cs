using System.Collections.Generic;
using CityService.Interface;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Runtime.Serialization;

namespace CityService.Implementation
{
    /// <summary>
    /// Implementation of ISerializer that returns the string representation as a json object with the "suggestions" field.
    /// </summary>
    public class JsonSerializer : ISerializer
    {
        public string Serialize(IEnumerable<Suggestion> suggestions)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(WrapperObject));
            using (MemoryStream ms = new MemoryStream())
            {
                WrapperObject wrapperObject = new WrapperObject
                {
                    Suggestions = suggestions
                };

                serializer.WriteObject(ms, wrapperObject);

                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        [DataContract]
        private class WrapperObject
        {
            [DataMember(Name="suggestions")]
            public IEnumerable<Suggestion> Suggestions { get; set; }
        }
    }
}

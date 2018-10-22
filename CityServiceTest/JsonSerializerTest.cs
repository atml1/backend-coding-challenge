using CityService.Implementation;
using CityService.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace CityServiceTest
{
    [TestClass]
    public class JsonSerializerTest
    {
        [TestMethod]
        public void BasicTest()
        {
            JsonSerializer serializer = new JsonSerializer();
            string result = serializer.Serialize(new Suggestion[]
            {
                new Suggestion { Name = "London, ON, Canada", Latitude = "42.98339", Longitude = "-81.23304", Score = 0.9 },
                new Suggestion { Name = "London, OH, USA", Latitude = "39.88645", Longitude = "-83.44825", Score = 0.5 },
                new Suggestion { Name = "London, KY, USA", Latitude = "37.12898", Longitude = "-84.08326", Score = 0.5 },
                new Suggestion { Name = "Londontowne, MD, USA", Latitude = "38.93345", Longitude = "-76.54941", Score = 0.3 },
            });

            string expected = 
                @"{
                  ""suggestions"": [
                    {
                      ""name"": ""London, ON, Canada"",
                      ""latitude"": ""42.98339"",
                      ""longitude"": ""-81.23304"",
                      ""score"": 0.9
                    },
                    {
                      ""name"": ""London, OH, USA"",
                      ""latitude"": ""39.88645"",
                      ""longitude"": ""-83.44825"",
                      ""score"": 0.5
                    },
                    {
                      ""name"": ""London, KY, USA"",
                      ""latitude"": ""37.12898"",
                      ""longitude"": ""-84.08326"",
                      ""score"": 0.5
                    },
                    {
                      ""name"": ""Londontowne, MD, USA"",
                      ""latitude"": ""38.93345"",
                      ""longitude"": ""-76.54941"",
                      ""score"": 0.3
                    }
                  ]
                }";

            // compare both without whitespace
            expected = Regex.Replace(expected, @"\s+", "");
            result = Regex.Replace(result, @"\s+", "");
            Assert.AreEqual(expected, result);
        }
    }
}

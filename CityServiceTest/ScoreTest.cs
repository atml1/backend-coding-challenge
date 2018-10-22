using CityService.Implementation;
using CityService.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CityServiceTest
{
    [TestClass]
    public class ScoreTest
    {
        [TestMethod]
        public void BasicTest()
        {
            Scorer scorer = new Scorer();
            City city = new City
            {
                FullName = "London, ON, Canada",
                ShortName = "London",
                Latitude = 42.98339,
                Longitude = -81.23304
            };

            double score = scorer.GetScore(city, "London", null, null);
            Assert.AreEqual(1.00, score); // perfect name, perfect distance
            score = scorer.GetScore(city, "Lon", null, null);
            Assert.AreEqual(0.75, score); // half name, perfect distance
            score = scorer.GetScore(city, "", null, null);
            Assert.AreEqual(0.50, score); // no name, perfect distance

            score = scorer.GetScore(city, "London", 42.98339, -81.23304);
            Assert.AreEqual(1.00, score); // perfect name, perfect distance
            score = scorer.GetScore(city, "", 42.98339, -81.23304);
            Assert.AreEqual(0.50, score); // no name, perfect distance
        }


        [TestMethod]
        public void DistanceTest()
        {
            Scorer scorer = new Scorer();
            City city = new City
            {
                FullName = "Test 123456798",
                ShortName = "Test",
                Latitude = 90,
                Longitude = 0
            };

            double score = scorer.GetScore(city, "Test", 90, 0);
            Assert.AreEqual(1.00, score); // perfect name, perfect distance
            score = scorer.GetScore(city, "", 90, 0);
            Assert.AreEqual(0.50, score); // no name, perfect distance
            score = scorer.GetScore(city, "", -90, 180);
            Assert.AreEqual(0.00, score); // no name, distance as far as possible
            score = scorer.GetScore(city, "", -90, -180);
            Assert.AreEqual(0.00, score); // no name, distance as far as possible (other way)
            score = scorer.GetScore(city, "", 0, 90);
            Assert.AreEqual(0.25, score); // no name, distance half way
            score = scorer.GetScore(city, "", 0, -90);
            Assert.AreEqual(0.25, score); // no name, distance half way (other way)
        }
    }
}

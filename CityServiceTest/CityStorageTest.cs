using System;
using System.Collections.Generic;
using CityService.Implementation;
using CityService.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CityServiceTest
{
    [TestClass]
    public class CityStorageTest
    {
        [TestMethod]
        public void BasicTest()
        {
            MockStorage mockStorage = new MockStorage
            {
                Words = new List<string>(),
                AddCalled = false,
                AutoCompleteCalled = false
            };
            CityStorage storage = new CityStorage
            {
                WordStorage = mockStorage
            };

            // need to add the short name to the word storage
            City city1 = new City { FullName = "London, ON, Canada", ShortName = "London", Latitude = 42.98339, Longitude = -81.23304 };
            storage.AddCity(city1);
            CollectionAssert.AreEquivalent(new string[] { "London" }, mockStorage.Words);
            Assert.IsTrue(mockStorage.AddCalled);
            mockStorage.AddCalled = false;

            // the short name has been added already, so no need to add again
            City city2 = new City { FullName = "London, OH, USA", ShortName = "London", Latitude = 39.88645, Longitude = -83.44825 };
            storage.AddCity(city2);
            City city3 = new City { FullName = "London, KY, USA", ShortName = "London", Latitude = 37.12898, Longitude = -84.08326 };
            storage.AddCity(city3);
            Assert.IsFalse(mockStorage.AddCalled);

            // a new short name, so need to add it too
            City city4 = new City { FullName = "Londontowne, MD, USA", ShortName = "Londontowne", Latitude = 38.93345, Longitude = -76.54941 };
            storage.AddCity(city4);
            CollectionAssert.AreEquivalent(new string[] { "London", "Londontowne" }, mockStorage.Words);
            Assert.IsTrue(mockStorage.AddCalled);
            mockStorage.AddCalled = false;

            // when requesting city, should get the name from the word storage
            Assert.IsFalse(mockStorage.AutoCompleteCalled);
            IEnumerable<City> cities = storage.GetCitiesStartsWith("Lon");
            Assert.IsTrue(mockStorage.AutoCompleteCalled);
            CollectionAssert.AreEquivalent(new City[] { city1, city2, city3, city4 }, cities.ToList());
        }

        /// <summary>
        /// Dummy implementation for testing purposes
        /// </summary>
        private class MockStorage : IWordStorage
        {
            public List<string> Words { get; set; }
            public bool AddCalled { get; set; }
            public bool AutoCompleteCalled { get; set; }
            
            public void Add(string word)
            {
                Words.Add(word);
                AddCalled = true;
            }

            public IEnumerable<string> AutoComplete(string beginning)
            {
                AutoCompleteCalled = true;
                return Words;
            }

            public void Clear()
            {
                throw new NotImplementedException(); // method not used in test
            }
        }
    }
}

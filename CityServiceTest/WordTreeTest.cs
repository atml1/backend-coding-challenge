using CityService.Implementation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CityServiceTest
{
    [TestClass]
    public class WordTreeTest
    {
        [TestMethod]
        public void BasicTest()
        {
            WordTree wordTree = new WordTree();
            wordTree.Add("London");
            wordTree.Add("Londontowne");

            IEnumerable<string> words = wordTree.AutoComplete("Lond");
            CollectionAssert.AreEquivalent(new string[] { "London", "Londontowne" }, words.ToList());
            words = wordTree.AutoComplete("London");
            CollectionAssert.AreEquivalent(new string[] { "London", "Londontowne" }, words.ToList());
            words = wordTree.AutoComplete("Londont");
            CollectionAssert.AreEquivalent(new string[] { "Londontowne" }, words.ToList());
            words = wordTree.AutoComplete("Londontowne");
            CollectionAssert.AreEquivalent(new string[] { "Londontowne" }, words.ToList());
            words = wordTree.AutoComplete("Londontownes");
            CollectionAssert.AreEquivalent(new string[] { }, words.ToList());
            words = wordTree.AutoComplete("Test");
            CollectionAssert.AreEquivalent(new string[] { }, words.ToList());

            wordTree.Clear();

            words = wordTree.AutoComplete("Lond");
            CollectionAssert.AreEquivalent(new string[] { }, words.ToList());
        }
    }
}

using CityService.Interface;
using System.Collections.Generic;

namespace CityService.Implementation
{
    /// <summary>
    /// Parses words storing them in a tree where each node is a letter. Each path from the root to the leaf
    /// represents a word. A leaf is represented by a node whose IsTerminal flag is set.
    /// </summary>
    public class WordTree : IWordStorage
    {
        private LetterNode _root = new LetterNode();

        public void Add(string word)
        {
            LetterNode currentNode = _root;
            for (int i = 0; i < word.Length; ++i)
            {
                char currentCharacter = word[i];
                if (!currentNode.Children.ContainsKey(currentCharacter))
                {
                    currentNode.Children[currentCharacter] = new LetterNode();
                }
                currentNode = currentNode.Children[currentCharacter];
            }
            currentNode.IsTerminal = true;
        }

        public IEnumerable<string> AutoComplete(string beginning)
        {
            LetterNode currentNode = _root;
            for (int i = 0; i < beginning.Length; ++i)
            {
                char currentCharacter = beginning[i];
                if (currentNode.Children.ContainsKey(currentCharacter))
                {
                    currentNode = currentNode.Children[currentCharacter];
                }
                else
                {
                    currentNode = null;
                    break;
                }
            }
            
            List<string> words = new List<string>();
            if (currentNode != null) // null if there are no matches
            {
                GetWords(currentNode, beginning, words);
            }
            return words;
        }

        public void Clear()
        {
            _root = new LetterNode();
        }

        // depth first search to get all the words
        private void GetWords(LetterNode node, string currentWord, List<string> words)
        {
            foreach (char character in node.Children.Keys)
            {
                GetWords(node.Children[character], currentWord + character, words);
            }
            if (node.IsTerminal)
            {
                words.Add(currentWord);
                return;
            }
        }

        private class LetterNode
        {
            public Dictionary<char, LetterNode> Children { get; private set; }
            public bool IsTerminal { get; set; }

            public LetterNode()
            {
                Children = new Dictionary<char, LetterNode>();
                IsTerminal = false;
            }
        }
    }
}

using System.Collections.Generic;

namespace CityService.Interface
{
    /// <summary>
    /// Store words to be looked up by the characters that the word starts with.
    /// </summary>
    public interface IWordStorage
    {
        /// <summary>
        /// Clears all words from the storage.
        /// </summary>
        void Clear();

        /// <summary>
        /// Adds a word to the storage.
        /// </summary>
        /// <param name="word">The word to add to the storage.</param>
        void Add(string word);

        /// <summary>
        /// Returns all possible words in the storage that begin with the specified characters.
        /// </summary>
        /// <param name="beginning">The first characters of the words to look up.</param>
        /// <returns>All words in the storage that begin with the specified characters.</returns>
        IEnumerable<string> AutoComplete(string beginning);
    }
}

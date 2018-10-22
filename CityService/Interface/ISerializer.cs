using System.Collections.Generic;

namespace CityService.Interface
{
    /// <summary>
    /// Serializes suggestions into a string representation.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes the given suggestions into a string representation
        /// </summary>
        /// <param name="suggestions">The suggestions to serialize</param>
        /// <returns>The string representation of the suggestions.</returns>
        string Serialize(IEnumerable<Suggestion> suggestions);
    }
}

# Coveo Backend Coding Challenge Solution
(forked from https://github.com/coveo/backend-coding-challenge)

## Requirements

Design a REST API endpoint that provides auto-complete suggestions for large cities.

- The endpoint is exposed at `/suggestions`
- The partial (or complete) search term is passed as a querystring parameter `q`
- The caller's location can optionally be supplied via querystring parameters `latitude` and `longitude` to help improve relative scores
- The endpoint returns a JSON response with an array of scored suggested matches
    - The suggestions are sorted by descending score
    - Each suggestion has a score between 0 and 1 (inclusive) indicating confidence in the suggestion (1 is most confident)
    - Each suggestion has a name which can be used to disambiguate between similarly named locations
    - Each suggestion has a latitude and longitude

## Additional Requirements

The following is a list of additional requirements that although wasn't explicitly mentioned, makes sense:

- The number of results should be limited, based on the querystring parameter `maxcount`. To allow the original requirement where all results may be returned, this additional parameter can be set to -1 to indicate all results.

## Technologies used

The following is a list of different technologies used to accomplish the task:

- The solution was developed in Visual Studio 2015 using .NET 4.6
- The unit tests were written using Visual Studio's unit testing tools
- UML diagrams were created using Dia
- Deployed onto the cloud via AWS EC2 hosting Windows Server 2012 R2 (new to me)
    - Contact me for IP.
- Source code uploaded to GitHub (new to me)

## Third party technologies used

The following is a list of third party technologies used

- Nancy (http://nancyfx.org/): A lightweight web framework used to host .NET REST applications. Licensed under MIT license.
    - This could have been replaced with ASP.NET for a more enterprise level framework, but that complicates the code for this example.

## Projects

The following are the projects located in the solution:

- CityService
    - This is the library where all the logic is located. 
- CityServiceTest
    - This is the library where all the unit tests for the logic is located.
- NancyRestServer
    - This is the application that runs the REST server. It routes the calls to the logic located in CityService library.

## Design

![Design](https://raw.githubusercontent.com/atml1/backend-coding-challenge/master/Documentation/Design.png)

The design was created with extensibility in mind. Most of the classes implement an interface and use other components through interfaces so that implementations can change without affecting other components.

### NacyRestServer project

The classes in NancyRestServer project (**Program**, **CityModule**, **CustomBootstrapper**) are all used to set up the Nancy framework to host the **CityService** class located in the CityService project. The **AppConfiguration** class is used to read configuration information from the App.config file. This approach allows configuration changes without recompiling the code. **AppConfiguration** implements *IAppConfiguration* so that changes to where the configuration data comes from (e.g. file, database, windows registry, etc) can be updated easily.

### CityService project

The **CityService** class in the CityServer project is the entry point into the logic of the solution. When AutoComplete (from the interface) is called, it uses *ICityStorage* to retrieve a list of possibly cities that start with the letters specified. This list of cities is scored using the *IScorer* to find how likely is the match given the name and location of the city. The **CityService** sorts this list and truncates it based on the specified number of responses to return. Finally the list of cities is converted to a list of **Suggestion** which is passed to the *ISerializer* to convert to a string and returned to the caller.

The implementations for *IScorer* and *ISerializer* are **Scorer** and **JsonSerializer** respectively. They are currently straight forward, but the interface architecture allows them to be more complex in the future if needed.

The implementation for *ICityStorage* is **CityStorage**, which is a bit more complex. This class is in charge of reading and parsing the provided data file. With the names of the cities in the file, it populates the *IWordStorage* for quick look up based on the beginning of a city name. The **CityStorage** also internally stores additional information about the city including its latitude/longitude and full name (with province/state and country). This population process happens one time, on startup. When a request is made to GetCitiesStartsWith, it looks for all possible city names in the *IWordStorage* and then finds in its internal storage all the **City** objects. It returns those **City** objects to the caller.

![Word Tree](https://raw.githubusercontent.com/atml1/backend-coding-challenge/master/Documentation/WordTree.png)

The implementation of *IWordStorage* is **WordTree**. This data structure stores each word as a path in a tree where each tranversal from one node to another represents a letter of the word. The above image represents a word tree that encodes 4 words: AMOS, LONDON, LONDONTOWNE, and LONDONDERRY. This datastructure is efficient to:

- create: O(n) with regards to number of characters in all words
- search: O(n) with regards to the number of characters in all returned words
- store: O(n) with regards to number of characters in all words (but likely much less since many characters from different words are stored using the same paths, especially near the top of the tree)

## Sample responses

These responses are meant to provide guidance. The exact values can vary based on the data source and scoring algorithm

**Near match**

    GET /suggestions?q=Londo&latitude=43.70011&longitude=-79.4163&maxcount=4

```json
{
  "suggestions": [
    {
      "name": "London, ON, Canada",
      "latitude": "42.98339",
      "longitude": "-81.23304",
      "score": 0.9
    },
    {
      "name": "London, OH, USA",
      "latitude": "39.88645",
      "longitude": "-83.44825",
      "score": 0.5
    },
    {
      "name": "London, KY, USA",
      "latitude": "37.12898",
      "longitude": "-84.08326",
      "score": 0.5
    },
    {
      "name": "Londontowne, MD, USA",
      "latitude": "38.93345",
      "longitude": "-76.54941",
      "score": 0.3
    }
  ]
}
```

**No match**

    GET /suggestions?q=SomeRandomCityInTheMiddleOfNowhere

```json
{
  "suggestions": []
}
```

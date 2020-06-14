using System;
using Firebase.Database;
using Nancy.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nancy;
using Firebase.Database.Query;
using ChatbotRestAPI.Models;
using System.Linq;
using System.Collections;
using Microsoft.Ajax.Utilities;
using System.Security.Policy;
using ChatbotRestAPI.Services;

namespace WebCrawler
{
    
    class FireBaseDatabase
	{
        private const String databaseSecret = "1g9slC6Jg0KBfYxqCOpZbccVhRHcinc1h2Crtd84";
        private const String databaseUrl = "https://travelchatapp.firebaseio.com/";
        private const String nodeBlogs = "DB/1/Blogs/";
        private const String nodeWorld = "DB/0/World";
        private static FirebaseClient firebaseClient;
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        public FireBaseDatabase()
        {
           
        }

        private static void InitFirbaseClient()
        {
            firebaseClient = new FirebaseClient(
               databaseUrl,
               new FirebaseOptions
               {
                   AuthTokenAsyncFactory = () => Task.FromResult(databaseSecret)

               });
        }

        public static  List<State> SelectStateBy(string field, string value)
        {
            InitFirbaseClient();
            List<State> states = new List<State>();
            var dbResult =  firebaseClient
             .Child("DB")
             .Child("0")
             .Child("World:")
             .Child("2")
             .Child("geo_states").OrderBy(field).EqualTo(value)
             .OnceAsync<State>().Result;

            if (dbResult != null)
                foreach (var result in dbResult)
                {
                    State state = result.Object;
                    states.Add(state);
                }

            return states;
        }

        private static IList GetCurrentDestinationType(string input)
        {
            //make first letter upper
            input = input.First().ToString().ToUpper() + input.Substring(1);

            List<City> filteredCities = FireBaseDatabase.SelectCitytBy("name", input);
            if (filteredCities != null && filteredCities.Count > 0)
                return filteredCities;

            List<Country> filteredCountries = FireBaseDatabase.SelectCountryBy("name", input);
            if (filteredCountries != null && filteredCountries.Count > 0)
                return filteredCountries;

            List<State> filteredStates = FireBaseDatabase.SelectStateBy("name", input);
            if (filteredStates != null && filteredStates.Count > 0)
                return filteredStates;

            List<Continent> filteredContinents = FireBaseDatabase.SelectContinentBy("name", input);
            if (filteredContinents != null && filteredContinents.Count > 0)
                return filteredContinents;

            return null;
        }


        //Gets the Destination and all the relationship of the destiantion
        public static Destination GetDestinationRelationship(string destination)
        {
            Destination d = new Destination();
            City city;
            Country country;
            Continent continent;
            State state;

            IList currentDestination = GetCurrentDestinationType(destination);
            if (typeof(City).IsInstanceOfType(currentDestination[0]))
            {
                city = (City)currentDestination[0];
                d.City.Add(city.Name);

                country = SelectCountryBy("id", city.Country_Id)[0];
                d.Country.Add(country.Name);

                state = SelectStateBy("id", city.State_id)[0];
                d.State.Add(state.Name);

                continent = SelectContinentBy("id", country.Continent_id)[0];
                d.Continent.Add(continent.Name);

            }
            else if(typeof(Country).IsInstanceOfType(currentDestination[0]))
            {
                country =(Country)currentDestination[0];
                d.Country.Add(country.Name);

                List<State> states = SelectStateBy("country_id", country.Id);
                foreach(State s in states)
                {
                    d.State.Add(s.Name);
                }

                List<City> cities = SelectCitytBy("country_id", country.Id);
                foreach (City c in cities)
                {
                    d.City.Add(c.Name);
                }

                continent = SelectContinentBy("id", country.Continent_id)[0];
                d.Continent.Add(continent.Name);
            }
            else if (typeof(State).IsInstanceOfType(currentDestination[0]))
            {
                state = (State)currentDestination[0];
                d.State.Add(state.Name);

                List<City> cities = SelectCitytBy("state_id", state.Id);
                foreach (City c in cities)
                {
                    d.City.Add(c.Name);
                }

                country = SelectCountryBy("id", state.Country_id)[0];
                d.Country.Add(country.Name);

           
                continent = SelectContinentBy("id", country.Continent_id)[0];
                d.Continent.Add(continent.Name);
            }
            else if (typeof(Continent).IsInstanceOfType(currentDestination[0]))
            {
                continent = (Continent)currentDestination[0];
                d.Continent.Add(continent.Name);

                List<Country> countries = SelectCountryBy("continent_id", continent.Id);
                foreach (Country c in countries)
                {
                    d.Country.Add(c.Name);
                }
            }

            return d;

        }
        public static  List<City> SelectCitytBy(string field, string value)
        {
            InitFirbaseClient();
            List<City> cities = new List<City>();
            var dbResult =  firebaseClient
             .Child("DB")
             .Child("0")
             .Child("World:")
             .Child("3")
             .Child("geo_cities").OrderBy(field).EqualTo(value)
             .OnceAsync<City>().Result;

            if (dbResult != null)
                foreach (var result in dbResult)
                {
                    City city = result.Object;
                    cities.Add(city);
                }

            return cities;
        }

        public static  List<Continent> SelectContinentBy(string field, string value)
        {
            InitFirbaseClient();
            List<Continent> continents = new List<Continent>();
            var dbResult =  firebaseClient
             .Child("DB")
             .Child("0")
             .Child("World:")
             .Child("0")
             .Child("geo_continents").OrderBy(field).EqualTo(value)
             .OnceAsync<Continent>().Result;

            if (dbResult != null)
                foreach (var result in dbResult)
                {
                    Continent continent = result.Object;
                    continents.Add(continent);
                }

            return continents;
        }
        public static List<Country> SelectCountryBy(string field, string value)
        {
            InitFirbaseClient();
            List<Country> countries = new List<Country>();
            var dbResult =  firebaseClient
             .Child("DB")
             .Child("0")
             .Child("World:")
             .Child("1")
             .Child("geo_countries").OrderBy(field).EqualTo(value)
             .OnceAsync<Country>().Result;

            if (dbResult != null)
                foreach (var result in dbResult)
                {
                    Country  country = result.Object;
                    countries.Add(country);
                }

            return countries;
        }
        public static void Insert(Blog blog)
        {
            InitFirbaseClient();
            firebaseClient.Child(nodeBlogs).PostAsync(serializer.Serialize(blog));
        }

        public static List<Blog> SelectAllBlogs()
        {
            InitFirbaseClient();
            List<Blog> blogs = new List<Blog>();
            //Retrieve data from Firebase
            var dbResult = firebaseClient
              .Child("DB")
              .Child("1")
              .Child("Blogs")
              .OnceAsync<Blog>().Result;

            if (dbResult != null)
                foreach (var result in dbResult)
                {
                    Blog blog = result.Object;
                    blog.Id = result.Key;
                    blogs.Add(blog);
                }

            return blogs;
        }
        public static List<Blog> SelectBlogsBy(string field, string value)
        {
            InitFirbaseClient();
            List<Blog> blogs = new List<Blog>();
            //Retrieve data from Firebase
            var dbResult =  firebaseClient
              .Child("DB")
              .Child("1")
              .Child("Blogs").OrderBy(field).EqualTo(value)
              .OnceAsync<Blog>().Result;

            if(dbResult!=null)
                foreach(var result in dbResult)
                {
                    Blog blog = result.Object;
                    blog.Id = result.Key;
                    blogs.Add(blog);
                }

            return blogs;
        }

        public static void UpdateBlog(string blogId, string fieldToUpdate, List<string> value)
        {
            var dbResult = firebaseClient
              .Child("DB")
              .Child("1")
              .Child("Blogs");

            var blogField = dbResult.Child(blogId); //.Child(fieldToUpdate);

            string valueToUpdate = TextProcessor.FormatValutToJson(fieldToUpdate, value);

            blogField.PatchAsync((JObject)JsonConvert.DeserializeObject(valueToUpdate)).Wait();
        }

        public static List<Blog> FilterBlogsByFilters(List<string> tags, List<Blog> blogs)
        {
            //return blogs.Where(blog => blog.Tags.Contains(tags[0])).ToList();
            return blogs.Where(blog => blog.Tags.Intersect(tags).Count() == tags.Count()).ToList();
        }
        public static List<Blog> FilterBlogsBy(List<string> tags, List<Blog> blogs)
        {
            List<Blog> filteredBlogs = new List<Blog>();
            if (tags.Contains(Constants.ACTIVITIES))
            {
                filteredBlogs = blogs.Where(blog => blog.Tags.Intersect(Constants.ACCOMODATION_TAGS).Count() == 0 
                && blog.Tags.Intersect(Constants.RESTAURANTS_TAGS).Count() == 0).ToList();
                if (tags.Count > 1)
                {
                    tags.Remove(Constants.ACTIVITIES);
                    filteredBlogs = FilterBlogsByFilters(tags, filteredBlogs);
                }
            }
            else if(tags.Contains(Constants.ACCOMODATION))
            {
                filteredBlogs = blogs.Where(blog => blog.Tags.Intersect(Constants.ACCOMODATION_TAGS).Count() != 0 
                && blog.Tags.Intersect(Constants.RESTAURANTS_TAGS).Count() == 0).ToList();
                if (tags.Count > 1)
                {
                    tags.Remove(Constants.ACCOMODATION);
                    filteredBlogs = FilterBlogsByFilters(tags, filteredBlogs);
                }
            }
            else if(tags.Contains(Constants.RESTAURANTS))
            {
                filteredBlogs = blogs.Where(blog => blog.Tags.Intersect(Constants.ACCOMODATION_TAGS).Count() == 0 && blog.Tags.Intersect(Constants.RESTAURANTS_TAGS).Count() != 0).ToList();
                if (tags.Count > 1)
                {
                    tags.Remove(Constants.RESTAURANTS);
                    filteredBlogs = FilterBlogsByFilters(tags, filteredBlogs);
                }
            }

            return filteredBlogs;
           
        }

        public static List<string> FilterDestinationsByBlogs(List<string> destinations, List<string> tags)
        {
            
            List<Blog> allBlogs = FireBaseDatabase.SelectAllBlogs();
            List<string> filteredDestinations = new List<string>();
            foreach (string destination in destinations)
            {
                List<Blog> blogs = allBlogs.Where(x => x.LocationName.ToLower().Equals(destination.ToLower())).ToList();
                blogs = FilterBlogsBy(tags, blogs);
                if(blogs.Count() > 0)
                    filteredDestinations.Add(destination);
            }

            return filteredDestinations;
        }

    }
}

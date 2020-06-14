using ChatbotRestAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Windows.Forms;
using WebCrawler;

namespace ChatbotRestAPI.Services
{
	public class ResponseControllerRepository
	{
        static object Lock = new object();


       
		public static string GetResponse(string input)
		{
            input = input.ToLower();

            List<string> tags = input.Split(' ').ToList();
            string location = input.Split(' ')[0].ToLower();

            tags.Remove(location);
            tags = TextProcessor.PreprocessTags(tags);

            
            List<JObject> placesNERtemp = new List<JObject>();
            ConcurrentBag<JObject> placesNER = new ConcurrentBag<JObject>();

            if (location != null)
            {
                Destination destination = FireBaseDatabase.GetDestinationRelationship(location);

                List<Blog> blogs =  FireBaseDatabase.SelectBlogsBy("locationName", location);

                if((blogs == null || blogs.Count ==0) && destination !=null)
                {
                    if (destination.Country.Count > 1)
                    {
                        List<string> countries = FireBaseDatabase.FilterDestinationsByBlogs(destination.Country, tags);
                        if(countries.Count > 0)
                            return JsonConvert.SerializeObject(countries);
                        else return "error";
                    }
                    else
                    if (destination.State.Count > 1)
                    {
                        List<string> states = FireBaseDatabase.FilterDestinationsByBlogs(destination.State, tags);
                        if (states.Count > 0)
                            return JsonConvert.SerializeObject(states);
                        else
                        {
                            List<string> cities = FireBaseDatabase.FilterDestinationsByBlogs(destination.City, tags);
                            if (cities.Count > 0)
                                return JsonConvert.SerializeObject(cities);
                            else return "error";
                        }
                    }
                    else
                    if (destination.City.Count > 1)
                    {
                        List<string> cities = FireBaseDatabase.FilterDestinationsByBlogs(destination.City, tags);
                        if (cities.Count > 0)
                            return JsonConvert.SerializeObject(cities);
                        else return "error";
                    }
                }

                List<Blog> filteredBlogsTags;
                if (blogs != null)
                {

                    filteredBlogsTags =  FireBaseDatabase.FilterBlogsBy(tags, blogs);


                    if (filteredBlogsTags != null && filteredBlogsTags.Count != 0)
                    {
                        blogs = filteredBlogsTags;
                    }
                    else return null;


                    string response="";

                    string text = "";

                    List<string> contentBlogs = new List<string>();

                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    Parallel.For<JObject>(0, blogs.Count, () => new JObject(), (i, loop, data) =>
                    {
                        string places;
                        lock (Lock)
                        {                 
                           text = Crawler.ReadTextFrom(blogs[(int)i].BlogLink);
                           text = TextProcessor.Preprocess(text);
                        }
                        places = RestAPICaller.GetPlacesNER (text, destination);
                        data = (JObject)JsonConvert.DeserializeObject(places);

                        return data;
                    },
                        (x) =>  placesNER.Add(x)
                    );

                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    System.IO.File.WriteAllText(@"Time.txt", elapsedMs.ToString());

                    placesNERtemp = placesNER.ToList();
                    placesNERtemp.Add((JObject)JsonConvert.DeserializeObject("{\"destination\":\"" + destination + "\"}"));
                    response = RestAPICaller.GetPlacesNERFinal(JsonConvert.SerializeObject(placesNERtemp));
                    
                   
                    return response;
                }
                else return "No Blogs with location!";
            }
            else return "No location!";

          
        }


        public static string ExtractParagraphs(string input)
        {
            input = input.ToLower();
            string [] splittedInput = input.Split('-');
            string location = splittedInput[0];
            string place = splittedInput[1];
            string typeInput = splittedInput[2];
            string response = "";

            List<Blog> blogs = FireBaseDatabase.SelectBlogsBy("locationName", location);


            if(blogs == null || blogs.Count == 0)
                blogs = FireBaseDatabase.SelectBlogsBy("locationName", place);

            if (blogs != null)
            {
                List<string> filters = new List<string>();
                filters.Add(typeInput);

                List <Blog> filteredBlogsTags = FireBaseDatabase.FilterBlogsBy(filters, blogs);
               
                string text;
                List<string> paragrpahs = new List<string>();

                if (filteredBlogsTags != null && filteredBlogsTags.Count != 0)
                {
                    blogs = filteredBlogsTags;
                }
                else return "There is nothing matching your preferences...";
                Parallel.For<string>(0, blogs.Count, () => "", (i, loop, data) =>
                {
                    lock (Lock)
                    {
                        text = Crawler.ReadTextFrom(blogs[(int)i].BlogLink);
                        text = Regex.Replace(text, @"((\n[A-Z a-z]+,)\s([A-Z a-z]+,)*\s*([A-Z a-z]+)\n)", "\n");
                        text = TextProcessor.Preprocess(text);
                    }

                    string json = RestAPICaller.GetParagraphs(text, place);
                    if (!json.All(x => !char.IsLetter(x)))
                    {
                            //response = json.Replace("\"", "");
                            data += json + "\n\n";
                    }
                    

                    return data;
                },
                        (x) => paragrpahs.Add(x)

                    );

                foreach (string paragrpah in paragrpahs)
                {
                    response += paragrpah;
                }
            }

            return response;
        }
        
        public static  bool CheckLocation(string input)
        {
            //make first letter upper
            input = input.First().ToString().ToUpper() + input.Substring(1);

            List<City> filteredCities =  FireBaseDatabase.SelectCitytBy("name", input);
            if (filteredCities != null && filteredCities.Count > 0 )
                return true;

            List<Country> filteredCountries =  FireBaseDatabase.SelectCountryBy("name", input);
            if (filteredCountries != null && filteredCountries.Count > 0)
                return true;

            List<State> filteredStates =  FireBaseDatabase.SelectStateBy("name", input);
            if (filteredStates != null && filteredStates.Count > 0)
                return true;

            List<Continent> filteredContinents =  FireBaseDatabase.SelectContinentBy("name", input);
            if (filteredContinents != null && filteredContinents.Count > 0)
                return true;

            return false;
        }



        public static string CreateDataset(string input)
        {
            try
            {
                Dataset.Create(input);
            }
            catch(Exception e)
            {
                return e.Message;
            }

            return "Dataset Created!";

        }

        public static string UpdateDatasetTags(string input)
        {
            try
            {
                List<Blog> blogs = FireBaseDatabase.SelectAllBlogs();
                Dataset.UpdateBlogsTags(blogs);
            }catch(Exception e)
            {
                return e.Message;
            }

            return "Dataset Updated!";
        }

       
    }
}
using ChatbotRestAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Windows.Forms;
using WebCrawler;

namespace ChatbotRestAPI.Services
{
	public class ResponseControllerRepository
	{
        private static List<string> PreprocessTags(List<string> tags)
        {
           
            tags.ForEach(tag => tag = new string(tag.Where(c => !char.IsPunctuation(c) && !char.IsWhiteSpace(c)).ToArray()));
            tags = tags.Where(tag => !tag.Equals("")).ToList();

            return tags;
        }

		public static string GetResponse(string input)
		{
            input = input.ToLower();

            List<string> tags = input.Split(' ').ToList();
            string location = input.Split(' ')[0].ToLower();

            tags.Remove(location);
            tags = PreprocessTags(tags);

            
            List<JObject> placesNER = new List<JObject>();

            if (location != null)
            {
                Destination destination = FireBaseDatabase.GetDestinationRelationship(location);

                List<Blog> blogs =  FireBaseDatabase.SelectBlogsBy("locationName", location);
                List<Blog> filteredBlogsTags;
                if (blogs != null)
                {

                    filteredBlogsTags =  FireBaseDatabase.FilterBlogsBy(tags, blogs);


                    if (filteredBlogsTags != null && filteredBlogsTags.Count != 0)
                    {
                        blogs = filteredBlogsTags;
                    }
                    else return null;

                    string text;
                    string response;
                    foreach (Blog blog in blogs)
                    {

                        text = Crawler.ReadTextFrom(blog.BlogLink);
                        text = Crawler.Preprocess(text);

                        response = RestAPICaller.GetPlacesNER(text, destination);
                        var data = (JObject)JsonConvert.DeserializeObject(response);

                        placesNER.Add(data);
                        break;       
                    }

                    //if (placesNER.Count > 1)
                    //{
                        placesNER.Add((JObject)JsonConvert.DeserializeObject("{\"destination\":\"" + destination + "\"}"));
                        response = RestAPICaller.GetPlacesNERFinal(JsonConvert.SerializeObject(placesNER));
                    //}
                    //else response = JsonConvert.SerializeObject(placesNER[0]);

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
            if (blogs != null)
            {
                List<string> filters = new List<string>();
                filters.Add(typeInput);

                List <Blog> filteredBlogsTags = FireBaseDatabase.FilterBlogsBy(filters, blogs);
               
                string text;

                if (filteredBlogsTags != null && filteredBlogsTags.Count != 0)
                {
                    blogs = filteredBlogsTags;
                }
                else return "There is nothing matching your preferences...";

                foreach (Blog blog in blogs)
                {
                    text = Crawler.ReadTextFrom(blog.BlogLink);
                    text = Regex.Replace(text, @"((\n[A-Z a-z]+,)\s([A-Z a-z]+,)*\s*([A-Z a-z]+)\n)", "\n");
                    text = Crawler.Preprocess(text);

                    string json = RestAPICaller.GetParagraphs(text, place);

                    if (!json.All(x => !char.IsLetter(x)))
                    {
                        //response = json.Replace("\"", "");
                        response += json + "\n\n";
                    }
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

       

        public static string TagsFromBlog(string input)
        {
            List<string> filters = new List<string>();
            //suggestions = new string[] { "cheap", "fun", "romantic", "interesting", "Type", "Ready" };
            //suggestions = new string[] { "night", "outdoor", "kids", "free", "Type", "Ready" };
            //suggestions = new string[] { "hotel", "hostel", "apartment", "romantic", "cheap", "Type", "Ready" };

            string [] filtersString = { "cheap", "fun", "romantic", "family", "interesting", "night", "outdoor", "kid", "free", "hotel", "hostel", "apartment", "romantic", "cheap" };
            List<Blog> blogs = FireBaseDatabase.SelectAllBlogs();
            List<Blog> filteredBlogsTags;
            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Clara2\Desktop\Licenta\TestBERT\TestTags.txt"))
            //{
            //  foreach (string filter in filtersString)
            // {
            // file.Write(filter);
            //   filters.Add(filter);
            // filteredBlogsTags = FireBaseDatabase.FilterBlogsByFilters(filters, blogs);
            // file.Write(filteredBlogsTags.Count);
            // file.Write("\n\n");
            // filters.Remove(filter);
            //}
            // }

            string word = input.Split(' ')[1];
            if (input.Contains("theculturetrip"))
            {
                blogs = blogs.Where(blog => blog.BlogLink.Contains("theculturetrip")).ToList();
                //blogs = blogs.GetRange(blogs.Count/2, blogs.Count/2);
                Crawler_theculturetrip crawlerTheCultureTrip = new Crawler_theculturetrip();       
                crawlerTheCultureTrip.CountWordInBlog(blogs, " "+word+" ");

                //crawlerTheCultureTrip.UpdateBlogsTags(blogs);
            }
            else if (input.Contains("abrokenbackpack"))
            {
                blogs = blogs.Where(blog => blog.BlogLink.Contains("abrokenbackpack")).ToList();
                Crawler_abrokenbackpack crawlerBrockenBackpack = new Crawler_abrokenbackpack();
                crawlerBrockenBackpack.CountWordInBlog(blogs, " " + word + " ");

                //crawlerBrockenBackpack.UpdateBlogsTags(blogs);

            }
            else return "Error";
           

            return "";


            return "Done";

        }
    }
}
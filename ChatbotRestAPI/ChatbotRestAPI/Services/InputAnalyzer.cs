using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace ChatbotRestAPI.Services
{
	public class InputAnalyzer
	{
        private static string apiKey = "30b5519e6d3285699abf92762bdb5664 ";
        public InputAnalyzer()
        {

        }

        private static string TopicExtractionAPI(string word)
        {
            try
            {
                var client = new RestClient("https://api.meaningcloud.com/topics-2.0");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", "key=" + apiKey + "&lang=en&txt=" + word + "&tt=a", ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);
                return response.Content;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return null;
        }

        private static List<string> ExtractTypes(string apiResponse)
        {
            List<string> types = new List<string>();
            MatchCollection matches = Regex.Matches(apiResponse, "type\":\"[a-zA-Z>]*");
            string type;
            foreach (Match m in matches)
            {
                Console.WriteLine(m.Value);
                if (m.Value.Contains("Top>"))
                {
                    type = Regex.Split(m.Value, "type\":\"Top>")[1];
                    if (!types.Contains(type) && !type.Equals(""))
                        types.Add(type);
                }
            }
            //"type":"Top>Location>GeoPoliticalEntity>Country"}
            return types;
        }
        public static Dictionary<List<string>, string> GetCategorizedWords(string input)
        {
            Dictionary<List<string>, string> categorizedWords = new Dictionary<List<string>, string>();
            List<string> tokenizedInput = input.Split(' ').ToList<string>();
            List<string> types = new List<string>();
            foreach (string word in tokenizedInput)
            {
                types = ExtractTypes(TopicExtractionAPI(word));
                if (types.Count != 0)
                    categorizedWords.Add(types, word);
            }

            return categorizedWords;
        }

        public static List<string> GetKeywords(Dictionary<List<string>, string> categorizedWords)
        {
            List<string> keywords = new List<string>();
            foreach(List<string> key in categorizedWords.Keys)
            {
                keywords.AddRange(key);
            }
            return keywords;
        }
        public static string GetLocation(Dictionary<List<string>, string> categorizedWords)
        {
            string location;
           
                foreach (List<string> key in categorizedWords.Keys)
                {
                    foreach (string type in key)
                    {
                        if (type.Contains("City") || type.Contains("Country"))
                        {
                            location = categorizedWords[key];
                            categorizedWords.Remove(key);
                            return location;
                        }
                    }
                }

            return null;
        }
    }
}
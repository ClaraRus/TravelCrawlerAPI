using ChatbotRestAPI.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace WebCrawler
{
	class RestAPICaller
	{
        public static string GetParagraphs(string text, string place)
        {
            string uriWebAPI;
            uriWebAPI = "http://127.0.0.1:8000/getparagraphs/";
            string json = "{\"text\":\"" + text + "\", \"place\":\"" + place + "\"}";

            return Post(json, uriWebAPI);
        }
        public static string GetTags(string text)
        {   
            string uriWebAPI;
            uriWebAPI = "http://127.0.0.1:8000/gettags/";
            string json = "{\"text\":\"" + text + "\"}";

            return Post(json, uriWebAPI);
        }

        public static string GetPlacesNERFinal(string response)
        {
            string uriWebAPI;
            uriWebAPI = "http://127.0.0.1:8000/getfinalresult/";

            return Post(response, uriWebAPI);
        }

        public static string GetPlacesNER(string text, Destination destination)
        {
            string dst = destination.ToString();
            string uriWebAPI;
            uriWebAPI = "http://127.0.0.1:8000/getactivities/";
            string json = "{\"text\":\"" + text + "\", \"destination\":\"" + dst + "\"}";

            return Post(json, uriWebAPI);
        }

        private static string Post(string json, string url)
        {
            var restClient = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddJsonBody(json);

            IRestResponse responseAPI = restClient.Execute(request);
            return responseAPI.Content;
        }


    }
}

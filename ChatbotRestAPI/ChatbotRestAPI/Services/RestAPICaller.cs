using ChatbotRestAPI.Models;
using Newtonsoft.Json.Linq;
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

           // int maxSize = 1000;

            //text = text.Replace('\n', ' ');
            
            //IEnumerable<string> croppedText = Enumerable.Range(0, text.Length / maxSize).Select(i => text.Substring(i * maxSize, maxSize));

            //foreach (string textPart in croppedText)
            // {
            uriWebAPI = "http://127.0.0.1:8000/gettags/";
            string json = "{\"text\":\"" + text + "\"}";

            return Post(json, uriWebAPI);
            //uriWebAPI += text;
            //string exceptionMessage = string.Empty;

            // Get web response by calling the CSharpPythonRestfulApiSimpleTest() method
            //string webResponse = RestAPICaller.GetResponse(uriWebAPI);

            //if (string.IsNullOrEmpty(exceptionMessage))
            //{
            // No errors occurred. Write the string web response     
            //return webResponse.ToString();
            //}
            //else
            //{
            // An error occurred. Write the exception message
            //Console.WriteLine(exceptionMessage);
            //return "Error";
            //}
            // }
            //return null;
        }

        private static string Post(string json, string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
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

       
        private static string GetResponse(string url)
        {
            int len = url.Length;

            //HttpUtility.UrlEncode(ulr)

            var request = (HttpWebRequest)WebRequest.Create(HttpUtility.UrlEncode(url));

            request.Method = "GET";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            
            var content = string.Empty;

            using (var response =  (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            return content;
        }
    }
}

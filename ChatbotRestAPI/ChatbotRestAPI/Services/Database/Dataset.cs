using ChatbotRestAPI.Services;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Http.Results;

namespace WebCrawler
{
     
    class Dataset
    {

        private static Dictionary<string, string> rootLinks = new Dictionary<string, string>() { { "theculturetrip", "" }, { "abrockenbackpack" , "" } };
        public static void Create(string input)
        {
            string rootLink = rootLinks[input];

            string className = rootLink.Split('/')[2];
            className = Regex.Split(className, ".com")[0];
            className = "WebCrawler.Crawler_" + className;

            Type t = Type.GetType(className);
            if (t != null)
            {
                Object instance = (Object)Activator.CreateInstance(t);
                MethodInfo methodInfo = t.GetMethod("StartCrawler");
                methodInfo.Invoke(instance, null);

                CreateBlogs(instance,t);
            }
            else throw new Exception("Create class:" + className);
        }

       
        public static void UpdateBlogsTags(List<Blog> blogs)
        {
            string text;
            foreach (Blog blog in blogs)
            {
                text = Crawler.ReadTextFrom(blog.BlogLink);
                text = TextProcessor.Preprocess(text);

                List<string> tagsList = new List<string>();
                tagsList = GetTags(text);

                tagsList = tagsList.Where(tag => tag.Length > 2).ToList();


                tagsList.AddRange(blog.Tags);
                tagsList = tagsList.Distinct<string>().ToList();

                FireBaseDatabase.UpdateBlog(blog.Id, "tags", tagsList);
            }
        }

        private static List<string> GetTags(string text)
        {
            List<string> tagsList = new List<string>();
            string tags;
            if (text != null)
            {
                text = TextProcessor.Preprocess(text);

                if (text.Length > 1)
                {
                    tags = RestAPICaller.GetTags(text);
                    if (tags != null)
                    {
                        tags = Regex.Replace(tags, "\"", "");
                        tags = tags.Replace("[", "");
                        tags = Regex.Replace(tags, "]", "");

                        foreach (string tag in tags.Split(','))
                            tagsList.Add(tag.Trim());
                    }
                }
            }

            return tagsList;
        }

        private static void CreateBlogs(Object instance, Type t)
        {
            List<string> tagsList;

            Blog blog;
            string text;
            string date;

            FieldInfo blogsField = t.GetField("blogs", BindingFlags.Instance
                   | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            Dictionary<string, List<string>> blogs = (Dictionary<string, List<string>>)blogsField.GetValue(instance);
            object[] parametersArray;

            foreach (string key in blogs.Keys)
            {
                foreach (string link in blogs[key])
                {
                    text = "";
                    date = "";

                    tagsList = new List<string>();
                    blog = new Blog();
                    blog.LocationName = key;
                    blog.BlogLink = link;

                    parametersArray = new object[] { link };
                    text =Crawler.ReadTextFrom(link);

                    tagsList = GetTags(text);
                    blog.Tags = tagsList;

                    date = Crawler.ReadDateFrom(link);
                    blog.Date = date;

                    FireBaseDatabase.Insert(blog);

                }
            }
        }

        public int CountWordInBlog(List<Blog> blogs, string word)
        {
            int count = 0;
            string text;
            foreach (Blog blog in blogs)
            {
                text = Crawler.ReadTextFrom(blog.BlogLink);
                text = TextProcessor.Preprocess(text);

                count += Regex.Matches(text.ToLower(), word.ToLower()).Count;

            }

            return count;

        }
    }
}

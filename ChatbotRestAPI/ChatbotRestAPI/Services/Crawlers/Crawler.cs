using HtmlAgilityPack;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebCrawler
{
    abstract class Crawler
    {

        private static HtmlDocument htmlDocument;
        protected List<string> continentLinks = new List<string>();
        protected List<string> countries = new List<string>();
        protected List<string> states = new List<string>();
        protected List<string> cities = new List<string>();
        protected Dictionary<string, List<string>> blogs = new Dictionary<string, List<string>>();

        protected string RootUrl { get; set; }
        protected string ContinentDivAttribute { get; set; }
        protected string ContinentDivAttributeName { get; set; }
        protected string CountryDivAttribute { get; set; }
        protected string CountryDivAttributeName { get; set; }
        protected string StateDivAttribute { get; set; }
        protected string StateDivAttributeName { get; set; }
        protected string CityDivAttribute { get; set; }
        protected string CityDivAttributeName { get; set; }
        protected string BlogDivAttribute { get; set; }
        protected string [] BlogDivAttributeName { get; set; }
        protected string PatternToFilter { get; set; }
        protected string PatternToFilterBlogs { get; set; }
        protected string TextDivAttribute { get; set; }
        protected string TextDivAttributeName { get; set; }
        protected string DateDivAttribute { get; set; }
        protected string DateDivAttributeName { get; set; }

        public Crawler()
        {

        }
        public static void initAsync(string url)
        {
            System.Net.HttpWebRequest client = (HttpWebRequest)HttpWebRequest.Create(url);

            client.Method = "GET";
            client.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";
            try
            {
                var response = (HttpWebResponse)client.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string html = streamReader.ReadToEndAsync().Result;
                        htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(html);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void ExtractCities()
        {
            List<string> temp = new List<string>();
           
                temp = GetLinks(CityDivAttribute, CityDivAttributeName);
                if (temp != null)
                {
                    cities.AddRange(temp);
                }
            
        }
        private void ExtractBlogs(string parentLink)
        {
            List<string> temp = new List<string>();
            string[] countryName;

            for (int i = 0; i < BlogDivAttributeName.Length; i++)
            {
                temp = GetLinks(BlogDivAttribute, BlogDivAttributeName[i]);
                if (temp != null)
                {
                    temp.RemoveAll(blog => !Regex.IsMatch(blog, PatternToFilterBlogs));
                    temp = temp.Distinct().ToList();

                    countryName = parentLink.Split('/');
                    
                    if (!countryName.Last().Equals(""))
                        blogs[countryName.Last()] = temp;
                    else blogs[countryName.GetValue(countryName.Length - 2).ToString()] = temp;

                    break;
                }
            }
        }

        private void ExtarctCountries()
        {
            List<string> temp = new List<string>();

            foreach (string continentLink in continentLinks)
            {

                initAsync(continentLink);
                temp = GetLinks(CountryDivAttribute, CountryDivAttributeName);

                if (temp != null)
                    countries.AddRange(temp);
            }

            countries = countries.Distinct().ToList();
            countries.RemoveAll(blog => !blog.Contains(PatternToFilter));
        }

        private void ExtractLinksUnderCountry()
        {
            List<string> temp = new List<string>();

            foreach (string countryLink in countries)
            {
                initAsync(countryLink);
                if (StateDivAttribute != null && (countryLink.Contains("usa") || countryLink.Contains("united-kingdom")))
                {
                    temp = GetLinks(StateDivAttribute, StateDivAttributeName);
                    if (temp != null)
                    {
                        states.AddRange(temp);
                    }
                }
                else
                if (CityDivAttribute != null)
                {
                    ExtractCities();
                }
                else
                {
                    ExtractBlogs(countryLink);
                }
            }

            
        }
        public void StartCrawler()
        {
            initAsync(RootUrl);

            if(continentLinks.Count == 0)
                continentLinks = GetLinks(ContinentDivAttribute, ContinentDivAttributeName);

            ExtarctCountries();

            ExtractLinksUnderCountry();

            if (states != null)
            {
                foreach (string stateLink in states)
                {
                    initAsync(stateLink);
                    if (CityDivAttribute != null)
                    {
                        ExtractCities();
                    }
                }
            }

            if (cities != null)
            {
                cities = cities.Distinct().ToList();
                cities.RemoveAll(blog => !blog.Contains(PatternToFilter));

                foreach (string cityLink in cities)
                {
                    Console.WriteLine(cityLink);
                    initAsync(cityLink);
                    ExtractBlogs(cityLink);
                }
            } 
        }

        protected static IEnumerable<HtmlNode> GetDescendants(string parent)
        {
             return htmlDocument.DocumentNode.Descendants(parent);
        }
        protected static List<HtmlNode> GetAllDescendants(string parent, string attribute, string valueAttribute)
        {
            IEnumerable<HtmlNode> divs = GetDescendants(parent);
            if (divs != null)
                return divs.Where(node => node.GetAttributeValue(attribute, "").Equals(valueAttribute)).ToList();
            else return null;

        }

        protected static HtmlNode GetFirstDescendant(string parent, string attribute, string valueAttribute)
        {
            IEnumerable<HtmlNode> divs = GetDescendants(parent);
            if (divs != null)
                return divs.Where(node => node.GetAttributeValue(attribute, "").Equals(valueAttribute)).FirstOrDefault();
            else return null;
        }

        protected static List<HtmlAttribute> GetChildAttributes(HtmlNode node, string descendant, string attribute)
        {
            IEnumerable<HtmlNode> descendants;
            List<HtmlAttribute> allAttributes = new List<HtmlAttribute>();
            List<HtmlAttribute> attributes = new List<HtmlAttribute>();
            if (node != null)
            {
                descendants = node.Descendants(descendant);
                if (descendants.Count() != 0)
                    foreach (HtmlNode d in descendants)
                    {
                        if ((attributes = d.ChildAttributes(attribute).ToList()) != null)
                            allAttributes.AddRange(attributes);
                    }
                return EraseAttributesDuplicates(allAttributes);
            }
            return null;
        }

        protected static List<HtmlAttribute> EraseAttributesDuplicates(List<HtmlAttribute> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list.FindAll(e => e.Value.Equals(list[i].Value)).Count > 1)
                    list.RemoveAt(i);
            }

            return list;
        }

        protected List<string> GetLinks(string divAttribute, string divAttributeName)
        {
            List<string> links = new List<string>();
            HtmlNode div = GetFirstDescendant("div", divAttribute, divAttributeName);

            List<HtmlNode> nodes;
            List<HtmlAttribute> allAttributes = new List<HtmlAttribute>();
            List<HtmlAttribute> attributes = new List<HtmlAttribute>();

            if (div != null)
            {
                nodes = div.ChildNodes.ToList();
            }
            else return null;

            foreach (HtmlNode node in nodes)
            {
                if ((attributes = GetChildAttributes(node, "a", "href")) != null)
                    allAttributes.AddRange(attributes);
            }

            string link;
            foreach (HtmlAttribute attribute in allAttributes)
            {
                link = attribute.Value;
                if (!link.Contains("https"))
                    link = RootUrl + link;
                    links.Add(link);
            }
           
            return links;
        }

        public string GetDate()
        {
            HtmlNode spanNode = GetFirstDescendant("span", DateDivAttribute, DateDivAttributeName);

            string text = spanNode.InnerText;
            string datePattern = "((Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Nov|Dec|Oct) (([1-3][0-9])|([0-9])), (20[0-9][0-9]))";
            Regex r = new Regex(datePattern);
            Match result = r.Match(text);

            return result.ToString();
        }

        public static string ReadTextFrom(string link)
        {
            string text = "";
            string className = link.Split('/')[2];
            className = Regex.Split(className, ".com")[0];
            className = "WebCrawler.Crawler_" + className;

            //Type objType = typeof(Crawler_abrokenbackpack);



            // Print the assembly qualified name.
            //Console.WriteLine($"Assembly qualified name:\n   {objType.AssemblyQualifiedName}.");

            Type t = Type.GetType(className);
            if (t != null)
            {

                Object instance = (Object)Activator.CreateInstance(t);
                PropertyInfo divAttribute = t.GetProperty("TextDivAttribute", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                PropertyInfo divAttributeName = t.GetProperty("TextDivAttributeName", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);

                initAsync(link);
                text = GetText((string)divAttribute.GetValue(instance), (string)divAttributeName.GetValue(instance));
            }

            return text;
        }


        public static string text="";
        public static string GetTextNewLine(HtmlNode node, string text)
        {
            if(node.HasChildNodes && !node.FirstChild.Name.Equals("ul") && !node.FirstChild.Name.Equals("#text"))
            {
                foreach (HtmlNode n in node.ChildNodes)
                {
                    if (!n.InnerText.Equals(""))
                        if (n.Attributes.Count > 0)
                        {
                            if (n.Attributes.Where(x => x.Value.Contains("button") || x.Value.Contains("instagram") || x.Value.Contains("image") || x.Value.Contains("caption") || x.Value.Contains("collapsible") || x.Value.Contains("clickable") || x.Value.Contains("sub-title")).Count() > 0)
                            {

                            }
                            else text = GetTextNewLine(n, text);
                        }  
                    else text = GetTextNewLine(n, text);
                }
                    

                return text;
            }
            else 
            if (node.Attributes.Count > 0)
            {
                if(node.Attributes.Where(x => x.Value.Contains("button") || x.Value.Contains("instagram") || x.Value.Contains("image") || x.Value.Contains("collapse") || x.Value.Contains("sub-title")).Count()>0)
                {
                    return text + "";
                }
               else return text + node.InnerText + "\n";
                    
                  
               
            }
            else return text + node.InnerText + "\n";
            
            
        }
        public static string GetText(string divAttribute, string divAttributeName)
        {
            string text = "";
            HtmlNode divNode = GetFirstDescendant("div", divAttribute, divAttributeName);
            List<HtmlNode> textNodes;
            List<HtmlNode> nodes = new List<HtmlNode>();
            
            if (divNode != null)
            {

                textNodes = divNode.ChildNodes.ToList();
            }
            else return text;


            foreach (HtmlNode textNode in textNodes)
            {
                //text += textNode.InnerText;
                text = GetTextNewLine(textNode,text);
            }

            return text;
        }

        private static string GetTextDescendants(HtmlNode textNode, string name)
        {
            string text="";
            List<HtmlNode> nodes = new List<HtmlNode>();

            nodes = textNode.Descendants(name).ToList();
            if (nodes != null && nodes.Count() != 0)
            {
                text += nodes.FirstOrDefault().InnerText;
            }

            return text;
        }


        static string RemoveAccents(string input)
        {
            string normalized = input.Normalize(NormalizationForm.FormKD);
            Encoding removal = Encoding.GetEncoding(Encoding.ASCII.CodePage,
                                                    new EncoderReplacementFallback(""),
                                                    new DecoderReplacementFallback(""));
            byte[] bytes = removal.GetBytes(normalized);
            return Encoding.ASCII.GetString(bytes);
        }

        public static string Preprocess(string text)
        {
            text = Regex.Replace(text, @"['’]+s", "");
            text = RemoveAccents(text);

            //remove non utf8 
            text = Regex.Replace(text, @"[^\u0000-\u007F]+", string.Empty);

            text = Regex.Replace(text, "&#[0-9]*;", " ");
            text = Regex.Replace(text, "&nbsp;", " ");
            text = Regex.Replace(text, "&amp;", " ");
            text = Regex.Replace(text, ";", " ");
            text = Regex.Replace(text, "[\x82-\xFF]", " ");
            text = Regex.Replace(text, "\\u2023", "");


            //remove string from under photo
            text = Regex.Replace(text, @"((([[0-9+A-Za-z_]+,\s+)+([A-Z0-9+a-z]+\s+)*)*\|\s+([0-9+A-Za-z_\s]+\/Flickr+(\|\s+[A-Z_a-z\s]+\/Flickr)*))", "\n");
            text = Regex.Replace(text, @"((([[0-9+A-Za-z_]+,\s+)+([A-Z0-9+a-z]+\s+)*)*\s+([0-9+A-Za-z_\s]+\/Flickr+))", "\n");
            //Remove More Info
            text = Regex.Replace(text, @"(More Info[A-Za-z\s0-9,+]+\sfeedback)", "");
            text = Regex.Replace(text, @"([\sa-zA-Z,$]+Add to Plan)", "\n");
            text = Regex.Replace(text, @"(More Info[A-Za-z\s0-9,+]+\s\$\$\$Add to Plan)", "");

            //Remove #instagram
            text = Regex.Replace(text, @"#([a-z]+)", "");
            //Remove Address
            text = Regex.Replace(text, @"(Address[\sa-zA-Z:&,.0-9]+)\+([0-9\s]+)", "\n");
            //Remove Address
            text = Regex.Replace(text, @"(([A-Za-z\s0-9.]+),\s)*\+([0-9\s]+)", "\n");
            //Remove instagram post
            text = Regex.Replace(text, @"(A photo posted by [A-Z&a-z,@0-9\s:()_]+PDT)", "\n");
            text = Regex.Replace(text, @"(A photo posted by [A-Za&-z,@0-9\s:()_]+PST)", "\n");
            text = Regex.Replace(text, @"(A photo posted by [A-Za-z,@&0-9\s:()_]+ at [0-9]+:[0-9]+)", "\n");
            text = Regex.Replace(text, @"PST", "\n");
            text = Regex.Replace(text, @"PDT", "\n");
            
            //text = Regex.Replace(text, "  ", "\n");

            text = Regex.Replace(text, @"(([\n\t])+|(\s\s)+)", "\n");
            //text = Regex.Replace(text, @"(([  ])+)", "");
            
            text = new string(text.Where(c => char.IsLetter(c) || char.IsDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c) || !char.IsControl(c)).ToArray());

            text = Regex.Replace(text, "([a-z])([A-Z])", "$1\n$2");
           
            text = Regex.Replace(text, "Give us feedback", "");

            return text;
        }

        public static List<string>  GetTags(string text)
        {
            List<string> tagsList = new List<string>();
            string tags;
            if (text != null)
            {
                text = Preprocess(text);

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

        public void UpdateBlogsTags(List<Blog> blogs)
        {
            foreach (Blog blog in blogs)
            {
                    text = Crawler.ReadTextFrom(blog.BlogLink);
                    text = Crawler.Preprocess(text);

                    List<string> tagsList = new List<string>();
                    tagsList = Crawler.GetTags(text);

                    tagsList = tagsList.Where(tag => tag.Length > 2).ToList();

                  
                    tagsList.AddRange(blog.Tags);
                    tagsList = tagsList.Distinct<string>().ToList();

                    FireBaseDatabase.UpdateBlog(blog.Id, "tags", tagsList);
            }
        }

        public int CountWordInBlog(List<Blog> blogs, string word)
        {
            int count = 0;

            foreach (Blog blog in blogs)
            {
                text = Crawler.ReadTextFrom(blog.BlogLink);
                text = Crawler.Preprocess(text);

                count += Regex.Matches(text.ToLower(), word.ToLower()).Count;

            }

            return count;

        }
        public void CreateBlogs()
        {
            List<string> tagsList = new List<string>();

            Blog blog;
            string text;
            string date;
            string tags;

            foreach (string key in blogs.Keys)
            {
                foreach (string link in blogs[key])
                {
                    text = "";
                    date = "";
                    tags = "";
                    tagsList = new List<string>();
                    blog = new Blog();
                    blog.LocationName = key;
                    blog.BlogLink = link;
                    initAsync(link);
                    text = Crawler.GetText(TextDivAttribute, TextDivAttributeName);

                    tagsList = GetTags(text);

                    blog.Tags = tagsList;

                    date = GetDate();
                    blog.Date = date;

                    FireBaseDatabase.Insert(blog);

                }
            }
        }
    }

}

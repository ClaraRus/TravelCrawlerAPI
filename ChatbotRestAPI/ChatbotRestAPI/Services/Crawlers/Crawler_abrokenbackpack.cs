using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebCrawler
{
    class Crawler_abrokenbackpack : Crawler
    {
        
        public Crawler_abrokenbackpack()
        {
            string[] blogAttrbutes = { "main-content" };

            ContinentDivAttribute = "class";
            ContinentDivAttributeName = "et_pb_row et_pb_row_3";
            RootUrl = "https://abrokenbackpack.com/";
            CountryDivAttribute = "class";
            CountryDivAttributeName = "et_pb_section et_pb_section_1 et_pb_with_background et_section_regular";
            StateDivAttribute = null;
            StateDivAttributeName = null;
            CityDivAttribute = null;
            CityDivAttributeName = null;
            BlogDivAttribute = "id";

            BlogDivAttributeName = blogAttrbutes;

            PatternToFilter = "category";
            PatternToFilterBlogs = "abrokenbackpack.com/([0-9])*/([0-9])*/([0-9])*/";
            TextDivAttribute = "class";
            TextDivAttributeName = "entry-content";

            DateDivAttribute = "class";
            DateDivAttributeName = "published";
        }


        

    }
        
}

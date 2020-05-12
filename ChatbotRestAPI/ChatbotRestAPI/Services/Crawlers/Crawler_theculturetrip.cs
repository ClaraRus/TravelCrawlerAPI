using System;
using System.Collections.Generic;
using System.Text;

namespace WebCrawler
{
	class Crawler_theculturetrip : Crawler
	{
        public Crawler_theculturetrip()
        {
            string[] blogAttrbutes = { "focus-generalstyled__FocusMainWrapper-busea-2 cbnMGd", "location-pagestyled__LocationMain-eijsx2-0 jIxKUi" };
            AddContinentLinks();
            //ContinentDivAttribute =
            //ContinentDivAttributeName =
            RootUrl = "https://theculturetrip.com";
            CountryDivAttribute = "id";
            CountryDivAttributeName = "subLocationWrap";
            StateDivAttribute = "id";
            StateDivAttributeName = "subLocationWrap";
            CityDivAttribute = "id";
            CityDivAttributeName = "subLocationWrap";
            BlogDivAttribute = "class";

            BlogDivAttributeName = blogAttrbutes;

            PatternToFilter = "theculturetrip";
            PatternToFilterBlogs = "theculturetrip.com/[a-z]*/[a-z]*/articles/";

            TextDivAttribute = "id";
            TextDivAttributeName = "page-content";

            DateDivAttribute = "data-automation-id";
            DateDivAttributeName = "author-info-date";
       
            


        }
  
            
            //Have no access from html to the drop-down menu that holds them 
            private void AddContinentLinks()
            {
                continentLinks.Add("https://theculturetrip.com/europe/");
                continentLinks.Add("https://theculturetrip.com/north-america/");
                continentLinks.Add("https://theculturetrip.com/asia/");
                continentLinks.Add("https://theculturetrip.com/africa/");
                continentLinks.Add("https://theculturetrip.com/south-america/");
                continentLinks.Add("https://theculturetrip.com/middle-east/");
                continentLinks.Add("https://theculturetrip.com/pacific/");
                
            }
            
	}
}

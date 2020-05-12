using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WebCrawler
{
     
    class DatabasePopulation
    {
       
        static void Main(string[] args)
        {

            FireBaseDatabase firebaseDB = new FireBaseDatabase();

            //Crawler_abrockenbackpack crawlerBrockenBackpack = new Crawler_abrockenbackpack();
            //crawlerBrockenBackpack.StartCrawler().Wait();
            //crawlerBrockenBackpack.CreateBlogs().Wait();

            Crawler_theculturetrip crawlerTheCultureTrip = new Crawler_theculturetrip();
            
            crawlerTheCultureTrip.StartCrawler();
            crawlerTheCultureTrip.CreateBlogs();

        }

    }


    /*string [] cities= { "India", "Cambodia", "Indonesia", "China", "Malaysia", "Vietnam", "Singapore" };
         string text;

         foreach (string city in cities)
         {
             text = crawler.GetBlogsText(city).Result;
             using (StreamWriter outputFile = new StreamWriter(Path.Combine("C:\\Users\\Clara2\\Desktop", city + "Blog.txt")))
             {
                 outputFile.WriteLine(text);
             }
         }*/
}

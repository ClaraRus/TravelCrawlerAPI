using ChatbotRestAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebCrawler;

namespace ChatbotRestAPI.Controllers
{

    public class ResponseController : ApiController
    {
        
        // GET      /GetResponse
        [System.Web.Http.ActionName("GetResponse")]
        public string GetResponse(string input)
        {
            try
            {
                return ResponseControllerRepository.GetResponse(input);
            }
            catch (Exception e)
            {
                 throw new Exception("Failed", e);
            }
            
        }



        // GET      /CheckLocation
        [System.Web.Http.ActionName("GetCheckLocation")]
        public string GetCheckLocation(string input)
        {
            try
            {
                return ResponseControllerRepository.CheckLocation(input).ToString();
            }
            catch (Exception e)
            {
                throw new Exception("Failed", e);
            }

        }

        // GET      /GetTags
        [System.Web.Http.ActionName("GetTagsFromBlog")]
        public string GetTagsFromBlog(string input)
        {
            try
            {
                return ResponseControllerRepository.TagsFromBlog(input).ToString();
            }
            catch (Exception e)
            {
                throw new Exception("Failed", e);
            }

        }

        // GET      /GetTags
        [System.Web.Http.ActionName("GetParagraphs")]
        public string GetParagraphs(string input)
        {
            try
            {
               
                return ResponseControllerRepository.ExtractParagraphs(input).ToString();
            }
            catch (Exception e)
            {
                string exception = e.Message;
                throw new Exception("Failed", e);
            }

        }



    }
}
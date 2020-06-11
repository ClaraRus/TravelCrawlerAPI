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

        string adminKey = "y1r3QaQoPS18ccBc99vy08QCJcM6ik";

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
        [System.Web.Http.ActionName("CreateDataset")]
        public string GetCreateDataset(string input)
        {
            try
            {
                if(input.Contains(adminKey))
                    return ResponseControllerRepository.CreateDataset(input).ToString();
                else return "Permission Denied!";
            }
            catch (Exception e)
            {
                throw new Exception("Failed", e);
            }

        }

        [System.Web.Http.ActionName("UpdateDatasetTags")]
        public string GetUpdateDataset(string input)
        {
            try
            {
                if (input.Contains(adminKey))
                    return ResponseControllerRepository.UpdateDatasetTags(input).ToString();
                else return "Permission Denied!";

            }
            catch (Exception e)
            {
                throw new Exception("Failed", e);
            }

        }

        // GET      /GetParagraphs
        [System.Web.Http.ActionName("GetParagraphs")]
        public string GetParagraphs(string input)
        {
            try
            {
               
                return ResponseControllerRepository.ExtractParagraphs(input).ToString();
            }
            catch (Exception e)
            {
                throw new Exception("Failed", e);
            }

        }



    }
}
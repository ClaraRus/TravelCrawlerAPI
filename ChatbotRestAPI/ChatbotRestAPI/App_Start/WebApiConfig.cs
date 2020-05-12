﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ChatbotRestAPI
{
	public static class WebApiConfig
	{
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
			name: "ApiByAction",
			routeTemplate: "api/response/GetResponse/{input}",
			defaults: new { controller = "response", action = "GetResponse" }
		);

			config.Routes.MapHttpRoute(
			name: "CheckLocation",
			routeTemplate: "api/response/GetCheckLocation/{input}",
			defaults: new { controller = "response", action = "GetCheckLocation" }
		);

			config.Routes.MapHttpRoute(
			name: "TagsFromBlog",
			routeTemplate: "api/response/GetTagsFromBlog/{input}",
			defaults: new { controller = "response", action = "GetTagsFromBlog" }
		);

			config.Routes.MapHttpRoute(
			name: "Paragraphs",
			routeTemplate: "api/response/GetParagraphs/{input}",
			defaults: new { controller = "response", action = "GetParagraphs" }
		);
			
			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}
	}
}

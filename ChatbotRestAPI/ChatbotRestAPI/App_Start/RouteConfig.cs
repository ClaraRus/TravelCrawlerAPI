using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ChatbotRestAPI
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
			name: "GetResponse",
			url: "Response/GetResponse",
			defaults: new { controller = "Response", action = "GetResponse" }
			);

			routes.MapRoute(
			name: "GetCheckLocation",
			url: "Response/GetCheckLocation",
			defaults: new { controller = "Response", action = "GetCheckLocation" }
			);

			routes.MapRoute(
			name: "GetTagsFromBlog",
			url: "Response/GetTagsFromBlog",
			defaults: new { controller = "Response", action = "GetTagsFromBlog" }
			);

			routes.MapRoute(
			name: "GetParagraphs",
			url: "Response/GetParagraphs",
			defaults: new { controller = "Response", action = "GetParagraphs" }
			);


			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}

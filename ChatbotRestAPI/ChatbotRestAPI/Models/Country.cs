using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatbotRestAPI.Models
{
	public class Country
	{
		public string Continent_id { set; get; }
		public string Id { set; get; }
		public string Name { set; get; }
		public string Sub_continent_id { set; get; }
	}
}
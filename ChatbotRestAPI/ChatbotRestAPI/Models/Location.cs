using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatbotRestAPI.Models
{
	public class World
	{
		public List<Continent> Geo_continents { set; get; }
		public List<Country> Geo_countries { set; get; }
		public List<State> Geo_states { set; get; }
		public List<SubContinents> Geo_sub_continents { set; get; }
		public List<City> Geo_cities { set; get; }


	}
}
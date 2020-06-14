using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatbotRestAPI.Models
{
	public class Destination
	{
		private List<string> city = new List<string>();
		private List<string> country = new List<string>();
		private List<string> state = new List<string>();
		private List<string> continent = new List<string>();
		public List<string> City
		{
			set { city = value; }
			get { return city; }
		}

		public List<string> Country
		{
			set { country = value; }
			get { return country; }
		}

		public List<string> State
		{
			set { state = value; }
			get { return state; }
		}

		public List<string> Continent
		{
			set { continent = value; }
			get { return continent; }
		}
		public override string ToString()
		{
			string dst = "";
			if(City!=null)
			{
				foreach(string c in City)
					dst += c+" ";
			}

			if(Country!=null)
			{
				foreach (string c in Country)
					dst += c+" ";
			}

			if(State!=null)
			{
				foreach (string s in State)
					dst += s+" ";
			}

			if(Continent!=null)
			{
				foreach (string c in Continent)
					dst += c+" ";
			}

			return dst;
		}
	}
}
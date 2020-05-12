using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatbotRestAPI.Models
{
	public class Destination
	{
		public string City { set; get; }
		public string Country { set; get; }
		public string State { set; get; }
		public string Continent { set; get; }

		public override string ToString()
		{
			string dst = "";
			if(City!=null)
			{
				dst += City+" ";
			}

			if(Country!=null)
			{
				dst += Country+" ";
			}

			if(State!=null)
			{
				dst += State+" ";
			}

			if(Continent!=null)
			{
				dst += Continent;
			}

			return dst;
		}
	}
}
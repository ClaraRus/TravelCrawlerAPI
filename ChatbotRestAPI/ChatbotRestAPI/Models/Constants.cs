using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatbotRestAPI.Models
{
	public class Constants
	{
		public static readonly string[] ACCOMODATION_TAGS = { "hotel", "hostel", "appartment"};
		public static readonly string[] RESTAURANTS_TAGS = { "food", "drinks", "restaurant", "caffe", "bar", "pub"};

		public static readonly string ACTIVITIES = "activities";
		public static readonly string ACCOMODATION = "accomodation";
		public static readonly string RESTAURANTS = "restaurants";

	}
}
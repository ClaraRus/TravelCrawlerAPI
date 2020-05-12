using System;
using System.Collections.Generic;
using System.Text;

namespace WebCrawler
{
	class Blog
	{
		public string BlogLink { set; get; }
		public string Date { set; get; }
		public string LocationName { set; get; }
		public List<string> Tags { set; get; }

		public string Id { set; get; }

	}
}

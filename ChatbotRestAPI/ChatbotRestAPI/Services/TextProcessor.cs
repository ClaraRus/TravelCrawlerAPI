using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ChatbotRestAPI.Services
{
	public class TextProcessor
	{
        public static string RemoveAccents(string input)
        {
            string normalized = input.Normalize(NormalizationForm.FormKD);
            Encoding removal = Encoding.GetEncoding(Encoding.ASCII.CodePage,
                                                    new EncoderReplacementFallback(""),
                                                    new DecoderReplacementFallback(""));
            byte[] bytes = removal.GetBytes(normalized);
            return Encoding.ASCII.GetString(bytes);
        }

        public static List<string> PreprocessTags(List<string> tags)
        {

            tags.ForEach(tag => tag = new string(tag.Where(c => !char.IsPunctuation(c) && !char.IsWhiteSpace(c)).ToArray()));
            tags = tags.Where(tag => !tag.Equals("")).ToList();

            return tags;
        }

        public static string FormatValutToJson(string fieldToUpdate, List<string> value)
        {
            string valueToUpdate = "{\"" + fieldToUpdate + "\":[";
            foreach (string v in value)
            {
                if (v.Equals(value.Last()))
                    valueToUpdate += "\"" + v + "\"";
                else valueToUpdate += "\"" + v + "\"" + ",";

            }
            valueToUpdate += "]}";

            return valueToUpdate;
        }

        public static string Preprocess(string text)
        {
            text = Regex.Replace(text, @"['’]+s", "");
            text = RemoveAccents(text);

            //remove non utf8 
            text = Regex.Replace(text, @"[^\u0000-\u007F]+", string.Empty);

            text = Regex.Replace(text, "&#[0-9]*;", " ");
            text = Regex.Replace(text, "&nbsp;", " ");
            text = Regex.Replace(text, "&amp;", " ");
            text = Regex.Replace(text, ";", " ");
            text = Regex.Replace(text, "[\x82-\xFF]", " ");
            text = Regex.Replace(text, "\\u2023", "");


            //remove string from under photo
            text = Regex.Replace(text, @"((([[0-9+A-Za-z_]+,\s+)+([A-Z0-9+a-z]+\s+)*)*\|\s+([0-9+A-Za-z_\s]+\/Flickr+(\|\s+[A-Z_a-z\s]+\/Flickr)*))", "\n");
            text = Regex.Replace(text, @"((([[0-9+A-Za-z_]+,\s+)+([A-Z0-9+a-z]+\s+)*)*\s+([0-9+A-Za-z_\s]+\/Flickr+))", "\n");
            //Remove More Info
            text = Regex.Replace(text, @"(More Info[A-Za-z\s0-9,+]+\sfeedback)", "");
            text = Regex.Replace(text, @"([\sa-zA-Z,$]+Add to Plan)", "\n");
            text = Regex.Replace(text, @"(More Info[A-Za-z\s0-9,+]+\s\$\$\$Add to Plan)", "");

            //Remove #instagram
            text = Regex.Replace(text, @"#([a-z]+)", "");
            //Remove Address
            text = Regex.Replace(text, @"(Address[\sa-zA-Z:&,.0-9]+)\+([0-9\s]+)", "\n");
            //Remove Address
            text = Regex.Replace(text, @"(([A-Za-z\s0-9.]+),\s)*\+([0-9\s]+)", "\n");
            //Remove instagram post
            text = Regex.Replace(text, @"(A photo posted by [A-Z&a-z,@0-9\s:()_]+PDT)", "\n");
            text = Regex.Replace(text, @"(A photo posted by [A-Za&-z,@0-9\s:()_]+PST)", "\n");
            text = Regex.Replace(text, @"(A photo posted by [A-Za-z,@&0-9\s:()_]+ at [0-9]+:[0-9]+)", "\n");
            text = Regex.Replace(text, @"PST", "\n");
            text = Regex.Replace(text, @"PDT", "\n");

            //text = Regex.Replace(text, "  ", "\n");

            text = Regex.Replace(text, @"(([\n\t])+|(\s\s)+)", "\n");
            //text = Regex.Replace(text, @"(([  ])+)", "");

            text = new string(text.Where(c => char.IsLetter(c) || char.IsDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c) || !char.IsControl(c)).ToArray());

            text = Regex.Replace(text, "([a-z])([A-Z])", "$1\n$2");

            text = Regex.Replace(text, "Give us feedback", "");

            return text;
        }

    }
}
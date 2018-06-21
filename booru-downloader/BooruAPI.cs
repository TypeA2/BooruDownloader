using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using System.Diagnostics;

namespace booru_downloader {
    public class BooruAPI {
        public class API {
            public abstract class APISource {

            }

            public class Danbooru : APISource {
                public static string Count = "https://danbooru.donmai.us/counts/posts.xml";
                public static string Posts = "https://danbooru.donmai.us/posts.xml";
                public static string Post = "https://danbooru.donmai.us/posts.xml";
                public static int PageLimit = 200;
            }

            public class Konachan : APISource {
                public static string Count = "https://konachan.com/post.xml";
                public static string Posts = "https://konachan.com/post.xml";
                public static string Post = "https://konachan.com/post.xml";
                public static int PageLimit = 100;
            }

            public class Yandere : APISource {
                public static string Count = "https://yande.re/post.xml";
                public static string Posts = "https://yande.re/post.xml";
                public static string Post = "https://yande.re/post.xml";
                public static int PageLimit = 100;
            }
        }

        public enum Source {
            Danbooru,
            Konachan,
            Yandere
        }

        public static async Task<ulong> GetPostCount(string tag, Source source) {
            if (source == Source.Danbooru) {

                return ulong.Parse( // Parse ulong from string
                    XElement.Parse( // Parse XML to XElement
                        await WebHelper.GetAsync(
                            WebHelper.FormatGet( // Format the GET request
                                API.Danbooru.Count, // The base URL
                                new KeyValuePair<string, string>("tags", tag) // The data in KeyValuePairs
                                )))
                        .Descendants("posts") // Get the descendants of the "posts" tag
                        .First().Value); // Get the string value (innerXML) of the first tag (always the <count> tag)

            } else if (source == Source.Konachan) {

                return ulong.Parse(
                    XElement.Parse(
                        await WebHelper.GetAsync(
                            WebHelper.FormatGet(
                                API.Konachan.Count,
                                new KeyValuePair<string, string>("tags", tag),
                                new KeyValuePair<string, string>("limit", 1.ToString()) // Limit to 1 to save time
                                )))
                        .Attribute("count").Value); // Get the string value of the "count" attribute on the main node

            } else if (source == Source.Yandere) {

                // Identical to Source.Konachan
                return ulong.Parse(
                    XElement.Parse(
                        await WebHelper.GetAsync(
                            WebHelper.FormatGet(
                                API.Yandere.Count,
                                new KeyValuePair<string, string>("tags", tag),
                                new KeyValuePair<string, string>("limit", 1.ToString())
                                )))
                        .Attribute("count").Value);
            }

            return 0UL;
        }

    }
}

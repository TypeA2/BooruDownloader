using System.Xml.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace booru_downloader {
    public class BooruAPI {
        public class API {

            public class Danbooru {
                public static string Count = "https://danbooru.donmai.us/counts/posts.xml";
                public static string Posts = "https://danbooru.donmai.us/posts.xml";
                // public static string Post = "https://danbooru.donmai.us/posts.xml";
                // public static string Aliases = "https://danbooru.donmai.us/tag_aliases.xml";
                public static int PageLimit = 200;
            }

            public class Konachan {
                public static string Count = "https://konachan.com/post.xml";
                public static string Posts = "https://konachan.com/post.xml";
                // public static string Post = "https://konachan.com/post.xml";
                public static int PageLimit = 100;
            }

            public class Yandere {
                public static string Count = "https://yande.re/post.xml";
                public static string Posts = "https://yande.re/post.xml";
                // public static string Post = "https://yande.re/post.xml";
                public static int PageLimit = 100;
            }
        }

        public struct Post {
            public ulong Id { get; }

            public DateTime Uploaded { get; }

            public string Source { get; }
            public string Md5 { get; }
            public string FileExt { get; }
            public ulong FileSize { get; }
            public string FileUrl { get; }

            public char Rating { get; }

            public ulong Width { get; }
            public ulong Height { get; }

            public string TagString { get; }

            public Post (ulong id,
                DateTime uploaded, 
                string source, string md5, string fileExt, ulong fileSize, string fileUrl,
                char rating,
                ulong width, ulong height,
                string tagString) {

                this.Id = id;

                this.Uploaded = uploaded;

                this.Source = source;
                this.Md5 = md5;
                this.FileExt = fileExt;
                this.FileSize = fileSize;
                this.FileUrl = fileUrl;

                this.Rating = rating;

                this.Width = width;
                this.Height = height;

                this.TagString = tagString;
            }
        }

        public enum Source {
            Danbooru,
            Konachan,
            Yandere
        }

        [Flags]
        public enum Rating {
            None                    = 0,
            Safe                    = 1,
            Questionable            = 2,
            Explicit                = 4,
            SafeQuestionable        = Safe | Questionable,
            QuestionableExplicit    = Questionable | Explicit,
            Any                     = Safe | Questionable | Explicit
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

        public static async Task<List<Post>> GetPage(string tags, ulong page, Source source, Rating rating, params string[] filters) {

            if (source == Source.Danbooru) {

                string responseString = await WebHelper.GetAsync(
                        WebHelper.FormatGet(
                            API.Danbooru.Posts,
                            new KeyValuePair<string, string>("tags", tags),
                            new KeyValuePair<string, string>("page", page.ToString()),
                            new KeyValuePair<string, string>("limit", API.Danbooru.PageLimit.ToString()
                            )));

                if (string.IsNullOrWhiteSpace(responseString)) {
                    return new List<Post>();
                }

                XElement response = XElement.Parse(responseString);

                List<XElement> posts = response.Descendants("post").ToList();

                List<Post> result = new List<Post>();

                foreach (XElement post in posts) {

                    string tagString = post.Descendants("tag-string").First().Value;
                    char ratingChar = post.Descendants("rating").First().Value[0];
                    bool skip = false;

                    foreach (string tag in filters) {
                        if (!tagString.Contains(tag)) {
                            skip = true;
                            break;
                        }
                    }

                    if (skip || !IsRating(ratingChar, rating)) {
                        continue;
                    }

                    string md5 = post.Descendants("md5").FirstOrDefault()?.Value;

                    if (string.IsNullOrEmpty(md5)) {
                        // Danbooru omits md5, file-ext, file-url if the tag requires an upgraded account

                        continue;
                    }

                    result.Add(new Post(
                        ulong.Parse(post.Descendants("id").First().Value),
                        DateTime.Parse(post.Descendants("created-at").First().Value),
                        post.Descendants("source").First().Value,
                        md5,
                        post.Descendants("file-ext").First().Value,
                        ulong.Parse(post.Descendants("file-size").First().Value),
                        post.Descendants("file-url").First().Value,
                        ratingChar,
                        ulong.Parse(post.Descendants("image-width").First().Value),
                        ulong.Parse(post.Descendants("image-height").First().Value),
                        tagString
                        ));
                }

                return result;
            } else if (source == Source.Konachan || source == Source.Yandere) {
                XElement response = XElement.Parse(
                    await WebHelper.GetAsync(
                        WebHelper.FormatGet(
                            (source == Source.Konachan) ? API.Konachan.Posts : API.Yandere.Posts,
                            new KeyValuePair<string, string>("tags", tags),
                            new KeyValuePair<string, string>("page", page.ToString()),
                            new KeyValuePair<string, string>("limit", (source == Source.Konachan) ? API.Konachan.PageLimit.ToString() : API.Yandere.PageLimit.ToString())
                            )));

                List<XElement> posts = response.Descendants("post").ToList();

                List<Post> result = new List<Post>();

                foreach (XElement post in posts) {

                    string tagString = post.Attribute("tags").Value;
                    char ratingChar = post.Attribute("rating").Value[0];
                    bool skip = false;
                    
                    foreach (string tag in filters) {
                        if (!tagString.Contains(tag)) {
                            skip = true;
                            break;
                        }
                    }

                    if (skip || !IsRating(ratingChar, rating)) {
                        continue;
                    }

                    result.Add(new Post(
                        ulong.Parse(post.Attribute("id").Value),
                        DateTimeOffset.FromUnixTimeSeconds(long.Parse(post.Attribute("created_at").Value)).DateTime,
                        post.Attribute("source").Value,
                        post.Attribute("md5").Value,
                        Path.GetExtension(post.Attribute("file_url").Value.Split('?')[0]),
                        ulong.Parse(post.Attribute("file_size").Value),
                        post.Attribute("file_url").Value,
                        ratingChar,
                        ulong.Parse(post.Attribute("width").Value),
                        ulong.Parse(post.Attribute("height").Value),
                        tagString
                        ));
                }

                return result;
            }

            return new List<Post>();
        }

        // TODO implement tag alias resolving

        /*public static async Task<string> ResolveTagAlias(string tag, Source source) {
            if (source == Source.Danbooru) {
                XElement resolved = XElement.Parse(
                    await WebHelper.GetAsync(
                        WebHelper.FormatGet(
                            API.Danbooru.Aliases,
                            new KeyValuePair<string, string>("search[antecedent_name]", tag)
                            )));

                if (resolved.Descendants("tag-alias").Count() == 0) {
                    return tag;
                } else {
                    return resolved
                        .Descendants("tag-alias")
                        .First()
                        .Descendants("consequent-name")
                        .First()
                        .Value;
                }
            } else if (source == Source.Konachan || source == Source.Yandere) {

            }

            return "";
        }*/

        public static bool IsRating(char test, Rating rating) {

            switch (test) {

                case 's':
                    return (rating & Rating.Safe) == Rating.Safe;

                case 'q':
                    return (rating & Rating.Questionable) == Rating.Questionable;


                case 'e':
                    return (rating & Rating.Explicit) == Rating.Explicit;

                default:
                    return false;
            }
        }
    }
}

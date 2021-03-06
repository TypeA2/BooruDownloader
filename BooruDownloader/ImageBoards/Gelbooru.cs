﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using BooruDownloader.Base;
using Newtonsoft.Json.Linq;

namespace BooruDownloader {
    [ImageBoard]
    public class Gelbooru : TaggedImageBoard {
        public override string Name { get => "Gelbooru"; }
        public override int MaxTagGroup { get => 6; }
        public override int MaxEntriesPerPage { get => 100; }
        public override string ApiBaseEndpoint { get => "https://gelbooru.com"; }

        public override Dictionary<string, string> Endpoints {
            get => new Dictionary<string, string> {
                ["Count"] = "/index.php?page=dapi&s=post&q=index",
                ["Posts"] = "/index.php?page=dapi&s=post&q=index&json=1"
            };
        }

        public override async Task<long> PostCount(IList<ImageTag> tags) {
            if (tags.TagCost() > MaxTagGroup) {
                throw new TooManyTagsException(
                    $"Cannot search for more than {MaxTagGroup} at a time (current: {tags.TagCost()}.");
            }

            XElement el = XElement.Parse(await PostRequestAsync(ApiBaseEndpoint, Endpoints["Count"],
                new Dictionary<string, string> {
                    ["tags"] = tags.AsTagString(),
                    ["limit"] = "1"
                }));

            return long.Parse(el.Attribute("count")?.Value ?? "0");
        }

        public override async Task<IList<Post>> GetPage(IList<ImageTag> tags, long page) {
            if (tags.TagCost() > MaxTagGroup) {
                throw new TooManyTagsException(
                    $"Cannot search for more than {MaxTagGroup} at a time (current: {tags.TagCost()}.");
            }

            string json_string = await PostRequestAsync(Endpoints["Posts"],
                new Dictionary<string, string> {
                    ["tags"] = tags.AsTagString(),
                    ["pid"] = page.ToString(),
                    ["limit"] = MaxEntriesPerPage.ToString()
                });

            JArray json = JArray.Parse(json_string);

            List<Post> posts = new List<Post>(json.Count);

            foreach (JToken token in json.Children()) {
                if (token["hash"] == null || token["file_url"] == null) {
                    continue;
                }

                Post post = token.ToObject<Post>();

                post.Board = Name;

                posts.Add(post);
            }

            return posts;
        }
    }
}

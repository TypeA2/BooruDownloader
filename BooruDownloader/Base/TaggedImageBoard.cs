using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooruDownloader.Base;
using Newtonsoft.Json;

namespace BooruDownloader {
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ImageBoardAttribute : Attribute { }

    public class TooManyTagsException : Exception {
        public TooManyTagsException(string message) : base(message) { }
        public TooManyTagsException(string message, Exception inner) : base(message, inner) { }
    }

    public class WrongSourceException : Exception {
        public WrongSourceException(string message) : base(message) { }
        public WrongSourceException(string message, Exception inner) : base(message, inner) { }
    }

    public abstract class TaggedImageBoard {
        public abstract string Name { get; }
        public abstract int MaxTagGroup { get; }
        public abstract int MaxEntriesPerPage { get; }
        public abstract string ApiBaseEndpoint { get; }
        public abstract Dictionary<string, string> Endpoints { get;}

        public override string ToString() {
            return Name;
        }

        public event EventHandler<long> NewMaximum;
        public event EventHandler<long> Progess;
        public event EventHandler<string> Working;
        public event EventHandler<string> WorkDone; 

        protected virtual void InvokeNewMaximum(long e) => NewMaximum?.Invoke(this, e);
        protected virtual void InvokeProgress(long e) => Progess?.Invoke(this, e);
        protected virtual void InvokeWorking(string e) => Working?.Invoke(this, e);
        protected virtual void InvokeWorkDone(string e = "") => WorkDone?.Invoke(this, e);

        public virtual async Task<IList<ImageTag>> GetBestTags(IList<ImageTag> tags, ImageTag rating = null) {
            if (tags.Count < MaxTagGroup && rating != null) {
                return new List<ImageTag>(tags) { rating }; ;
            }

            if (tags.Count <= MaxTagGroup) {
                return tags;
            }

            List<Tuple<string, long, ImageTag>> tag_counts =
                new List<Tuple<string, long, ImageTag>>(tags.Count);

            InvokeNewMaximum(tags.Count);

            for (int i = 0; i < tags.Count; i++) {
                ImageTag tag = tags[i];
                if (tag.Weight > MaxTagGroup) {
                    continue;
                }

                InvokeWorking($"Getting count for tag: {tag}");
                InvokeProgress(i + 1);


                tag_counts.Add(new Tuple<string, long, ImageTag>(tag.TagString, await PostCount(tag), tag));
            }

            InvokeWorking("Estimating best tag combination");

            tag_counts.Sort((e0, e1) => e0.Item2.CompareTo(e1.Item2));

            List<ImageTag> best_tags = tag_counts
                .Take(MaxTagGroup)
                .Select(kvp => tags.First(t => t.TagString == kvp.Item1))
                .ToList();

            InvokeWorkDone();

            if (rating != null && best_tags.Count < MaxTagGroup) {
                best_tags.Add(rating);
            }

            return best_tags;
        }

        public virtual async Task<long> PageCount(IList<ImageTag> tags) {
            InvokeNewMaximum(1);
            InvokeWorking($"Getting tag count for {String.Join(" ", tags)}");

            long count = await PostCount(tags);

            InvokeProgress(1);

            return (long) Math.Ceiling((double) count / MaxEntriesPerPage);
        }

        public virtual async Task<IList<Post>> GetPages(
            IList<ImageTag> query_tags, IList<ImageTag> tags, Post.RatingFlags rating, long page_count = -1,
            MainWindow event_handler = null) {

            bool check_canceled = (event_handler != null);

            if (page_count == -1) {
                page_count = await PageCount(query_tags);
            }

            List<Post> posts = new List<Post>();

            List<ImageTag> manual_tags = tags.Where(tag => query_tags.Any(t => t != tag)).ToList();

            InvokeNewMaximum(page_count);

            for (long i = 0; i < page_count; i++) {
                if (check_canceled
                    && event_handler.CancelQueued) {
                    return null;
                }
                InvokeWorking($"Getting page {i + 1} of {page_count} for: {query_tags.AsTagString(' ')}");
                InvokeProgress(i + 1);

                posts.AddRange((await GetPage(query_tags, i + 1))
                    .Where(post => post.IsRating(rating))
                    .Where(post => manual_tags.AppliesTo(post)).ToList());
            }

            return posts;
        }

        public abstract Task<IList<Post>> GetPage(IList<ImageTag> tags, long page);

        public abstract Task<long> PostCount(IList<ImageTag> tags);

        public virtual async Task<long> PostCount(ImageTag tag) {
            return await PostCount(new List<ImageTag> { tag });
        }



        public virtual async Task<string> PostRequestAsync(
            string base_endpoint, string endpoint, Dictionary<string, string> data) {
            return await WebHelper.GetTextAsync(base_endpoint, endpoint, data);
        }

        public virtual async Task<string> PostRequestAsync(string endpoint, Dictionary<string, string> data) {
            return await PostRequestAsync(ApiBaseEndpoint, endpoint, data);
        }

        public virtual async Task<T> PostRequestAsync<T>(
            string base_endpoint, string endpoint, Dictionary<string, string> data) {
            return JsonConvert.DeserializeObject<T>(await PostRequestAsync(base_endpoint, endpoint, data));
        }

        public virtual async Task<T> PostRequestAsync<T>(string endpoint, Dictionary<string, string> data) {
            return JsonConvert.DeserializeObject<T>(await PostRequestAsync(endpoint, data));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BooruDownloader.Utilities;

namespace BooruDownloader.Base {

    public class ImageTagParseException : Exception {
        public ImageTagParseException(string message) : base(message) { }
        public ImageTagParseException(string message, Exception inner) : base(message, inner) { }
    }

    public static class ImageTagExtensions {
        public static string AsTagString(this IEnumerable<ImageTag> tags, char concat = '+') =>
            String.Join(concat.ToString(), tags);

        public static bool AppliesTo(this IEnumerable<ImageTag> tags, Post post) =>
            tags.All(tag => tag.AppliesTo(post));

        public static bool TagEquals(this string source, string tag) {
            tag = tag.Replace('*', '%');

            return new Regex(
                @"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\")
                    .Replace(tag, ch => @"\" + ch)
                    .Replace("%", ".*") + @"\z",
                RegexOptions.Singleline).IsMatch(source);
        }
    }

    public class ImageTag {

        public virtual string TagString { get; }
        public virtual long Weight {
            get { return this.TagWeight(); }
        }

        public virtual bool IsNegated { get; }
        public virtual TagFilter Filter { get; }

        public enum TagFilter {
            Contain,
            Either,
            ID,
            Date,
            Rating,
            Parent,
            TagCount,
            Score,
            MD5,
            Width,
            Height,
            Ratio,
            MPixels,
            Filesize,
            Filetype
        }

        public ImageTag(string tag, TagFilter type, bool negated) {
            TagString = tag;
            Filter = type;
            IsNegated = negated;
        }

        public ImageTag(ImageTag other) {
            TagString = other.TagString;
            Filter = other.Filter;
            IsNegated = other.IsNegated;
        }

        public long TagWeight() {
            switch (Filter) {
                case TagFilter.Either:
                    return TagString.Split(
                        new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Length;

                case TagFilter.Rating:
                    return (!IsNegated
                            && (TagString == "rating:s"
                                || TagString == "rating:safe"))
                        ? 0 : 1;
            }

            return 1;
        }



        public virtual bool AppliesTo(Post post) {
            IList<string> tags = SplitTags(post.Tags);

            switch (Filter) {
                case TagFilter.Contain:
                    return tags.Any(tag => tag.TagEquals(TagString.Substring(IsNegated ? 1 : 0))) == !IsNegated;

                case TagFilter.Either:
                    return TagString.Split(' ')
                               .Any(part => tags.Contains(part.Substring(1)));

                case TagFilter.ID:
                    return IDApplies(TagString.Substring(
                               "id:".Length + (IsNegated ? 1 : 0)), post.ID) == !IsNegated;

                case TagFilter.Date:
                    return DateApplies(TagString.Substring("date:".Length), post.CreatedAt);

                case TagFilter.Rating:
                    return RatingApplies(TagString.Substring(
                               "rating:".Length + (IsNegated ? 1 : 0)), post.Rating) == !IsNegated;

                case TagFilter.Parent:
                    return ParentApplies(TagString.Substring(
                               "parent:".Length + (IsNegated ? 1 : 0)), post) == !IsNegated;

                case TagFilter.TagCount:
                    return ((ulong) tags.Count) == UInt64.Parse(TagString.Substring("tagcount:".Length));

                case TagFilter.Score:
                    return ScoreApplies(TagString.Substring("score:".Length), post.Score);

                case TagFilter.MD5:
                    return TagString.Substring("md5:".Length) == post.MD5;

                case TagFilter.Width:
                    return DimensionApplies(TagString.Substring("width:".Length), post.Width);

                case TagFilter.Height:
                    return DimensionApplies(TagString.Substring("height:".Length), post.Height);

                case TagFilter.Ratio:
                    return RatioApplies(TagString.Substring("ratio:".Length), 
                        (double) post.Width / post.Height);

                case TagFilter.MPixels:
                    return MPixelsApplies(TagString.Substring("mpixels:".Length), 
                        post.Width * post.Height);

                case TagFilter.Filesize:
                    return FilesizeApplies(TagString.Substring("filesize:".Length), post.FileSize);

                case TagFilter.Filetype:
                    return TagString.Substring("filetype:".Length) == post.FileUrl.FileExtension();
            }

            return false;
        }

        public static bool IDApplies(string tag, long id) {
            if (Int64.TryParse(tag, out long result)) {
                return result == id;
            }

            if (tag.EndsWith("..")) {
                return id >= Int64.Parse(tag.Substring(tag.Length - 2));
            }

            if (tag.StartsWith(">=")) {
                return id >= Int64.Parse(tag.Substring(2));
            }

            if (tag[0] == '>') {
                return id > Int64.Parse(tag.Substring(1));
            }

            if (tag.StartsWith("..") || tag.StartsWith("<=")) {
                return id <= Int64.Parse(tag.Substring(2));
            }

            if (tag.All(c => Char.IsDigit(c) || c == '.')
                && tag.Count(c => c == '.') == 2
                && tag.Contains("..")) {
                long[] range = tag.Split(new[] { ".." }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(Int64.Parse).ToArray();

                if (range.Length != 2) {
                    throw new ImageTagParseException("ID range invalid");
                }

                return id >= range[0] && id <= range[1];
            }

            if (tag.Contains('c')) {
                return tag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(Int64.Parse)
                    .Any(_id => _id == id);
            }

            throw new ImageTagParseException("ID format invalid");
        }

        public static bool DateApplies(string tag, DateTime date) {
            if (DateTime.TryParse(tag, out DateTime result)) {
                return result == date;
            }

            if (tag.EndsWith("..")) {
                return date >= DateTime.Parse(tag.Substring(tag.Length - 2));
            }

            if (tag.StartsWith(">=")) {
                return date >= DateTime.Parse(tag.Substring(2));
            }

            if (tag[0] == '>') {
                return date > DateTime.Parse(tag.Substring(1));
            }

            if (tag.StartsWith("..") || tag.StartsWith("<=")) {
                return date <= DateTime.Parse(tag.Substring(2));
            }

            if (tag.Contains("..")) {
                DateTime[] range = tag.Split(new[] { ".." }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(DateTime.Parse).ToArray();

                if (range.Length != 2) {
                    throw new ImageTagParseException("Date range invalid");
                }

                return date >= range[0] && date <= range[1];
            }

            if (tag.Contains(',')) {
                return tag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(DateTime.Parse)
                    .Any(_date => _date == date);
            }

            throw new ImageTagParseException("Date format invalid");
        }

        public static bool RatingApplies(string tag, string rating) {
            return tag == rating;
        }

        public static bool ParentApplies(string tag, Post post) {
            if (tag == "none") {
                return post.ParentID == null;
            }

            if (tag == "any") {
                return post.ParentID != null;
            }

            if (Int64.TryParse(tag, out long parent)) {
                return post.ParentID == parent;
            }

            throw new ImageTagParseException("Parent format invalid");
        }

        public static bool ScoreApplies(string tag, long score) {
            if (Int64.TryParse(tag, out long result)) {
                return result == score;
            }

            if (tag.EndsWith("..")) {
                return score >= Int64.Parse(tag.Substring(tag.Length - 2));
            }

            if (tag.StartsWith(">=")) {
                return score >= Int64.Parse(tag.Substring(2));
            }

            if (tag[0] == '>') {
                return score > Int64.Parse(tag.Substring(1));
            }

            if (tag.StartsWith("..") || tag.StartsWith("<=")) {
                return score <= Int64.Parse(tag.Substring(2));
            }

            if (tag.Contains("..")) {
                long[] range = tag.Split(new[] { ".." }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(Int64.Parse).ToArray();

                if (range.Length != 2) {
                    throw new ImageTagParseException("Score range invalid");
                }

                return score >= range[0] && score <= range[1];
            }

            if (tag.Contains(',')) {
                return tag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(Int64.Parse)
                    .Any(_score => _score == score);
            }

            throw new ImageTagParseException("Score format invalid");
        }

        public static bool DimensionApplies(string tag, long size) {
            if (Int64.TryParse(tag, out long result)) {
                return result == size;
            }

            if (tag.EndsWith("..")) {
                return size >= Int64.Parse(tag.Substring(tag.Length - 2));
            }

            if (tag.StartsWith(">=")) {
                return size >= Int64.Parse(tag.Substring(2));
            }

            if (tag[0] == '>') {
                return size > Int64.Parse(tag.Substring(1));
            }

            if (tag.StartsWith("..") || tag.StartsWith("<=")) {
                return size <= Int64.Parse(tag.Substring(2));
            }

            if (tag.Contains("..")) {
                long[] range = tag.Split(new[] { ".." }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(Int64.Parse).ToArray();

                if (range.Length != 2) {
                    throw new ImageTagParseException("Dimension range invalid");
                }

                return size >= range[0] && size <= range[1];
            }

            if (tag.Contains(',')) {
                return tag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(Int64.Parse)
                    .Any(_size => _size == size);
            }

            throw new ImageTagParseException("Dimension format invalid");
        }

        public static bool RatioApplies(string tag, double ratio) {
            if (!tag.Contains(',')
                && (tag.Count(c => c == ':') != 1
                    && tag.Count(c => c == ':') != 2)) {
                throw new ImageTagParseException("Invalid ratio format");
            }

            string[] parts = tag.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (Double.TryParse(parts[0], out double width)
                && Double.TryParse(parts[1], out double height)) {
                return ratio.Equals(width / height);
            }

            if (tag.EndsWith("..")) {
                return ratio >= (Double.Parse(parts[0]) / Double.Parse(parts[1].Substring(parts[1].Length - 2)));
            }

            if (tag.StartsWith(">=")) {
                return ratio >= (Double.Parse(parts[0].Substring(2)) / Double.Parse(parts[1]));
            }

            if (tag[0] == '>') {
                return ratio > (Double.Parse(parts[0].Substring(1)) / Double.Parse(parts[1]));
            }

            if (tag.StartsWith("..") || tag.StartsWith("<=")) {
                return ratio <= (Double.Parse(parts[0].Substring(2)) / Double.Parse(parts[1]));
            }

            if (tag.Contains("..")) {
                double[] range = tag.Split(new[] { ".." }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries))
                    .Select(r => Double.Parse(r[0]) / Double.Parse(r[1]))
                    .ToArray();

                if (range.Length != 2) {
                    throw new ImageTagParseException("Ratio range invalid");
                }

                return ratio >= range[0] && ratio <= range[1];
            }

            if (tag.Contains(',')) {
                return tag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(part => part.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries))
                    .Select(r => Double.Parse(r[0]) / Double.Parse(r[1]))
                    .Any(_ratio => _ratio.Equals(ratio));
            }

            throw new ImageTagParseException("Ratio format invalid");
        }

        public static bool MPixelsApplies(string tag, long pixels) {
            if (Double.TryParse(tag, out double result)) {
                return result.Equals(pixels);
            }

            if (tag.EndsWith("..")) {
                return pixels >= (Double.Parse(tag.Substring(tag.Length - 2)) * 1000000);
            }

            if (tag.StartsWith(">=")) {
                return pixels >= (Double.Parse(tag.Substring(2)) * 1000000);
            }

            if (tag[0] == '>') {
                return pixels > (Double.Parse(tag.Substring(1)) * 1000000);
            }

            if (tag.StartsWith("..") || tag.StartsWith("<=")) {
                return pixels <= (Double.Parse(tag.Substring(2)) * 1000000);
            }

            if (tag.Contains("..")) {
                double[] range = tag.Split(new[] { ".." }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => Double.Parse(p) * 1000000).ToArray();

                if (range.Length != 2) {
                    throw new ImageTagParseException("MPixels range invalid");
                }

                return pixels >= range[0] && pixels <= range[1];
            }

            if (tag.Contains(',')) {
                return tag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => Double.Parse(p) * 1000000)
                    .Any(_pixels => _pixels.Equals(pixels));
            }

            throw new ImageTagParseException("MPixels format invalid");
        }

        public static bool FilesizeApplies(string tag, long size) {
            try {
                double result = FileSizeParser.ParseSize(tag);

                return result.Equals(size);
            } catch (FileSizeParseException) { }

            if (tag.EndsWith("..")) {
                return size >= FileSizeParser.ParseSize(tag.Substring(tag.Length - 2));
            }

            if (tag.StartsWith(">=")) {
                return size >= FileSizeParser.ParseSize(tag.Substring(2));
            }

            if (tag[0] == '>') {
                return size > FileSizeParser.ParseSize(tag.Substring(1));
            }

            if (tag.StartsWith("..") || tag.StartsWith("<=")) {
                return size <= FileSizeParser.ParseSize(tag.Substring(2));
            }

            if (tag.Contains("..")) {
                double[] range = tag.Split(new[] { ".." }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(FileSizeParser.ParseSize).ToArray();

                if (range.Length != 2) {
                    throw new ImageTagParseException("Filesize range invalid");
                }

                return size >= range[0] && size <= range[1];
            }

            throw new ImageTagParseException("Filesize format invalid");
        }



        public static IList<ImageTag> ParseTags(IList<string> tags) {
            List<ImageTag> parsed = new List<ImageTag>(tags.Count);
            string or_tags = String.Empty;

            foreach (string tag in tags) {
                if (tag.Length == 1
                    && (tag[0] == '-' || tag[0] == '~')) {
                    throw new ImageTagParseException("Incomplete tag");
                }

                if (tag[0] == '-' && tag.Contains('*')) {
                    throw new ImageTagParseException("Cannot exclude wildcards");
                }

                if (tag[0] == '~' && tag.Contains('*')) {
                    throw new ImageTagParseException("Cannot or-search wildcards");
                }

                if ((tag[0] == '-' && tag[1] == '~')
                    || (tag[0] == '~' && tag[1] == '-')) {
                    throw new ImageTagParseException("Cannot negate or-searches");
                }

                if (tag[0] == '~') {
                    or_tags += tag + ' ';
                } else {
                    parsed.Add(ParseTag(tag));
                }
            }

            if (or_tags.Length > 0) {
                parsed.Add(new ImageTag(or_tags.TrimEnd(' '), TagFilter.Either, false));
            }

            return parsed;
        }

        public static IList<ImageTag> ParseTags(string tags) {
            return ParseTags(SplitTags(tags));
        }

        public static ImageTag ParseTag(string tag) {
            if (tag[0] == '~') {
                throw new ImageTagParseException("Cannot parse single or-filter");
            }

            string tt = tag;
            if (tag[0] == '-') {
                tt = tag.Substring(1);
            }

            TagFilter filter = TagFilter.Contain;

            if (tt.StartsWith("filetype:")) {
                filter = TagFilter.Filetype;
            } else if (tt.StartsWith("filesize:")) {
                if (tag[0] == '-') {
                    throw new ImageTagParseException("Cannot negate filesize");
                }

                filter = TagFilter.Filesize;
            } else if (tt.StartsWith("mpixels:")) {
                if (tag[0] == '-') {
                    throw new ImageTagParseException("Cannot negate mpixels");
                }

                filter = TagFilter.MPixels;
            } else if (tt.StartsWith("ratio:")) {
                if (tag[0] == '-') {
                    throw new ImageTagParseException("Cannot negate ratio");
                }

                filter = TagFilter.Ratio;
            } else if (tt.StartsWith("height:")) {
                if (tag[0] == '-') {
                    throw new ImageTagParseException("Cannot negate height");
                }

                filter = TagFilter.Height;
            } else if (tt.StartsWith("width:")) {
                if (tag[0] == '-') {
                    throw new ImageTagParseException("Cannot negate width");
                }

                filter = TagFilter.Width;
            } else if (tt.StartsWith("md5:")) {
                if (tag[0] == '-') {
                    throw new ImageTagParseException("Cannot negate MD5");
                }
                if (tt.Length != 36) {
                    throw new ImageTagParseException("MD5 filter malformed");
                }

                filter = TagFilter.MD5;
            } else if (tt.StartsWith("score:")) {
                if (tag[0] == '-') {
                    throw new ImageTagParseException("Cannot negate score");
                }

                filter = TagFilter.Score;
            } else if (tt.StartsWith("tagcount:")) {
                if (tag[0] == '-') {
                    throw new ImageTagParseException("Cannot negate tagcount");
                }

                filter = TagFilter.TagCount;
            } else if (tt.StartsWith("parent:")) {
                string parent = tt.Substring("parent:".Length);
                if (parent != "none"
                    && parent != "any" 
                    && !parent.All(Char.IsDigit)) {
                    throw new ImageTagParseException("Parent filter malformed");
                }

                filter = TagFilter.Parent;
            }  else if (tt.StartsWith("rating:")) {
                string rating = tt.Substring("rating:".Length);

                if (rating != "explicit"
                    && rating != "e"
                    && rating != "questionable"
                    && rating != "q"
                    && rating != "safe"
                    && rating != "s") {
                    throw new ImageTagParseException("Rating filter malformed");
                }

                filter = TagFilter.Rating;
            } else if (tt.StartsWith("date:")) {
                if (tag[0] == '-') {
                    throw new ImageTagParseException("Date cannot be negated");
                }

                filter = TagFilter.Date;
            } else if (tt.StartsWith("id:")) {
                filter = TagFilter.ID;
            }

            if (filter != TagFilter.Contain
                && tag.Contains("*")) {
                throw new ImageTagParseException("Cannot use filters with wildcards");
            }

            return new ImageTag(tag, filter, tag[0] == '-');
        }

        public static IList<string> SplitTags(string tag_string) {
            return tag_string.Split(new[] { ' ', '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);
        }


        public override string ToString() {
            return TagString;
        }
    }
}

﻿using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BooruDownloader.Base {
    public class TimestampConverter : DateTimeConverterBase {
        private static readonly DateTime _epoch = 
            new DateTime(1970, 1, 1, 
                0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, 
            JsonSerializer serializer) {

            writer.WriteRawValue(((DateTime) value - _epoch)
                .TotalSeconds.ToString(CultureInfo.InvariantCulture));
        }

        public override object ReadJson(JsonReader reader, Type objectType, 
            object existingValue, JsonSerializer serializer) {

            if (reader.Value == null) {
                return null;
            }

            // Danbooru
            if (reader.Value is DateTime) {
                return (DateTime) reader.Value;
            }

            // Gelbooru
            if (reader.Value is string val) {
                
                return DateTime.ParseExact(val.Substring(0, 20) + val.Substring(26), 
                    "ddd MMM dd HH':'mm:''ss yyyy",
                    CultureInfo.InvariantCulture);
            }

            return _epoch.AddSeconds((long) reader.Value);

        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Post {

        [Flags]
        public enum RatingFlags {
            None = 0,
            Safe = 1,
            Questionable = 2,
            Explicit = 4,
            SafeQuestionable = Safe | Questionable,
            QuestionableExplicit = Questionable | Explicit,
            Any = Safe | Questionable | Explicit
        }

        public bool IsRating(RatingFlags flags) {
            switch (flags) {
                case RatingFlags.Safe:
                    return Rating == "s";

                case RatingFlags.Questionable:
                    return Rating == "q";

                case RatingFlags.Explicit:
                    return Rating == "e";

                case RatingFlags.SafeQuestionable:
                    return Rating == "s" || Rating == "q";

                case RatingFlags.QuestionableExplicit:
                    return Rating == "q" || Rating == "e";

                case RatingFlags.Any:
                    return Rating == "s" || Rating == "q" || Rating == "e";
            }

            return false;
        }

        public static string RatingString(RatingFlags flags) {
            switch (flags) {
                case RatingFlags.Safe:
                    return "rating:s";

                case RatingFlags.Questionable:
                    return "rating:q";

                case RatingFlags.Explicit:
                    return "rating:e";

                case RatingFlags.SafeQuestionable:
                    return "-rating:e";

                case RatingFlags.QuestionableExplicit:
                    return "-rating:s";
            }

            return String.Empty;
        }

        public static ImageTag RatingTag(RatingFlags flags) {
            switch (flags) {
                case RatingFlags.Safe:
                case RatingFlags.Questionable:
                case RatingFlags.Explicit:
                    return new ImageTag(RatingString(flags), ImageTag.TagFilter.Rating, false);

                case RatingFlags.SafeQuestionable:
                case RatingFlags.QuestionableExplicit:
                    return new ImageTag(RatingString(flags), ImageTag.TagFilter.Rating, true);
            }

            return null;
        }

        public string Board { get; set; }

        [JsonProperty("id", Required = Required.Always)]
        public long ID { get; set; }

        [JsonProperty("created_at", Required = Required.Always), 
         JsonConverter(typeof(TimestampConverter))]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("score", Required = Required.Always)]
        public long Score { get; set; }

        [JsonProperty("md5", Required = Required.DisallowNull)]
        public string MD5 { get; set; }

        [JsonProperty("rating", Required = Required.Always)]
        public string Rating { get; set; }

        [JsonProperty("file_size", Required = Required.DisallowNull)]
        public long FileSize { get; set; }

        [JsonProperty("source", Required = Required.Always)]
        public string Source { get; set; }

        [JsonProperty("parent_id", Required = Required.AllowNull)]
        public long? ParentID { get; set; }

        [JsonProperty("file_url", Required = Required.Always)]
        public string FileUrl { get; set; }

        // Danbooru
        /// <summary>
        /// ====== Use Width ======
        /// </summary>
        [JsonProperty("image_width", Required = Required.DisallowNull)]
        public long ImageWidth {
            get { return Width; }
            set { Width = value; }
        }

        /// <summary>
        /// ====== Use Height ======
        /// </summary>
        [JsonProperty("image_height", Required = Required.DisallowNull)]
        public long ImageHeight {
            get { return Height; }
            set { Height = value; }
        }

        /// <summary>
        /// ====== Use Tags ======
        /// </summary>
        [JsonProperty("tag_string", Required = Required.DisallowNull)]
        public string TagString {
            get { return Tags; }
            set { Tags = value; }
        }

        [JsonProperty("file_ext", Required = Required.DisallowNull)]
        public string FileExt { get; set; }

        [JsonProperty("uploader_id", Required = Required.DisallowNull)]
        public long UploaderID { get; set; }

        // Konachan / Yande.re
        [JsonProperty("width", Required = Required.DisallowNull)]
        public long Width { get; set; }

        [JsonProperty("height", Required = Required.DisallowNull)]
        public long Height { get; set; }

        [JsonProperty("tags", Required = Required.DisallowNull)]
        public string Tags { get; set; }
        
        /// <summary>
        /// ====== Use UploaderID ======
        /// </summary>
        [JsonProperty("creator_id", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long CreatorID {
            get { return UploaderID; }
            set { UploaderID = value; }
        }

        // Gelbooru
        /// <summary>
        /// ====== Use MD5 ======
        /// </summary>
        [JsonProperty("hash", Required = Required.DisallowNull)]
        public string Hash {
            get { return MD5; }
            set { MD5 = value; }
        }
        
    }
}

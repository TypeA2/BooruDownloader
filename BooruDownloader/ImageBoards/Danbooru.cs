using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using BooruDownloader.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BooruDownloader.Properties;

namespace BooruDownloader {

    [ValueConversion(typeof(ulong), typeof(string))]
    public class UserLevelConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return "API Level: " + ((Danbooru.UserEntry.UserLevel) (ulong) (value ?? 20)).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return ulong.Parse(((string) (value ?? "API Level: 0")).Substring(11));
        }
    }

    [ImageBoard]
    public class Danbooru : ApiImageBoard {
        public class UserEntry {
            [Serializable]
            public enum UserLevel {
                Unregistered = 0,
                Member = 20,
                Gold = 30,
                Platinum = 31,
                Builder = 32,
                Janitor = 35,
                Moderator = 40,
                Admin = 50
            }

            [JsonProperty("id")]
            public ulong ID { get; set; }

            [JsonProperty("created_at")]
            public DateTime CreatedAt { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("inviter_id", Required = Required.AllowNull)]
            public ulong InviterID { get; set; }

            [JsonProperty("level")]
            public UserLevel Level { get; set; }

            [JsonProperty("base_upload_limit")]
            public ulong BaseUploadLimit { get; set; }

            [JsonProperty("post_upload_count")]
            public ulong PostUploadCount { get; set; }

            [JsonProperty("post_update_coun")]
            public ulong PostUpdateCount { get; set; }

            [JsonProperty("note_update_count")]
            public ulong NoteUpdateCount { get; set; }

            [JsonProperty("is_banned")]
            public bool IsBanned { get; set; }

            [JsonProperty("can_approve_posts")]
            public bool CanApprovePosts { get; set; }

            [JsonProperty("can_upload_free")]
            public bool CanUploadFree { get; set; }

            [JsonProperty("is_super_voter")]
            public bool IsSuperVoter { get; set; }

            [JsonProperty("level_string")]
            public string LevelString { get; set; }
        }

        public class CountsEntry {
            [JsonProperty("counts")]
            public PostCountEntry Counts { get; set; }
        }

        public class PostCountEntry {
            [JsonProperty("posts")]
            public long Posts { get; set; }
        }

        public Danbooru() {
            this.Credentials = new ApiCredentials {
                Username = string.Empty,
                ApiKey = string.Empty
            };
        }

        public Danbooru(ApiCredentials credentials) : this() {
            this.Credentials = credentials;
        }

        public override string Name { get => "Danbooru"; }

        public override int MaxTagGroup {
            get {
                if (!Settings.Default.UseAPI) {
                    return 2;
                }

                switch ((UserEntry.UserLevel) Settings.Default.DanbooruAccountLevel) {
                    case UserEntry.UserLevel.Unregistered:
                    case UserEntry.UserLevel.Member:
                        return 2;
                    case UserEntry.UserLevel.Gold:
                        return 6;
                    case UserEntry.UserLevel.Platinum:
                    case UserEntry.UserLevel.Builder:
                    case UserEntry.UserLevel.Janitor:
                    case UserEntry.UserLevel.Moderator:
                    case UserEntry.UserLevel.Admin:
                        return 12;

                    default:
                        return 2;
                }
            }
        }
        public override int MaxEntriesPerPage { get => 200; }
        public override string ApiBaseEndpoint { get => "https://danbooru.donmai.us"; }

        public override Dictionary<string, string> Endpoints {
            get {
                return new Dictionary<string, string> {
                    ["DMail"] = "/dmails.json",
                    ["Users"] ="/users.json",
                    ["Counts"] = "/counts/posts.json",
                    ["Posts"] = "/posts.json"
                };
            }
        }

        public static void ClearAllCredentials() {
            Settings.Default.DanbooruUsername = string.Empty;
            Settings.Default.DanbooruKey = string.Empty;
            Settings.Default.DanbooruID = 0;
            Settings.Default.DanbooruAccountLevel = 0;
            Settings.Default.DanbooruCredentialsHash = string.Empty;
            Settings.Default.Save();
        }

        public override async Task<bool> ValidateAndSaveCredentials() {
            if (!ValidateSavedCredentials()) {
                return await RetrieveAccountDataFromAPI();
            }

            return true;
        }

        private static string GetCurrentCredentialsHash() {
            string hash_data = Settings.Default.DanbooruUsername
                               + Settings.Default.DanbooruKey
                               + Settings.Default.DanbooruID
                               + Settings.Default.DanbooruAccountLevel;

            SHA512 hash = SHA512.Create();
            hash.ComputeHash(Encoding.UTF8.GetBytes(hash_data));

            return hash.Hash.ToHexString();
        }

        protected override bool ValidateSavedCredentials() {
            if (Settings.Default.DanbooruUsername != Credentials.Username
                || Settings.Default.DanbooruKey != Credentials.ApiKey) {
                return false;
            }

            return GetCurrentCredentialsHash() == Settings.Default.DanbooruCredentialsHash;
        }

        private async Task<bool> RetrieveAccountDataFromAPI() {
            if (!await this.ValidateCredentialsByDmail()) {
                return false;
            }

            UserEntry user = await GetUserByName(Credentials.Username);

            if (user == null) {
                return false;
            }
            
            Settings.Default.DanbooruUsername = Credentials.Username;
            Settings.Default.DanbooruKey = Credentials.ApiKey;
            Settings.Default.DanbooruID = user.ID;
            Settings.Default.DanbooruAccountLevel = (ulong) user.Level;
            Settings.Default.Save();

            Settings.Default.DanbooruCredentialsHash = GetCurrentCredentialsHash();
            Settings.Default.Save();

            return true;
        } 

        private async Task<bool> ValidateCredentialsByDmail() {
            HttpStatusCode code;

            try {
                code = ((HttpWebResponse) await WebHelper.CreateRequest(
                    this.ApiBaseEndpoint, this.Endpoints["DMail"],
                    new Dictionary<string, string> {
                        { "login", Credentials.Username },
                        { "api_key", Credentials.ApiKey }
                    }).GetResponseAsync()).StatusCode;
            } catch (WebException) {
                return false;
            }

            return code == HttpStatusCode.OK;
        }

        private async Task<UserEntry> GetUserByName(string name, int index = 0) {
            List<UserEntry> users = await WebHelper.GetJsonAsync<List<UserEntry>>(this.ApiBaseEndpoint,
                this.Endpoints["Users"], new Dictionary<string, string> {
                    { "search[name_matches]", name }
                });

            return users.Count >= 1 ? users[0] : null;
        }



        public override async Task<IList<Post>> GetPage(IList<ImageTag> tags, long page) {
            if (tags.TagCost() > MaxTagGroup) {
                throw new TooManyTagsException(
                    $"Cannot search for more than {MaxTagGroup} at a time (current: {tags.TagCost()}.");
            }

            string json_string = await PostRequestAsync(Endpoints["Posts"],
                new Dictionary<string, string> {
                    ["tags"] = tags.AsTagString(),
                    ["page"] = page.ToString(),
                    ["limit"] = MaxEntriesPerPage.ToString()
                });

            JArray json = JArray.Parse(json_string);

            List<Post> posts = new List<Post>(json.Count);

            foreach (JToken token in json.Children()) {
                if (token["md5"] == null || token["file_url"] == null) {
                    continue;
                }

                Post post = token.ToObject<Post>();

                post.Board = Name;

                posts.Add(post);
            }

            return posts;
        }

        public override async Task<long> PostCount(IList<ImageTag> tags) {
            if (tags.TagCost() > MaxTagGroup) {
                throw new TooManyTagsException(
                    $"Cannot search for more than {MaxTagGroup} at a time (current: {tags.TagCost()}.");
            }

            return (await PostRequestAsync<CountsEntry>(
                    Endpoints["Counts"],
                    new Dictionary<string, string> { ["tags"] = tags.AsTagString() }))
                .Counts.Posts;
        }
    }
}

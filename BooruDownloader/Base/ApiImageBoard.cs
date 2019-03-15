using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooruDownloader.Properties;

namespace BooruDownloader {

    public class ApiCredentialsException : Exception {
        public ApiCredentialsException(string message) : base(message) { }
        public ApiCredentialsException(string message, Exception inner) : base(message, inner) { }
    }

    public abstract class ApiImageBoard : TaggedImageBoard {
        public struct ApiCredentials {
            public string Username { get; set; }
            public string ApiKey { get; set; }
        }

        public ApiCredentials Credentials { get; set; }

        public abstract Task<bool> ValidateAndSaveCredentials();

        protected abstract bool ValidateSavedCredentials();

        public override async Task<string> PostRequestAsync(
            string base_endpoint, string endpoint, Dictionary<string, string> data) {

            if (Settings.Default.UseAPI && ValidateSavedCredentials()) {

                if (!ValidateSavedCredentials()) {
                    throw new ApiCredentialsException("Saved API credentials are not consistent");
                }

                if (data == null) {
                    data = new Dictionary<string, string>(2);
                }

                data.Add("login", Credentials.Username);
                data.Add("api_key", Credentials.ApiKey);
            }

            return await base.PostRequestAsync(base_endpoint, endpoint, data);
        }
    }
}

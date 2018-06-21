using System;

namespace booru_downloader {
    public class DisplaySource {
        public string Key { get; }
        public BooruAPI.Source Value { get; }

        public override string ToString() {
            return this.Key;
        }

        public DisplaySource (string key, BooruAPI.Source value) {
            this.Key = key;
            this.Value = value;
        }
    }
}

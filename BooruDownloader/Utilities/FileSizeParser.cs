using System;
using System.Collections.Generic;

namespace BooruDownloader.Utilities {

    public class FileSizeParseException : Exception {
        public FileSizeParseException(string message) : base(message) { }
        public FileSizeParseException(string message, Exception inner) : base(message, inner) { }
    }

    public static class FileSizeParser {
        private static readonly IList<string>_exts = new List<string> {
            "b", "kb", "mb", "gb", "tb", "pb", "eb", "zb", "yb"
        };

        public static double ParseSize(string value) {
            value = value.Trim();

            int ext_index = 0;

            for (int i = value.Length - 1; i >= 0; i--) {
                if (!Char.IsLetter(value, i)) {
                    ext_index = i + 1;
                    break;
                }
            }

            int suffix_index = (ext_index >= value.Length)
                ? 0
                : _exts.IndexOf(value.Substring(ext_index).Trim().ToLower());

            if (suffix_index < 0) {
                throw new FileSizeParseException($"Invalid suffix {value.Substring(ext_index).Trim()}");
            }

            return Double.Parse(value.Substring(0, ext_index)) * Math.Pow(1024, suffix_index);
        }
    }
}

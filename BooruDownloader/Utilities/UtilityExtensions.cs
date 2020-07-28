using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using BooruDownloader.Base;

namespace BooruDownloader {
    public static class UtilityExtensions {
        private static readonly char[] _hex_lookup = {
            '0', '1', '2', '3',
            '4', '5', '6', '7',
            '8', '9', 'A', 'B',
            'C', 'D', 'E', 'F'
        };

        public static string ToHexString(this byte[] data) {
            char[] result = new char[data.Length * 2];

            for (int i = 0; i < data.Length; i++) {
                result[i * 2] = _hex_lookup[(data[i] & 0xF0) >> 4];
                result[(i * 2) + 1] = _hex_lookup[data[i] & 0x0F];
            }

            return new string(result);
        }

        public static bool Like(this string source, string find) {
            return new Regex(
                @"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\")
                    .Replace(find, ch => @"\" + ch)
                    .Replace('_', '.')
                    .Replace("%", ".*") + @"\z",
                RegexOptions.Singleline).IsMatch(source);
        }

        public static long TagCost(this IList<ImageTag> tags) {
            return tags.Sum(item => item.Weight);
        }

        public static string FileExtension(this string path) {
            string end = System.IO.Path.GetExtension(path) ?? path;
            int index = end.IndexOf('?') - 1;

            return (index < 0) ? end.Substring(1) : end.Substring(1, index);
        }

        public static string CleanPath(this string path, char fill = '_') =>
            new Regex(@"(.)\\1+")
                .Replace(Path.GetInvalidPathChars()
                    .Concat((new[] { '\\', '/', '*', '?', '|', '<', '>', ':' }))
                    .Aggregate(path, (current, c) => current.Replace(c, fill)), "$1");

        public static string BytesString(this long size) {
            if (size > 0x1000000000000000) {
                return $"{(size >> 50) / 1024.0:0.#} EiB";
            }

            if (size > 0x4000000000000) {
                return $"{((size >> 40) / 1024.0):0.#} PiB";
            }

            if (size > 0x10000000000) {
                return $"{(size >> 30) / 1024.0:0.#} TiB";
            }

            if (size > 0x40000000) {
                return $"{(size >> 20) / 1024.0:0.#} GiB";
            }

            if (size > 0x100000) {
                return $"{(size >> 10) / 1024.0:0.#} MiB";
            }

            return (size > 0x400) ? $"{size / 1024.0:0.#} KiB" : $"{size} bytes";
        }

        public static IList<IList<T>> Combinations<T>(this IList<T> elements, int n, bool cap = false) {
            int count = (int) Math.Pow(2, elements.Count) - 1;

            List<IList<T>> result = new List<IList<T>>(count);

            for (int i = 1; i < count + 1; i++) {
                result.Add(new List<T>());

                for (int j = 0; j < elements.Count; j++) {
                    if ((i >> j) % 2 != 0) {
                        result.Last().Add(elements[j]);
                    }
                }
            }

            return cap ? result.Where(e => e.Count <= n).ToList() : result.Where(e => e.Count == n).ToList();
        }

        public static IEnumerable<string> RemoveAfterFirst(this IEnumerable<string> strings, string pattern, StringComparison comparison = StringComparison.Ordinal) {
            List<string> copy = new List<string>(strings);
            for (int i = 0; i < copy.Count; ++i) {
                if (copy[i].Contains(pattern)) {
                    copy[i] = copy[i].Substring(0, copy[i].IndexOf(pattern, comparison));
                }
            }

            return copy;
        }

        public static string ToSentence(this IEnumerable<string> strings) {
            // https://apidock.com/rails/Array/to_sentence
            List<string> copy = strings.ToList();

            switch (copy.Count) {
                case 1: return copy.First();
                case 2: return string.Join(" and ", copy);
                default:
                    string last = copy.Last();
                    copy.RemoveAt(copy.Count - 1);

                    return $"{string.Join(", ", copy)}, and {last}";
            }
        }
    }
}


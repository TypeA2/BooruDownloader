using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Cache;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Windows;
using BooruDownloader.Base;

namespace BooruDownloader {

    public class InvalidPathException : Exception {
        public InvalidPathException(string message) : base(message) { }
        public InvalidPathException(string message, Exception inner) : base(message, inner) { }
    }

    public class InvalidInputException : Exception {
        public InvalidInputException(string message) : base(message) { }
        public InvalidInputException(string message, Exception inner) : base(message, inner) { }
    }

    public partial class MainWindow {
        private bool _is_downloading;

        public bool CancelQueued { get; private set; }

        public async void StartDownloadSequence() {
            _is_downloading = true;

            LockView();

            List<ImageTag> tags = ImageTag.ParseTags(Tags.Text) as List<ImageTag>;

            TaggedImageBoard current_board = AvailableImageBoards[this.Source.SelectedIndex];

            ImageTag rating_tag = Post.RatingTag(CurrentRating());

            if (!(await current_board.GetBestTags(tags, rating_tag) is List<ImageTag> best_tags)) {
                DownloadFailed("Failed to get optimal tags");
                return;
            }

            if (CancelQueued) {
                ResetView("Download canceled");
                return;
            }

            List<Post> posts;
            try {
                posts = await current_board.GetPages(best_tags, tags,
                    CurrentRating(), -1, this) as List<Post>;

                if (posts == null) {

                    ResetView("Download canceled");
                    return;
                }
            } catch (WebException e) {
                DownloadFailed(e.Message);
                return;
            }

            if (posts.Count == 0) {
                DownloadFailed($"No posts were found for {best_tags.AsTagString(' ')}");
                ResetView();
                return;
            }

            if (CancelQueued) {
                ResetView("Download canceled");
                return;
            }

            string output_base;
            try {
                output_base = GetOutputPath(tags);
            } catch (InvalidPathException e) {
                DownloadFailed(e.Message);
                return;
            }

            await DownloadPosts(posts, output_base);

            ResetView("Downloading finished");
        }

        private async Task DownloadPosts(IList<Post> posts, string output_base) {
            long sequence_start = Int64.Parse(StartSequence.Text);
            bool skip_existing = OverwriteExisting.IsChecked == false;

            _NewMaximum(posts.Count);

            long file_marker = 0;
            long total_received = 0;
            long total_size = posts.Sum(p => p.FileSize);

            void DownloadProgressHandler(object sender, DownloadProgressChangedEventArgs args) {
                total_received = file_marker + args.BytesReceived;
                UpdateStats(total_received, total_size);
            }

            WebHelper.Client.DownloadProgressChanged -= WebClientDownloadProgress;
            WebHelper.Client.DownloadProgressChanged += DownloadProgressHandler;

            for (int i = 0; i < posts.Count; i++) {
                if (CancelQueued) {
                    ResetView("Download canceled");
                    break;
                }

                file_marker = total_received;

                Post post = posts[i];

                string output_file = String.Empty;

                string file_ext = String.IsNullOrWhiteSpace(post.FileExt)
                    ? Path.GetExtension(post.FileUrl)?.Substring(1)
                    : post.FileExt;

                if (RadioID.IsChecked == true) {
                    output_file = $"{post.ID}.{file_ext}";
                } else if (RadioMD5.IsChecked == true) {
                    output_file = $"{post.MD5}.{file_ext}";
                } else if (RadioSequence.IsChecked == true) {
                    output_file = $"{sequence_start + i}.{file_ext}";
                }

                if (output_file.Length == 0) {
                    WebHelper.Client.DownloadProgressChanged -= DownloadProgressHandler;
                    WebHelper.Client.DownloadProgressChanged += WebClientDownloadProgress;
                    
                    throw new InvalidInputException($"Output path could not be formatted for {post.ID} from {post.Board}");
                }

                string output_path = $"{output_base}{Path.DirectorySeparatorChar}{output_file}";

                if (skip_existing
                    && File.Exists(output_path)) {
                    continue;
                }

                _SetStatus($"Downloading {output_file} ({post.ID})");
                _Progress(i + 1);

                try {
                    await WebHelper.Client.DownloadFileTaskAsync(post.FileUrl, output_path);
                } catch (WebException e) {
                    MessageBox.Show($"Download for {post.ID} failed with error: {e.Message}. Attempting to continue.");
                }

                UpdateCounter();
            }

            WebHelper.Client.DownloadProgressChanged -= DownloadProgressHandler;
            WebHelper.Client.DownloadProgressChanged += WebClientDownloadProgress;
        }


        private string GetOutputPath(IEnumerable<ImageTag> tags = null) {
            string path = OutputPath.Text;

            if (!File.GetAttributes(path).HasFlag(FileAttributes.Directory)) {

                if (!Directory.Exists(path)) {
                    try {
                        Directory.CreateDirectory(path);
                    } catch (UnauthorizedAccessException e) {
                        throw new InvalidPathException($"Failed creating output path with error {e.Message}");
                    }
                } else {
                    throw new InvalidPathException("Path does not point to a directory");
                }
            }

            try {
                DirectorySecurity ds = Directory.GetAccessControl(path);
            } catch (UnauthorizedAccessException) {
                throw new InvalidPathException("No write access in target directory");
            }

            if (CreateSubfolder.IsChecked == true) {
                path = path + Path.DirectorySeparatorChar + tags.AsTagString('_').CleanPath();

                try {
                    Directory.CreateDirectory(path);
                } catch (IOException e) {
                    throw new InvalidPathException($"Failed creating ouput subfolder with error {e.Message}");
                }
            }

            return path;
        }


        public void StopDownloadSequence() {
            if (_is_downloading) {
                Cancel.IsEnabled = false;
                CancelQueued = true;
                return;
            }

            ResetView();
        }


        private void DownloadFailed(string err) {
            MessageBox.Show(err, "Download failed",
                MessageBoxButton.OK);

            ResetView();
        }

        private void ResetView(string message = null) {
            _is_downloading = false;
            CancelQueued = false;

            UnlockView();
            ResetProgress();

            UpdateStats(null);

            if (message != null) {
                _SetStatus(message);
            }
        }

        private void LockView() {
            _ChangeViewState(false);
        }

        private void UnlockView() {
            _ChangeViewState(true);
        }

        private void _ChangeViewState(bool enable) {
            this.Tags.IsEnabled = enable;
            this.Rating.IsEnabled = enable;
            this.FormatTags.IsEnabled = enable;
            this.Format.IsEnabled = enable;
            this.Source.IsEnabled = enable;
            this.ApiKeys.IsEnabled = enable;
            this.UseAPI.IsEnabled = enable;
            this.OutputPath.IsEnabled = enable;
            this.Browse.IsEnabled = enable;
            this.CreateSubfolder.IsEnabled = enable;
            this.Cancel.IsEnabled = !enable;
            this.Start.IsEnabled = enable;
        }
    }
}
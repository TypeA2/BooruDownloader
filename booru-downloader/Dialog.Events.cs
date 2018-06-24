using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System;
using System.Net;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace booru_downloader {

    partial class Dialog {

        private bool cancel = false;

        private void InitializeEvents() {
            this.radioSequence.CheckedChanged += this.RadioSequence_CheckedChanged;
            this.buttonBrowseOutput.Click += this.ButtonBrowseOutput_Click;
            this.textOutputFolder.TextChanged += this.TextOutputFolder_TextChanged;
            this.buttonStart.Click += this.ButtonStart_Click;
            this.buttonCancel.Click += this.ButtonCancel_Click;
        }

        private void RadioSequence_CheckedChanged(object sender, EventArgs e) {
            // Enable and disable the numeric sequence selector depending on whether the output filename method is "Number sequence" or not

            RadioButton radio = (RadioButton) sender;

            this.labelStartAt.Enabled = radio.Checked;
            this.numericSequenceStart.Enabled = radio.Checked;
        }

        private void ButtonBrowseOutput_Click(object sender, EventArgs e) {

            using (CommonOpenFileDialog dlg = new CommonOpenFileDialog()) {
                dlg.IsFolderPicker = true;

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok) {
                    this.textOutputFolder.Text = dlg.FileName;
                }
            }
        }

        private void TextOutputFolder_TextChanged(object sender, EventArgs e) {
            // Enable start button depending on whether the selected file path is valid

            TextBox box = (TextBox) sender;

            if (Directory.Exists(box.Text)) {
                this.labelError.Text = "";

                this.buttonStart.Enabled = true;
            } else {
                this.labelError.Text = "Selected folder does not exist!";

                this.buttonStart.Enabled = false;
            }
        }

        private async void ButtonStart_Click(object sender, EventArgs e) {

            this.labelError.Text = "";

            // Format the tags, removing duplicates and extra whitespace
            this.textTags.Text = this.textTags.Text.Replace(System.Environment.NewLine, " ");
            this.textTags.Text = Regex.Replace(this.textTags.Text, @"\s+", " ");

            string[] tags = this.textTags.Text.Split(' ').Distinct().ToArray();
            
            this.textTags.Text = string.Join(" ", tags);

            if (tags.Length > 0 && !string.IsNullOrEmpty(this.textTags.Text)) {
                
                ((Button) sender).Enabled = false;

                this.groupRating.Enabled = false;
                this.groupSaveFormat.Enabled = false;
                this.comboSource.Enabled = false;
                this.checkCreateSubfolder.Enabled = false;
                this.textOutputFolder.Enabled = false;
                this.buttonBrowseOutput.Enabled = false;
                this.textTags.Enabled = false;
                this.labelSearchTags.Enabled = false;
                this.labelSource.Enabled = false;
                this.labelOutputFolder.Enabled = false;

                this.buttonCancel.Enabled = true;

                this.progressBar.Minimum = 0;
                this.progressBar.Maximum = tags.Length;

                this.labelProgress.Text = $"0 / {tags.Length}";

                BooruAPI.Source source = ((DisplaySource) this.comboSource.SelectedItem).Value;
                int pageLimit = 0;

                if (source == BooruAPI.Source.Danbooru) {

                    pageLimit = BooruAPI.API.Danbooru.PageLimit;

                } else if (source == BooruAPI.Source.Konachan) {

                    pageLimit = BooruAPI.API.Konachan.PageLimit;

                } else if (source == BooruAPI.Source.Yandere) {

                    pageLimit = BooruAPI.API.Yandere.PageLimit;

                }

                BooruAPI.Rating rating = BooruAPI.Rating.Any;

                if (this.radioSafe.Checked) {
                    rating = BooruAPI.Rating.Safe;
                } else if (this.radioQuestionable.Checked) {
                    rating = BooruAPI.Rating.Questionable;
                } else if (this.radioExplicit.Checked) {
                    rating = BooruAPI.Rating.Explicit;
                } else if (this.radioSafeQuestionable.Checked) {
                    rating = BooruAPI.Rating.SafeQuestionable;
                } else if (this.radioQuestionableExplicit.Checked) {
                    rating = BooruAPI.Rating.QuestionableExplicit;
                }

                Dictionary<string, ulong> countsDict = new Dictionary<string, ulong>();
                
                for (int i = 0; i < tags.Length; i++) {

                    if (this.cancel) {
                        this.cancel = false;

                        this.ResetUI();

                        return;
                    }

                    // Workaround for Vista's delayed progress bar
                    this.progressBar.Value = i + 1;
                    this.progressBar.Value = i;
                    this.progressBar.Value = i + 1;


                    this.labelProgress.Text = $"{i + 1} / {tags.Length}";

                    this.labelStatus.Text = $"Retrieving post count for {tags[i]}";

                    countsDict.Add(tags[i], await BooruAPI.GetPostCount(tags[i], source));
                }

                this.labelStatus.Text = $"Got post count for {tags.Length} tag{ ((tags.Length == 1) ? '\0' : 's')}";

                List<KeyValuePair<string, ulong>> counts = countsDict.ToList();

                counts.Sort((e1, e2) => e1.Value.CompareTo(e2.Value));

                KeyValuePair<string, ulong>[] baseTags = counts.Take(2).ToArray();
                string baseTagsString = $"{baseTags[0].Key}{((baseTags.Length > 1) ? " " + baseTags[1].Key : "")}";

                this.labelStatus.Text = $"Getting post count for base tags \"{baseTagsString}\"";

                this.progressBar.Value = this.progressBar.Maximum;

                ulong postCount = await BooruAPI.GetPostCount(baseTagsString.Replace(' ', '+'), source);
                ulong pageCount = (ulong) Math.Ceiling(postCount / (double) pageLimit);

                this.labelStatus.Text = $"Gathering {pageCount} pages";

                List<BooruAPI.Post> posts = null;

                try {
                    posts = new List<BooruAPI.Post>(pageLimit * ((int) pageCount - 1));

                } catch (ArgumentOutOfRangeException) {

                    MessageBox.Show($"No posts found for the following tags:\"{baseTagsString}\"");

                    return;
                }

                this.progressBar.Maximum = (int) pageCount;
                this.progressBar.Value = 0;
                this.labelProgress.Text = $"0 / {pageCount}";

                for (ulong i = 0; i < pageCount; i++) {

                    if (this.cancel) {
                        this.cancel = false;

                        this.ResetUI();

                        return;
                    }

                    try {
                        posts.AddRange(await BooruAPI.GetPage(baseTagsString.Replace(' ', '+'), i, source, rating, tags));
                    } catch (InvalidOperationException) {
                        // Sometimes the returned list is empty, we don't care
                    }
                    
                    this.progressBar.Value = (int) i + 1;

                    this.labelProgress.Text = $"{i + 1} / {pageCount}";
                }

                string basePath = this.textOutputFolder.Text;

                if (this.checkCreateSubfolder.Checked) {

                    basePath = Path.Combine(basePath, string.Join("_", string.Join("_", tags).Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)));

                    Directory.CreateDirectory(basePath);
                }

                this.labelStatus.Text = $"Downloading {posts.Count} posts";

                this.progressBar.Maximum = posts.Count;
                this.progressBar.Value = 0;

                this.labelProgress.Text = $"0 / {posts.Count}";

                for (int i = 0; i < posts.Count; i++) {

                    if (this.cancel) {
                        this.cancel = false;

                        this.ResetUI();

                        return;
                    }

                    BooruAPI.Post post = posts.ElementAt(i);

                    string fname = post.Id.ToString() + "." + post.FileExt;

                    if (this.radioMD5.Checked) {
                        fname = post.Md5 + "." + post.FileExt;
                    } else if (this.radioSequence.Checked) {
                        fname = ((int) this.numericSequenceStart.Value + i).ToString() + "." + post.FileExt;
                    }

                    string targetFile = Path.Combine(basePath, string.Join("_", fname.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)));

                    using (WebClient client = new WebClient()) {
                        await client.DownloadFileTaskAsync(post.FileUrl, targetFile);
                    }

                    this.progressBar.Value = i + 1;

                    this.labelProgress.Text = $"{i + 1} / {posts.Count}";
                }

                MessageBox.Show("Downloading finished");

                this.ResetUI();
                this.cancel = false;

            } else {
                this.labelError.Text = "No tags found!";
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e) {
            this.cancel = true;
        }
    }
}
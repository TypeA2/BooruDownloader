using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using Microsoft.WindowsAPICodePack.Dialogs;

using System.Diagnostics;

namespace booru_downloader {

    partial class Dialog {

        private void InitializeEvents() {
            this.radioSequence.CheckedChanged += this.RadioSequence_CheckedChanged;
            this.buttonBrowseOutput.Click += this.ButtonBrowseOutput_Click;
            this.textOutputFolder.TextChanged += this.TextOutputFolder_TextChanged;
            this.buttonStart.Click += this.ButtonStart_Click;
        }

        private void RadioSequence_CheckedChanged(object sender, System.EventArgs e) {
            // Enable and disable the numeric sequence selector depending on whether the output filename method is "Number sequence" or not

            RadioButton radio = (RadioButton) sender;

            this.labelStartAt.Enabled = radio.Checked;
            this.numericSequenceStart.Enabled = radio.Checked;
        }

        private void ButtonBrowseOutput_Click(object sender, System.EventArgs e) {

            using (CommonOpenFileDialog dlg = new CommonOpenFileDialog()) {
                dlg.IsFolderPicker = true;

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok) {
                    this.textOutputFolder.Text = dlg.FileName;
                }
            }
        }

        private void TextOutputFolder_TextChanged(object sender, System.EventArgs e) {
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

        private async void ButtonStart_Click(object sender, System.EventArgs e) {

            this.labelError.Text = "";

            // Format the tags, removing duplicates and extra whitespace
            this.textTags.Text = this.textTags.Text.Replace(System.Environment.NewLine, " ");
            this.textTags.Text = Regex.Replace(this.textTags.Text, @"\s+", " ");

            string[] tags = this.textTags.Text.Split(' ').Distinct().ToArray();

            this.textTags.Text = string.Join(" ", tags);

            if (tags.Length > 0 && !string.IsNullOrEmpty(this.textTags.Text)) {

                ((Button) sender).Enabled = false;

                BooruAPI.Source source = ((DisplaySource) this.comboSource.SelectedItem).Value;

                Dictionary<string, ulong> countsDict = new Dictionary<string, ulong>();
                
                for (int i = 0; i < tags.Length; i++) {

                    this.labelStatus.Text = $"Retrieving post count for {tags[i]} ({i + 1} / {tags.Length})";

                    countsDict.Add(tags[i], await BooruAPI.GetPostCount(tags[i], source));
                }

                this.labelStatus.Text = $"Got post count for {tags.Length} tag{ ((tags.Length == 1) ? '\0' : 's')}";

                List<KeyValuePair<string, ulong>> counts = countsDict.ToList();

                counts.Sort((e1, e2) => e1.Value.CompareTo(e2.Value));

                KeyValuePair<string, ulong>[] baseTags = counts.Take(2).ToArray();

                this.labelStatus.Text = $"Getting post count for base tags \"{baseTags[0].Key} {baseTags[1].Key}\""

            } else {
                this.labelError.Text = "No tags found!";
            }

            
        }
    }
}
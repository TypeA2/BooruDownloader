using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using BooruDownloader.Base;

namespace BooruDownloader {

    [ValueConversion(typeof(int), typeof(string))]
    public class ApiTagLimitConverter : IValueConverter {

        private static readonly string _prefix_string = "Tag limit: ";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) {
                return _prefix_string + '0';
            }

            return _prefix_string + MainWindow.AvailableImageBoards[(int) value].MaxTagGroup;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return int.Parse(((string) (value ?? _prefix_string + '0')).Substring(_prefix_string.Length));
        }
    }

    public partial class MainWindow {

        private long _vailidty_counter;

        public void AddValidState() {
            this._vailidty_counter += 1;

            CheckValidState();
        }

        public void RemoveValidState() {
            this._vailidty_counter -= 1;

            CheckValidState();
        }

        public void CheckValidState() {
            this.Start.IsEnabled = this._vailidty_counter >= 0;
        }

        public void RefreshSourceView() {
            ApiLimit.GetBindingExpression(ContentProperty)?.UpdateTarget();
        }

        private void DoFormatTags() {
            this.Tags.Text = String.Join("\n", 
                Regex.Replace(this.Tags.Text.Replace(' ', '\n'), 
                        @"^\s+$[\r\n]*", string.Empty,
                        RegexOptions.Multiline)
                    .Split('\n')
                    .Where(s => !String.IsNullOrWhiteSpace(s))
                    .ToList());

            this.Tags.CaretIndex = this.Tags.Text.Length;
        }

        private void DoUpdateTagsState() {
            int current_limit = AvailableImageBoards[this.Source.SelectedIndex].MaxTagGroup;

            IList<ImageTag> tags = ImageTag.ParseTags(this.Tags.Text);

            bool weight_error = (tags.TagCost() > current_limit);

            if (tags.Any(tag => tag.Filter == ImageTag.TagFilter.Rating)) {
                throw new ImageTagParseException("Rating tag not needed");
            }
            
            this.TagsBorder.BorderBrush = weight_error
                ? Brushes.Orange : this.textbox_border;

            this.CurrentTagCount.Content = $"Current:  {tags.TagCost()}";
            
            this.CurrentTagCount.Foreground = weight_error
                ? Brushes.Orange : Brushes.Black;
        }

        public Post.RatingFlags CurrentRating() {
            if (RadioSafe.IsChecked == true) {
                return Post.RatingFlags.Safe;
            }

            if (RadioQuestionable.IsChecked == true) {
                return Post.RatingFlags.Questionable;
            }

            if (RadioExplicit.IsChecked == true) {
                return Post.RatingFlags.Explicit;
            }

            if (RadioSafeQuestionable.IsChecked == true) {
                return Post.RatingFlags.SafeQuestionable;
            }

            if (RadioQuestionableExplicit.IsChecked == true) {
                return Post.RatingFlags.QuestionableExplicit;
            }

            return (RadioAny.IsChecked == true) ?
                Post.RatingFlags.Any : Post.RatingFlags.None;
        }



        private void UpdateCounter() {
            ProgessCounter.Text = $"{ProgressBar.Value} / {ProgressBar.Maximum}";
        }

        private void _NewMaximum(long max) {
            ProgressBar.Maximum = max;
            ProgressBar.Value = 0;
        }

        private void _Progress(long val) {
            ProgressBar.Value = val;
        }

        private void _SetStatus(string text) {
            StatusText.Text = text;
        }

        private void ResetProgress() {
            _NewMaximum(100);

            ProgessCounter.Text = "0 / 0";

            _SetStatus("");
        }



        private void UpdateStats(long current, long max) {
            StatsText.Text = $"{current.BytesString()} / {max.BytesString()}";
        }

        private void UpdateStats(string message) {
            StatsText.Text = message;
        }
    }
}
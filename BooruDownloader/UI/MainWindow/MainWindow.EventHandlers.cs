using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BooruDownloader.Base;
using Microsoft.WindowsAPICodePack.Dialogs;
using BooruDownloader.Properties;

namespace BooruDownloader {
    public partial class MainWindow {

        private void ShowAPIKeysMenu(object sender, RoutedEventArgs e) {
            ApiKeys child = new ApiKeys {
                ShowInTaskbar = false,
                Owner = this
            };

            child.ShowDialog();

            this.RefreshSourceView();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e) {
            WriteAllSettings();
        }

        private void NumericTextFieldKeypress(object sender, KeyEventArgs e) {
            if (e.Key < Key.D0 || e.Key > Key.D9) {
                e.Handled = true;
            }
        }

        private void Source_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
            int index = this.Source.SelectedIndex;
            
            if (AvailableImageBoards[index].GetType().IsSubclassOf(typeof(ApiImageBoard))) {
                this.UseAPI.Content = "Use API key";
                
                this.UseAPI.IsEnabled = true;
                this.ApiLevel.IsEnabled = true;
            } else {
                this.UseAPI.IsEnabled = false;
                this.UseAPI.Content = "API not used";

                this.ApiLevel.IsEnabled = false;
            }
        }

        private void OutputPath_OnTextChanged(object sender, TextChangedEventArgs e) {
            if (Directory.Exists(this.OutputPath.Text)) {
                this.OutputPathBorder.BorderBrush = this.textbox_border;
                this.AddValidState();
            } else {
                this.OutputPathBorder.BorderBrush = Brushes.Red;
                this.RemoveValidState();
            }
        }

        private void Browse_OnClick(object sender, RoutedEventArgs e) {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog()) {
                dialog.IsFolderPicker = true;
                dialog.DefaultDirectory = Directory.Exists(this.OutputPath.Text)
                    ? this.OutputPath.Text
                    : ShellFunctions.GetUserFolder(ShellFunctions.UserFolder.Downloads);

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                    this.OutputPath.Text = dialog.FileName;
                }
            }
        }

        private void RadioFormat_OnClick(object sender, RoutedEventArgs e) {
            StartSequence.IsEnabled = (sender as RadioButton)?.Name == "RadioSequence";
        }

        private void Tags_OnTextChanged(object sender, TextChangedEventArgs e) {
            if (!e.Changes.Select(change =>
                    (change.AddedLength > 0 && change.RemovedLength == 0)
                    || (change.AddedLength == 0 && change.RemovedLength >= 0)
                ).Any(v => v)) {
                return;
            }

            if (Settings.Default.FormatTags &&
                e.Changes
                    .Select(change => change.AddedLength >= 0
                                      && change.RemovedLength == 0
                                      && change.Offset < Tags.Text.Length
                                      && !Tags.Text.Substring(change.Offset, change.AddedLength)
                                          .Contains(' '))
                    .Any(v => v)) {

                DoFormatTags();
            }


            try {
                DoUpdateTagsState();

                ErrorLabel.Content = String.Empty;
                Start.IsEnabled = true;
            } catch (ImageTagParseException ex) {
                TagsBorder.BorderBrush = Brushes.Red;
                ErrorLabel.Content = ex.Message;
                Start.IsEnabled = false;
            }
        }

        private void FormatTags_OnChecked(object sender, RoutedEventArgs e) {
            if (Settings.Default.FormatTags) {
                this.DoFormatTags();
            }
        }

        private void Start_OnClick(object sender, RoutedEventArgs e) {
            StartDownloadSequence();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e) {
            StopDownloadSequence();
        }

        private void UseAPI_OnChecked(object sender, RoutedEventArgs e) {
            RefreshSourceView();
        }



        private void NewMaximumHandler(object sender, long arg) {
            _NewMaximum(arg);

            UpdateCounter();
        }

        private void ProgressHandler(object sender, long arg) {
            _Progress(arg);

            UpdateCounter();
        }

        private void WorkingHandler(object sender, string arg) {
            _SetStatus(arg);
        }

        private void WorkDoneHandler(object sender, string arg) {
            _SetStatus(arg);
        }




        private void WebClientDownloadProgress(object sender, DownloadProgressChangedEventArgs args) {
            UpdateStats(args.BytesReceived, args.TotalBytesToReceive);
        }
    }
}

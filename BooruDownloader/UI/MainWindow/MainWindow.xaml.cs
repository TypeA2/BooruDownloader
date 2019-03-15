using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using BooruDownloader.Properties;

namespace BooruDownloader {

    public partial class MainWindow {

        public static readonly List<TaggedImageBoard> AvailableImageBoards =
            AppDomain.CurrentDomain.GetAssemblies().AsEnumerable()
                .Select(a => a.GetTypes())
                .Select(t => t.Where(type => type.GetCustomAttributes(typeof(ImageBoardAttribute), true).Length > 0))
                .SelectMany(t => t)
                .Select(t => (TaggedImageBoard) Activator.CreateInstance(t))
                .ToList();


        private readonly Brush textbox_border;

        public MainWindow() {
            InitializeComponent();

            this.AdditionalComponentSetup(out this.textbox_border);

            this.LoadAllSettings();

            this.AttachEventHandlers();

            DoUpdateTagsState();
        }

        private void AdditionalComponentSetup(out Brush default_border) {
            default_border = this.OutputPath.BorderBrush;
        }

        private void AttachEventHandlers() {
            foreach (TaggedImageBoard board in AvailableImageBoards) {
                board.NewMaximum += this.NewMaximumHandler;
                board.Progess += this.ProgressHandler;
                board.Working += this.WorkingHandler;
                board.WorkDone += this.WorkDoneHandler;
            }

            WebHelper.Client.DownloadProgressChanged += WebClientDownloadProgress;

        }

        private void WriteAllSettings() {
            Settings.Default.CurrentTags = this.Tags.Text;
            Settings.Default.CurrentRating = this.Rating.Children.OfType<RadioButton>()
                .First(n => n.IsChecked ?? false)?.Name.Substring(5) ?? "Any";
            Settings.Default.CurrentFormat = this.Format.Children.OfType<RadioButton>()
                .First(n => n.IsChecked ?? false)?.Name.Substring(5) ?? "MD5";
            Settings.Default.StartSequence = ulong.Parse(this.StartSequence.Text);
            Settings.Default.CurrentSource = this.Source.Text;
            Settings.Default.CurrentPath = this.OutputPath.Text;
            Settings.Default.Save();
        }

        private void LoadAllSettings() {
            
            RadioButton target_rating = this.Rating.Children.OfType<RadioButton>()
                .FirstOrDefault(n => n.Name == "Radio" + Settings.Default.CurrentRating);
            if (target_rating != null) {
                target_rating.IsChecked = true;
            }

            RadioButton target_format = this.Format.Children.OfType<RadioButton>()
                .FirstOrDefault(n => n.Name == "Radio" + Settings.Default.CurrentFormat);
            if (target_format != null) {
                target_format.IsChecked = true;
            }

            this.StartSequence.Text = Settings.Default.StartSequence.ToString();

            // There has to be a better way?
            for (int i = 0; i < this.Source.Items.Count; i++) {
                if (this.Source.Items[i].ToString() == Settings.Default.CurrentSource) {
                    this.Source.SelectedIndex = i;
                    break;
                }
            }

            this.OutputPath.Text = string.IsNullOrEmpty(Settings.Default.CurrentPath)
                ? ShellFunctions.GetUserFolder(ShellFunctions.UserFolder.Pictures)
                : Settings.Default.CurrentPath;

            // Needs to be loaded after CurrentSource to avoid crashes in TextChanged handler
            this.Tags.Text = Settings.Default.CurrentTags;

            if (AvailableImageBoards.First(board => board.Name == "Danbooru") is ApiImageBoard danbooru) {
                danbooru.Credentials
                    = new ApiImageBoard.ApiCredentials {
                        Username = Settings.Default.DanbooruUsername,
                        ApiKey = Settings.Default.DanbooruKey
                    };
            }
            
        }
    }
}

using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BooruDownloader {
    public partial class ApiKeys : Window {

        private bool is_validating;

        private bool name_invalid;
        private bool key_invalid;

        private readonly Brush textbox_border;

        public ApiKeys() {
            InitializeComponent();

            textbox_border = NameDanbooru.BorderBrush;
        }

        private async void ConfirmInput(object sender, RoutedEventArgs e) {
            if (NameDanbooru.Text.Length == 0
                && KeyDanbooru.Text.Length == 0) {
                Danbooru.ClearAllCredentials();
                Close();
                return;
            }

            if (NameDanbooru.Text.Length == 0) {
                NameDanbooru.BorderBrush = Brushes.Red;
                name_invalid = true;
                return;
            }

            if (KeyDanbooru.Text.Length == 0) {
                KeyDanbooru.BorderBrush = Brushes.Red;
                key_invalid = true;
                return;
            }

            Danbooru db = new Danbooru(new ApiImageBoard.ApiCredentials {
                Username = NameDanbooru.Text,
                ApiKey = KeyDanbooru.Text
            });

            CancelButton.IsEnabled = false;
            ConfirmButton.IsEnabled = false;
            ConfirmButton.Content = "Validating...";

            is_validating = true;

            bool is_valid = await db.ValidateAndSaveCredentials();

            is_validating = false;

            if (is_valid) {

                ApiImageBoard danbooru =
                    (MainWindow.AvailableImageBoards.First(board => board.Name == "Danbooru") as ApiImageBoard) ??
                    new Danbooru();

                danbooru.Credentials
                    = new ApiImageBoard.ApiCredentials {
                        Username = NameDanbooru.Text,
                        ApiKey = KeyDanbooru.Text
                    };

                Close();
                return;
            }

            MessageBox.Show("The given API credentials do not seem to be correct.", "Invalid API credentials",
                MessageBoxButton.OK);

            CancelButton.IsEnabled = true;
            ConfirmButton.IsEnabled = true;
            ConfirmButton.Content = "Confirm";
        }

        private void ApiKeys_OnClosing(object sender, CancelEventArgs e) {
            e.Cancel = this.is_validating;
        }

        private void NameDanbooru_OnTextChanged(object sender, TextChangedEventArgs e) {
            if (this.name_invalid) {
                this.NameDanbooru.BorderBrush = this.textbox_border;
                this.name_invalid = false;
            }
        }

        private void KeyDanbooru_OnTextChanged(object sender, TextChangedEventArgs e) {
            if (this.key_invalid) {
                this.KeyDanbooru.BorderBrush = this.textbox_border;
                this.key_invalid = false;
            }
        }
    }
}

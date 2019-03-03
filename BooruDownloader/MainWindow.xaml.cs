using System.Windows;

namespace BooruDownloader {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        ~MainWindow() {
            WriteAllSettings();
        }

        private void WriteAllSettings() {
            
        }

        private void ShowAPIKeysMenu(object sender, RoutedEventArgs e) {
            APIKeys child = new APIKeys {
                ShowInTaskbar = false,
                Owner = this
            };

            child.ShowDialog();
        }
    }
}

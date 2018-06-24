using System.Windows.Forms;

namespace booru_downloader {
    public partial class Dialog : System.Windows.Forms.Form {

        public Dialog (DisplaySource[] sources) {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            this.comboSource.Items.AddRange(sources);
            this.comboSource.SelectedIndex = 0;

            this.InitializeEvents();

            this.textOutputFolder.Text = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures);
        }

        private void ResetUI() {
            this.groupRating.Enabled = true;
            this.groupSaveFormat.Enabled = true;
            this.comboSource.Enabled = true;
            this.checkCreateSubfolder.Enabled = true;
            this.textOutputFolder.Enabled = true;
            this.buttonBrowseOutput.Enabled = true;
            this.textTags.Enabled = true;
            this.labelSearchTags.Enabled = true;
            this.labelSource.Enabled = true;
            this.labelOutputFolder.Enabled = true;
            this.buttonStart.Enabled = true;

            this.buttonCancel.Enabled = false;

            this.progressBar.Value = 0;
            this.labelProgress.Text = "0 / 0";

            this.labelStatus.Text = "";
        }
    }
}

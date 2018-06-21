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
    }
}

using System.Windows.Forms;

namespace booru_downloader {
    public partial class Dialog : System.Windows.Forms.Form {

        public Dialog(string[] sources) {
            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            this.comboSource.DataSource = sources;

            this.InitializeEvents();
        }
    }
}

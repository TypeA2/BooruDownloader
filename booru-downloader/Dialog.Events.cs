using System.Windows.Forms;

namespace booru_downloader {
    partial class Dialog {
        private void InitializeEvents() {
            this.radioSequence.CheckedChanged += new System.EventHandler(this.RadioSequence_CheckedChanged);
        }

        private void RadioSequence_CheckedChanged(object sender, System.EventArgs e) {
            RadioButton radio = (RadioButton) sender;

            this.labelStartAt.Enabled = radio.Checked;
            this.numericSequenceStart.Enabled = radio.Checked;
        }
    }
}
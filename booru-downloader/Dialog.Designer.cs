namespace booru_downloader {
    partial class Dialog {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dialog));
            this.textTags = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioAny = new System.Windows.Forms.RadioButton();
            this.radioQuestionableExplicit = new System.Windows.Forms.RadioButton();
            this.radioSafeQuestionable = new System.Windows.Forms.RadioButton();
            this.radioExplicit = new System.Windows.Forms.RadioButton();
            this.radioQuestionable = new System.Windows.Forms.RadioButton();
            this.radioSafe = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numericSequenceStart = new System.Windows.Forms.NumericUpDown();
            this.checkSaveMetadata = new System.Windows.Forms.CheckBox();
            this.labelStartAt = new System.Windows.Forms.Label();
            this.radioSequence = new System.Windows.Forms.RadioButton();
            this.radioID = new System.Windows.Forms.RadioButton();
            this.radioMD5 = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.textOutputFolder = new System.Windows.Forms.TextBox();
            this.buttonBrowseOutput = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.checkCreateSubfolder = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboSource = new System.Windows.Forms.ComboBox();
            this.labelError = new System.Windows.Forms.Label();
            this.labelProgress = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSequenceStart)).BeginInit();
            this.SuspendLayout();
            // 
            // textTags
            // 
            this.textTags.Location = new System.Drawing.Point(15, 25);
            this.textTags.Multiline = true;
            this.textTags.Name = "textTags";
            this.textTags.Size = new System.Drawing.Size(240, 120);
            this.textTags.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Search tags";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioAny);
            this.groupBox1.Controls.Add(this.radioQuestionableExplicit);
            this.groupBox1.Controls.Add(this.radioSafeQuestionable);
            this.groupBox1.Controls.Add(this.radioExplicit);
            this.groupBox1.Controls.Add(this.radioQuestionable);
            this.groupBox1.Controls.Add(this.radioSafe);
            this.groupBox1.Location = new System.Drawing.Point(261, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 133);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Rating";
            // 
            // radioAny
            // 
            this.radioAny.AutoSize = true;
            this.radioAny.Checked = true;
            this.radioAny.Location = new System.Drawing.Point(98, 65);
            this.radioAny.Name = "radioAny";
            this.radioAny.Size = new System.Drawing.Size(43, 17);
            this.radioAny.TabIndex = 5;
            this.radioAny.TabStop = true;
            this.radioAny.Text = "Any";
            this.radioAny.UseVisualStyleBackColor = true;
            // 
            // radioQuestionableExplicit
            // 
            this.radioQuestionableExplicit.AutoSize = true;
            this.radioQuestionableExplicit.Location = new System.Drawing.Point(98, 42);
            this.radioQuestionableExplicit.Name = "radioQuestionableExplicit";
            this.radioQuestionableExplicit.Size = new System.Drawing.Size(131, 17);
            this.radioQuestionableExplicit.TabIndex = 4;
            this.radioQuestionableExplicit.Text = "Questionable / Explicit";
            this.radioQuestionableExplicit.UseVisualStyleBackColor = true;
            // 
            // radioSafeQuestionable
            // 
            this.radioSafeQuestionable.AutoSize = true;
            this.radioSafeQuestionable.Location = new System.Drawing.Point(99, 19);
            this.radioSafeQuestionable.Name = "radioSafeQuestionable";
            this.radioSafeQuestionable.Size = new System.Drawing.Size(120, 17);
            this.radioSafeQuestionable.TabIndex = 3;
            this.radioSafeQuestionable.Text = "Safe / Questionable";
            this.radioSafeQuestionable.UseVisualStyleBackColor = true;
            // 
            // radioExplicit
            // 
            this.radioExplicit.AutoSize = true;
            this.radioExplicit.Location = new System.Drawing.Point(6, 65);
            this.radioExplicit.Name = "radioExplicit";
            this.radioExplicit.Size = new System.Drawing.Size(58, 17);
            this.radioExplicit.TabIndex = 2;
            this.radioExplicit.Text = "Explicit";
            this.radioExplicit.UseVisualStyleBackColor = true;
            // 
            // radioQuestionable
            // 
            this.radioQuestionable.AutoSize = true;
            this.radioQuestionable.Location = new System.Drawing.Point(6, 42);
            this.radioQuestionable.Name = "radioQuestionable";
            this.radioQuestionable.Size = new System.Drawing.Size(87, 17);
            this.radioQuestionable.TabIndex = 1;
            this.radioQuestionable.Text = "Questionable";
            this.radioQuestionable.UseVisualStyleBackColor = true;
            // 
            // radioSafe
            // 
            this.radioSafe.AutoSize = true;
            this.radioSafe.Location = new System.Drawing.Point(6, 19);
            this.radioSafe.Name = "radioSafe";
            this.radioSafe.Size = new System.Drawing.Size(47, 17);
            this.radioSafe.TabIndex = 0;
            this.radioSafe.Text = "Safe";
            this.radioSafe.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numericSequenceStart);
            this.groupBox2.Controls.Add(this.checkSaveMetadata);
            this.groupBox2.Controls.Add(this.labelStartAt);
            this.groupBox2.Controls.Add(this.radioSequence);
            this.groupBox2.Controls.Add(this.radioID);
            this.groupBox2.Controls.Add(this.radioMD5);
            this.groupBox2.Location = new System.Drawing.Point(15, 151);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(486, 100);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Save format";
            // 
            // numericSequenceStart
            // 
            this.numericSequenceStart.Enabled = false;
            this.numericSequenceStart.Location = new System.Drawing.Point(290, 68);
            this.numericSequenceStart.Name = "numericSequenceStart";
            this.numericSequenceStart.Size = new System.Drawing.Size(120, 20);
            this.numericSequenceStart.TabIndex = 5;
            // 
            // checkSaveMetadata
            // 
            this.checkSaveMetadata.AutoSize = true;
            this.checkSaveMetadata.Checked = true;
            this.checkSaveMetadata.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkSaveMetadata.Location = new System.Drawing.Point(252, 19);
            this.checkSaveMetadata.Name = "checkSaveMetadata";
            this.checkSaveMetadata.Size = new System.Drawing.Size(98, 17);
            this.checkSaveMetadata.TabIndex = 4;
            this.checkSaveMetadata.Text = "Save metadata";
            this.checkSaveMetadata.UseVisualStyleBackColor = true;
            // 
            // labelStartAt
            // 
            this.labelStartAt.AutoSize = true;
            this.labelStartAt.Enabled = false;
            this.labelStartAt.Location = new System.Drawing.Point(243, 70);
            this.labelStartAt.Name = "labelStartAt";
            this.labelStartAt.Size = new System.Drawing.Size(41, 13);
            this.labelStartAt.TabIndex = 3;
            this.labelStartAt.Text = "Start at";
            // 
            // radioSequence
            // 
            this.radioSequence.AutoSize = true;
            this.radioSequence.Location = new System.Drawing.Point(7, 68);
            this.radioSequence.Name = "radioSequence";
            this.radioSequence.Size = new System.Drawing.Size(112, 17);
            this.radioSequence.TabIndex = 2;
            this.radioSequence.Text = "Number sequence";
            this.radioSequence.UseVisualStyleBackColor = true;
            // 
            // radioID
            // 
            this.radioID.AutoSize = true;
            this.radioID.Location = new System.Drawing.Point(7, 44);
            this.radioID.Name = "radioID";
            this.radioID.Size = new System.Drawing.Size(36, 17);
            this.radioID.TabIndex = 1;
            this.radioID.Text = "ID";
            this.radioID.UseVisualStyleBackColor = true;
            // 
            // radioMD5
            // 
            this.radioMD5.AutoSize = true;
            this.radioMD5.Checked = true;
            this.radioMD5.Location = new System.Drawing.Point(7, 20);
            this.radioMD5.Name = "radioMD5";
            this.radioMD5.Size = new System.Drawing.Size(86, 17);
            this.radioMD5.TabIndex = 0;
            this.radioMD5.TabStop = true;
            this.radioMD5.Text = "Original MD5";
            this.radioMD5.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 321);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Output folder";
            // 
            // textOutputFolder
            // 
            this.textOutputFolder.Location = new System.Drawing.Point(93, 318);
            this.textOutputFolder.Name = "textOutputFolder";
            this.textOutputFolder.Size = new System.Drawing.Size(330, 20);
            this.textOutputFolder.TabIndex = 5;
            // 
            // buttonBrowseOutput
            // 
            this.buttonBrowseOutput.Location = new System.Drawing.Point(429, 316);
            this.buttonBrowseOutput.Name = "buttonBrowseOutput";
            this.buttonBrowseOutput.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowseOutput.TabIndex = 6;
            this.buttonBrowseOutput.Text = "Browse";
            this.buttonBrowseOutput.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(22, 344);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(374, 23);
            this.progressBar.TabIndex = 7;
            // 
            // buttonStart
            // 
            this.buttonStart.Enabled = false;
            this.buttonStart.Location = new System.Drawing.Point(429, 373);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 8;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Location = new System.Drawing.Point(348, 373);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // checkCreateSubfolder
            // 
            this.checkCreateSubfolder.AutoSize = true;
            this.checkCreateSubfolder.Checked = true;
            this.checkCreateSubfolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkCreateSubfolder.Location = new System.Drawing.Point(22, 284);
            this.checkCreateSubfolder.Name = "checkCreateSubfolder";
            this.checkCreateSubfolder.Size = new System.Drawing.Size(103, 17);
            this.checkCreateSubfolder.TabIndex = 10;
            this.checkCreateSubfolder.Text = "Create subfolder";
            this.checkCreateSubfolder.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 260);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Source";
            // 
            // comboSource
            // 
            this.comboSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboSource.FormattingEnabled = true;
            this.comboSource.Location = new System.Drawing.Point(134, 257);
            this.comboSource.Name = "comboSource";
            this.comboSource.Size = new System.Drawing.Size(121, 21);
            this.comboSource.TabIndex = 12;
            // 
            // labelError
            // 
            this.labelError.AutoSize = true;
            this.labelError.ForeColor = System.Drawing.Color.Red;
            this.labelError.Location = new System.Drawing.Point(19, 373);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(0, 13);
            this.labelError.TabIndex = 13;
            // 
            // labelProgress
            // 
            this.labelProgress.AutoSize = true;
            this.labelProgress.Location = new System.Drawing.Point(402, 349);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(30, 13);
            this.labelProgress.TabIndex = 14;
            this.labelProgress.Text = "0 / 0";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(19, 373);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 13);
            this.labelStatus.TabIndex = 15;
            // 
            // Dialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 401);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.labelProgress);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.comboSource);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.checkCreateSubfolder);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonBrowseOutput);
            this.Controls.Add(this.textOutputFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textTags);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Dialog";
            this.Text = "booru-downloader";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericSequenceStart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textTags;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioAny;
        private System.Windows.Forms.RadioButton radioQuestionableExplicit;
        private System.Windows.Forms.RadioButton radioSafeQuestionable;
        private System.Windows.Forms.RadioButton radioExplicit;
        private System.Windows.Forms.RadioButton radioQuestionable;
        private System.Windows.Forms.RadioButton radioSafe;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown numericSequenceStart;
        private System.Windows.Forms.CheckBox checkSaveMetadata;
        private System.Windows.Forms.Label labelStartAt;
        private System.Windows.Forms.RadioButton radioSequence;
        private System.Windows.Forms.RadioButton radioID;
        private System.Windows.Forms.RadioButton radioMD5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textOutputFolder;
        private System.Windows.Forms.Button buttonBrowseOutput;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkCreateSubfolder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboSource;
        private System.Windows.Forms.Label labelError;
        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Label labelStatus;
    }
}


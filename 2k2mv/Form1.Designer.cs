namespace _2k2mv
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_inputDir = new System.Windows.Forms.Button();
            this.label_inputDir = new System.Windows.Forms.Label();
            this.label_inputDir_status = new System.Windows.Forms.Label();
            this.label_outputDir_status = new System.Windows.Forms.Label();
            this.label_outputDir = new System.Windows.Forms.Label();
            this.button_outputDir = new System.Windows.Forms.Button();
            this.label_iconv_status = new System.Windows.Forms.Label();
            this.label_iconv = new System.Windows.Forms.Label();
            this.button_iconv = new System.Windows.Forms.Button();
            this.label_dconv_status = new System.Windows.Forms.Label();
            this.label_dconv = new System.Windows.Forms.Label();
            this.button_dconv = new System.Windows.Forms.Button();
            this.label_mconv_status = new System.Windows.Forms.Label();
            this.label_mconv = new System.Windows.Forms.Label();
            this.button_mconv = new System.Windows.Forms.Button();
            this.folderBrowserDialogInput = new System.Windows.Forms.FolderBrowserDialog();
            this.copyMissingImagesCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox_dconv = new System.Windows.Forms.GroupBox();
            this.updateOnlyMapDataCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox_mconv = new System.Windows.Forms.GroupBox();
            this.label_mvconv = new System.Windows.Forms.Label();
            this.button_mvconv = new System.Windows.Forms.Button();
            this.label_mvconv_status = new System.Windows.Forms.Label();
            this.groupBox_mvconv = new System.Windows.Forms.GroupBox();
            this.groupBox_dconv.SuspendLayout();
            this.groupBox_mconv.SuspendLayout();
            this.groupBox_mvconv.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_inputDir
            // 
            this.button_inputDir.Location = new System.Drawing.Point(160, 12);
            this.button_inputDir.Name = "button_inputDir";
            this.button_inputDir.Size = new System.Drawing.Size(75, 23);
            this.button_inputDir.TabIndex = 2;
            this.button_inputDir.Text = "Browse";
            this.button_inputDir.UseVisualStyleBackColor = true;
            this.button_inputDir.Click += new System.EventHandler(this.button_inputDir_Click);
            // 
            // label_inputDir
            // 
            this.label_inputDir.AutoSize = true;
            this.label_inputDir.Location = new System.Drawing.Point(30, 17);
            this.label_inputDir.Name = "label_inputDir";
            this.label_inputDir.Size = new System.Drawing.Size(76, 13);
            this.label_inputDir.TabIndex = 3;
            this.label_inputDir.Text = "Input Directory";
            // 
            // label_inputDir_status
            // 
            this.label_inputDir_status.AutoSize = true;
            this.label_inputDir_status.Location = new System.Drawing.Point(259, 17);
            this.label_inputDir_status.Name = "label_inputDir_status";
            this.label_inputDir_status.Size = new System.Drawing.Size(43, 13);
            this.label_inputDir_status.TabIndex = 4;
            this.label_inputDir_status.Text = "Not Set";
            // 
            // label_outputDir_status
            // 
            this.label_outputDir_status.AutoSize = true;
            this.label_outputDir_status.Location = new System.Drawing.Point(259, 58);
            this.label_outputDir_status.Name = "label_outputDir_status";
            this.label_outputDir_status.Size = new System.Drawing.Size(43, 13);
            this.label_outputDir_status.TabIndex = 7;
            this.label_outputDir_status.Text = "Not Set";
            // 
            // label_outputDir
            // 
            this.label_outputDir.AutoSize = true;
            this.label_outputDir.Location = new System.Drawing.Point(30, 58);
            this.label_outputDir.Name = "label_outputDir";
            this.label_outputDir.Size = new System.Drawing.Size(84, 13);
            this.label_outputDir.TabIndex = 6;
            this.label_outputDir.Text = "Output Directory";
            // 
            // button_outputDir
            // 
            this.button_outputDir.Location = new System.Drawing.Point(160, 53);
            this.button_outputDir.Name = "button_outputDir";
            this.button_outputDir.Size = new System.Drawing.Size(75, 23);
            this.button_outputDir.TabIndex = 5;
            this.button_outputDir.Text = "Browse";
            this.button_outputDir.UseVisualStyleBackColor = true;
            this.button_outputDir.Click += new System.EventHandler(this.button_outputDir_Click);
            // 
            // label_iconv_status
            // 
            this.label_iconv_status.AutoSize = true;
            this.label_iconv_status.Location = new System.Drawing.Point(257, 218);
            this.label_iconv_status.Name = "label_iconv_status";
            this.label_iconv_status.Size = new System.Drawing.Size(78, 13);
            this.label_iconv_status.TabIndex = 10;
            this.label_iconv_status.Text = "Waiting to start";
            // 
            // label_iconv
            // 
            this.label_iconv.AutoSize = true;
            this.label_iconv.Location = new System.Drawing.Point(28, 218);
            this.label_iconv.Name = "label_iconv";
            this.label_iconv.Size = new System.Drawing.Size(115, 13);
            this.label_iconv.TabIndex = 9;
            this.label_iconv.Text = "Convert Tileset Images";
            // 
            // button_iconv
            // 
            this.button_iconv.Enabled = false;
            this.button_iconv.Location = new System.Drawing.Point(158, 213);
            this.button_iconv.Name = "button_iconv";
            this.button_iconv.Size = new System.Drawing.Size(75, 23);
            this.button_iconv.TabIndex = 8;
            this.button_iconv.Text = "Convert";
            this.button_iconv.UseVisualStyleBackColor = true;
            this.button_iconv.Click += new System.EventHandler(this.button_iconv_Click);
            // 
            // label_dconv_status
            // 
            this.label_dconv_status.AutoSize = true;
            this.label_dconv_status.Location = new System.Drawing.Point(244, 24);
            this.label_dconv_status.Name = "label_dconv_status";
            this.label_dconv_status.Size = new System.Drawing.Size(78, 13);
            this.label_dconv_status.TabIndex = 10;
            this.label_dconv_status.Text = "Waiting to start";
            // 
            // label_dconv
            // 
            this.label_dconv.AutoSize = true;
            this.label_dconv.Location = new System.Drawing.Point(15, 24);
            this.label_dconv.Name = "label_dconv";
            this.label_dconv.Size = new System.Drawing.Size(104, 13);
            this.label_dconv.TabIndex = 9;
            this.label_dconv.Text = "Convert Tileset Data";
            // 
            // button_dconv
            // 
            this.button_dconv.Enabled = false;
            this.button_dconv.Location = new System.Drawing.Point(145, 19);
            this.button_dconv.Name = "button_dconv";
            this.button_dconv.Size = new System.Drawing.Size(75, 23);
            this.button_dconv.TabIndex = 8;
            this.button_dconv.Text = "Convert";
            this.button_dconv.UseVisualStyleBackColor = true;
            this.button_dconv.Click += new System.EventHandler(this.button_dconv_Click);
            // 
            // label_mconv_status
            // 
            this.label_mconv_status.AutoSize = true;
            this.label_mconv_status.Location = new System.Drawing.Point(245, 24);
            this.label_mconv_status.Name = "label_mconv_status";
            this.label_mconv_status.Size = new System.Drawing.Size(78, 13);
            this.label_mconv_status.TabIndex = 15;
            this.label_mconv_status.Text = "Waiting to start";
            // 
            // label_mconv
            // 
            this.label_mconv.AutoSize = true;
            this.label_mconv.Location = new System.Drawing.Point(16, 24);
            this.label_mconv.Name = "label_mconv";
            this.label_mconv.Size = new System.Drawing.Size(73, 13);
            this.label_mconv.TabIndex = 14;
            this.label_mconv.Text = "Convert Maps";
            // 
            // button_mconv
            // 
            this.button_mconv.Enabled = false;
            this.button_mconv.Location = new System.Drawing.Point(146, 19);
            this.button_mconv.Name = "button_mconv";
            this.button_mconv.Size = new System.Drawing.Size(75, 23);
            this.button_mconv.TabIndex = 13;
            this.button_mconv.Text = "Convert";
            this.button_mconv.UseVisualStyleBackColor = true;
            this.button_mconv.Click += new System.EventHandler(this.button_mconv_Click);
            // 
            // copyMissingImagesCheckBox
            // 
            this.copyMissingImagesCheckBox.AutoSize = true;
            this.copyMissingImagesCheckBox.Location = new System.Drawing.Point(18, 59);
            this.copyMissingImagesCheckBox.Name = "copyMissingImagesCheckBox";
            this.copyMissingImagesCheckBox.Size = new System.Drawing.Size(329, 17);
            this.copyMissingImagesCheckBox.TabIndex = 16;
            this.copyMissingImagesCheckBox.Text = "Also copy missing chipset images from RTP to the input directory";
            this.copyMissingImagesCheckBox.UseVisualStyleBackColor = true;
            this.copyMissingImagesCheckBox.CheckedChanged += new System.EventHandler(this.copyMissingImagesCheckBox_CheckedChanged);
            // 
            // groupBox_dconv
            // 
            this.groupBox_dconv.Controls.Add(this.button_dconv);
            this.groupBox_dconv.Controls.Add(this.copyMissingImagesCheckBox);
            this.groupBox_dconv.Controls.Add(this.label_dconv);
            this.groupBox_dconv.Controls.Add(this.label_dconv_status);
            this.groupBox_dconv.Location = new System.Drawing.Point(13, 95);
            this.groupBox_dconv.Name = "groupBox_dconv";
            this.groupBox_dconv.Size = new System.Drawing.Size(464, 100);
            this.groupBox_dconv.TabIndex = 17;
            this.groupBox_dconv.TabStop = false;
            // 
            // updateOnlyMapDataCheckBox
            // 
            this.updateOnlyMapDataCheckBox.AutoSize = true;
            this.updateOnlyMapDataCheckBox.Location = new System.Drawing.Point(19, 59);
            this.updateOnlyMapDataCheckBox.Name = "updateOnlyMapDataCheckBox";
            this.updateOnlyMapDataCheckBox.Size = new System.Drawing.Size(369, 17);
            this.updateOnlyMapDataCheckBox.TabIndex = 18;
            this.updateOnlyMapDataCheckBox.Text = "Update only tile data in existing maps and do not overwrite MapInfos.json";
            this.updateOnlyMapDataCheckBox.UseVisualStyleBackColor = true;
            this.updateOnlyMapDataCheckBox.CheckedChanged += new System.EventHandler(this.updateOnlyMapDataCheckBox_CheckedChanged);
            // 
            // groupBox_mconv
            // 
            this.groupBox_mconv.Controls.Add(this.button_mconv);
            this.groupBox_mconv.Controls.Add(this.updateOnlyMapDataCheckBox);
            this.groupBox_mconv.Controls.Add(this.label_mconv);
            this.groupBox_mconv.Controls.Add(this.label_mconv_status);
            this.groupBox_mconv.Location = new System.Drawing.Point(13, 254);
            this.groupBox_mconv.Name = "groupBox_mconv";
            this.groupBox_mconv.Size = new System.Drawing.Size(464, 100);
            this.groupBox_mconv.TabIndex = 19;
            this.groupBox_mconv.TabStop = false;
            // 
            // label_mvconv
            // 
            this.label_mvconv.AutoSize = true;
            this.label_mvconv.Location = new System.Drawing.Point(57, 24);
            this.label_mvconv.Name = "label_mvconv";
            this.label_mvconv.Size = new System.Drawing.Size(105, 13);
            this.label_mvconv.TabIndex = 20;
            this.label_mvconv.Text = "Transfer MV tile data";
            // 
            // button_mvconv
            // 
            this.button_mvconv.Location = new System.Drawing.Point(197, 19);
            this.button_mvconv.Name = "button_mvconv";
            this.button_mvconv.Size = new System.Drawing.Size(75, 23);
            this.button_mvconv.TabIndex = 21;
            this.button_mvconv.Text = "Transfer";
            this.button_mvconv.UseVisualStyleBackColor = true;
            this.button_mvconv.Click += new System.EventHandler(this.button_mvconv_Click);
            // 
            // label_mvconv_status
            // 
            this.label_mvconv_status.AutoSize = true;
            this.label_mvconv_status.Location = new System.Drawing.Point(301, 23);
            this.label_mvconv_status.Name = "label_mvconv_status";
            this.label_mvconv_status.Size = new System.Drawing.Size(78, 13);
            this.label_mvconv_status.TabIndex = 22;
            this.label_mvconv_status.Text = "Waiting to start";
            // 
            // groupBox_mvconv
            // 
            this.groupBox_mvconv.Controls.Add(this.button_mvconv);
            this.groupBox_mvconv.Controls.Add(this.label_mvconv_status);
            this.groupBox_mvconv.Controls.Add(this.label_mvconv);
            this.groupBox_mvconv.Location = new System.Drawing.Point(507, 254);
            this.groupBox_mvconv.Name = "groupBox_mvconv";
            this.groupBox_mvconv.Size = new System.Drawing.Size(465, 100);
            this.groupBox_mvconv.TabIndex = 23;
            this.groupBox_mvconv.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 462);
            this.Controls.Add(this.groupBox_mvconv);
            this.Controls.Add(this.label_inputDir);
            this.Controls.Add(this.groupBox_mconv);
            this.Controls.Add(this.button_inputDir);
            this.Controls.Add(this.button_outputDir);
            this.Controls.Add(this.button_iconv);
            this.Controls.Add(this.groupBox_dconv);
            this.Controls.Add(this.label_iconv);
            this.Controls.Add(this.label_outputDir_status);
            this.Controls.Add(this.label_outputDir);
            this.Controls.Add(this.label_iconv_status);
            this.Controls.Add(this.label_inputDir_status);
            this.Name = "MainForm";
            this.Text = "2k2mv";
            this.groupBox_dconv.ResumeLayout(false);
            this.groupBox_dconv.PerformLayout();
            this.groupBox_mconv.ResumeLayout(false);
            this.groupBox_mconv.PerformLayout();
            this.groupBox_mvconv.ResumeLayout(false);
            this.groupBox_mvconv.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_inputDir;
        private System.Windows.Forms.Label label_inputDir;
        private System.Windows.Forms.Label label_inputDir_status;
        private System.Windows.Forms.Label label_outputDir_status;
        private System.Windows.Forms.Label label_outputDir;
        private System.Windows.Forms.Button button_outputDir;
        private System.Windows.Forms.Label label_iconv_status;
        private System.Windows.Forms.Label label_iconv;
        private System.Windows.Forms.Button button_iconv;
        private System.Windows.Forms.Label label_dconv_status;
        private System.Windows.Forms.Label label_dconv;
        private System.Windows.Forms.Button button_dconv;
        private System.Windows.Forms.Label label_mconv_status;
        private System.Windows.Forms.Label label_mconv;
        private System.Windows.Forms.Button button_mconv;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogInput;
        private System.Windows.Forms.CheckBox copyMissingImagesCheckBox;
        private System.Windows.Forms.GroupBox groupBox_dconv;
        private System.Windows.Forms.CheckBox updateOnlyMapDataCheckBox;
        private System.Windows.Forms.GroupBox groupBox_mconv;
        private System.Windows.Forms.Label label_mvconv;
        private System.Windows.Forms.Button button_mvconv;
        private System.Windows.Forms.Label label_mvconv_status;
        private System.Windows.Forms.GroupBox groupBox_mvconv;
    }
}


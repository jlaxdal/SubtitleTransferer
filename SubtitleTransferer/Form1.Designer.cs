namespace SubtitleTransferer
{
    partial class SMR
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SMR));
            this.listBox_subtitleFiles = new System.Windows.Forms.CheckedListBox();
            this.lbl_status = new System.Windows.Forms.Label();
            this.txt_startFolder = new System.Windows.Forms.TextBox();
            this.btn_startScan = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_SelectAllFiles = new System.Windows.Forms.Button();
            this.txtBox_summary = new System.Windows.Forms.TextBox();
            this.btn_moveAndRenameSelectedSubtitles = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.chkBoxDefaultCC = new System.Windows.Forms.CheckBox();
            this.chkBox_CopyAction = new System.Windows.Forms.CheckBox();
            this.chkBoxDebug = new System.Windows.Forms.CheckBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button3 = new System.Windows.Forms.Button();
            this.toolTipDebug = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipDefaultCC = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipTransferAction = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // listBox_subtitleFiles
            // 
            this.listBox_subtitleFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox_subtitleFiles.CheckOnClick = true;
            this.listBox_subtitleFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox_subtitleFiles.FormattingEnabled = true;
            this.listBox_subtitleFiles.HorizontalScrollbar = true;
            this.listBox_subtitleFiles.IntegralHeight = false;
            this.listBox_subtitleFiles.Location = new System.Drawing.Point(3, 37);
            this.listBox_subtitleFiles.Name = "listBox_subtitleFiles";
            this.listBox_subtitleFiles.Size = new System.Drawing.Size(930, 235);
            this.listBox_subtitleFiles.TabIndex = 0;
            this.listBox_subtitleFiles.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listBox_subtitleFiles_ItemCheck);
            // 
            // lbl_status
            // 
            this.lbl_status.AutoSize = true;
            this.lbl_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_status.Location = new System.Drawing.Point(12, 9);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(90, 24);
            this.lbl_status.TabIndex = 1;
            this.lbl_status.Text = "Initializing";
            // 
            // txt_startFolder
            // 
            this.txt_startFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_startFolder.Location = new System.Drawing.Point(106, 284);
            this.txt_startFolder.Name = "txt_startFolder";
            this.txt_startFolder.Size = new System.Drawing.Size(827, 20);
            this.txt_startFolder.TabIndex = 2;
            this.txt_startFolder.TextChanged += new System.EventHandler(this.txt_startFolder_TextChanged);
            // 
            // btn_startScan
            // 
            this.btn_startScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_startScan.Enabled = false;
            this.btn_startScan.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_startScan.Location = new System.Drawing.Point(3, 313);
            this.btn_startScan.Name = "btn_startScan";
            this.btn_startScan.Size = new System.Drawing.Size(111, 30);
            this.btn_startScan.TabIndex = 3;
            this.btn_startScan.Text = "Start Scan";
            this.btn_startScan.UseVisualStyleBackColor = true;
            this.btn_startScan.Click += new System.EventHandler(this.btn_startScan_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(3, 278);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 29);
            this.button1.TabIndex = 4;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.select_startFolder);
            // 
            // btn_SelectAllFiles
            // 
            this.btn_SelectAllFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_SelectAllFiles.Enabled = false;
            this.btn_SelectAllFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_SelectAllFiles.Location = new System.Drawing.Point(121, 313);
            this.btn_SelectAllFiles.Name = "btn_SelectAllFiles";
            this.btn_SelectAllFiles.Size = new System.Drawing.Size(122, 30);
            this.btn_SelectAllFiles.TabIndex = 5;
            this.btn_SelectAllFiles.Text = "Select All";
            this.btn_SelectAllFiles.UseVisualStyleBackColor = true;
            this.btn_SelectAllFiles.Click += new System.EventHandler(this.btnSelectAllFiles_Click);
            // 
            // txtBox_summary
            // 
            this.txtBox_summary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBox_summary.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBox_summary.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBox_summary.HideSelection = false;
            this.txtBox_summary.Location = new System.Drawing.Point(3, 349);
            this.txtBox_summary.Multiline = true;
            this.txtBox_summary.Name = "txtBox_summary";
            this.txtBox_summary.ReadOnly = true;
            this.txtBox_summary.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtBox_summary.Size = new System.Drawing.Size(930, 130);
            this.txtBox_summary.TabIndex = 7;
            this.txtBox_summary.WordWrap = false;
            // 
            // btn_moveAndRenameSelectedSubtitles
            // 
            this.btn_moveAndRenameSelectedSubtitles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_moveAndRenameSelectedSubtitles.Enabled = false;
            this.btn_moveAndRenameSelectedSubtitles.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_moveAndRenameSelectedSubtitles.Location = new System.Drawing.Point(290, 313);
            this.btn_moveAndRenameSelectedSubtitles.Name = "btn_moveAndRenameSelectedSubtitles";
            this.btn_moveAndRenameSelectedSubtitles.Size = new System.Drawing.Size(289, 30);
            this.btn_moveAndRenameSelectedSubtitles.TabIndex = 8;
            this.btn_moveAndRenameSelectedSubtitles.Text = "Transfer and Rename Selected";
            this.btn_moveAndRenameSelectedSubtitles.UseVisualStyleBackColor = true;
            this.btn_moveAndRenameSelectedSubtitles.Click += new System.EventHandler(this.btn_moveAndRenameSelectedSubtitles_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(755, 318);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(77, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "testbutton";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // chkBoxDefaultCC
            // 
            this.chkBoxDefaultCC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkBoxDefaultCC.AutoSize = true;
            this.chkBoxDefaultCC.Checked = true;
            this.chkBoxDefaultCC.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxDefaultCC.Location = new System.Drawing.Point(687, 16);
            this.chkBoxDefaultCC.Name = "chkBoxDefaultCC";
            this.chkBoxDefaultCC.Size = new System.Drawing.Size(161, 17);
            this.chkBoxDefaultCC.TabIndex = 10;
            this.chkBoxDefaultCC.Text = "Prefer SDH/Closed Captions";
            this.toolTipDefaultCC.SetToolTip(this.chkBoxDefaultCC, "Prefer to have subtitles that are for the hearing\r\nimpaired selected (SDH or Clos" +
        "ed Caption subtitles)");
            this.chkBoxDefaultCC.UseVisualStyleBackColor = true;
            this.chkBoxDefaultCC.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // chkBox_CopyAction
            // 
            this.chkBox_CopyAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkBox_CopyAction.AutoSize = true;
            this.chkBox_CopyAction.Checked = true;
            this.chkBox_CopyAction.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBox_CopyAction.Location = new System.Drawing.Point(477, 16);
            this.chkBox_CopyAction.Name = "chkBox_CopyAction";
            this.chkBox_CopyAction.Size = new System.Drawing.Size(204, 17);
            this.chkBox_CopyAction.TabIndex = 11;
            this.chkBox_CopyAction.Text = "Copy subtitles instead of Moving them";
            this.toolTipTransferAction.SetToolTip(this.chkBox_CopyAction, "If this is checked then the subtitles are COPIED,\r\nif it\'s not checked the subtit" +
        "le files are MOVED\r\n");
            this.chkBox_CopyAction.UseVisualStyleBackColor = true;
            this.chkBox_CopyAction.CheckedChanged += new System.EventHandler(this.chkBox_CopyAction_CheckedChanged);
            // 
            // chkBoxDebug
            // 
            this.chkBoxDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkBoxDebug.AutoSize = true;
            this.chkBoxDebug.Location = new System.Drawing.Point(854, 16);
            this.chkBoxDebug.Name = "chkBoxDebug";
            this.chkBoxDebug.Size = new System.Drawing.Size(58, 17);
            this.chkBoxDebug.TabIndex = 12;
            this.chkBoxDebug.Text = "Debug";
            this.toolTipDebug.SetToolTip(this.chkBoxDebug, "Select this to show debug information in the summary");
            this.chkBoxDebug.UseVisualStyleBackColor = true;
            this.chkBoxDebug.CheckedChanged += new System.EventHandler(this.chkBoxDebug_CheckedChanged);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(3, 485);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(933, 23);
            this.progressBar1.TabIndex = 13;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.Location = new System.Drawing.Point(838, 318);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(93, 23);
            this.button3.TabIndex = 14;
            this.button3.Text = "Clear Summary";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.ClearSummary);
            // 
            // SMR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 509);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.chkBoxDebug);
            this.Controls.Add(this.chkBox_CopyAction);
            this.Controls.Add(this.chkBoxDefaultCC);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btn_moveAndRenameSelectedSubtitles);
            this.Controls.Add(this.txtBox_summary);
            this.Controls.Add(this.btn_SelectAllFiles);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btn_startScan);
            this.Controls.Add(this.txt_startFolder);
            this.Controls.Add(this.lbl_status);
            this.Controls.Add(this.listBox_subtitleFiles);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1800, 1000);
            this.MinimumSize = new System.Drawing.Size(840, 490);
            this.Name = "SMR";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SubtitleTransferer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox listBox_subtitleFiles;
        private System.Windows.Forms.Label lbl_status;
        private System.Windows.Forms.TextBox txt_startFolder;
        private System.Windows.Forms.Button btn_startScan;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_SelectAllFiles;
        private System.Windows.Forms.TextBox txtBox_summary;
        private System.Windows.Forms.Button btn_moveAndRenameSelectedSubtitles;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox chkBoxDefaultCC;
        private System.Windows.Forms.CheckBox chkBox_CopyAction;
        private System.Windows.Forms.CheckBox chkBoxDebug;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ToolTip toolTipDebug;
        private System.Windows.Forms.ToolTip toolTipDefaultCC;
        private System.Windows.Forms.ToolTip toolTipTransferAction;
    }
}


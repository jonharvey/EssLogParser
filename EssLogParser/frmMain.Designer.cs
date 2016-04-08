namespace EssLogParser
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.ofdEssLog = new System.Windows.Forms.OpenFileDialog();
            this.lblFile = new System.Windows.Forms.Label();
            this.txtFilename = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnParse = new System.Windows.Forms.Button();
            this.dgvSessionInfo = new System.Windows.Forms.DataGridView();
            this.btnShowFilter = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSessionInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // ofdEssLog
            // 
            this.ofdEssLog.Filter = "Log Files (*.log, *.txt)|*.log;*.txt|All Files (*.*)|*.*";
            this.ofdEssLog.Title = "Please select Essbase log file";
            // 
            // lblFile
            // 
            this.lblFile.AutoSize = true;
            this.lblFile.Location = new System.Drawing.Point(9, 29);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(70, 16);
            this.lblFile.TabIndex = 0;
            this.lblFile.Text = "Filename :";
            // 
            // txtFilename
            // 
            this.txtFilename.BackColor = System.Drawing.Color.White;
            this.txtFilename.Location = new System.Drawing.Point(89, 29);
            this.txtFilename.Name = "txtFilename";
            this.txtFilename.ReadOnly = true;
            this.txtFilename.Size = new System.Drawing.Size(317, 22);
            this.txtFilename.TabIndex = 1;
            this.txtFilename.TextChanged += new System.EventHandler(this.txtFilename_TextChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(412, 29);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(33, 24);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnParse
            // 
            this.btnParse.Enabled = false;
            this.btnParse.Location = new System.Drawing.Point(471, 25);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(133, 33);
            this.btnParse.TabIndex = 3;
            this.btnParse.Text = "&Parse";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
            // 
            // dgvSessionInfo
            // 
            this.dgvSessionInfo.AllowDrop = true;
            this.dgvSessionInfo.AllowUserToAddRows = false;
            this.dgvSessionInfo.AllowUserToDeleteRows = false;
            this.dgvSessionInfo.AllowUserToOrderColumns = true;
            this.dgvSessionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSessionInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSessionInfo.Location = new System.Drawing.Point(12, 79);
            this.dgvSessionInfo.Name = "dgvSessionInfo";
            this.dgvSessionInfo.ReadOnly = true;
            this.dgvSessionInfo.RowTemplate.Height = 24;
            this.dgvSessionInfo.Size = new System.Drawing.Size(1235, 782);
            this.dgvSessionInfo.TabIndex = 4;
            this.dgvSessionInfo.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvSessionInfo_DragDrop);
            this.dgvSessionInfo.DragEnter += new System.Windows.Forms.DragEventHandler(this.dgvSessionInfo_DragEnter);
            this.dgvSessionInfo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvSessionInfo_MouseDown);
            // 
            // btnShowFilter
            // 
            this.btnShowFilter.Enabled = false;
            this.btnShowFilter.Location = new System.Drawing.Point(621, 25);
            this.btnShowFilter.Name = "btnShowFilter";
            this.btnShowFilter.Size = new System.Drawing.Size(133, 33);
            this.btnShowFilter.TabIndex = 5;
            this.btnShowFilter.Text = "&Show Filter";
            this.btnShowFilter.UseVisualStyleBackColor = true;
            this.btnShowFilter.Click += new System.EventHandler(this.btnShowFilter_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 873);
            this.Controls.Add(this.btnShowFilter);
            this.Controls.Add(this.dgvSessionInfo);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtFilename);
            this.Controls.Add(this.lblFile);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Essbase Log Parser";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSessionInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog ofdEssLog;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.TextBox txtFilename;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.DataGridView dgvSessionInfo;
        private System.Windows.Forms.Button btnShowFilter;
    }
}


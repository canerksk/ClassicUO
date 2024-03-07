namespace ClassicUO
{
    partial class Launcher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Launcher));
            listBox1 = new System.Windows.Forms.ListBox();
            progressBar1 = new System.Windows.Forms.ProgressBar();
            UpdaterBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            statusLabel = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new System.Drawing.Point(12, 12);
            listBox1.Name = "listBox1";
            listBox1.Size = new System.Drawing.Size(282, 244);
            listBox1.TabIndex = 0;
            // 
            // progressBar1
            // 
            progressBar1.Location = new System.Drawing.Point(12, 296);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new System.Drawing.Size(282, 10);
            progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            progressBar1.TabIndex = 1;
            // 
            // statusLabel
            // 
            statusLabel.Location = new System.Drawing.Point(12, 259);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new System.Drawing.Size(282, 34);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "-";
            // 
            // Launcher
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(306, 317);
            Controls.Add(statusLabel);
            Controls.Add(progressBar1);
            Controls.Add(listBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Launcher";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            FormClosed += Launcher_FormClosed;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker UpdaterBackgroundWorker;
        private System.Windows.Forms.Label statusLabel;
    }
}
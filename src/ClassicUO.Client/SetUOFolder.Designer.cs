namespace ClassicUO
{
    partial class SetUOFolder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetUOFolder));
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            textBox1 = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            button1 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // textBox1
            // 
            textBox1.Enabled = false;
            textBox1.Location = new System.Drawing.Point(12, 34);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(409, 23);
            textBox1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 16);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(409, 15);
            label1.TabIndex = 1;
            label1.Text = "Gözat'ı tıklayarak Ultima Online klasörü içerisindeki UO.exe dosyasını seçiniz.";
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(427, 34);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(57, 23);
            button1.TabIndex = 2;
            button1.Text = "Gözat";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // SetUOFolder
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(496, 70);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(textBox1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "SetUOFolder";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "SetUOFolder";
            FormClosed += SetUOFolder_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}
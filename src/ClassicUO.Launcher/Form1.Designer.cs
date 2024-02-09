namespace ClassicUO.Launcher
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            startButton = new Button();
            listBox1 = new ListBox();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            uoListComboBox = new ComboBox();
            displaydriverListComboBox = new ComboBox();
            highdpi_checkBox = new CheckBox();
            label2 = new Label();
            label1 = new Label();
            ResponseLabel = new Label();
            progressBar1 = new ProgressBar();
            UpdaterBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // startButton
            // 
            startButton.Location = new Point(5, 315);
            startButton.Name = "startButton";
            startButton.Size = new Size(389, 42);
            startButton.TabIndex = 0;
            startButton.Text = "Başlat";
            startButton.UseVisualStyleBackColor = true;
            startButton.Visible = false;
            startButton.Click += startButton_Click;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(6, 6);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(369, 184);
            listBox1.TabIndex = 1;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(5, 5);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(389, 223);
            tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(listBox1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(381, 195);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Giriş";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(uoListComboBox);
            tabPage2.Controls.Add(displaydriverListComboBox);
            tabPage2.Controls.Add(highdpi_checkBox);
            tabPage2.Controls.Add(label2);
            tabPage2.Controls.Add(label1);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(381, 195);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Ayarlar";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // uoListComboBox
            // 
            uoListComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            uoListComboBox.FormattingEnabled = true;
            uoListComboBox.Location = new Point(6, 30);
            uoListComboBox.Name = "uoListComboBox";
            uoListComboBox.Size = new Size(364, 23);
            uoListComboBox.TabIndex = 5;
            uoListComboBox.SelectedIndexChanged += uoListComboBox_SelectedIndexChanged;
            uoListComboBox.SelectionChangeCommitted += uoListComboBox_SelectionChangeCommitted;
            // 
            // displaydriverListComboBox
            // 
            displaydriverListComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            displaydriverListComboBox.FormattingEnabled = true;
            displaydriverListComboBox.Items.AddRange(new object[] { "Default", "OpenGL", "Vulkan" });
            displaydriverListComboBox.Location = new Point(7, 86);
            displaydriverListComboBox.Name = "displaydriverListComboBox";
            displaydriverListComboBox.Size = new Size(364, 23);
            displaydriverListComboBox.TabIndex = 4;
            displaydriverListComboBox.SelectedIndexChanged += displaydriverListComboBox_SelectedIndexChanged;
            displaydriverListComboBox.SelectionChangeCommitted += displaydriverListComboBox_SelectionChangeCommitted;
            // 
            // highdpi_checkBox
            // 
            highdpi_checkBox.AutoSize = true;
            highdpi_checkBox.Location = new Point(6, 126);
            highdpi_checkBox.Name = "highdpi_checkBox";
            highdpi_checkBox.Size = new Size(127, 19);
            highdpi_checkBox.TabIndex = 3;
            highdpi_checkBox.Text = "Yüksek Çözünürlük";
            highdpi_checkBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(7, 68);
            label2.Name = "label2";
            label2.Size = new Size(103, 15);
            label2.TabIndex = 1;
            label2.Text = "Görüntü Arabirimi";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 12);
            label1.Name = "label1";
            label1.Size = new Size(112, 15);
            label1.TabIndex = 0;
            label1.Text = "Ultima Online Dizini";
            // 
            // ResponseLabel
            // 
            ResponseLabel.ForeColor = Color.Black;
            ResponseLabel.Location = new Point(5, 238);
            ResponseLabel.Name = "ResponseLabel";
            ResponseLabel.Size = new Size(389, 40);
            ResponseLabel.TabIndex = 6;
            ResponseLabel.Text = ":";
            ResponseLabel.Visible = false;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(5, 286);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(389, 23);
            progressBar1.TabIndex = 4;
            // 
            // UpdaterBackgroundWorker
            // 
            UpdaterBackgroundWorker.WorkerReportsProgress = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(397, 364);
            Controls.Add(ResponseLabel);
            Controls.Add(progressBar1);
            Controls.Add(tabControl1);
            Controls.Add(startButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Launcher";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button startButton;
        private ListBox listBox1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label2;
        private Label label1;
        private ProgressBar progressBar1;
        private ComboBox uoListComboBox;
        private ComboBox displaydriverListComboBox;
        private CheckBox highdpi_checkBox;
        private Label ResponseLabel;
        private System.ComponentModel.BackgroundWorker UpdaterBackgroundWorker;
    }
}

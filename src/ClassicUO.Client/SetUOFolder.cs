using ClassicUO.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;

namespace ClassicUO
{
    public partial class SetUOFolder : Form
    {
        public SetUOFolder()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.CheckFileExists = true;
            fileDialog.Multiselect = false;
            fileDialog.Filter = "UO.exe|UO.exe";
            fileDialog.Title = "UO Dizinini seçiniz";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = fileDialog.FileName;
                string folderPath = Path.GetDirectoryName(filePath);
                textBox1.Text = folderPath;
                Settings.GlobalSettings.UltimaOnlineDirectory = folderPath;
                Settings.GlobalSettings.Save();
                Mythic launcherForm = new Mythic();
                launcherForm.Show();
                Hide();
            }
            else
            {
                MessageBox.Show("Ultima Online dosya yolu seçilmedi!\nSisteminizde kurulu Ultima Online sürümü bulunmuyorsa oyuna giriş yapılmamaktadır. Lütfen bir Ultima Online sürümü yükleyiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void SetUOFolder_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}

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
                Launcher launcherForm = new Launcher();
                launcherForm.Show();
                Hide();
            }
            else
            {
                MessageBox.Show("Dosya seçilmedi!");
            }

        }

        private void SetUOFolder_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}

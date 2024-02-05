using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassicUO.Updater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            LaunchArguments la = new LaunchArguments();

            // append the classicuo path location and version [optionally if you run cuo directly from the UO folder]
            //la.Append("cuopath", "C:\\UO");
            //la.Append("clientversion", "0.0.0.0");

            // IP & port
            //la.Append("ip", "127.0.0.1");
            //la.Append("starthash", "2593");

            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixTimeInMsBase = now.ToUnixTimeMilliseconds();
            long StartHashInterval1 = unixTimeInMsBase - 20;
            long StartHashInterval2 = unixTimeInMsBase + 20;

            la.Append("starthash", unixTimeInMsBase.ToString());


            string cuopath = Application.StartupPath;
            string args = la.ToString();

            bool is_unix = Environment.OSVersion.Platform != PlatformID.Win32NT &&
                             Environment.OSVersion.Platform != PlatformID.Win32Windows &&
                             Environment.OSVersion.Platform != PlatformID.Win32S &&
                             Environment.OSVersion.Platform != PlatformID.WinCE;

            string cuo_to_run = "cuo";
            if (!is_unix)
            {
                cuo_to_run += ".dll";
            }


            //MessageBox.Show(cuopath + "/" + cuo_to_run);


            Process.Start(new ProcessStartInfo()
            {
                WorkingDirectory = cuopath, // classicuo path
                Arguments = args, // classicuo startup args
                CreateNoWindow = false, // run cuo in another indipendent window
                FileName = Path.Combine(cuopath, cuo_to_run) // classicuo path + cuo name
            });


        }


    }




    sealed class LaunchArguments
    {
        private readonly Dictionary<string, string> _args = new Dictionary<string, string>();

        // key does not need of '-'
        public void Append(string key, string value)
        {
            _args[key] = value;
        }

        // serialize all arguments
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> k in _args)
            {
                sb.AppendFormat("-{0} \"{1}\"", k.Key, k.Value);
                sb.Append(" ");
            }
            return sb.ToString();
        }

        public void Clear()
        {
            _args.Clear();
        }
    }




}

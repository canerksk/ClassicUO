using System.Net;

namespace ClassicUO.Launcher
{
    internal static class Program
    {

        private static Mutex mutex = new Mutex(true, "ClassicUO.Launcher");

        [STAThread]
        static void Main()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ApplicationConfiguration.Initialize();

            if (!mutex.WaitOne(TimeSpan.Zero, false))
            {
                MessageBox.Show("Launcher zaten çalýþýyor.", "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Application.Run(new Form1());
        }
    }
}
using AutoUpdaterDotNET;
using ClassicUO.Game;
using ClassicUO.Utility;
using ClassicUO.Utility.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ClassicUO
{
    public partial class Launcher : Form
    {
        public static UpdateStates UpdateStateCurrent = UpdateStates.None;

        IPluginHost pluginHost;

        public Launcher()
        {
            CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

#if RELEASE
            #region App Update Trigger
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.ReportErrors = true;
            AutoUpdater.Start(Game.Constants.AUTOUPDATER_XML_URL);
            AutoUpdater.TopMost = true;
            AutoUpdater.Mandatory = true;
            AutoUpdater.UpdateMode = Mode.Forced;
            AutoUpdater.RunUpdateAsAdmin = false;
            AutoUpdater.InstalledVersion = new Version(Bootstrap.Fvi.FileVersion);
            AutoUpdater.ShowRemindLaterButton = false;
            AutoUpdater.InstallationPath = Application.StartupPath;
            #endregion
#else
            Client.Run(pluginHost);
#endif

            Text = Constants.SERVNAME_LONG + " Güncelleme";

        }

        public void StatusLabelUpdate(string responseText = "", Color responseTextForeColor = default(Color))
        {
            listBox1.Items.Add(responseText);
            statusLabel.Visible = !string.IsNullOrEmpty(responseText);
            statusLabel.ForeColor = responseTextForeColor != default(Color) ? responseTextForeColor : Color.Black;
            statusLabel.Text = responseText;

            listBox1.TopIndex = listBox1.Items.Count - 1;
        }

        public void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    DialogResult dialogResult;
                    if (args.Mandatory.Value)
                    {
                        StatusLabelUpdate("Yeni sürüm bulundu!", Color.Green);

                        dialogResult =
                            MessageBox.Show(
                                $@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion}. This is required update. Press Ok to begin updating the application.", @"Güncelleme",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                    }
                    else
                    {
                        StatusLabelUpdate("Yeni sürüm bulundu!", Color.Green);

                        dialogResult =
                            MessageBox.Show(
                                $@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion}. Do you want to update the application now?", @"Güncelleme",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);
                    }

                    // Uncomment the following line if you want to show standard update dialog instead.
                    // AutoUpdater.ShowUpdateForm(args);

                    if (dialogResult.Equals(DialogResult.Yes) || dialogResult.Equals(DialogResult.OK))
                    {
                        try
                        {
                            if (AutoUpdater.DownloadUpdate(args))
                            {
                                Application.Exit();
                            }
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {

#if RELEASE
                    #region Mul Update Trigger
                    UpdaterBackgroundWorker.WorkerReportsProgress = true;
                    //UpdateStateCurrent = UpdateStates.Ready;
                    UpdaterBackgroundWorker.DoWork += new DoWorkEventHandler(UpdateTheClient);
                    UpdaterBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UpdateTheClientCompleted);
                    UpdaterBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(UpdateTheClientProgressChanged);
                    StatusLabelUpdate("Güncelleme kontrol ediliyor...", Color.Black);

                    if (UpdaterBackgroundWorker.IsBusy && UpdateStateCurrent != UpdateStates.Ready)
                    {
                        StatusLabelUpdate("Güncelleme iş parçası şu an meşgul!", Color.Black);
                        return;
                    }
                    else
                    {
                        UpdateStateCurrent = UpdateStates.Updating;
                        UpdaterBackgroundWorker.RunWorkerAsync();
                    }
                    #endregion
#endif

                    StatusLabelUpdate("Uygulama güncel.", Color.Green);
                }
            }
            else
            {
                if (args.Error is WebException)
                {
                    MessageBox.Show(@"There is a problem reaching update server. Please check your internet connection and try again later.", @"Güncelleme", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(args.Error.Message, args.Error.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        public static string HextoString(string InputText)
        {
            byte[] bb = Enumerable.Range(0, InputText.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(InputText.Substring(x, 2), 16)).ToArray();
            return Encoding.ASCII.GetString(bb);
        }

        #region MulUpdate
        public enum UpdateDurum
        {
            Bilgi,
            Uyari,
            Hata
        }

        public enum UpdateStates
        {
            None,
            Ready,
            Updating,
            Downloading,
            FinishedDownloading,
            Error
        }

        public UpdateStates UpdateState
        {
            get { return UpdateStateCurrent; }
            set
            {
                UpdateStateCurrent = value;

                switch (UpdateStateCurrent)
                {
                    case UpdateStates.Ready:

#if RELEASE
                        try
                        {
                            string currenthash = Crc32.Crc32Hesapla(Bootstrap.ExePath);
                            if (string.IsNullOrEmpty(currenthash))
                            {
                                Client.ShowErrorMessage("Uygulama bilgisi alunamadı!");
                                Process.GetCurrentProcess().Kill();
                                return;
                            }
                            byte[] diffhex = Encoding.ASCII.GetBytes(new WebClient().DownloadString(new Uri(Constants.WEB_MAIN_URL + "client-dat/" + currenthash)));
                            
                            string diffhexstring = Encoding.ASCII.GetString(diffhex);
                            string currhexstring = Launcher.HextoString(currenthash);

                            Console.WriteLine("diffhexstring: " + Encoding.ASCII.GetString(diffhex).Trim());
                            Console.WriteLine("currhexstring: " + currhexstring.Trim());

                            if (Encoding.ASCII.GetString(diffhex).Trim() != currhexstring.Trim())
                            {
                                Client.ShowErrorMessage("Client doğrulaması hatalı!");
                                Application.Exit();
                                Process.GetCurrentProcess().Kill();
                                return;
                            }
                        }
                        catch (WebException we)
                        {
                            Client.ShowErrorMessage("Uzak sunucu ile doğrulama yapılamadı!");
                            Console.WriteLine(we.Message + "\n" + we.Status.ToString());
                            Log.Error(we.Message + "\n" + we.Status.ToString());
                            Process.GetCurrentProcess().Kill();
                            return;

                        }
                        catch (NotSupportedException ne)
                        {
                            Client.ShowErrorMessage("Uzak sunucu ile doğrulama yapılamadı!");
                            Console.WriteLine(ne.Message);
                            Log.Error(ne.Message);
                            Process.GetCurrentProcess().Kill();
                            return;
                        }
#endif
                        StatusLabelUpdate("Hazır!", Color.Green);
                        Close();
                        Client.Run(pluginHost);
                        //this.Close();
                        //this.Hide();
                        break;

                    case UpdateStates.Updating:
                        StatusLabelUpdate("Güncelleniyor...", Color.Green);
                        break;

                    case UpdateStates.Downloading:
                        StatusLabelUpdate("İndiriliyor...", Color.Green);
                        break;

                    case UpdateStates.FinishedDownloading:
                        StatusLabelUpdate("İndirme tamamlandı.", Color.Green);
                        break;

                    case UpdateStates.Error:
                        StatusLabelUpdate("Güncelleme başarısız!", Color.Red);
                        break;

                    case UpdateStates.None:
                        StatusLabelUpdate("Güncelleme yok.", Color.Black);
                        break;

                    default:
                        break;
                }
            }
        }

        public class ReportStatus
        {
            public string Message;
            public UpdateDurum Durum;

            public ReportStatus(UpdateDurum durum, string message)
            {
                Durum = durum;
                Message = message;
            }
        }

        public void UpdateTheClient(object sender, DoWorkEventArgs e)
        {
            try
            {
                StatusLabelUpdate("Güncelleme bilgisi getiriliyor...", Color.Black);

                ((BackgroundWorker)sender).ReportProgress(1, new ReportStatus(UpdateDurum.Bilgi, "Güncelleme bilgisi getiriliyor"));
                string updatesListRaw = DownloadUpdatesListRaw();

                if (updatesListRaw == null)
                {
                    ((BackgroundWorker)sender).ReportProgress(0, new ReportStatus(UpdateDurum.Hata, "Güncelleme bilgisi okunamadı"));
                    return;
                }

                foreach (var line in updatesListRaw.Split('\n'))
                {
                    if (line.Trim().Length == 0)
                        continue;

                    string[] fileInfoReceived = line.Trim().Split(',');
                    Dictionary<string, string> remoteFileInfo = new Dictionary<string, string>();

                    remoteFileInfo.Add("filename", fileInfoReceived[0]);
                    remoteFileInfo.Add("size", fileInfoReceived[1]);
                    remoteFileInfo.Add("crc32b", fileInfoReceived[2].ToLower(CultureInfo.InvariantCulture));
                    remoteFileInfo.Add("local_filename", GetLocalFileName(remoteFileInfo["filename"]));

                    ((BackgroundWorker)sender).ReportProgress(1, new ReportStatus(UpdateDurum.Bilgi, "Kontrol Edilen Dosya:" + Path.GetFileName(remoteFileInfo["local_filename"])));

                    if (IsLocalFileNeedsUpdating(remoteFileInfo))
                    {

                        StatusLabelUpdate(remoteFileInfo["filename"] + " dosyasının güncellenmesi gerekiyor...", Color.Black);

                        UpdateState = UpdateStates.Downloading;

                        ((BackgroundWorker)sender).ReportProgress(1, remoteFileInfo);
                        while (UpdateState == UpdateStates.Downloading)
                        {
                            Thread.Sleep(150);
                        }

                        StatusLabelUpdate("İndirilen dosya: " + remoteFileInfo["filename"], Color.Black);

                        if (File.Exists(remoteFileInfo["local_filename"]))
                        {
                            StatusLabelUpdate("Eski dosya siliniyor: " + remoteFileInfo["local_filename"], Color.Black);
                            File.Delete(remoteFileInfo["local_filename"]);
                        }

                        //var local_downloaded_filename = "/";
                        var local_downloaded_filename = Path.Combine(Application.StartupPath, remoteFileInfo["filename"]);

                        //Console.WriteLine("renaming {0} -> {1}", local_downloaded_filename + ".part", local_downloaded_filename);
                        File.Move(local_downloaded_filename + ".part", local_downloaded_filename);

                        if (isZip(remoteFileInfo["filename"]))
                        {
                            //listBox1.Items.Add("Dosya çıkartılıyor: " + remoteFileInfo["filename"]);
                            unzipFile(local_downloaded_filename);
                            File.Delete(local_downloaded_filename);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                StatusLabelUpdate("Hata: " + ex.Message, Color.Red);
                ((BackgroundWorker)sender).ReportProgress(0, new ReportStatus(UpdateDurum.Hata, "Hata Oluştu: " + ex.Message));
            }
        }

        public void UpdateTheClientCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (UpdateState != UpdateStates.Error)
            {
                UpdateState = UpdateStates.Ready;
            }
        }

        public void UpdateTheClientProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is ReportStatus durum)
            {
                switch (durum.Durum)
                {
                    case UpdateDurum.Bilgi:
                        break;
                    case UpdateDurum.Uyari:
                        break;
                    case UpdateDurum.Hata:
                        UpdateState = UpdateStates.Error;
                        break;
                    default:
                        break;
                }
            }
            else if (e.UserState is IDictionary)
            {
                var remoteFileInfo = (Dictionary<string, string>)e.UserState;
                //Console.WriteLine("Downloading {0}", remoteFileInfo["filename"]);
                downloadFile(remoteFileInfo["filename"]);

            }
        }

        public string DownloadUpdatesListRaw()
        {
            var client = new WebClient();

            try
            {
                string ret = client.DownloadString(new Uri(Game.Constants.WEB_UPDATER_PATH_URL, "list_updates.php"));
                return ret;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.Status.ToString());
            }
            return null;
        }

        public static string GetLocalFileName(string remote_filename)
        {
            string local_filename = null;

            if (Path.GetExtension(remote_filename) == ".zip") // remove .zip extention
            {
                local_filename = remote_filename.Substring(0, remote_filename.Length - 4);
            }
            local_filename = Path.Combine(Application.StartupPath, local_filename);

            return local_filename;
        }

        public bool IsLocalFileNeedsUpdating(Dictionary<string, string> remoteFileInfo)
        {

            string local_filename = remoteFileInfo["local_filename"];
            StatusLabelUpdate("Dosya kontrolü: " + Path.GetFileName(local_filename).ToLower(), Color.Black);

            if (!File.Exists(local_filename))
            {
                StatusLabelUpdate("Dosya bulunamadı: " + remoteFileInfo["filename"], Color.Red);
                return true;
            }

            var localfile_size = (new FileInfo(local_filename)).Length;

            if (localfile_size != int.Parse(remoteFileInfo["size"]))
            {
                StatusLabelUpdate("Dosya boyutu eşleşmedi: " + remoteFileInfo["size"], Color.Red);
                return true;
            }

            string local_crc32;
            try
            {
                local_crc32 = getFileCrc32(local_filename);
            }
            catch (IOException)
            {
                //StatusLabelUpdate(false, "Başka bir uygulama tarafından kullanıldığı için güncellenemedi: " + Path.GetFileName(local_filename), Color.Red);
                return false;
            }
            if ((remoteFileInfo["crc32b"] != local_crc32) && (remoteFileInfo["crc32b"] != local_crc32.Substring(1) || local_crc32[0] != '0'))
            {
                StatusLabelUpdate("Uyumsuz hash kodu: " + Path.GetFileName(local_filename), Color.Red);
                return true;
            }

            return false;
        }

        public void downloadFile(string remote_filename)
        {
            string local_path = Path.Combine(Application.StartupPath, remote_filename) + ".part";
            var client = new WebClient();
            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.DownloadFileCompleted += client_DownloadFileCompleted;
            client.DownloadFileAsync(new Uri(Game.Constants.WEB_UPDATER_PATH_URL, remote_filename), remote_filename + ".part");
        }

        public void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            UpdateState = UpdateStates.FinishedDownloading;
            return;
        }

        public void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = (int)((e.BytesReceived * 100) / e.TotalBytesToReceive);
        }

        #endregion


        #region ZipHangling

        public static bool isZip(string filename)
        {
            if (Path.GetExtension(filename).ToLower() == ".zip")
            {
                return true;
            }
            return false;
        }

        public static void unzipFile(string archive_name)
        {
            using (ZipArchive zip = ZipFile.OpenRead(archive_name))
            {
                string local_filename = null;

                if (Path.GetExtension(archive_name) == ".zip") // remove .zip extension
                {
                    local_filename = Path.GetFileNameWithoutExtension(archive_name);
                }

                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    string zippedFilename = Path.GetFileName(entry.FullName);

                    if (!zippedFilename.Equals(local_filename, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Eğer dosya adı uyummuyorsa, atla
                        continue;
                    }

                    string destinationPath = Path.Combine(Path.GetDirectoryName(archive_name), entry.FullName);

                    entry.ExtractToFile(destinationPath, overwrite: true);
                }
            }
        }
        #endregion


        #region Hasing
        public static string getFileCrc32(string filename)
        {
            return getFileCrc32(filename, null);
        }

        public static string getFileCrc32(string filename, BackgroundWorker sender)
        {
            Utility.Crc32 crc32 = new Utility.Crc32();
            string hash = string.Empty;

            using (FileStream fs = File.Open(filename, FileMode.Open))
                foreach (byte b in crc32.ComputeHash(fs)) hash += b.ToString("x2").ToLower();
            return hash.ToLower(CultureInfo.InvariantCulture);
        }

        #endregion

        private void Launcher_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}

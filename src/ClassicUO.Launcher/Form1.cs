
using System.Diagnostics;
using System.Net;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using ClassicUO.IO;
using ClassicUO.Configuration;
using AutoUpdaterDotNET;
using System.IO.Compression;
using System.Text.Json;
using System.Reflection;

namespace ClassicUO.Launcher
{
    public partial class Form1 : Form
    {

        public static string ClassicUOSettingFile = Path.Combine(Application.StartupPath, "settings.json");
        public static string ClassicUOPath = Path.Combine(Application.StartupPath, "cuo.exe");

        public static UpdateStates UpdateStateCurrent = UpdateStates.None;
        public static readonly FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);


        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            YarimInenDosyalariSil();
            #region App Update Trigger
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.ReportErrors = true;
            AutoUpdater.Start(Game.Constants.AUTOUPDATER_XML_URL);
            AutoUpdater.TopMost = true;
            AutoUpdater.Mandatory = true;
            AutoUpdater.UpdateMode = Mode.Forced;
            AutoUpdater.RunUpdateAsAdmin = false;
            AutoUpdater.InstalledVersion = new Version(fvi.FileVersion);
            AutoUpdater.ShowRemindLaterButton = false;
            AutoUpdater.InstallationPath = Application.StartupPath;
            #endregion


        }


        // Element Events
        private void Form1_Load(object sender, EventArgs e)
        {

            // Kurulu uo sürümlerini listele
            uoListComboBox.Items.Clear();
            for (int i = UOFileUltimaDLL.knownRegkeys.Length - 1; i >= 1; i--)
            {
                string exePath;
                if (Environment.Is64BitOperatingSystem)
                    exePath = UOFileUltimaDLL.GetPath(string.Format(@"Wow6432Node\{0}", UOFileUltimaDLL.knownRegkeys[i]));
                else
                    exePath = UOFileUltimaDLL.GetPath(UOFileUltimaDLL.knownRegkeys[i]);
                if (exePath != null)
                {
                    uoListComboBox.Items.Add(exePath);
                }
            }
            if (!string.IsNullOrEmpty(GetClientSettingFileContent("ultimaonlinedirectory")))
            {
                if (Directory.Exists(GetClientSettingFileContent("ultimaonlinedirectory")) || File.Exists(Path.Combine(GetClientSettingFileContent("ultimaonlinedirectory"), "tiledata.mul")))
                {
                    int indexUO = uoListComboBox.Items.IndexOf(GetClientSettingFileContent("ultimaonlinedirectory"));
                    if (indexUO != -1)
                    {
                        uoListComboBox.SelectedIndex = indexUO;
                    }
                }
            }

            if (Settings.GlobalSettings.ForceDriver == 0)
            {
                displaydriverListComboBox.SelectedIndex = 0;
            }
            else if (Settings.GlobalSettings.ForceDriver == 1)
            {
                displaydriverListComboBox.SelectedIndex = 1;
            }
            else if (Settings.GlobalSettings.ForceDriver == 2)
            {
                displaydriverListComboBox.SelectedIndex = 2;
            }
            else
            {
                displaydriverListComboBox.SelectedIndex = 0;
            }

            //
            if (CUOEnviroment.IsHighDPI)
            {
                highdpi_checkBox.Checked = true;
            }
            if (!File.Exists(ClassicUOSettingFile))
            {
                StartButtonStateSet(false, "Ayarlar dosyasý bulunamadý! (settings.json)", Color.Red);
                return;
            }
            if (!File.Exists(ClassicUOPath))
            {
                StartButtonStateSet(false, "Client bulunamadý!", Color.Red);
                return;
            }
            if (uoListComboBox.Items.Count <= 0)
            {
                StartButtonStateSet(false, "Sisteminizde kurulu Ultima Online bulunamadý!", Color.Red);
                return;
            }

            //if (uoListComboBox.Items.Count > 0)
            //{
            //StartButtonStateSet(true, "Sisteminizde kurulu toplam " + uoListComboBox.Items.Count + " adet Ultima Online bulundu!", Color.Green);
            // if (uoListComboBox.Text != Settings.GlobalSettings.UltimaOnlineDirectory)
            //{
            //uoListComboBox.SelectedIndex = 0;
            //}
            //if (uoListComboBox.SelectedIndex <= 0)
            ///{
            //uoListComboBox.SelectedIndex = 0;
            //}
            //}
            //else
            //{
            //StartButtonStateSet(false, "Sisteminizde kurulu Ultima Online bulunamadý!", Color.Red);
            //if (uoListComboBox.SelectedIndex <= 0)
            //{
            //uoListComboBox.SelectedIndex = -1;
            //}
            //return;
            //}



        }


        private string? GetClientSettingFileContent(string key)
        {
            string? value = null;
            if (File.Exists(ClassicUOSettingFile))
            {
                // settings.json içindeki deðerini oku
                string SettingJsonFileReadAllText = File.ReadAllText(ClassicUOSettingFile);
                using (var jsonDocument = JsonDocument.Parse(SettingJsonFileReadAllText))
                {
                    value = jsonDocument.RootElement.GetProperty(key).GetString();
                }
            }
            return value;
        }


        private void startButton_Click(object sender, EventArgs e)
        {
            if (!File.Exists(ClassicUOSettingFile))
            {
                StartButtonStateSet(false, "Ayarlar dosyasý bulunamadý! (settings.json)", Color.Red);
                return;
            }
            if (!File.Exists(ClassicUOPath))
            {
                StartButtonStateSet(false, "Client bulunamadý!", Color.Red);
                return;
            }
            if (uoListComboBox.Items.Count <= 0)
            {
                StartButtonStateSet(false, "Sisteminizde kurulu Ultima Online bulunamadý!", Color.Red);
                return;
            }

            if (!string.IsNullOrEmpty(GetClientSettingFileContent("ultimaonlinedirectory")))
            {
                if (!Directory.Exists(GetClientSettingFileContent("ultimaonlinedirectory")) || !File.Exists(Path.Combine(GetClientSettingFileContent("ultimaonlinedirectory"), "tiledata.mul")))
                {
                    StartButtonStateSet(false, "Ultima Online dizini geçerli deðil.", Color.Red);
                    uoListComboBox.SelectedIndex = -1;
                    tabControl1.SelectedIndex = 1;
                    return;
                }
            }
            else
            {
                StartButtonStateSet(false, "Ultima Online dizini ayarlanmamýþ.", Color.Red);
                uoListComboBox.SelectedIndex = -1;
                tabControl1.SelectedIndex = 1;
                return;
            }



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
            //la.Append("highdpi", highdpi_checkBox.Checked ? "1" : "0");

            string cuopath = Application.StartupPath;
            string args = la.ToString();

            bool is_unix = Environment.OSVersion.Platform != PlatformID.Win32NT &&
                             Environment.OSVersion.Platform != PlatformID.Win32Windows &&
                             Environment.OSVersion.Platform != PlatformID.Win32S &&
                             Environment.OSVersion.Platform != PlatformID.WinCE;

            string cuo_to_run = "cuo";
            if (!is_unix)
            {
                cuo_to_run += ".exe";
            }

            Process.Start(new ProcessStartInfo()
            {
                UseShellExecute = true,
                WorkingDirectory = cuopath, // classicuo path
                Arguments = args, // classicuo startup args
                Verb = "runas", // run as administrator
                CreateNoWindow = false, // run cuo in another indipendent window
                FileName = Path.Combine(cuopath, cuo_to_run) // classicuo path + cuo name
            });

            this.WindowState = FormWindowState.Minimized;
            //this.Close();

        }

        private void uoListComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selectedText = uoListComboBox.Text; // Seçili öðenin metnini alýr
            int selectedIndex = uoListComboBox.SelectedIndex; // Seçili öðenin dizinini alýr

            if (!Directory.Exists(selectedText))
            {
                StartButtonStateSet(false, "Klasör bulunamadý.", Color.Red);
                return;
            }
            var uoexepath = Path.Combine(selectedText, "UO.exe");
            if (!File.Exists(uoexepath))
            {
                StartButtonStateSet(false, "Seçilen klasör bir Ultima Online klasörü deðil.", Color.Red);
                return;
            }

            if (selectedIndex >= 0)
            {
                StartButtonStateSet(true, "Ultima Online dizini baþarýyla seçildi:" + selectedText, Color.Green);
                if (selectedText != Settings.GlobalSettings.UltimaOnlineDirectory)
                {
                    Settings.GlobalSettings.UltimaOnlineDirectory = selectedText;
                    Settings.GlobalSettings.Save();
                }
            }

        }


        private void uoListComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {



        }

        private void displaydriverListComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        private void displaydriverListComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selectedText = displaydriverListComboBox.Text; // Seçili öðenin metnini alýr
            int selectedIndex = displaydriverListComboBox.SelectedIndex; // Seçili öðenin dizinini alýr

            if (selectedIndex == 0)
            {
                Settings.GlobalSettings.ForceDriver = 0;
            }
            else if (selectedIndex == 1)
            {
                Settings.GlobalSettings.ForceDriver = 1;
            }
            else if (selectedIndex == 2)
            {
                Settings.GlobalSettings.ForceDriver = 2;
            }
            else
            {
                Settings.GlobalSettings.ForceDriver = 0;
            }

            //Settings.GlobalSettings.Save();
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


        public void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    DialogResult dialogResult;
                    if (args.Mandatory.Value)
                    {
                        StartButtonStateSet(true, "Yeni sürüm bulundu!", Color.Green);

                        dialogResult =
                            MessageBox.Show(
                                $@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion}. This is required update. Press Ok to begin updating the application.", @"Güncelleme",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                    }
                    else
                    {
                        StartButtonStateSet(true, "Yeni sürüm bulundu!", Color.Green);

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

                    #region Mul Update Trigger
                    UpdaterBackgroundWorker.WorkerReportsProgress = true;
                    //UpdateStateCurrent = UpdateStates.Ready;
                    UpdaterBackgroundWorker.DoWork += new DoWorkEventHandler(UpdateTheClient);
                    UpdaterBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UpdateTheClientCompleted);
                    UpdaterBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(UpdateTheClientProgressChanged);
                    StartButtonStateSet(false, "Güncelleme kontrol ediliyor...", Color.Orange);

                    if (UpdaterBackgroundWorker.IsBusy && UpdateStateCurrent != UpdateStates.Ready)
                    {
                        StartButtonStateSet(false, "Güncelleme iþ parçacýðý þu an meþgul!", Color.Orange);
                        return;
                    }
                    else
                    {
                        UpdateStateCurrent = UpdateStates.Updating;
                        UpdaterBackgroundWorker.RunWorkerAsync();
                    }
                    #endregion


                    StartButtonStateSet(true, "Uygulama güncel.", Color.Green);
                    //MessageBox.Show(@"Uygulama güncel!", @"Güncelleme",  MessageBoxButtons.OK, MessageBoxIcon.Information);
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





        // Element Functions
        private void StartButtonStateSet(bool state, string responseText = "", Color responseTextForeColor = default(Color))
        {
            listBox1.Items.Add(responseText);

            ResponseLabel.Visible = !string.IsNullOrEmpty(responseText); // Cevap metni varsa etiketi görünür yapýn
            ResponseLabel.ForeColor = responseTextForeColor != default(Color) ? responseTextForeColor : Color.Black; // Cevap metni rengini belirleyin
            ResponseLabel.Text = responseText; // Cevap metnini ayarlayýn
            startButton.Enabled = state;
            startButton.Visible = state;
        }


        // Mul Güncelleme

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

                        StartButtonStateSet(true, "Hazýr!", Color.Green);
                        break;

                    case UpdateStates.Updating:
                        StartButtonStateSet(false, "Güncelleniyor...", Color.Orange);
                        break;

                    case UpdateStates.Downloading:
                        StartButtonStateSet(false, "Ýndiriliyor...", Color.Orange);
                        break;

                    case UpdateStates.FinishedDownloading:
                        StartButtonStateSet(false, "Ýndirme tamamlandý.", Color.Orange);
                        break;

                    case UpdateStates.Error:
                        StartButtonStateSet(false, "Güncelleme baþarýsýz!", Color.Red);
                        break;

                    case UpdateStates.None:
                        StartButtonStateSet(false, "Güncelleme yok.", Color.Orange);
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
                //StartButtonStateSet(false, "Güncelleme bilgisi getiriliyor...", Color.Black);

                ((BackgroundWorker)sender).ReportProgress(1, new ReportStatus(UpdateDurum.Bilgi, "Güncelleme bilgisi getiriliyor"));
                string updatesListRaw = DownloadUpdatesListRaw();

                if (updatesListRaw == null)
                {
                    ((BackgroundWorker)sender).ReportProgress(0, new ReportStatus(UpdateDurum.Hata, "Güncelleme bilgisi okunamadý"));
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

                        StartButtonStateSet(false, remoteFileInfo["filename"] + " dosyasýnýn güncellenmesi gerekiyor...", Color.Orange);

                        UpdateState = UpdateStates.Downloading;

                        ((BackgroundWorker)sender).ReportProgress(1, remoteFileInfo);
                        while (UpdateState == UpdateStates.Downloading)
                        {
                            Thread.Sleep(150);
                            //listBox1.Items.Add("Ýndirmenin bitmesi bekleniyor...");
                        }

                        //listBox1.Items.Add("Ýndirilen dosya: " + remoteFileInfo["filename"]);
                        //listBox1.SelectedIndex = listBox1.Items.Count - 1;
                        //listBox1.SelectedIndex = -1;

                        if (File.Exists(remoteFileInfo["local_filename"]))
                        {
                            //listBox1.Items.Add("Eski dosya siliniyor: " + remoteFileInfo["local_filename"]);
                            //listBox1.SelectedIndex = listBox1.Items.Count - 1;
                            //listBox1.SelectedIndex = -1;
                            File.Delete(remoteFileInfo["local_filename"]);
                        }

                        //var local_downloaded_filename = "/";
                        var local_downloaded_filename = Path.Combine(Application.StartupPath, remoteFileInfo["filename"]);

                        //Console.WriteLine("renaming {0} -> {1}", local_downloaded_filename + ".part", local_downloaded_filename);
                        File.Move(local_downloaded_filename + ".part", local_downloaded_filename);

                        if (isZip(remoteFileInfo["filename"]))
                        {
                            //listBox1.Items.Add("Dosya çýkartýlýyor: " + remoteFileInfo["filename"]);
                            unzipFile(local_downloaded_filename);
                            File.Delete(local_downloaded_filename);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                StartButtonStateSet(false, "Hata: " + ex.Message, Color.Red);
                //MessageBox.Show("Hata Oluþtu: " + ex.Message, "HATA", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                ((BackgroundWorker)sender).ReportProgress(0, new ReportStatus(UpdateDurum.Hata, "Hata Oluþtu: " + ex.Message));
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
            StartButtonStateSet(false, "Dosya kontrolü: " + Path.GetFileName(local_filename).ToLower(), Color.Black);

            if (!File.Exists(local_filename))
            {
                StartButtonStateSet(false, "Dosya bulunamadý: " + remoteFileInfo["filename"], Color.Red);
                return true;
            }

            var localfile_size = (new FileInfo(local_filename)).Length;

            if (localfile_size != int.Parse(remoteFileInfo["size"]))
            {
                StartButtonStateSet(false, "Dosya boyutu eþleþmedi: " + remoteFileInfo["size"], Color.Red);
                return true;
            }

            string local_crc32;
            try
            {
                local_crc32 = getFileCrc32(local_filename);
            }
            catch (IOException)
            {
                StartButtonStateSet(false, "Baþka bir uygulama tarafýndan kullanýldýðý için güncellenemedi: " + Path.GetFileName(local_filename), Color.Red);
                return false;
            }
            if ((remoteFileInfo["crc32b"] != local_crc32) && (remoteFileInfo["crc32b"] != local_crc32.Substring(1) || local_crc32[0] != '0'))
            {
                StartButtonStateSet(false, "Uyumsuz hash kodu: " + Path.GetFileName(local_filename), Color.Red);
                return true;
            }

            return false;
        }


        public void downloadFile(string remote_filename)
        {
            //Application.OpenForms.OfType<Azeroth>().FirstOrDefault().statusLabel.Text = remote_filename + " dosyasý indiriliyor...";

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
                        // Eðer dosya adý uyuþmuyorsa, atla
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



        public static void YarimInenDosyalariSil()
        {
            string ClientPath = Path.Combine(CUOEnviroment.ExecutablePath);

            if (Directory.Exists(ClientPath))
            {
                string[] DownloadedPartFiles = Directory.GetFiles(ClientPath, "*.part");
                string[] DownloadedMulZipFiles = Directory.GetFiles(ClientPath, "*.mul.zip");
                string[] DownloadedZipFiles = Directory.GetFiles(ClientPath, "*.zip");
                string[] DownloadedRarFiles = Directory.GetFiles(ClientPath, "*.rar");

                if (Directory.Exists(ClientPath))
                {
                    foreach (string partrarfile in DownloadedRarFiles)
                    {
                        if (File.Exists(partrarfile))
                            File.Delete(partrarfile);
                    }

                    foreach (string partzipfile in DownloadedZipFiles)
                    {
                        if (File.Exists(partzipfile))
                            File.Delete(partzipfile);
                    }

                    foreach (string partfile in DownloadedPartFiles)
                    {
                        if (File.Exists(partfile))
                            File.Delete(partfile);
                    }

                    foreach (string partmulzipfile in DownloadedMulZipFiles)
                    {
                        if (File.Exists(partmulzipfile))
                            File.Delete(partmulzipfile);
                    }

                }
            }
        }


    }


}

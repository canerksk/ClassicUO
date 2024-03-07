#region license

// Copyright (c) 2024, andreakarasho
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 3. All advertising materials mentioning features or use of this software
//    must display the following acknowledgement:
//    This product includes software developed by andreakarasho - https://github.com/andreakarasho
// 4. Neither the name of the copyright holder nor the
//    names of its contributors may be used to endorse or promote products
//    derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS ''AS IS'' AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using ClassicUO.Configuration;
using ClassicUO.Game;
using ClassicUO.Game.Managers;
using ClassicUO.IO;
using ClassicUO.Network;
using ClassicUO.Utility;
using ClassicUO.Utility.Logging;
using SDL2;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Data;
using System.Windows.Forms;
using ClassicUO.Assets;
using System.Net;
using System.Net.NetworkInformation;
using System.Xml.Serialization;
using System.Xml;
using System.Text.Json;
using System.Drawing;

namespace ClassicUO
{
    internal static class Bootstrap
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetDllDirectory(string lpPathName);

        public static string ExePath = Assembly.GetExecutingAssembly().Location;
        public static string RootDiskDrive = Path.GetPathRoot(Environment.SystemDirectory);
        public static string ArtMulPath = UOFileManager.GetUOFilePath("art.mul");
        public static string ArtMulHash;
        public static FileInfo ArtMulFileInfo ;
        public static string ArtMulBoyutu_Byte;
        public static string ClientHash;
        public static string CUOVersion = CUOEnviroment.Version.ToString();
        public static FileInfo ClientFileInfo;
        public static string ClientDosyaBoyutu_Byte;
        public static string HDDID = HardwareInfo.getUniqueID(RootDiskDrive);
        public static string PCID = HardwareInfo.GetHDDID();
        public static string CPUID = HardwareInfo.getCPUID();
        public static int SiteStatus = 0;
        public static FileVersionInfo Fvi = FileVersionInfo.GetVersionInfo(ExePath);

        [UnmanagedCallersOnly(EntryPoint = "Initialize", CallConvs = new Type[] { typeof(CallConvCdecl) })]
        static unsafe void Initialize(IntPtr* argv, int argc, HostBindings* hostSetup)
        {
            var args = new string[argc];
            for (int i = 0; i < argc; i++)
            {
                args[i] = Marshal.PtrToStringAnsi(argv[i]);
            }

            var host = new UnmanagedAssistantHost(hostSetup);
            Boot(host, args);
        }


        [STAThread]
        public static void Main(string[] args) => Boot(null, args);


        public static void Boot(UnmanagedAssistantHost pluginHost, string[] args)
        {
            YarimInenDosyalariSil();

            CheckBlocked(HDDID);

            /*
            // Blocked pc:start
            XmlSerializer serpc = new XmlSerializer(typeof(Blocker.PCList));
            XmlReader readarpclist = XmlReader.Create(Constants.BLOCKPC_XML_URL);
            Blocker.PCList itempc = (Blocker.PCList)serpc.Deserialize(readarpclist);
            foreach (var pcblock in itempc.Item)
            {
                if (pcblock.PCHash == HDDID)
                {
                    MessageBox.Show("Bu bilgisayar sunucudan engellenmiş.\nEngellenme nedeni: " + pcblock.Neden, "HATA", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Process.GetCurrentProcess().Kill();
                    return;
                }
            }
            readarpclist.Close();
            // Blocked pc:end
            */

            // Site bağlantısını kontrol Et
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Constants.WEB_MAIN_URL);
                req.Timeout = 5000; // İstek zaman aşımı süresi (ms) ayarlayabilirsiniz
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    SiteStatus = (int)response.StatusCode;
                }
            }
            catch (WebException)
            {
                SiteStatus = -1;
            }

#if RELEASE
            try
            {
                string clientdaturl = new WebClient().DownloadString(Constants.CLIENT_DAT_URL);
                byte[] diffhex = Encoding.ASCII.GetBytes(clientdaturl);

                var currenthash = Crc32.Crc32Hesapla(ExePath);
                var currenthex = HextoString(currenthash);
                // Console.WriteLine(Encoding.ASCII.GetString(diffhex));
                // Console.WriteLine(currenthex);

                if (Encoding.ASCII.GetString(diffhex) != currenthex)
                {
                    Client.ShowErrorMessage("Client doğrulaması hatalı!");
                    Process.GetCurrentProcess().Kill();
                    return;
                }
            }
            catch (WebException we)
            {
                Log.Error(we.Message + "\n" + we.Status.ToString());
            }
            catch (NotSupportedException ne)
            {
                Log.Error(ne.Message);
            }
#endif


            // multi check
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length >= 3)
            {
                Client.ShowErrorMessage("Bu uygulamayı üst üste en fazla 3 kere başlatabilirsiniz.");
                Process.GetCurrentProcess().Kill();
                return;
            }
            // name check
            if (Process.GetCurrentProcess().ProcessName != Assembly.GetExecutingAssembly().GetName().Name)
            {
                Client.ShowErrorMessage("Client bulunamadı!");
                Process.GetCurrentProcess().Kill();
                return;
            }
            if (Process.GetCurrentProcess().ProcessName != "cuo" && Assembly.GetExecutingAssembly().GetName().Name != "Mythic")
            {
                Client.ShowErrorMessage("Client bulunamadı!");
                Process.GetCurrentProcess().Kill();
                return;
            }
            // on process check
            bool isRunningInVirtualBox = CheckIfRunningInVirtualBox();
            if (isRunningInVirtualBox)
            {
                Client.ShowErrorMessage("Uygulama sanal makinede çalışıyor. Çalıştırma işlemi iptal ediliyor.");
                Process.GetCurrentProcess().Kill();
                return;
            }
            bool isRunningInSandboxie = CheckIfRunningInSandboxie();
            if (isRunningInSandboxie)
            {
                Client.ShowErrorMessage("Bu uygulama sanal uygulama üzerinde çalışamaz.");
                Process.GetCurrentProcess().Kill();
                return;
            }
            if (GetModuleHandle("SbieDll.dll").ToInt32() != 0)
            {
                Client.ShowErrorMessage("Bu uygulama sanal uygulama üzerinde çalışamaz.");
                Process.GetCurrentProcess().Kill();
                return;
            }

            // on process check
            Process[] ProcessList = Process.GetProcesses();
            foreach (Process proc in ProcessList)
            {
                if (proc.MainWindowTitle.Equals("The Wireshark Network Analyzer"))
                {
                    Client.ShowErrorMessage("Bu uygulama sanal uygulama üzerinde çalışamaz.");
                    Process.GetCurrentProcess().Kill();
                    return;
                }
                if (proc.MainWindowTitle.Equals("WPE PRO"))
                {
                    Client.ShowErrorMessage("Bu uygulama sanal uygulama üzerinde çalışamaz.");
                    Process.GetCurrentProcess().Kill();
                    return;
                }
            }

            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            Log.Start(LogTypes.All);

            CUOEnviroment.GameThread = Thread.CurrentThread;
            CUOEnviroment.GameThread.Name = "CUO_MAIN_THREAD";
#if !DEBUG
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendLine("######################## [START LOG] ########################");

#if DEV_BUILD
                sb.AppendLine($"Ultima Online [DEV_BUILD] - {CUOEnviroment.Version} - {DateTime.Now}");
#else
                sb.AppendLine($"Ultima Online [STANDARD_BUILD] - {CUOEnviroment.Version} - {DateTime.Now}");
#endif

                sb.AppendLine($"OS: {Environment.OSVersion.Platform} {(Environment.Is64BitOperatingSystem ? "x64" : "x86")}");

                sb.AppendLine($"Thread: {Thread.CurrentThread.Name}");
                sb.AppendLine();

                if (Settings.GlobalSettings != null)
                {
                    sb.AppendLine($"Shard: {Constants.SPHERE_IP}");
                    sb.AppendLine($"ClientVersion: {Constants.CLIENTVERSION}");
                    sb.AppendLine();
                }

                sb.AppendFormat("Exception:\n{0}\n", e.ExceptionObject);
                sb.AppendLine("######################## [END LOG] ########################");
                sb.AppendLine();
                sb.AppendLine();

                Log.Panic(e.ExceptionObject.ToString());
                string path = Path.Combine(CUOEnviroment.ExecutablePath, "Logs");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                using (LogFile crashfile = new LogFile(path, "crash.txt"))
                {
                    crashfile.WriteAsync(sb.ToString()).RunSynchronously();
                }
            };
#endif

            ReadSettingsFromArgs(args);

            if (CUOEnviroment.IsHighDPI)
            {
                Environment.SetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI", "1");
            }
            //Environment.SetEnvironmentVariable("FNA3D_FORCE_DRIVER", "OpenGL");

            // NOTE: this is a workaroud to fix d3d11 on windows 11 + scale windows
            Environment.SetEnvironmentVariable("FNA3D_D3D11_FORCE_BITBLT", "1");
            Environment.SetEnvironmentVariable("FNA3D_BACKBUFFER_SCALE_NEAREST", "1");
            Environment.SetEnvironmentVariable("FNA3D_OPENGL_FORCE_COMPATIBILITY_PROFILE", "1");
            Environment.SetEnvironmentVariable(SDL.SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + Path.Combine(CUOEnviroment.ExecutablePath, "Data", "Plugins"));
            string globalSettingsPath = Settings.GetSettingsFilepath();

            if (!Directory.Exists(Path.GetDirectoryName(globalSettingsPath)) || !File.Exists(globalSettingsPath))
            {
                // settings specified in path does not exists, make new one
                {
                    // TODO: 
                    Settings.GlobalSettings.Save();
                }
            }

            Settings.GlobalSettings = ConfigurationResolver.Load<Settings>(globalSettingsPath, SettingsJsonContext.RealDefault.Settings);
            CUOEnviroment.IsOutlands = Settings.GlobalSettings.ShardType == 2;

            ReadSettingsFromArgs(args);

            // still invalid, cannot load settings
            if (Settings.GlobalSettings == null)
            {
                Settings.GlobalSettings = new Settings();
                Settings.GlobalSettings.Save();
            }

            if (!CUOEnviroment.IsUnix)
            {
                string libsPath = Path.Combine(CUOEnviroment.ExecutablePath, Environment.Is64BitProcess ? "x64" : "x86");

                SetDllDirectory(libsPath);
            }

            if (string.IsNullOrWhiteSpace(Settings.GlobalSettings.Language))
            {
                Log.Trace("language is not set. Trying to get the OS language.");
                try
                {
                    Settings.GlobalSettings.Language = CultureInfo.InstalledUICulture.ThreeLetterWindowsLanguageName;

                    if (string.IsNullOrWhiteSpace(Settings.GlobalSettings.Language))
                    {
                        Log.Warn("cannot read the OS language. Rolled back to ENU");

                        Settings.GlobalSettings.Language = "ENU";
                    }

                    Log.Trace($"language set: '{Settings.GlobalSettings.Language}'");
                }
                catch
                {
                    Log.Warn("cannot read the OS language. Rolled back to ENU");

                    Settings.GlobalSettings.Language = "ENU";
                }
            }

            /*
            if (string.IsNullOrWhiteSpace(Settings.GlobalSettings.UltimaOnlineDirectory))
            {
                Settings.GlobalSettings.UltimaOnlineDirectory = CUOEnviroment.ExecutablePath;
            }
            */

            const uint INVALID_UO_DIRECTORY = 0x100;
            const uint INVALID_UO_VERSION = 0x200;

            uint flags = 0;

            if (!Directory.Exists(Settings.GlobalSettings.UltimaOnlineDirectory) || !File.Exists(Path.Combine(Settings.GlobalSettings.UltimaOnlineDirectory, "tiledata.mul")))
            {
                flags |= INVALID_UO_DIRECTORY;
            }

            string clientVersionText = Constants.CLIENTVERSION;
            if (!ClientVersionHelper.IsClientVersionValid(Constants.CLIENTVERSION, out ClientVersion clientVersion))
            {
                Log.Warn($"Client version [{clientVersionText}] is invalid, let's try to read the client.exe");

                // mmm something bad happened, try to load from client.exe [windows only]
                if (!ClientVersionHelper.TryParseFromFile(Path.Combine(Settings.GlobalSettings.UltimaOnlineDirectory, "client.exe"), out clientVersionText) || !ClientVersionHelper.IsClientVersionValid(clientVersionText, out clientVersion))
                {
                    Log.Error("Invalid client version: " + clientVersionText);
                    flags |= INVALID_UO_VERSION;
                }
                else
                {
                    Log.Trace($"Found a valid client.exe [{clientVersionText} - {clientVersion}]");
                }
            }

            if (flags != 0)
            {
                /*
                if ((flags & INVALID_UO_DIRECTORY) != 0)
                {
                    Client.ShowErrorMessage(ResGeneral.YourUODirectoryIsInvalid);
                }
                else if ((flags & INVALID_UO_VERSION) != 0)
                {
                    Client.ShowErrorMessage(ResGeneral.YourUOClientVersionIsInvalid);
                }
                */
                //SetUOFolder setuofolderForm = new SetUOFolder();
                //setuofolderForm.Show();
                Application.Run(new SetUOFolder());
            }
            else
            {
                switch (Settings.GlobalSettings.ForceDriver)
                {
                    case 1: // OpenGL
                        Environment.SetEnvironmentVariable("FNA3D_FORCE_DRIVER", "OpenGL");

                        break;

                    case 2: // Vulkan
                        Environment.SetEnvironmentVariable("FNA3D_FORCE_DRIVER", "Vulkan");

                        break;
                }

                if (File.Exists(ExePath))
                {
                    ClientHash = Crc32.Crc32Hesapla(ExePath);
                    FileInfo ClientFileInfo = new FileInfo(ExePath);
                    ClientDosyaBoyutu_Byte = ClientFileInfo.Length.ToString();
                    //MessageBox.Show(ClientDosyaBoyutu_Byte.ToString());
                }

                if (File.Exists(ArtMulPath))
                {
                    ArtMulHash = Crc32.Crc32Hesapla(ArtMulPath);
                    FileInfo ArtMulFileInfo = new FileInfo(ArtMulPath);
                    ArtMulBoyutu_Byte = ArtMulFileInfo.Length.ToString();
                }

                Application.Run(new Launcher());
                //Client.Run(pluginHost);
            }

            Log.Trace("Closing...");
        }

        private static void ReadSettingsFromArgs(string[] args)
        {
            for (int i = 0; i <= args.Length - 1; i++)
            {
                string cmd = args[i].ToLower();

                // NOTE: Command-line option name should start with "-" character
                if (cmd.Length == 0 || cmd[0] != '-')
                {
                    continue;
                }

                cmd = cmd.Remove(0, 1);
                string value = string.Empty;

                if (i < args.Length - 1)
                {
                    if (!string.IsNullOrWhiteSpace(args[i + 1]) && !args[i + 1].StartsWith("-"))
                    {
                        value = args[++i];
                    }
                }

                Log.Trace($"ARG: {cmd}, VALUE: {value}");

                switch (cmd)
                {
                    // Here we have it! Using `-settings` option we can now set the filepath that will be used 
                    // to load and save ClassicUO main settings instead of default `./settings.json`
                    // NOTE: All individual settings like `username`, `password`, etc passed in command-line options
                    // will override and overwrite those in the settings file because they have higher priority

                    case "settings":
                        Settings.CustomSettingsFilepath = value;

                        break;

                    case "highdpi":
                        //CUOEnviroment.IsHighDPI = true;
                        Settings.GlobalSettings.HighDPI = bool.Parse(value);
                        break;

                    case "username":
                        Settings.GlobalSettings.Username = value;

                        break;

                    case "password":
                        Settings.GlobalSettings.Password = Crypter.Encrypt(value);

                        break;

                    case "password_enc": // Non-standard setting, similar to `password` but for already encrypted password
                        Settings.GlobalSettings.Password = value;

                        break;

                    //case "ip":
                    //  Settings.GlobalSettings.IP = value;

                    // break;

                    // case "port":
                    //  Settings.GlobalSettings.Port = ushort.Parse(value);

                    // break;

                    case "filesoverride":
                    case "uofilesoverride":
                        UOFilesOverrideMap.OverrideFile = value;

                        break;

                    case "ultimaonlinedirectory":
                    case "uopath":
                        Settings.GlobalSettings.UltimaOnlineDirectory = value;

                        break;

                    case "profilespath":
                        Settings.GlobalSettings.ProfilesPath = value;

                        break;

                    // case "clientversion":
                    //Settings.GlobalSettings.ClientVersion = value;

                    // break;

                    case "lastcharactername":
                    case "lastcharname":
                        LastCharacterManager.OverrideLastCharacter(value);

                        break;

                    case "lastservernum":
                        Settings.GlobalSettings.LastServerNum = ushort.Parse(value);

                        break;

                    case "last_server_name":
                        Settings.GlobalSettings.LastServerName = value;
                        break;

                    case "fps":
                        int v = int.Parse(value);

                        if (v < Constants.MIN_FPS)
                        {
                            v = Constants.MIN_FPS;
                        }
                        else if (v > Constants.MAX_FPS)
                        {
                            v = Constants.MAX_FPS;
                        }

                        Settings.GlobalSettings.FPS = v;

                        break;

                    case "debug":
                        CUOEnviroment.Debug = true;

                        break;

                    case "profiler":
                        Profiler.Enabled = bool.Parse(value);

                        break;

                    case "saveaccount":
                        Settings.GlobalSettings.SaveAccount = bool.Parse(value);

                        break;

                    case "autologin":
                        Settings.GlobalSettings.AutoLogin = bool.Parse(value);

                        break;

                    case "reconnect":
                        Settings.GlobalSettings.Reconnect = bool.Parse(value);

                        break;

                    case "reconnect_time":

                        if (!int.TryParse(value, out int reconnectTime) || reconnectTime < 1000)
                        {
                            reconnectTime = 1000;
                        }

                        Settings.GlobalSettings.ReconnectTime = reconnectTime;

                        break;

                    case "login_music":
                    case "music":
                        Settings.GlobalSettings.LoginMusic = bool.Parse(value);

                        break;

                    case "login_music_volume":
                    case "music_volume":
                        Settings.GlobalSettings.LoginMusicVolume = int.Parse(value);

                        break;

                    // ======= [SHARD_TYPE_FIX] =======
                    // TODO old. maintain it for retrocompatibility
                    case "shard_type":
                    case "shard":
                        Settings.GlobalSettings.ShardType = int.Parse(value);

                        break;
                    // ================================

                    case "outlands":
                        CUOEnviroment.IsOutlands = true;

                        break;

                    case "fixed_time_step":
                        Settings.GlobalSettings.FixedTimeStep = bool.Parse(value);

                        break;

                    case "skiploginscreen":
                        CUOEnviroment.SkipLoginScreen = true;

                        break;


                    //case "plugins":
                    //    Settings.GlobalSettings.Plugins = string.IsNullOrEmpty(value) ? new string[0] : value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    //    break;

                    case "use_verdata":
                        Settings.GlobalSettings.UseVerdata = bool.Parse(value);

                        break;

                    case "maps_layouts":

                        Settings.GlobalSettings.MapsLayouts = value;

                        break;

                    //case "encryption":
                    //Settings.GlobalSettings.Encryption = byte.Parse(value);

                    //break;

                    case "force_driver":
                        if (byte.TryParse(value, out byte res))
                        {
                            switch (res)
                            {
                                case 1: // OpenGL
                                    Settings.GlobalSettings.ForceDriver = 1;

                                    break;

                                case 2: // Vulkan
                                    Settings.GlobalSettings.ForceDriver = 2;

                                    break;

                                default: // use default
                                    Settings.GlobalSettings.ForceDriver = 0;

                                    break;
                            }
                        }
                        else
                        {
                            Settings.GlobalSettings.ForceDriver = 0;
                        }

                        break;

                    case "packetlog":

                        PacketLogger.Default.Enabled = true;
                        PacketLogger.Default.CreateFile();

                        break;

                    case "language":

                        switch (value?.ToUpperInvariant())
                        {
                            case "RUS": Settings.GlobalSettings.Language = "RUS"; break;
                            case "FRA": Settings.GlobalSettings.Language = "FRA"; break;
                            case "DEU": Settings.GlobalSettings.Language = "DEU"; break;
                            case "ESP": Settings.GlobalSettings.Language = "ESP"; break;
                            case "JPN": Settings.GlobalSettings.Language = "JPN"; break;
                            case "KOR": Settings.GlobalSettings.Language = "KOR"; break;
                            case "PTB": Settings.GlobalSettings.Language = "PTB"; break;
                            case "ITA": Settings.GlobalSettings.Language = "ITA"; break;
                            case "CHT": Settings.GlobalSettings.Language = "CHT"; break;
                            default:

                                Settings.GlobalSettings.Language = "ENU";
                                break;

                        }

                        break;

                    case "no_server_ping":

                        CUOEnviroment.NoServerPing = true;

                        break;
                }
            }
        }

        
        public static string HextoString(string InputText)
        {
            byte[] bb = Enumerable.Range(0, InputText.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(InputText.Substring(x, 2), 16)).ToArray();
            return Encoding.ASCII.GetString(bb);
        }

        public static bool CheckIfRunningInVirtualBox()
        {
            IntPtr hwnd = NativeMethods.FindWindow("#32770", "VirtualBox");
            return hwnd != IntPtr.Zero;
        }


        public static bool CheckIfRunningInSandboxie()
        {
            IntPtr hwnd = NativeMethods.FindWindow("#32770", "Sandboxie Start/Run");
            return hwnd != IntPtr.Zero;
        }

        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        }


        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);


        public static void CheckBlocked(string pcid)
        {
            string apiUrl = Constants.WEB_MAIN_URL + "api/blocker-pc?pc_id=" + pcid;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiUrl);
                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            using (var reader = new System.IO.StreamReader(stream))
                            {
                                var responseJson = reader.ReadToEnd();

                                var responseData = JsonSerializer.Deserialize<ApiResponse>(responseJson);
                                Console.WriteLine(responseJson);
                                if (responseData != null)
                                {
                                    //Console.WriteLine(responseData.Blocked);
                                    //Console.WriteLine(responseData.Reason);
                                    if (responseData.Blocked)
                                    {
                                        Client.ShowErrorMessage("Bu bilgisayar engellendi.\n\nSebebi: " + responseData.Reason);
                                        Process.GetCurrentProcess().Kill();
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Client.ShowErrorMessage("Web isteği beklenmeyen bir yanit döndürdü. Durum kodu:" + response.StatusCode);
                        //Console.WriteLine("API'den beklenmeyen bir yanıt alındı. Status Code: " + response.StatusCode);
                    }
                }
            }
            catch (WebException ex)
            {
                // Handle web exception
                Client.ShowErrorMessage("Web isteği sırasında bir hata oluştu: " + ex.Message);
            }
        }

        public class ApiResponse
        {
            public bool Blocked { get; set; }
            public string Reason { get; set; }
        }


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
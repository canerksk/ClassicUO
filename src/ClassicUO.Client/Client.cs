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

using ClassicUO.Assets;
using ClassicUO.Configuration;
using ClassicUO.Game;
using ClassicUO.Game.Data;
using ClassicUO.Game.UI.Controls;
using ClassicUO.IO;
using ClassicUO.Network;
using ClassicUO.Network.Encryption;
using ClassicUO.Resources;
using ClassicUO.Utility;
using ClassicUO.Utility.Logging;
using ClassicUO.Utility.Platforms;
using Microsoft.Xna.Framework.Graphics;
using SDL2;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace ClassicUO
{
    sealed class UltimaOnline
    {
        public Renderer.Animations.Animations Animations { get; private set; }
        public Renderer.Arts.Art Arts { get; private set; }
        public Renderer.Gumps.Gump Gumps { get; private set; }
        public Renderer.Texmaps.Texmap Texmaps { get; private set; }
        public Renderer.Lights.Light Lights { get; private set; }
        public Renderer.MultiMaps.MultiMap MultiMaps { get; private set; }
        public Renderer.Sounds.Sound Sounds { get; private set; }
        public World World { get; private set; }
        public GameCursor GameCursor { get; private set; }

        public ClientVersion Version { get; private set; }
        public ClientFlags Protocol { get; set; }
        public string ClientPath { get; private set; }


        public UltimaOnline()
        {

        }

        public unsafe void Load(GameController game)
        {
            LoadUOFiles();

            const int TEXTURE_WIDTH = 32;
            const int TEXTURE_HEIGHT = 2048;
            const int LIGHTS_TEXTURE_WIDTH = 32;
            const int LIGHTS_TEXTURE_HEIGHT = 63;

            var hueSamplers = new Texture2D[3];
            hueSamplers[0] = new Texture2D(game.GraphicsDevice, TEXTURE_WIDTH, TEXTURE_HEIGHT);
            hueSamplers[1] = new Texture2D(game.GraphicsDevice, TEXTURE_WIDTH, TEXTURE_HEIGHT);
            hueSamplers[2] = new Texture2D(game.GraphicsDevice,LIGHTS_TEXTURE_WIDTH, LIGHTS_TEXTURE_HEIGHT);

            var buffer = new uint[Math.Max(
                LIGHTS_TEXTURE_WIDTH * LIGHTS_TEXTURE_HEIGHT,
                TEXTURE_WIDTH * TEXTURE_HEIGHT * 2
            )];

            fixed (uint* ptr = buffer)
            {
                HuesLoader.Instance.CreateShaderColors(buffer);

                hueSamplers[0].SetDataPointerEXT(
                    0,
                    null,
                    (IntPtr)ptr,
                    TEXTURE_WIDTH * TEXTURE_HEIGHT * sizeof(uint)
                );
                hueSamplers[1].SetDataPointerEXT(
                    0,
                    null,
                    (IntPtr)ptr + TEXTURE_WIDTH * TEXTURE_HEIGHT * sizeof(uint),
                    TEXTURE_WIDTH * TEXTURE_HEIGHT * sizeof(uint)
                );

                LightColors.CreateLightTextures(buffer, LIGHTS_TEXTURE_HEIGHT);
                hueSamplers[2].SetDataPointerEXT(
                    0,
                    null,
                    (IntPtr)ptr,
                    LIGHTS_TEXTURE_WIDTH * LIGHTS_TEXTURE_HEIGHT * sizeof(uint)
                );
            }

            game.GraphicsDevice.Textures[1] = hueSamplers[0];
            game.GraphicsDevice.Textures[2] = hueSamplers[1];
            game.GraphicsDevice.Textures[3] = hueSamplers[2];

            Animations = new Renderer.Animations.Animations(game.GraphicsDevice);
            Arts = new Renderer.Arts.Art(game.GraphicsDevice);
            Gumps = new Renderer.Gumps.Gump(game.GraphicsDevice);
            Texmaps = new Renderer.Texmaps.Texmap(game.GraphicsDevice);
            Lights = new Renderer.Lights.Light(game.GraphicsDevice);
            MultiMaps = new Renderer.MultiMaps.MultiMap(game.GraphicsDevice);
            Sounds = new Renderer.Sounds.Sound();

            LightColors.LoadLights();

            World = new World();
            GameCursor = new GameCursor(World);
        }

        public void Unload()
        {
            ArtLoader.Instance?.Dispose();
            GumpsLoader.Instance?.Dispose();
            TexmapsLoader.Instance?.Dispose();
            AnimationsLoader.Instance?.Dispose();
            LightsLoader.Instance?.Dispose();
            TileDataLoader.Instance?.Dispose();
            AnimDataLoader.Instance?.Dispose();
            ClilocLoader.Instance?.Dispose();
            FontsLoader.Instance?.Dispose();
            HuesLoader.Instance?.Dispose();
            MapLoader.Instance?.Dispose();
            MultiLoader.Instance?.Dispose();
            MultiMapLoader.Instance?.Dispose();
            ProfessionLoader.Instance?.Dispose();
            SkillsLoader.Instance?.Dispose();
            SoundsLoader.Instance?.Dispose();
            SpeechesLoader.Instance?.Dispose();
            Verdata.File?.Dispose();
            World?.Map?.Destroy();
        }


        private void LoadUOFiles()
        {
            string uoPath = Settings.GlobalSettings.UltimaOnlineDirectory;
            string clientPath = CUOEnviroment.ExecutablePath;

            Log.Trace($"Ultima Online path: {uoPath}");

            Log.Trace("Loading files...");

            /*
            if (!string.IsNullOrWhiteSpace(Settings.GlobalSettings.ClientVersion))
            {
                // sanitize client version
                Settings.GlobalSettings.ClientVersion = Settings.GlobalSettings.ClientVersion.Replace(",", ".").Replace(" ", "").ToLower();
            }
            */

            //string clientVersionText = Settings.GlobalSettings.ClientVersion;
            string clientVersionText = Constants.CLIENTVERSION;

            // check if directory is good
            if (!Directory.Exists(uoPath))
            {
                Log.Error("Invalid UO directory: " + uoPath);
                Client.ShowErrorMessage(string.Format(ResErrorMessages.ClientPathIsNotAValidUODirectory, uoPath));

                throw new InvalidClientDirectory($"'{uoPath}' is not a valid directory");
            }

            // try to load the client version
            if (!ClientVersionHelper.IsClientVersionValid(clientVersionText, out ClientVersion clientVersion))
            {
                Log.Warn($"Client version [{clientVersionText}] is invalid, let's try to read the client.exe");

                // mmm something bad happened, try to load from client.exe
                if (!ClientVersionHelper.TryParseFromFile(Path.Combine(uoPath, "client.exe"), out clientVersionText) || !ClientVersionHelper.IsClientVersionValid(clientVersionText, out clientVersion))
                {
                    Log.Error("Invalid client version: " + clientVersionText);
                    Client.ShowErrorMessage(string.Format(ResGumps.ImpossibleToDefineTheClientVersion0, clientVersionText));

                    throw new InvalidClientVersion($"Invalid client version: '{clientVersionText}'");
                }

                Log.Trace($"Found a valid client.exe [{clientVersionText} - {clientVersion}]");

                // update the wrong/missing client version in settings.json
                //Settings.GlobalSettings.ClientVersion = clientVersionText;
            }

            Version = clientVersion;
            ClientPath = uoPath;

            Protocol = ClientFlags.CF_T2A;

            if (Version >= ClientVersion.CV_200)
            {
                Protocol |= ClientFlags.CF_RE;
            }

            if (Version >= ClientVersion.CV_300)
            {
                Protocol |= ClientFlags.CF_TD;
            }

            if (Version >= ClientVersion.CV_308)
            {
                Protocol |= ClientFlags.CF_LBR;
            }

            if (Version >= ClientVersion.CV_308Z)
            {
                Protocol |= ClientFlags.CF_AOS;
            }

            if (Version >= ClientVersion.CV_405A)
            {
                Protocol |= ClientFlags.CF_SE;
            }

            if (Version >= ClientVersion.CV_60144)
            {
                Protocol |= ClientFlags.CF_SA;
            }

            Log.Trace($"Client path: '{clientPath}'");
            //Log.Trace($"Client version: {clientVersion}");
            Log.Trace($"Client version: {clientVersion}");
            Log.Trace($"Protocol: {Protocol}");

            // ok now load uo files
            UOFileManager.Load(Version, Settings.GlobalSettings.UltimaOnlineDirectory, Settings.GlobalSettings.UseVerdata, Settings.GlobalSettings.Language);
            StaticFilters.Load();

            BuffTable.Load();
            ChairTable.Load();

            Log.Trace("Network calibration...");
            //ATTENTION: you will need to enable ALSO ultimalive server-side, or this code will have absolutely no effect!
            UltimaLive.Enable();
            PacketsTable.AdjustPacketSizeByVersion(Version);

            if (Constants.CLIENTENCRYPTION != 0)
            {
                Log.Trace("Calculating encryption by client version...");
                EncryptionHelper.CalculateEncryption(Version);
                Log.Trace($"encryption: {EncryptionHelper.Type}");

                if (EncryptionHelper.Type != (ENCRYPTION_TYPE) Constants.CLIENTENCRYPTION)
                {
                    Log.Warn($"Encryption found: {EncryptionHelper.Type}");
                    //Settings.GlobalSettings.Encryption = (byte) EncryptionHelper.Type;
                }
            }
        }
    }


    internal static class Client
    {
        public static GameController Game { get; private set; }


        public static string ArtMulPath = UOFileManager.GetUOFilePath("art.mul");
        public static string ClientExePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static string ClientHash;
        public static string ArtMulHash;
        public static string CUOVersion = CUOEnviroment.Version.ToString();
        public static FileInfo ArtMulFileInfo = new FileInfo(ArtMulPath);
        public static long ArtMulBoyutu_Byte = ArtMulFileInfo.Length;
        public static FileInfo ClientFileInfo = new FileInfo(ClientExePath);
        public static long ClientDosyaBoyutu_Byte = ClientFileInfo.Length;
        public static int SiteStatus;
        public static System.Timers.Timer WindowTitleRestoreTimer;
        public static System.Timers.Timer ProgramCloseTimer;
        private static World _world;

        public static void Run(IPluginHost pluginHost)
        {
            Debug.Assert(Game == null);

            Log.Trace("Running game...");

            // client.exe dosyası varsa hashını al
            if (File.Exists(ClientExePath))
            {
                ClientHash = Crc32.Crc32Hesapla(ClientExePath);
            }
            // art.mul dosyası varsa hashını al
            if (File.Exists(ArtMulPath))
            {
                ArtMulHash = Crc32.Crc32Hesapla(ArtMulPath);
            }

            // Site bağlantısını kontrol Et
            try
            {

                // Disable the warning.
#pragma warning disable SYSLIB0014
                // HTTP isteği oluştur
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Constants.WEB_MAIN_URL);
                // Re-enable the warning.
#pragma warning restore SYSLIB0014

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

            using (Game = new GameController(pluginHost))
            {
                // https://github.com/FNA-XNA/FNA/wiki/7:-FNA-Environment-Variables#fna_graphics_enable_highdpi
                CUOEnviroment.IsHighDPI = Environment.GetEnvironmentVariable("FNA_GRAPHICS_ENABLE_HIGHDPI") == "1";

                if (CUOEnviroment.IsHighDPI)
                {
                    Log.Trace("HIGH DPI - ENABLED");
                }

                Game.Run();

                StartTimers();
            }

            Log.Trace("Exiting game...");
        }

        public static void ShowErrorMessage(string msg)
        {
            SDL.SDL_ShowSimpleMessageBox(SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR, "HATA", msg, IntPtr.Zero);
        }

        public static void StartTimers()
        {
            //Console.WriteLine("Start Timer");
            Log.Trace("Start Timer");

            Random random = new Random();

            ProgramCloseTimer = new System.Timers.Timer(random.Next(7000, 10000));
            //ProgramCloseTimer = new System.Timers.Timer(5000);
            ProgramCloseTimer.Elapsed += OnTimedEvent_ProgramCloseTimer;
            ProgramCloseTimer.AutoReset = true;
            ProgramCloseTimer.Start();
            ProgramCloseTimer.Enabled = true;

            WindowTitleRestoreTimer = new System.Timers.Timer(random.Next(10000, 20000));
            WindowTitleRestoreTimer.Elapsed += OnTimedEvent_WindowTitleRestoreTimer;
            WindowTitleRestoreTimer.AutoReset = true;
            WindowTitleRestoreTimer.Enabled = true;

        }



        private static void OnTimedEvent_ProgramCloseTimer(Object source, System.Timers.ElapsedEventArgs e)
        {

        }


        public static void OnTimedEvent_WindowTitleRestoreTimer(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (_world.InGame)
            {
                if (!string.IsNullOrEmpty(_world.Player.Name))
                {
                    Client.Game.SetWindowTitle(_world.Player.Name);
                    if (!string.IsNullOrEmpty(_world.ServerName))
                    {
                        Client.Game.SetWindowTitle(_world.Player.Name + " (" + _world.ServerName + ")");
                    }
                }
            }
            else
            {
                Client.Game.SetWindowTitle("");
            }
        }



    }



}
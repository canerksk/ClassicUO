#region license

// Copyright (c) 2021, andreakarasho
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

#define USE_MMF

using ClassicUO.Utility.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace ClassicUO.IO
{
    public unsafe class UOFile : DataReader
    {
        public UOFile(string filepath, bool loadFile = false)
        {
            FilePath = filepath;

            if (loadFile)
            {
                Load();
            }
        }

        public string FilePath { get; }
#if USE_MMF
        protected MemoryMappedViewAccessor _accessor;
        protected MemoryMappedFile _file;
#endif

        protected virtual void Load()
        {
            //Log.Trace($"Loading file:\t\t{FilePath}");
            var fileName = Path.GetFileName(FilePath);
            string basePath = Directory.GetCurrentDirectory();

            var localFile = Path.Combine(basePath, fileName);
            FileInfo fileInfo = new FileInfo(FilePath);

            if (File.Exists(localFile))
            {
                fileInfo = new FileInfo(localFile);
            }
            else
            {
                fileInfo = new FileInfo(FilePath);
            }

            Log.Trace($"Loading file:\t\t{fileInfo}");

            long size = fileInfo.Length;

            if (size > 0)
            {
#if USE_MMF
                _file = MemoryMappedFile.CreateFromFile
                (
                    File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite),
                    null,
                    0,
                    MemoryMappedFileAccess.Read,
                    HandleInheritability.None,
                    false
                );

                _accessor = _file.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);

                byte* ptr = null;

                try
                {
                    _accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                    SetData(ptr, (long) _accessor.SafeMemoryMappedViewHandle.ByteLength);
                }
                catch
                {
                    _accessor.SafeMemoryMappedViewHandle.ReleasePointer();

                    throw new Exception("Something goes wrong...");
                }
#endif
            }
            else
            {
                Log.Error($"{FilePath}  size must be > 0");
            }
        }

        public virtual void FillEntries(ref UOFileIndex[] entries)
        {
        }

        public virtual void Dispose()
        {
#if USE_MMF
            _accessor.SafeMemoryMappedViewHandle.ReleasePointer();
            _accessor.Dispose();
            _file.Dispose();
#endif
            Log.Trace($"Unloaded:\t\t{FilePath}");
        }
    }


    public class UOFileUltimaDLL
    {

        public delegate void FileSaveHandler();

        private static Dictionary<string, string> m_MulPath;
        private static string m_Directory;
        private static string m_RootDir;


        public static string Directory
        {
            get { return m_Directory; }
            set
            {
                m_Directory = value;
                LoadMulPath();
            }
        }

        private static string[] m_Files = new string[]
        {
            "anim.idx",
            "anim.mul",
            "anim2.idx",
            "anim2.mul",
            "anim3.idx",
            "anim3.mul",
            "anim4.idx",
            "anim4.mul",
            "anim5.idx",
            "anim5.mul",
            "animdata.mul",
            "art.mul",
            "artidx.mul",
            "artlegacymul.uop",
            "body.def",
            "bodyconv.def",
            "classic.exe",
            "cliloc.custom1",
            "cliloc.custom2",
            "cliloc.deu",
            "cliloc.enu",
            "cliloc.tur",
            "equipconv.def",
            "facet00.mul",
            "facet01.mul",
            "facet02.mul",
            "facet03.mul",
            "facet04.mul",
            "facet05.mul",
            "fonts.mul",
            "gump.def",
            "gumpart.mul",
            "gumpidx.mul",
            "gumpartlegacymul.uop",
            "hues.mul",
            "light.mul",
            "lightidx.mul",
            "map0.mul",
            "map1.mul",
            "map2.mul",
            "map3.mul",
            "map4.mul",
            "map5.mul",
            "map0legacymul.uop",
            "map1legacymul.uop",
            "map2legacymul.uop",
            "map3legacymul.uop",
            "map4legacymul.uop",
            "map5legacymul.uop",
            "mapdif0.mul",
            "mapdif1.mul",
            "mapdif2.mul",
            "mapdif3.mul",
            "mapdif4.mul",
            "mapdifl0.mul",
            "mapdifl1.mul",
            "mapdifl2.mul",
            "mapdifl3.mul",
            "mapdifl4.mul",
            "mobtypes.txt",
            "multi.idx",
            "multi.mul",
            "multimap.rle",
            "radarcol.mul",
            "skillgrp.mul",
            "skills.idx",
            "skills.mul",
            "sound.def",
            "sound.mul",
            "soundidx.mul",
            "soundlegacymul.uop",
            "speech.mul",
            "stadif0.mul",
            "stadif1.mul",
            "stadif2.mul",
            "stadif3.mul",
            "stadif4.mul",
            "stadifi0.mul",
            "stadifi1.mul",
            "stadifi2.mul",
            "stadifi3.mul",
            "stadifi4.mul",
            "stadifl0.mul",
            "stadifl1.mul",
            "stadifl2.mul",
            "stadifl3.mul",
            "stadifl4.mul",
            "staidx0.mul",
            "staidx1.mul",
            "staidx2.mul",
            "staidx3.mul",
            "staidx4.mul",
            "staidx5.mul",
            "statics0.mul",
            "statics1.mul",
            "statics2.mul",
            "statics3.mul",
            "statics4.mul",
            "statics5.mul",
            "texidx.mul",
            "texmaps.mul",
            "tiledata.mul",
            "unifont.mul",
            "unifont1.mul",
            "unifont2.mul",
            "unifont3.mul",
            "unifont4.mul",
            "unifont5.mul",
            "unifont6.mul",
            "unifont7.mul",
            "unifont8.mul",
            "unifont9.mul",
            "unifont10.mul",
            "unifont11.mul",
            "unifont12.mul",
            //"uotd.exe",
            "verdata.mul"
        };


        /*
        public static string UOFoldersGet()
        {
            string dir = null;

            //for (int i = UOFile.knownRegkeys.Length - 1; i >= 0; i--)
            for (int i = knownRegkeys.Length - 1; i >= 1; i--)
            {
                string exePath;

                if (Environment.Is64BitOperatingSystem)
                    exePath = GetPath(string.Format(@"Wow6432Node\{0}", knownRegkeys[i]));
                else
                    exePath = GetPath(knownRegkeys[i]);

                if (exePath != null)
                {
                    dir = exePath;
                }
            }

            return dir;
        }
        */

        public static string UOFoldersGet()
        {
            string dir = null;

            string exePath;
            if (Environment.Is64BitOperatingSystem)
                exePath = GetPath("Wow6432Node\\Origin Worlds Online\\Ultima Online\\1.0");
            else
                exePath = GetPath("Origin Worlds Online\\Ultima Online\\1.0");

            if (exePath != null)
            {
                dir = exePath;
            }
            return dir;
        }



        public static void LoadMulPath()
        {
            m_MulPath = new Dictionary<string, string>();
            m_RootDir = Directory;
            if (m_RootDir == null)
                m_RootDir = "";
            foreach (string file in m_Files)
            {
                string filePath = Path.Combine(m_RootDir, file);
                if (File.Exists(filePath))
                    m_MulPath[file] = file;
                else
                    m_MulPath[file] = "";
            }
        }


        public static readonly string[] knownRegkeys = new string[] {
            @"Origin Worlds Online\Ultima Online\1.0",
            @"Origin Worlds Online\Ultima Online Third Dawn\1.0",
            @"EA GAMES\Ultima Online Samurai Empire",
            @"EA GAMES\Ultima Online Samurai Empire\1.0",
            @"EA GAMES\Ultima Online Samurai Empire\1.00.0000",
            @"EA GAMES\Ultima Online: Samurai Empire\1.0",
            @"EA GAMES\Ultima Online: Samurai Empire\1.00.0000",
            @"EA Games\Ultima Online: Mondain's Legacy",
            @"EA Games\Ultima Online: Mondain's Legacy\1.0",
            @"EA Games\Ultima Online: Mondain's Legacy\1.00.0000",
            @"Electronic Arts\EA Games\Ultima Online Stygian Abyss Classic",
            @"Electronic Arts\EA Games\Ultima Online Classic",
        };


        public static readonly string[] knownRegPathkeys = new string[] {
            "ExePath",
            "Install Dir",
            "InstallDir"
        };

        public static string GetPath(string regkey)
        {
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(string.Format(@"SOFTWARE\{0}", regkey));

                if (key == null)
                {
                    key = Registry.CurrentUser.OpenSubKey(string.Format(@"SOFTWARE\{0}", regkey));

                    if (key == null)
                        return null;
                }

                string path = null;
                foreach (string pathkey in knownRegPathkeys)
                {
                    path = key.GetValue(pathkey) as string;

                    if ((path == null) || (path.Length <= 0))
                        continue;

                    if (pathkey == "InstallDir")
                        path = path + @"\";

                    if (!System.IO.Directory.Exists(path) && !File.Exists(path))
                        continue;

                    break;
                }

                if (path == null)
                    return null;

                if (!System.IO.Directory.Exists(path))
                    path = Path.GetDirectoryName(path);

                if ((path == null) || (!System.IO.Directory.Exists(path)))
                    return null;

                return path;
            }
            catch
            {
                return null;
            }
        }
    }

}
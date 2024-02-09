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

using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ClassicUO.Utility
{
    public static class Hasher
    {
        public static string Md5 { get; set; }
        public static string Sha1 { get; set; }
        public static string Crc32 { get; set; }

        public static string Md5Hesapla(string str)
        {
            // 1. Yontem
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            Md5 = BitConverter.ToString(hash).Replace("-", "").ToUpper();
            return Md5;

        }

        public static string Sha1Hesapla(string dosyaAdi)
        {
            SHA1 sha1Islemi = SHA1.Create();
            Stream sha1AkisOku = File.OpenRead(dosyaAdi);
            Sha1 = BitConverter.ToString(sha1Islemi.ComputeHash(sha1AkisOku)).Replace("-", "");
            return Sha1;
        }


        public static string Crc32Hesapla(string dosyaAdi)
        {
            Crc32 crc32Islemi = new Crc32();
            Stream crc32AkisOku = File.OpenRead(dosyaAdi);
            Crc32 = BitConverter.ToString(crc32Islemi.ComputeHash(crc32AkisOku)).Replace("-", "");
            return Crc32;
        }

        // Genel hesaplama işlemlerini yaparak ilgili bilgileri FrmHash'a döndr
        //public static void Hesapla()
        //{
        //    TxtDosyaAdi.Text = Path.GetFileName(FrmHash.DosyaAdresi);

        //    TxtCrc32.Text = Crc32Hesapla(FrmHash.DosyaAdresi);
        //    TxtMd5.Text = Md5Hesapla(FrmHash.DosyaAdresi);
        //    TxtSha1.Text = Sha1Hesapla(FrmHash.DosyaAdresi);
        //}

    }
}
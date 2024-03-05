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

using System;
using System.Collections.Generic;
using System.Text;
using ClassicUO.Configuration;
using ClassicUO.Game.Data;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Game.UI.Gumps;
using ClassicUO.Assets;
using ClassicUO.Renderer;
using ClassicUO.Utility;
using ClassicUO.Game.Scenes;
using System.Collections.Concurrent;
using Microsoft.Xna.Framework.Audio;

namespace ClassicUO.Game.Managers
{

    public class MessageQueue
    {
        private class MsgInfo
        {
            public MsgInfo(uint ser, ushort body, byte type, ushort hue, ushort font, string lang, string name)
            {
                Serial = ser;
                Body = body;
                Type = type;
                Hue = hue;
                Font = font;
                Lang = lang;
                Name = name;
            }

            public TimeSpan Delay;
            public DateTime NextSend;
            public int Count;
            //ser, body, type, hue, font, lang, name
            public uint Serial;
            public ushort Body, Hue, Font;
            public byte Type;
            public string Lang, Name;
        }



        private class MessageTimer : Timer
        {
            private readonly World _world;

            public MessageTimer() : base(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1))
            {

            }

            protected override void OnTick()
            {
                if (m_Table.Count <= 0)
                    return;

                List<string> toremove = new List<string>();
                foreach (KeyValuePair<string, MsgInfo> de in m_Table)
                {
                    string txt = de.Key;
                    MsgInfo msg = de.Value;

                    if (msg.NextSend <= DateTime.UtcNow)
                    {
                        if (msg.Count > 0)
                        {
                            switch (msg.Lang)
                            {
                                case "O":
                                    //msg.Mobile.OverheadMessage(msg.Hue, msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt);
                                    //_world.Player.AddMessage(MessageType.System, msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt, TextType.SYSTEM);

                                    _world.MessageManager.HandleMessage
                                    (
                                        null,
                                        msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt,
                                        "",
                                        0x0,
                                        MessageType.Regular,
                                        3,
                                        TextType.SYSTEM,
                                        true
                                    );

                                    break;
                                case "A":
                                    // _world.Player.AddMessage(MessageType.System, msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt, TextType.SYSTEM);

                                    _world.MessageManager.HandleMessage
                                    (
                                        null,
                                        msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt,
                                        "",
                                        0x0,
                                        MessageType.Regular,
                                        3,
                                        TextType.SYSTEM,
                                        true
                                    );

                                    // Client.Instance.SendToClient(new AsciiMessage(msg.Serial, msg.Body, msg.Type,
                                    //msg.Hue, msg.Font, msg.Name,
                                    //msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt));
                                    break;
                                default:

                                    _world.MessageManager.HandleMessage
                                    (
                                        null,
                                        msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt,
                                        "",
                                        0x0,
                                        MessageType.Regular,
                                        3,
                                        TextType.SYSTEM,
                                        true
                                    );

                                    //Client.Instance.SendToClient(new UnicodeMessage(msg.Serial, msg.Body, msg.Type,
                                    //msg.Hue, msg.Font, msg.Lang, msg.Name,
                                    //msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt));
                                    break;
                            }

                            msg.Count = 0;
                            msg.NextSend = DateTime.UtcNow + msg.Delay;
                        }
                        else
                        {
                            if (txt != null)
                                toremove.Add(txt);
                        }
                    }
                }

                for (int i = toremove.Count - 1; i >= 0; --i)
                {
                    m_Table.TryRemove(toremove[i], out var msg);
                }
            }
        }

        private static Timer m_Timer = new MessageTimer();
        private static ConcurrentDictionary<string, MsgInfo> m_Table = new ConcurrentDictionary<string, MsgInfo>();

        static MessageQueue()
        {
            m_Timer.Start();
        }

       // public static bool Enqueue(World ser, int hue, string lang, string text)
        //{
        //    return Enqueue(ser, 0, MessageType.Regular, (ushort)hue, 3, lang, "System", text);
        //}

        public static bool Enqueue(
            uint ser, 
            ushort body, 
            byte type, 
            ushort hue, 
            ushort font, 
            string lang, 
            string name, 
            string text)
        {

            MsgInfo m;

            if (!m_Table.TryGetValue(text, out m) || m == null)
            {
                m_Table[text] = m = new MsgInfo(ser, body, type, hue, font, lang, name);

                m.Count = 0;

                m.Delay = TimeSpan.FromSeconds((text.Length / 50 + 1) * 1000);

                m.NextSend = DateTime.UtcNow + m.Delay;

                return true;
            }

            m.Count++;

            return false;
        }
    }

}
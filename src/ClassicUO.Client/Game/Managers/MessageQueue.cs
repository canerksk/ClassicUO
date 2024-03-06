#region license
// Razor: An Ultima Online Assistant
// Copyright (c) 2022 Razor Development Community on GitHub <https://github.com/markdwags/Razor>
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using ClassicUO.Game.Data;
using ClassicUO.Game.GameObjects;
using ClassicUO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ClassicUO.Game.Managers;
using ClassicUO.Game;
using Microsoft.Xna.Framework.Audio;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using ClassicUO.Network;

namespace ClassicUO.Game.Managers
{
    public class MessageQueue
    {
        private static World _world;

        private class MsgInfo
        {
            public MsgInfo(World world, ushort body, byte type, ushort hue, ushort font, string lang,
                string name)
            {
                Body = body;
                Type = type;
                Hue = hue;
                Font = font;
                Lang = lang;
                Name = name;
                World = world;
            }

            public TimeSpan Delay;
            public DateTime NextSend;
            public int Count;

            public World World;
            //ser, body, type, hue, font, lang, name
            public ushort Body, Hue, Font;
            public byte Type = 0;
            public string Lang, Name;
        }

        private class MessageTimer : Timer
        {
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
                                    Console.WriteLine(msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt);
                                    //GameActions.Print(_world, msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt, msg.Hue, MessageType.Regular, 0, false);
                                    break;
                                case "A":
                                    //PacketHandlers.Talk(_world);

                                    //Talk(new Talk(_world, msg.Body, msg.Type,
                                    //    msg.Hue, msg.Font, msg.Name,
                                    //    msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt));

                                    //Console.WriteLine($"{txt} [{msg.Count}]");

                                    Console.WriteLine(msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt);
                                    //GameActions.Print(_world, msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt, msg.Hue, MessageType.Regular, 0, false);
                                    break;
                                default:
                                    //Client.Instance.SendToClient(new UnicodeMessage(msg.Serial, msg.Body, msg.Type,
                                    //    msg.Hue, msg.Font, msg.Lang, msg.Name,
                                    //    msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt));

                                    Console.WriteLine(msg.Count > 1 ? $"{txt} [{msg.Count}]" : txt);
                                    break;
                            }

                            msg.Count = 0;
                            msg.NextSend = DateTime.UtcNow + msg.Delay;
                        }
                        else
                        {
                            if (txt != null)
                                toremove.Add(txt);
                            Console.WriteLine($"{txt} [{msg.Count}]");
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

        //public static bool Enqueue(Mobile m, int hue, string lang, string text)
        //{
           // return Enqueue(0xFFFFFFFF, m, 0, MessageType.Regular, (ushort)hue, 3, lang, "System", text);
        //}

        public static bool Enqueue(ushort body, byte type, ushort hue, ushort font, string lang, string name, string text)
        {
            MsgInfo m;

            if (!m_Table.TryGetValue(text, out m) || m == null)
            {
                m_Table[text] = m = new MsgInfo(_world, body, type, hue, font, lang, name);

                m.Count = 0;

                m.Delay = TimeSpan.FromSeconds((text.Length / 50 + 1) * 3.5);

                m.NextSend = DateTime.UtcNow + m.Delay;

                return true;
            }

            m.Count++;

            return false;
        }
    }
}
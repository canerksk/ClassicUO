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
using System.Windows.Forms;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Scenes;
using ClassicUO.Input;
using ClassicUO.Network;
using ClassicUO.Renderer;
using ClassicUO.Resources;
using ClassicUO.Utility.Logging;
using Microsoft.Xna.Framework;

namespace ClassicUO.Game.Managers
{
    internal sealed class CommandManager
    {
        private readonly Dictionary<string, Action<string[]>> _commands = new Dictionary<string, Action<string[]>>();
        private readonly World _world;
        private long _commanddelay;
        public Camera Camera { get; } = new Camera(0.5f, 2.5f, 0.1f);
        public CommandManager(World world)
        {
            _world = world;
        }

        public void Initialize()
        {
            if (CUOEnviroment.IsMythic)
            {

                Register
                (
                    "shopkeeper",
                    s =>
                    {
                        if (_world.Player != null)
                        {
                            NetClient.Socket.Send_TextCommand(0x0F5, "1");
                        }
                    }
                );


                Register
                (
                    "potions",
                    s =>
                    {
                        if (_world.Player != null)
                        {
                            NetClient.Socket.Send_TextCommand(0x0F5, "0");
                        }
                    }
                );


                Register
                (
                    "mbulettin",
                    s =>
                    {
                        if (_world.Player != null)
                        {
                            NetClient.Socket.Send_TextCommand(0x0F5, "2");
                        }
                    }
                );

                Register
                (
                    "shaker",
                    s =>
                    {
                        if (_world.Player != null)
                        {
                        }
                    }
                );

                Register
                (
                    "party",
                    s =>
                    {
                        if (_world.Player != null)
                        {
                            NetClient.Socket.Send_TextCommand(0x0F5, "3");
                        }
                    }
                );

                Register
                (
                    "info",
                    s =>
                    {
                        if (_world.TargetManager.IsTargeting)
                        {
                            _world.TargetManager.CancelTarget();
                        }

                        _world.TargetManager.SetTargeting(CursorTarget.SetTargetClientSide, CursorType.Target, TargetType.Neutral);
                    }
                );

                Register
                (
                    "datetime",
                    s =>
                    {
                        if (_world.Player != null)
                        {
                            GameActions.Print(_world, string.Format(ResGeneral.CurrentDateTimeNowIs0, DateTime.Now), 946, Data.MessageType.Regular, 1, true);
                        }
                    }
                );

                Register
                (
                    "hue",
                    s =>
                    {
                        if (_world.TargetManager.IsTargeting)
                        {
                            _world.TargetManager.CancelTarget();
                        }

                        _world.TargetManager.SetTargeting(CursorTarget.HueCommandTarget, CursorType.Target, TargetType.Neutral);
                    }
                );


                Register
                (
                    "debug",
                    s =>
                    {
                        CUOEnviroment.Debug = !CUOEnviroment.Debug;

                    }
                );
            }
        }


        public void Register(string name, Action<string[]> callback)
        {
            name = name.ToLower();

            if (!_commands.ContainsKey(name))
            {
                _commands.Add(name, callback);
            }
            else
            {
                Log.Error($"Attempted to register command: '{name}' twice.");
            }
        }

        public void UnRegister(string name)
        {
            name = name.ToLower();

            if (_commands.ContainsKey(name))
            {
                _commands.Remove(name);
            }
        }

        public void UnRegisterAll()
        {
            _commands.Clear();
        }

        public void Execute(string name, params string[] args)
        {
            if (_commanddelay > Time.Ticks)
            {
                GameActions.Print(_world, "You must wait to perform another action.", 946, Data.MessageType.Regular, 1, true);
                return;
            }

            name = name.ToLower();

            if (_commands.TryGetValue(name, out Action<string[]> action))
            {
                action.Invoke(args);
                _commanddelay = Time.Ticks + 100;
            }
            else
            {
                GameActions.Print(_world, $"Geçersiz komut: '{name}'", 38, Data.MessageType.System, 1, true);
                Log.Warn($"Command: '{name}' not exists");
            }
        }

        public void OnHueTarget(Entity entity)
        {
            if (entity != null)
            {
                _world.TargetManager.Target(entity);
            }

            Mouse.LastLeftButtonClickTime = 0;
            GameActions.Print(_world, string.Format(ResGeneral.ItemID0Hue1, entity.Graphic, entity.Hue));
        }

    }
}
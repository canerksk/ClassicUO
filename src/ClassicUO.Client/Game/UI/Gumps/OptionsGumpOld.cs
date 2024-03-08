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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClassicUO.Configuration;
using ClassicUO.Game.Data;
using ClassicUO.Game.GameObjects;
using ClassicUO.Game.Managers;
using ClassicUO.Game.Scenes;
using ClassicUO.Game.UI.Controls;
using ClassicUO.Input;
using ClassicUO.Assets;
using ClassicUO.Network;
using ClassicUO.Renderer;
using ClassicUO.Resources;
using ClassicUO.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static SDL2.SDL;
using System.Text;

namespace ClassicUO.Game.UI.Gumps
{
    internal class OptionsGumpOld : Gump
    {
        public string TusName;

        private const byte FONT = 0x00;
        private const ushort HUE_FONT = 0x0;

        private const int WIDTH = 630;
        private const int HEIGHT = 450;

        private const int MAIN_X = 0;
        private const int MAIN_Y = 0;

        private const int TEXTBOX_HEIGHT = 25;

        private int SetDefaultClickCount = 0;


        //private static Texture2D _logoTexture2D;
        private Combobox _auraType;
        private Combobox _autoOpenCorpseOptions;
        private InputField _autoOpenCorpseRange;

        private Button _prevButton, _nextButton, _applybutton, _cancelbutton, _defaultbutton, _okbutton, _addButton, _delButton;

        private Button _soundButton, _macroButton, _speechButton, _toolTipsButton, _fontsButton, _displayButton, _generalButton, _filterButton;

        private NiceButton _alldelButton, _convertMacro, _openFolder;


        //experimental
        private Checkbox _autoOpenDoors, _autoOpenCorpse, _skipEmptyCorpse, _disableTabBtn, _disableCtrlQWBtn, _disableDefaultHotkeys, _disableArrowBtn, _disableAutoMove, _overrideContainerLocation, _smoothDoors, _showTargetRangeIndicator, _customBars, _customBarsBBG, _saveHealthbars;
        private HSliderBar _cellSize;
        private Checkbox _containerScaleItems, _containerDoubleClickToLoot, _relativeDragAnDropItems, _useLargeContianersGumps, _highlightContainersWhenMouseIsOver;


        // containers
        private HSliderBar _containersScale;
        private Combobox _cotType;
        private DataBox _databox;
        private HSliderBar _delay_before_display_tooltip, _tooltip_zoom, _tooltip_background_opacity;
        private Combobox _dragSelectModifierKey;
        private Combobox _backpackStyle;
        private Checkbox _hueContainerGumps;


        //counters
        private Checkbox _enableCounters, _highlightOnUse, _highlightOnAmount, _enableAbbreviatedAmount;
        private Checkbox _enableDragSelect, _dragSelectHumanoidsOnly;

        // sounds
        private Checkbox _enableSounds, _enableMusic, _footStepsSound, _combatMusic, _musicInBackground, _loginMusic;

        // fonts
        private FontSelector _fontSelectorChat;
        private Checkbox _forceUnicodeJournal;
        private InputField _gameWindowHeight;

        private Checkbox _gameWindowLock, _gameWindowFullsize;
        // GameWindowPosition
        private InputField _gameWindowPositionX;
        private InputField _gameWindowPositionY;

        // GameWindowSize
        private InputField _gameWindowWidth;
        private Combobox _gridLoot;
        private Checkbox _hideScreenshotStoredInMessage;
        public static Checkbox _highlightObjects,
                         /*_smoothMovements, */
                         _enablePathfind,
                         _useShiftPathfind,
                         _alwaysRun,
                         _alwaysRunUnlessHidden,
                         _showHpMobile,
                         _highlightByPoisoned,
                         _highlightByParalyzed,
                         _highlightByInvul,
                         _drawRoofs,
                         _treeToStumps,
                         _hideVegetation,
                         _noColorOutOfRangeObjects,
                         _useCircleOfTransparency,
                         _enableTopbar,
                         _holdDownKeyTab,
                         _holdDownKeyAlt,
                         _closeAllAnchoredGumpsWithRClick,
                         _chatAfterEnter,
                         _chatAdditionalButtonsCheckbox,
                         _chatShiftEnterCheckbox,
                         _enableCaveBorder;
        private static Checkbox _holdShiftForContext, _holdShiftToSplitStack, _clientnotifyballon, _inputautofocused, _reduceFPSWhenInactive, _sallosEasyGrab, _partyInviteGump, _objectsFading, _textFading, _holdAltToMoveGumps;
        private static Combobox _hpComboBox, _healtbarType, _fieldsType, _hpComboBoxShowWhen, _uiType;

        // infobar
        private static List<InfoBarBuilderControl> _infoBarBuilderControls;
        private static Combobox _infoBarHighlightType;

        // combat & spells
        private static ClickableColorBox _innocentColorPickerBox, _friendColorPickerBox, _crimialColorPickerBox, _canAttackColorPickerBox, _enemyColorPickerBox, _murdererColorPickerBox, _neutralColorPickerBox, _beneficColorPickerBox, _harmfulColorPickerBox;
        private static HSliderBar _lightBar;
        private static Checkbox _buffBarTime, _uiButtonsSingleClick, _queryBeforAttackCheckbox, _queryBeforeBeneficialCheckbox, _spellColoringCheckbox, _spellFormatCheckbox, _enableFastSpellsAssign;

        // macro
        private static MacroControl _macroControl;
        private static Checkbox _overrideAllFonts;
        private Combobox _overrideAllFontsIsUnicodeCheckbox;
        private Combobox _overrideContainerLocationSetting;
        private static ClickableColorBox _poisonColorPickerBox, _paralyzedColorPickerBox, _invulnerableColorPickerBox;
        private static NiceButton _randomizeColorsButton;
        private static Checkbox _restorezoomCheckbox, _zoomCheckbox;
        private static InputField _rows, _columns, _highlightAmount, _abbreviatedAmount;

        // speech
        private Checkbox _scaleSpeechDelay, _saveJournalCheckBox;
        private static Checkbox _showHouseContent;
        private Checkbox _showInfoBar;
        private Checkbox _ignoreAllianceMessages;
        private Checkbox _ignoreGuildMessages;

        // general
        private HSliderBar _sliderFPS, _circleOfTranspRadius;
        private HSliderBar _sliderSpeechDelay;
        private HSliderBar _sliderZoom;
        private HSliderBar _soundsVolume, _musicVolume, _loginMusicVolume;
        private ClickableColorBox _speechColorPickerBox, _emoteColorPickerBox, _yellColorPickerBox, _whisperColorPickerBox, _partyMessageColorPickerBox, _guildMessageColorPickerBox, _allyMessageColorPickerBox, _chatMessageColorPickerBox, _partyAuraColorPickerBox;
        private InputField _spellFormatBox;
        private ClickableColorBox _tooltip_font_hue;
        private FontSelector _tooltip_font_selector;
        private HSliderBar _dragSelectStartX, _dragSelectStartY;
        private Checkbox _dragSelectAsAnchor;

        // video
        private Checkbox _use_old_status_gump, _windowBorderless, _enableDeathScreen, _enableBlackWhiteEffect, _altLights, _enableLight, _enableShadows, _enableShadowsStatics, _auraMouse, _runMouseInSeparateThread, _useColoredLights, _darkNights, _partyAura, _hideChatGradient, _animatedWaterEffect;
        private Combobox _lightLevelType;
        private Checkbox _use_smooth_boat_movement;
        private HSliderBar _terrainShadowLevel;

        private Checkbox _use_tooltip;
        private Checkbox _useStandardSkillsGump, _showMobileNameIncoming, _showCorpseNameIncoming;
        private Checkbox _showStatsMessage, _showSkillsMessage;
        private HSliderBar _showSkillsMessageDelta;


        public static Profile _currentProfile = ProfileManager.CurrentProfile;

        public OptionsGumpOld(World world) : base(world, 0, 0)
        {

            Add
           (
               new ResizePic(0x0A28)
               {
                   X = MAIN_X + 50,
                   Y = MAIN_Y,
                   Width = WIDTH - 2,
                   Height = HEIGHT - 2,
               }
           );


            Add
            (
                //_soundButton = new Button(0, 0xDA, 0xD9, 0xDA)
                _soundButton = new Button(0, 0xDA, 0xDA, 0xDA)
                {
                    X = MAIN_X + 10,
                    Y = MAIN_Y + 45,
                    ButtonAction = ButtonAction.SwitchPage,
                    ToPage = 2
                }
            );
            _soundButton.SetTooltip(ResGumps.Music);


            Add
            (
                //_toolTipsButton = new Button(0, 0xDC, 0xDB, 0xDC)
                _toolTipsButton = new Button(0, 0xDC, 0xDC, 0xDC)
                {
                    X = MAIN_X + 10,
                    Y = MAIN_Y + 110,
                    ButtonAction = ButtonAction.SwitchPage,
                    ToPage = 5
                }
            );
            _toolTipsButton.SetTooltip(ResGumps.Tooltip);

            Add
            (
                //_fontsButton = new Button(0, 0xDE, 0xDD, 0xDE)
                _fontsButton = new Button(0, 0xDE, 0xDE, 0xDE)
                {
                    X = MAIN_X + 10,
                    Y = MAIN_Y + 175,
                    ButtonAction = ButtonAction.SwitchPage,
                    ToPage = 6
                }
            );
            _fontsButton.SetTooltip(ResGumps.Fonts);


            Add
            (
                //_speechButton = new Button(0, 0xE0, 0xDF, 0xE0)
                _speechButton = new Button(0, 0xE0, 0xE0, 0xE0)
                {
                    X = MAIN_X + 10,
                    Y = MAIN_Y + 240,
                    ButtonAction = ButtonAction.SwitchPage,
                    ToPage = 7
                }
            );
            _speechButton.SetTooltip(ResGumps.Speech);


            Add
            (
                //_macroButton = new Button(0, 0xED, 0xEC, 0xED)
                _macroButton = new Button(0, 0xED, 0xED, 0xED)
                {
                    X = MAIN_X + 10,
                    Y = MAIN_Y + 305,
                    ButtonAction = ButtonAction.SwitchPage,
                    ToPage = 4
                }
            );
            _macroButton.SetTooltip("Macro");

            Add
            (
                //_displayButton = new Button(0, 0xE4, 0xE4, 0xE3)
                _displayButton = new Button(0, 0xE4, 0xE4, 0xE4)
                {
                    X = MAIN_X + 665,
                    Y = MAIN_Y + 110,
                    ButtonAction = ButtonAction.SwitchPage,
                    ToPage = 3
                }
            );
            _displayButton.SetTooltip(ResGumps.Video);

            Add
            (
                //_fontsButton = new Button(0, 0xE6, 0xE6, 0xE5)
                _fontsButton = new Button(0, 0xE6, 0xE5, 0xE5)
                {
                    X = MAIN_X + 665,
                    Y = MAIN_Y + 175,
                    ButtonAction = ButtonAction.SwitchPage,
                    ToPage = 8
                }
            );
            _fontsButton.SetTooltip("Reputation");

            Add
            (
                //_generalButton = new Button(0, 0xE8, 0xE8, 0xE7)
                _generalButton = new Button(0, 0xE8, 0xE8, 0xE8)
                {
                    X = MAIN_X + 665,
                    Y = MAIN_Y + 240,
                    ButtonAction = ButtonAction.SwitchPage,
                    ToPage = 1
                }
            );
            _generalButton.SetTooltip(ResGumps.General);

            Add
            (
                //_filterButton = new Button(0, 0xEB, 0xEB, 0xEA)
                _filterButton = new Button(0, 0xEB, 0xEB, 0xEB)
                {
                    X = MAIN_X + 665,
                    Y = MAIN_Y + 305,
                    ButtonAction = ButtonAction.SwitchPage,
                    ToPage = 9
                }
            );
            _filterButton.SetTooltip(ResGumps.Counters + "\n" + ResGumps.Containers + "\n" + ResGumps.InfoBar + "\n" + ResGumps.IgnoreListManager);

            /*
            int i = 0;

            Add
            (
                new NiceButton
                (
                    10,
                    10 + 30 * i++,
                    140,
                    25,
                    ButtonAction.SwitchPage,
                    ResGumps.General
                ) { IsSelected = true, ButtonParameter = 1 }
            );

            Add
            (
                new NiceButton
                (
                    10,
                    10 + 30 * i++,
                    140,
                    25,
                    ButtonAction.SwitchPage,
                    ResGumps.Sound
                ) { ButtonParameter = 2 }
            );

            Add
            (
                new NiceButton
                (
                    10,
                    10 + 30 * i++,
                    140,
                    25,
                    ButtonAction.SwitchPage,
                    ResGumps.Video
                ) { ButtonParameter = 3 }
            );

            Add
            (
                new NiceButton
                (
                    10,
                    10 + 30 * i++,
                    140,
                    25,
                    ButtonAction.SwitchPage,
                    ResGumps.Macros
                ) { ButtonParameter = 4 }
            );

            Add
            (
                new NiceButton
                (
                    10,
                    10 + 30 * i++,
                    140,
                    25,
                    ButtonAction.SwitchPage,
                    ResGumps.Tooltip
                ) { ButtonParameter = 5 }
            );

            Add
            (
                new NiceButton
                (
                    10,
                    10 + 30 * i++,
                    140,
                    25,
                    ButtonAction.SwitchPage,
                    ResGumps.Fonts
                ) { ButtonParameter = 6 }
            );

            Add
            (
                new NiceButton
                (
                    10,
                    10 + 30 * i++,
                    140,
                    25,
                    ButtonAction.SwitchPage,
                    ResGumps.Speech
                ) { ButtonParameter = 7 }
            );

            Add
            (
                new NiceButton
                (
                    10,
                    10 + 30 * i++,
                    140,
                    25,
                    ButtonAction.SwitchPage,
                    ResGumps.CombatSpells
                ) { ButtonParameter = 8 }
            );

            Add
            (
                new NiceButton
                (
                    10,
                    10 + 30 * i++,
                    140,
                    25,
                    ButtonAction.SwitchPage,
                    ResGumps.Counters
                ) { ButtonParameter = 9 }
            );

            Add
            (
                new NiceButton
                (
                    10,
                    10 + 30 * i++,
                    140,
                    25,
                    ButtonAction.SwitchPage,
                    ResGumps.InfoBar
                ) { ButtonParameter = 10 }
            );

            Add
            (
                new NiceButton
                (
                    10,
                    10 + 30 * i++,
                    140,
                    25,
                    ButtonAction.SwitchPage,
                    ResGumps.Containers
                ) { ButtonParameter = 11 }
            );

            Add
            (
                new NiceButton
                (
                    10,
                    10 + 30 * i++,
                    140,
                    25,
                    ButtonAction.SwitchPage,
                    ResGumps.Experimental
                ) { ButtonParameter = 12 }
            );

            Add
            (
                new NiceButton
                (
                    10,
                    10 + 30 * i++,
                    140,
                    25,
                    ButtonAction.Activate,
                    ResGumps.IgnoreListManager
                )
                {
                    ButtonParameter = (int)Buttons.OpenIgnoreList
                }
            );


            Add
            (
                new Line
                (
                    160,
                    5,
                    1,
                    HEIGHT - 10,
                    Color.Gray.PackedValue
                )
            );

            */

            int offsetX = MAIN_X + 60;
            int offsetY = MAIN_Y + 60;

            Add
            (
                new Line
                (
                    MAIN_X + 160,
                    MAIN_Y + 5,
                    1,
                    HEIGHT - 10,
                    Color.Gray.PackedValue
                )
            );



            Add
            (
                new Line
                (
                    MAIN_X + 160,
                    MAIN_Y + 405 + 35 + 1,
                    WIDTH - 160,
                    1,
                    Color.Gray.PackedValue
                )
            );



            // start: Cancel Button
            Add
            (
                _cancelbutton = new Button((int)Buttons.Cancel, 0x00F3, 0x00F1, 0x00F2)
                {
                    X = 154 + offsetX,
                    Y = 345 + offsetY,
                    ButtonAction = ButtonAction.Activate
                }
            );
            _cancelbutton.SetTooltip("Vazgeç ve değişiklikleri iptal et");



            // start: Apply button
            Add
            (
               _applybutton = new Button((int)Buttons.Apply, 0x00EF, 0x00F0, 0x00EE)
               {
                   X = 248 + offsetX,
                   Y = 345 + offsetY,
                   ButtonAction = ButtonAction.Activate
               }
            );
            _applybutton.SetTooltip("Değişiklikleri uygula");
            // end: Apply button

            // start: Default button
            Add
            (
                _defaultbutton = new Button((int)Buttons.Default, 0x00F6, 0x00F4, 0x00F5)
                {
                    X = 346 + offsetX,
                    Y = 345 + offsetY,
                    ButtonAction = ButtonAction.Activate
                }
            );
            _defaultbutton.SetTooltip("Varsayılan ayarları geri yükle");
            // end: Default button

            // start: Okay button
            Add
            (
                _okbutton = new Button((int)Buttons.Ok, 0x00F9, 0x00F8, 0x00F7)
                {
                    X = 443 + offsetX,
                    Y = 345 + offsetY,
                    ButtonAction = ButtonAction.Activate
                }
            );
            _okbutton.SetTooltip("Değişiklikleri kaydet");
            // end: Okay button

            AcceptMouseInput = true;
            CanMove = true;
            CanCloseWithRightClick = true;

            BuildGeneral();
            BuildSounds();
            BuildVideo();
            BuildCommands();
            BuildFonts();
            BuildSpeech();
            BuildCombat();
            BuildTooltip();
            BuildCounters();
            BuildInfoBar();
            BuildContainers();
            BuildExperimental();

            ChangePage(1);
        }

        /*
        private static Texture2D LogoTexture
        {
            get
            {
                if (_logoTexture2D == null || _logoTexture2D.IsDisposed)
                {
                    using var stream = new MemoryStream(Loader.GetCuoLogo().ToArray());
                    _logoTexture2D = Texture2D.FromStream(Client.Game.GraphicsDevice, stream);
                }

                return _logoTexture2D;
            }
        }
        */

        private void ButtonGraphicSet(Button element, ushort graphic)
        {
            /*
            _soundButton.ButtonGraphicNormal = 0xDA;
            _macroButton.ButtonGraphicNormal = 0xED;
            _speechButton.ButtonGraphicNormal = 0xE0;
            _toolTipsButton.ButtonGraphicNormal = 0xDC;
            _fontsButton.ButtonGraphicNormal = 0xDE;
            _displayButton.ButtonGraphicNormal = 0xE4;
            _generalButton.ButtonGraphicNormal = 0xE8;
            _filterButton.ButtonGraphicNormal = 0xEB;
            
            _soundButton.ButtonGraphicOver = 0xDA;
            _macroButton.ButtonGraphicOver = 0xED;
            _speechButton.ButtonGraphicOver = 0xE0;
            _toolTipsButton.ButtonGraphicOver = 0xDC;
            _fontsButton.ButtonGraphicOver = 0xDE;
            _displayButton.ButtonGraphicOver = 0xE4;
            _generalButton.ButtonGraphicOver = 0xE8;
            _filterButton.ButtonGraphicOver = 0xEB;

            _soundButton.ButtonGraphicPressed = 0xDA;
            _macroButton.ButtonGraphicPressed = 0xED;
            _speechButton.ButtonGraphicPressed = 0xE0;
            _toolTipsButton.ButtonGraphicPressed = 0xDC;
            _fontsButton.ButtonGraphicPressed = 0xDE;
            _displayButton.ButtonGraphicPressed = 0xE4;
            _generalButton.ButtonGraphicPressed = 0xE8;
            _filterButton.ButtonGraphicPressed = 0xEB;
            */

            element.ButtonGraphicNormal = graphic;
        }


        private void BuildGeneral()
        {
            const int PAGE = 1;

            ScrollArea rightArea = new ScrollArea
            (
                MAIN_X + 70,    // sağdan sola
                MAIN_Y + 50,    // yukardan aşağıya
                WIDTH - 35,     // genişlik
                340,            // yükseklik
                true
            );

            //ButtonGraphicSet(_generalButton, 0xE8);

            int startX = 5;
            int startY = 5;


            DataBox box = new DataBox(startX, startY, rightArea.Width - 15, 1);
            box.WantUpdateSize = true;
            rightArea.Add(box);


            SettingsSection section = AddSettingsSection(box, "Genel");

            section.Add
            (
                _clientnotifyballon = AddCheckBox
                (
                    null,
                    "Anlık bildirimleri aç",
                    _currentProfile.ClientNotifyBalloonActive,
                    startX,
                    startY
                )
            );

            section.Add
            (
                _inputautofocused = AddCheckBox
                (
                    null,
                    "Yazım yerlerine otomatik odaklan",
                    _currentProfile.InputAutoFocused,
                    startX,
                    startY
                )
            );

            section.Add
            (
                _highlightObjects = AddCheckBox
                (
                    null,
                    ResGumps.HighlightObjects,
                    _currentProfile.HighlightGameObjects,
                    startX,
                    startY
                )
            );

            section.Add
            (
                _enablePathfind = AddCheckBox
                (
                    null,
                    ResGumps.EnablePathfinding,
                    _currentProfile.EnablePathfind,
                    startX,
                    startY
                )
            );

            section.AddRight
            (
                _useShiftPathfind = AddCheckBox
                (
                    null,
                    ResGumps.ShiftPathfinding,
                    _currentProfile.UseShiftToPathfind,
                    startX,
                    startY
                )
            );

            section.Add
            (
                _alwaysRun = AddCheckBox
                (
                    null,
                    ResGumps.AlwaysRun,
                    _currentProfile.AlwaysRun,
                    startX,
                    startY
                )
            );

            section.AddRight
            (
                _alwaysRunUnlessHidden = AddCheckBox
                (
                    null,
                    ResGumps.AlwaysRunHidden,
                    _currentProfile.AlwaysRunUnlessHidden,
                    startX,
                    startY
                )
            );


            section.Add
            (
                _autoOpenDoors = AddCheckBox
                (
                    null,
                    ResGumps.AutoOpenDoors,
                    _currentProfile.AutoOpenDoors,
                    startX,
                    startY,
                    true
                )
            );

            section.AddRight
            (
                _smoothDoors = AddCheckBox
                (
                    null,
                    ResGumps.SmoothDoors,
                    _currentProfile.SmoothDoors,
                    startX,
                    startY
                )
            );

            section.Add
            (
                _autoOpenCorpse = AddCheckBox
                (
                    null,
                    ResGumps.AutoOpenCorpses,
                    _currentProfile.AutoOpenCorpses,
                    startX,
                    startY,
                    true
                )
            );


            section.PushIndent();
            section.Add(AddLabel(null, ResGumps.CorpseOpenRange, 0, 0));

            section.AddRight
            (
                _autoOpenCorpseRange = AddInputField
                (
                    null,
                    startX,
                    startY,
                    50,
                    TEXTBOX_HEIGHT,
                    ResGumps.CorpseOpenRange,
                    50,
                    false,
                    true,
                    5
                )
            );

            _autoOpenCorpseRange.SetText(_currentProfile.AutoOpenCorpseRange.ToString());

            section.Add
            (
                _skipEmptyCorpse = AddCheckBox
                (
                    null,
                    ResGumps.SkipEmptyCorpses,
                    _currentProfile.SkipEmptyCorpse,
                    startX,
                    startY,
                    true
                )
            );

            section.Add(AddLabel(null, ResGumps.CorpseOpenOptions, startX, startY));

            section.AddRight
            (
                _autoOpenCorpseOptions = AddCombobox
                (
                    null,
                    new[]
                    {
                        ResGumps.CorpseOpt_None, ResGumps.CorpseOpt_NotTar, ResGumps.CorpseOpt_NotHid,
                        ResGumps.CorpseOpt_Both
                    },
                    _currentProfile.CorpseOpenOptions,
                    startX,
                    startY,
                    150,
                    true
                ),
                2
            );


            section.PopIndent();

            section.Add
            (
                _noColorOutOfRangeObjects = AddCheckBox
                (
                    rightArea,
                    ResGumps.OutOfRangeColor,
                    _currentProfile.NoColorObjectsOutOfRange,
                    startX,
                    startY
                )
            );

            section.Add
            (
                _sallosEasyGrab = AddCheckBox
                (
                    null,
                    ResGumps.SallosEasyGrab,
                    _currentProfile.SallosEasyGrab,
                    startX,
                    startY,
                    true
                )
            );

            section.Add
            (
                _showHouseContent = AddCheckBox
                (
                    null,
                    ResGumps.ShowHousesContent,
                    _currentProfile.ShowHouseContent,
                    startX,
                    startY,
                    true
                )
            );

            _showHouseContent.IsVisible = Client.Game.UO.Version >= ClientVersion.CV_70796;

            section.Add
            (
                _use_smooth_boat_movement = AddCheckBox
                (
                    null,
                    ResGumps.SmoothBoat,
                    _currentProfile.UseSmoothBoatMovement,
                    startX,
                    startY
                )
            );

            _use_smooth_boat_movement.IsVisible = Client.Game.UO.Version >= ClientVersion.CV_7090;


            SettingsSection section2 = AddSettingsSection(box, "Mobiles");
            section2.Y = section.Bounds.Bottom + 40;

            section2.Add
            (
                _showHpMobile = AddCheckBox
                (
                    null,
                    ResGumps.ShowHP,
                    _currentProfile.ShowMobilesHP,
                    startX,
                    startY
                )
            );

            int mode = _currentProfile.MobileHPType;

            if (mode < 0 || mode > 2)
            {
                mode = 0;
            }

            section2.AddRight
            (
                _hpComboBox = AddCombobox
                (
                    null,
                    new[] { ResGumps.HP_Percentage, ResGumps.HP_Line, ResGumps.HP_Both },
                    mode,
                    startX,
                    startY,
                    100
                )
            );

            section2.AddRight(AddLabel(null, ResGumps.HP_Mode, startX, startY));

            mode = _currentProfile.MobileHPShowWhen;

            if (mode != 0 && mode > 2)
            {
                mode = 0;
            }

            section2.AddRight
            (
                _hpComboBoxShowWhen = AddCombobox
                (
                    null,
                    new[] { ResGumps.HPShow_Always, ResGumps.HPShow_Less, ResGumps.HPShow_Smart },
                    mode,
                    startX,
                    startY,
                    100
                ),
                2
            );

            section2.Add
            (
                _highlightByPoisoned = AddCheckBox
                (
                    null,
                    ResGumps.HighlightPoisoned,
                    _currentProfile.HighlightMobilesByPoisoned,
                    startX,
                    startY
                )
            );

            section2.PushIndent();

            section2.Add
            (
                _poisonColorPickerBox = AddColorBox
                (
                    null,
                    startX,
                    startY,
                    _currentProfile.PoisonHue,
                    ResGumps.PoisonedColor
                )
            );

            section2.AddRight(AddLabel(null, ResGumps.PoisonedColor, 0, 0), 2);
            section2.PopIndent();

            section2.Add
            (
                _highlightByParalyzed = AddCheckBox
                (
                    null,
                    ResGumps.HighlightParalyzed,
                    _currentProfile.HighlightMobilesByParalize,
                    startX,
                    startY
                )
            );

            section2.PushIndent();

            section2.Add
            (
                _paralyzedColorPickerBox = AddColorBox
                (
                    null,
                    startX,
                    startY,
                    _currentProfile.ParalyzedHue,
                    ResGumps.ParalyzedColor
                )
            );

            section2.AddRight(AddLabel(null, ResGumps.ParalyzedColor, 0, 0), 2);

            section2.PopIndent();

            section2.Add
            (
                _highlightByInvul = AddCheckBox
                (
                    null,
                    ResGumps.HighlightInvulnerable,
                    _currentProfile.HighlightMobilesByInvul,
                    startX,
                    startY
                )
            );

            section2.PushIndent();

            section2.Add
            (
                _invulnerableColorPickerBox = AddColorBox
                (
                    null,
                    startX,
                    startY,
                    _currentProfile.InvulnerableHue,
                    ResGumps.InvulColor
                )
            );

            section2.AddRight(AddLabel(null, ResGumps.InvulColor, 0, 0), 2);
            section2.PopIndent();

            section2.Add
            (
                _showMobileNameIncoming = AddCheckBox
                (
                    null,
                    ResGumps.ShowIncMobiles,
                    _currentProfile.ShowNewMobileNameIncoming,
                    startX,
                    startY
                )
            );

            section2.Add
            (
                _showCorpseNameIncoming = AddCheckBox
                (
                    null,
                    ResGumps.ShowIncCorpses,
                    _currentProfile.ShowNewCorpseNameIncoming,
                    startX,
                    startY
                )
            );

            section2.Add(AddLabel(null, ResGumps.AuraUnderFeet, startX, startY));

            section2.AddRight
            (
                _auraType = AddCombobox
                (
                    null,
                    new[]
                    {
                        ResGumps.AuraType_None, ResGumps.AuraType_Warmode, ResGumps.AuraType_CtrlShift,
                        ResGumps.AuraType_Always
                    },
                    _currentProfile.AuraUnderFeetType,
                    startX,
                    startY,
                    100
                ),
                2
            );

            section2.PushIndent();

            section2.Add
            (
                _partyAura = AddCheckBox
                (
                    null,
                    ResGumps.CustomColorAuraForPartyMembers,
                    _currentProfile.PartyAura,
                    startX,
                    startY
                )
            );

            section2.PushIndent();

            section2.Add
            (
                _partyAuraColorPickerBox = AddColorBox
                (
                    null,
                    startX,
                    startY,
                    _currentProfile.PartyAuraHue,
                    ResGumps.PartyAuraColor
                )
            );

            section2.AddRight(AddLabel(null, ResGumps.PartyAuraColor, 0, 0));
            section2.PopIndent();
            section2.PopIndent();

            SettingsSection section3 = AddSettingsSection(box, "Gumps & Context");
            section3.Y = section2.Bounds.Bottom + 40;

            section3.Add
            (
                _enableTopbar = AddCheckBox
                (
                    null,
                    ResGumps.DisableMenu,
                    _currentProfile.TopbarGumpIsDisabled,
                    0,
                    0
                )
            );

            section3.Add
            (
                _holdDownKeyAlt = AddCheckBox
                (
                    null,
                    ResGumps.AltCloseGumps,
                    _currentProfile.HoldDownKeyAltToCloseAnchored,
                    0,
                    0
                )
            );

            section3.Add
            (
                _holdAltToMoveGumps = AddCheckBox
                (
                    null,
                    ResGumps.AltMoveGumps,
                    _currentProfile.HoldAltToMoveGumps,
                    0,
                    0
                )
            );

            section3.Add
            (
                _closeAllAnchoredGumpsWithRClick = AddCheckBox
                (
                    null,
                    ResGumps.ClickCloseAllGumps,
                    _currentProfile.CloseAllAnchoredGumpsInGroupWithRightClick,
                    0,
                    0
                )
            );

            section3.Add
            (
                _useStandardSkillsGump = AddCheckBox
                (
                    null,
                    ResGumps.StandardSkillGump,
                    _currentProfile.StandardSkillsGump,
                    0,
                    0
                )
            );

            section3.Add
            (
                _use_old_status_gump = AddCheckBox
                (
                    null,
                    ResGumps.UseOldStatusGump,
                    _currentProfile.UseOldStatusGump,
                    startX,
                    startY,
                    true
                )
            );

            _use_old_status_gump.IsVisible = !CUOEnviroment.IsOutlands;

            section3.Add
            (
                _partyInviteGump = AddCheckBox
                (
                    null,
                    ResGumps.ShowGumpPartyInv,
                    _currentProfile.PartyInviteGump,
                    0,
                    0
                )
            );

            section3.Add
            (
                _customBars = AddCheckBox
                (
                    null,
                    ResGumps.UseCustomHPBars,
                    _currentProfile.CustomBarsToggled,
                    0,
                    0,
                    true
                )
            );

            section3.AddRight
            (
                _customBarsBBG = AddCheckBox
                (
                    null,
                    ResGumps.UseBlackBackgr,
                    _currentProfile.CBBlackBGToggled,
                    0,
                    0,
                    true
                )
            );

            section3.Add
            (
                _saveHealthbars = AddCheckBox
                (
                    null,
                    ResGumps.SaveHPBarsOnLogout,
                    _currentProfile.SaveHealthbars,
                    0,
                    0
                )
            );

            section3.PushIndent();
            section3.Add(AddLabel(null, ResGumps.CloseHPGumpWhen, 0, 0));

            mode = _currentProfile.CloseHealthBarType;

            if (mode < 0 || mode > 2)
            {
                mode = 0;
            }

            _healtbarType = AddCombobox
            (
                null,
                new[] { ResGumps.HPType_None, ResGumps.HPType_MobileOOR, ResGumps.HPType_MobileDead },
                mode,
                0,
                0,
                150,
                true
            );

            section3.AddRight(_healtbarType);

            section3.PopIndent();

            section3.Add(AddLabel(null, ResGumps.GridLoot, startX, startY));

            section3.AddRight
            (
                _gridLoot = AddCombobox
                (
                    null,
                    new[] { ResGumps.GridLoot_None, ResGumps.GridLoot_GridOnly, ResGumps.GridLoot_Both },
                    _currentProfile.GridLootType,
                    startX,
                    startY,
                    120,
                    true
                ),
                2
            );

            section3.Add(AddLabel(null, "UI Type", startX, startY));

            section3.AddRight
            (
                _uiType = AddCombobox
                (
                    null,
                    new[] { "Default","Style-1" },
                    _currentProfile.UIType,
                    startX,
                    startY,
                    120,
                    false
                ),
                2
            );

            section3.Add
            (
                _holdShiftForContext = AddCheckBox
                (
                    null,
                    ResGumps.ShiftContext,
                    _currentProfile.HoldShiftForContext,
                    0,
                    0
                )
            );

            section3.Add
            (
                _holdShiftToSplitStack = AddCheckBox
                (
                    null,
                    ResGumps.ShiftStack,
                    _currentProfile.HoldShiftToSplitStack,
                    0,
                    0
                )
            );


            SettingsSection section4 = AddSettingsSection(box, "Miscellaneous");
            section4.Y = section3.Bounds.Bottom + 40;

            section4.Add
            (
                _useCircleOfTransparency = AddCheckBox
                (
                    null,
                    ResGumps.EnableCircleTrans,
                    _currentProfile.UseCircleOfTransparency,
                    startX,
                    startY
                )
            );

            section4.AddRight
            (
                _circleOfTranspRadius = AddHSlider
                (
                    null,
                    Constants.MIN_CIRCLE_OF_TRANSPARENCY_RADIUS,
                    Constants.MAX_CIRCLE_OF_TRANSPARENCY_RADIUS,
                    _currentProfile.CircleOfTransparencyRadius,
                    startX,
                    startY,
                    200
                )
            );

            section4.PushIndent();
            section4.Add(AddLabel(null, ResGumps.CircleTransType, startX, startY));
            int cottypeindex = _currentProfile.CircleOfTransparencyType;
            string[] cotTypes = { ResGumps.CircleTransType_Full, ResGumps.CircleTransType_Gradient };

            if (cottypeindex < 0 || cottypeindex > cotTypes.Length)
            {
                cottypeindex = 0;
            }

            section4.AddRight
            (
                _cotType = AddCombobox
                (
                    null,
                    cotTypes,
                    cottypeindex,
                    startX,
                    startY,
                    150
                ),
                2
            );

            section4.PopIndent();

            section4.Add
            (
                _hideScreenshotStoredInMessage = AddCheckBox
                (
                    null,
                    ResGumps.HideScreenshotStoredInMessage,
                    _currentProfile.HideScreenshotStoredInMessage,
                    0,
                    0
                )
            );

            section4.Add
            (
                _objectsFading = AddCheckBox
                (
                    null,
                    ResGumps.ObjAlphaFading,
                    _currentProfile.UseObjectsFading,
                    startX,
                    startY
                )
            );

            section4.Add
            (
                _textFading = AddCheckBox
                (
                    null,
                    ResGumps.TextAlphaFading,
                    _currentProfile.TextFading,
                    startX,
                    startY
                )
            );

            section4.Add
            (
                _showTargetRangeIndicator = AddCheckBox
                (
                    null,
                    ResGumps.ShowTarRangeIndic,
                    _currentProfile.ShowTargetRangeIndicator,
                    startX,
                    startY
                )
            );

            section4.Add
            (
                _enableDragSelect = AddCheckBox
                (
                    null,
                    ResGumps.EnableDragHPBars,
                    _currentProfile.EnableDragSelect,
                    startX,
                    startY
                )
            );

            section4.PushIndent();
            section4.Add(AddLabel(null, ResGumps.DragKey, startX, startY));

            section4.AddRight
            (
                _dragSelectModifierKey = AddCombobox
                (
                    null,
                    new[] { ResGumps.KeyMod_None, ResGumps.KeyMod_Ctrl, ResGumps.KeyMod_Shift },
                    _currentProfile.DragSelectModifierKey,
                    startX,
                    startY,
                    100
                )
            );

            section4.Add
            (
                _dragSelectHumanoidsOnly = AddCheckBox
                (
                    null,
                    ResGumps.DragHumanoidsOnly,
                    _currentProfile.DragSelectHumanoidsOnly,
                    startX,
                    startY
                )
            );

            section4.Add(new Label(ResGumps.DragSelectStartingPosX, true, HUE_FONT));
            section4.Add(_dragSelectStartX = new HSliderBar(startX, startY, 200, 0, Client.Game.Scene.Camera.Bounds.Width, _currentProfile.DragSelectStartX, HSliderBarStyle.MetalWidgetRecessedBar, true, 0, HUE_FONT));

            section4.Add(new Label(ResGumps.DragSelectStartingPosY, true, HUE_FONT));
            section4.Add(_dragSelectStartY = new HSliderBar(startX, startY, 200, 0, Client.Game.Scene.Camera.Bounds.Height, _currentProfile.DragSelectStartY, HSliderBarStyle.MetalWidgetRecessedBar, true, 0, HUE_FONT));
            section4.Add
            (
                _dragSelectAsAnchor = AddCheckBox
                (
                    null, ResGumps.DragSelectAnchoredHB, _currentProfile.DragSelectAsAnchor, startX,
                    startY
                )
            );

            section4.PopIndent();

            section4.Add
            (
                _showStatsMessage = AddCheckBox
                (
                    null,
                    ResGumps.ShowStatsChangedMessage,
                    _currentProfile.ShowStatsChangedMessage,
                    startX,
                    startY
                )
            );

            section4.Add
            (
                _showSkillsMessage = AddCheckBox
                (
                    null,
                    ResGumps.ShowSkillsChangedMessageBy,
                    _currentProfile.ShowStatsChangedMessage,
                    startX,
                    startY
                )
            );

            section4.PushIndent();

            section4.AddRight
            (
                _showSkillsMessageDelta = AddHSlider
                (
                    null,
                    0,
                    100,
                    _currentProfile.ShowSkillsChangedDeltaValue,
                    startX,
                    startY,
                    150
                )
            );

            section4.PopIndent();


            SettingsSection section5 = AddSettingsSection(box, "Terrain & Statics");
            section5.Y = section4.Bounds.Bottom + 40;

            section5.Add
            (
                _drawRoofs = AddCheckBox
                (
                    null,
                    ResGumps.HideRoofTiles,
                    //!_currentProfile.DrawRoofs,
                    false,
                    startX,
                    startY,
                    true
                )
            );

            section5.Add
            (
                _treeToStumps = AddCheckBox
                (
                    null,
                    ResGumps.TreesStumps,
                    _currentProfile.TreeToStumps,
                    startX,
                    startY,
                    true
                )
            );

            section5.Add
            (
                _hideVegetation = AddCheckBox
                (
                    null,
                    ResGumps.HideVegetation,
                    _currentProfile.HideVegetation,
                    startX,
                    startY
                )
            );

            section5.Add
            (
                _enableCaveBorder = AddCheckBox
                (
                    null,
                    ResGumps.MarkCaveTiles,
                    _currentProfile.EnableCaveBorder,
                    startX,
                    startY
                )
            );


            section5.Add(AddLabel(null, ResGumps.HPFields, startX, startY));

            mode = _currentProfile.FieldsType;

            if (mode < 0 || mode > 2)
            {
                mode = 0;
            }

            section5.AddRight
            (
                _fieldsType = AddCombobox
                (
                    null,
                    new[] { ResGumps.HPFields_Normal, ResGumps.HPFields_Static, ResGumps.HPFields_Tile },
                    mode,
                    startX,
                    startY,
                    150,
                    true
                )
            );


            Add(rightArea, PAGE);
        }

        private void BuildSounds()
        {
            const int PAGE = 2;

            ScrollArea rightArea = new ScrollArea
            (
                MAIN_X + 70,      // sağdan sola
                MAIN_Y + 50,        // yukardan aşağıya
                WIDTH - 35,     // genişlik
                340,            // yükseklik
                true
            );

            //ButtonGraphicSet(_soundButton, 0xD9);

            int startX = 5;
            int startY = 5;

            const int VOLUME_WIDTH = 200;

            _enableSounds = AddCheckBox
            (
                rightArea,
                ResGumps.Sounds,
                _currentProfile.EnableSound,
                startX,
                startY
            );

            _enableMusic = AddCheckBox
            (
                rightArea,
                ResGumps.Music,
                _currentProfile.EnableMusic,
                startX,
                startY + _enableSounds.Height + 2
            );

            _loginMusic = AddCheckBox
            (
                rightArea,
                ResGumps.LoginMusic,
                Settings.GlobalSettings.LoginMusic,
                startX,
                startY + _enableSounds.Height + 2 + _enableMusic.Height + 2
            );

            startX = 120;
            startY += 2;

            _soundsVolume = AddHSlider
            (
                rightArea,
                0,
                100,
                _currentProfile.SoundVolume,
                startX,
                startY,
                VOLUME_WIDTH
            );

            _musicVolume = AddHSlider
            (
                rightArea,
                0,
                100,
                _currentProfile.MusicVolume,
                startX,
                startY + _enableSounds.Height + 2,
                VOLUME_WIDTH
            );

            _loginMusicVolume = AddHSlider
            (
                rightArea,
                0,
                100,
                Settings.GlobalSettings.LoginMusicVolume,
                startX,
                startY + _enableSounds.Height + 2 + _enableMusic.Height + 2,
                VOLUME_WIDTH
            );

            startX = 5;
            startY += _loginMusic.Bounds.Bottom + 2;

            _footStepsSound = AddCheckBox
            (
                rightArea,
                ResGumps.PlayFootsteps,
                _currentProfile.EnableFootstepsSound,
                startX,
                startY
            );

            startY += _footStepsSound.Height + 2;

            _combatMusic = AddCheckBox
            (
                rightArea,
                ResGumps.CombatMusic,
                _currentProfile.EnableCombatMusic,
                startX,
                startY
            );

            startY += _combatMusic.Height + 2;

            _musicInBackground = AddCheckBox
            (
                rightArea,
                ResGumps.ReproduceSoundsAndMusic,
                _currentProfile.ReproduceSoundsInBackground,
                startX,
                startY
            );

            startY += _musicInBackground.Height + 2;

            Add(rightArea, PAGE);
        }


        private void BuildVideo()
        {
            const int PAGE = 3;

            ScrollArea rightArea = new ScrollArea
            (
                MAIN_X + 70,      // sağdan sola
                MAIN_Y + 50,        // yukardan aşağıya
                WIDTH - 35,     // genişlik
                340,            // yükseklik
                true
            );

            int startX = 5;
            int startY = 5;


            Label text = AddLabel(rightArea, ResGumps.FPS, startX, startY);
            startX += text.Bounds.Right + 5;

            _sliderFPS = AddHSlider
            (
                rightArea,
                Constants.MIN_FPS,
                Constants.MAX_FPS,
                Settings.GlobalSettings.FPS,
                startX,
                startY,
                250
            );

            startY += text.Bounds.Bottom + 5;

            _reduceFPSWhenInactive = AddCheckBox
            (
                rightArea,
                ResGumps.FPSInactive,
                _currentProfile.ReduceFPSWhenInactive,
                startX,
                startY
            );

            startY += _reduceFPSWhenInactive.Height + 2;

            startX = 5;
            startY += 20;


            DataBox box = new DataBox(startX, startY, rightArea.Width - 15, 1);
            box.WantUpdateSize = true;
            rightArea.Add(box);

            SettingsSection section = AddSettingsSection(box, "Game window");

            section.Add
            (
                _gameWindowFullsize = AddCheckBox
                (
                    null,
                    ResGumps.AlwaysUseFullsizeGameWindow,
                    _currentProfile.GameWindowFullSize,
                    startX,
                    startY
                )
            );

            section.Add
            (
                _windowBorderless = AddCheckBox
                (
                    null,
                    ResGumps.BorderlessWindow,
                    _currentProfile.WindowBorderless,
                    startX,
                    startY
                )
            );

            section.Add
            (
                _gameWindowLock = AddCheckBox
                (
                    null,
                    ResGumps.LockGameWindowMovingResizing,
                    _currentProfile.GameWindowLock,
                    startX,
                    startY
                )
            );

            section.Add(AddLabel(null, ResGumps.GamePlayWindowPosition, startX, startY));

            section.AddRight
            (
                _gameWindowPositionX = AddInputField
                (
                    null,
                    startX,
                    startY,
                    50,
                    TEXTBOX_HEIGHT,
                    null,
                    50,
                    false,
                    true
                ),
                4
            );

            var camera = Client.Game.Scene.Camera;

            _gameWindowPositionX.SetText(camera.Bounds.X.ToString());

            section.AddRight
            (
                _gameWindowPositionY = AddInputField
                (
                    null,
                    startX,
                    startY,
                    50,
                    TEXTBOX_HEIGHT,
                    null,
                    50,
                    false,
                    true
                )
            );

            _gameWindowPositionY.SetText(camera.Bounds.Y.ToString());


            section.Add(AddLabel(null, ResGumps.GamePlayWindowSize, startX, startY));

            section.AddRight
            (
                _gameWindowWidth = AddInputField
                (
                    null,
                    startX,
                    startY,
                    50,
                    TEXTBOX_HEIGHT,
                    null,
                    50,
                    false,
                    true
                )
            );

            _gameWindowWidth.SetText(camera.Bounds.Width.ToString());

            section.AddRight
            (
                _gameWindowHeight = AddInputField
                (
                    null,
                    startX,
                    startY,
                    50,
                    TEXTBOX_HEIGHT,
                    null,
                    50,
                    false,
                    true
                )
            );

            _gameWindowHeight.SetText(camera.Bounds.Height.ToString());


            SettingsSection section2 = AddSettingsSection(box, "Zoom");
            section2.Y = section.Bounds.Bottom + 40;
            section2.Add(AddLabel(null, ResGumps.DefaultZoom, startX, startY));

            var cameraZoomCount = (int)((camera.ZoomMax - camera.ZoomMin) / camera.ZoomStep);
            var cameraZoomIndex = cameraZoomCount - (int)((camera.ZoomMax - camera.Zoom) / camera.ZoomStep);

            section2.AddRight
            (
                _sliderZoom = AddHSlider
                (
                    null,
                    0,
                    cameraZoomCount,
                    cameraZoomIndex,
                    startX,
                    startY,
                    100
                )
            );

            section2.Add
            (
                _zoomCheckbox = AddCheckBox
                (
                    null,
                    ResGumps.EnableMouseWheelForZoom,
                    _currentProfile.EnableMousewheelScaleZoom,
                    startX,
                    startY
                )
            );

            section2.Add
            (
                _restorezoomCheckbox = AddCheckBox
                (
                    null,
                    ResGumps.ReleasingCtrlRestoresScale,
                    _currentProfile.RestoreScaleAfterUnpressCtrl,
                    startX,
                    startY
                )
            );


            SettingsSection section3 = AddSettingsSection(box, "Lights");
            section3.Y = section2.Bounds.Bottom + 40;

            section3.Add
            (
                _altLights = AddCheckBox
                (
                    null,
                    ResGumps.AlternativeLights,
                    _currentProfile.UseAlternativeLights,
                    startX,
                    startY,
                    true
                )
            );

            section3.Add
            (
                _enableLight = AddCheckBox
                (
                    null,
                    ResGumps.LightLevel,
                    _currentProfile.UseCustomLightLevel,
                    startX,
                    startY,
                    true
                )
            );

            section3.AddRight
            (
                _lightBar = AddHSlider
                (
                    null,
                    0,
                    0x1E,
                    0x1E - _currentProfile.LightLevel,
                    startX,
                    startY,
                    250
                )
            );

            section3.Add(AddLabel(null, ResGumps.LightLevelType, startX, startY));

            section3.AddRight
            (
                _lightLevelType = AddCombobox
                (
                    null,
                    new[] { ResGumps.LightLevelTypeAbsolute, ResGumps.LightLevelTypeMinimum },
                    _currentProfile.LightLevelType,
                    startX,
                    startY,
                    150,
                    true
                )
            );

            section3.Add
            (
                _darkNights = AddCheckBox
                (
                    null,
                    ResGumps.DarkNights,
                    _currentProfile.UseDarkNights,
                    startX,
                    startY,
                    true
                )
            );

            section3.Add
            (
                _useColoredLights = AddCheckBox
                (
                    null,
                    ResGumps.UseColoredLights,
                    _currentProfile.UseColoredLights,
                    startX,
                    startY
                )
            );


            SettingsSection section4 = AddSettingsSection(box, "Misc");
            section4.Y = section3.Bounds.Bottom + 40;

            section4.Add
            (
                _enableDeathScreen = AddCheckBox
                (
                    null,
                    ResGumps.EnableDeathScreen,
                    _currentProfile.EnableDeathScreen,
                    startX,
                    startY,
                    true
                )
            );

            section4.AddRight
            (
                _enableBlackWhiteEffect = AddCheckBox
                (
                    null,
                    ResGumps.BlackWhiteModeForDeadPlayer,
                    _currentProfile.EnableBlackWhiteEffect,
                    startX,
                    startY,
                    true
                )
            );

            section4.Add
            (
                _runMouseInSeparateThread = AddCheckBox
                (
                    null,
                    ResGumps.RunMouseInASeparateThread,
                    Settings.GlobalSettings.RunMouseInASeparateThread,
                    startX,
                    startY
                )
            );

            section4.Add
            (
                _auraMouse = AddCheckBox
                (
                    null,
                    ResGumps.AuraOnMouseTarget,
                    _currentProfile.AuraOnMouse,
                    startX,
                    startY
                )
            );

            section4.Add
            (
                _animatedWaterEffect = AddCheckBox
                (
                    null,
                    ResGumps.AnimatedWaterEffect,
                    _currentProfile.AnimatedWaterEffect,
                    startX,
                    startY
                )
            );


            SettingsSection section5 = AddSettingsSection(box, "Shadows");
            section5.Y = section4.Bounds.Bottom + 40;

            section5.Add
            (
                _enableShadows = AddCheckBox
                (
                    null,
                    ResGumps.Shadows,
                    _currentProfile.ShadowsEnabled,
                    startX,
                    startY
                )
            );

            section5.PushIndent();

            section5.Add
            (
                _enableShadowsStatics = AddCheckBox
                (
                    null,
                    ResGumps.ShadowStatics,
                    _currentProfile.ShadowsStatics,
                    startX,
                    startY
                )
            );

            section5.PopIndent();

            section5.Add(AddLabel(null, ResGumps.TerrainShadowsLevel, startX, startY));
            section5.AddRight(_terrainShadowLevel = AddHSlider(null, Constants.MIN_TERRAIN_SHADOWS_LEVEL, Constants.MAX_TERRAIN_SHADOWS_LEVEL, _currentProfile.TerrainShadowsLevel, startX, startY, 200));

            Add(rightArea, PAGE);
        }


        private void BuildCommands()
        {
            const int PAGE = 4;

            ScrollArea rightArea = new ScrollArea
            (
                MAIN_X + 70,      // sağdan sola
                MAIN_Y + 50,        // yukardan aşağıya
                WIDTH - 35,     // genişlik
                340,            // yükseklik
                true
            );


            Add
            (
                new Line
                (
                    190,
                    52 + 25 + 2,
                    150,
                    1,
                    Color.Gray.PackedValue
                ),
                PAGE
            );

            Add
            (
                new Line
                (
                    191 + 150,
                    21,
                    1,
                    418,
                    Color.Gray.PackedValue
                ),
                PAGE
            );

            Add
            (
                _addButton = new Button((int)Buttons.NewMacro, 0x99C, 0x99D, 0x99E)
                {
                    X = MAIN_X + 220,
                    Y = MAIN_Y + 100,
                    ButtonAction = ButtonAction.Activate
                }
            );
            _addButton.SetTooltip("Yeni macro ekle");
            Add(_addButton, PAGE);


            Add
            (
                _delButton = new Button((int)Buttons.DeleteMacro, 0x99F, 0x9A0, 0x9A1)
                {
                    X = MAIN_X + 275,
                    Y = MAIN_Y + 100,
                    ButtonAction = ButtonAction.Activate
                }
            );
            _delButton.SetTooltip("Macro sil");
            Add(_delButton, PAGE);

            Add
            (
                _prevButton = new Button((int)Buttons.PrevMacroButton, 0x9A2, 0x9A3, 0x9A4)
                {
                    X = MAIN_X + 345,
                    Y = MAIN_Y + 100,
                    ButtonAction = ButtonAction.Activate
                }
            );
            _prevButton.SetTooltip("Önceki macro");
            Add(_prevButton, PAGE);

            Add
            (
                _nextButton = new Button((int)Buttons.NextMacroButton, 0x9A5, 0x9A6, 0x9A7)
                {
                    X = MAIN_X + 430,
                    Y = MAIN_Y + 100,
                    ButtonAction = ButtonAction.Activate
                }
            );
            _nextButton.SetTooltip("Sonraki macro");

            Add(_nextButton, PAGE);


            Add(_alldelButton = new NiceButton(100, 405, 90, 20, ButtonAction.Activate, "Hepsini sil", 0, TEXT_ALIGN_TYPE.TS_CENTER, 0x28)
            {
                IsSelectable = false,
                ButtonParameter = (int)Buttons.AllDeleteMacro
            });

            Add(_alldelButton, PAGE);


            Add(_convertMacro = new NiceButton(450, 30, 90, 20, ButtonAction.Activate, "Macro aktar", 0, TEXT_ALIGN_TYPE.TS_CENTER, 0x44)
            {
                IsSelectable = false,
                ButtonParameter = (int)Buttons.ConvertMacro
            });

            Add(_convertMacro, PAGE);


            Add(_openFolder = new NiceButton(545, 30, 90, 20, ButtonAction.Activate, "Klasörü aç", 0, TEXT_ALIGN_TYPE.TS_CENTER, 0xFF)
            {
                IsSelectable = false,
                ButtonParameter = (int)Buttons.OpenMacrosFolder
            });

            Add(_openFolder, PAGE);

            int startX = 5;
            int startY = 5;

            DataBox databox = new DataBox(startX, startY, 1, 1);
            databox.WantUpdateSize = true;
            rightArea.Add(databox);



            _addButton.MouseUp += (sender, e) =>
            {
                MacroManager manager = World.Macros;
                var allMacros = manager.GetAllMacros();
                Macro lastMacro = null;
                if (allMacros.Count > 0) lastMacro = allMacros.Last();

                string name = string.Empty;
                if (lastMacro is object)
                {
                    var lastMacroSplitted = lastMacro.Name.Split(':');
                    int macroCount = 0;
                    if (lastMacroSplitted.Count() == 2)
                    {
                        int.TryParse(lastMacroSplitted[1], out macroCount);
                        name = "Macro: " + (macroCount + 1);
                    }
                }

                if (string.IsNullOrEmpty(name))
                    name = "Macro: " + (manager.GetAllMacros().Count + 1);

                NiceButton nb;

                databox.Add
                (
                    nb = new NiceButton
                    (
                        0,
                        0,
                        130,
                        25,
                        ButtonAction.Activate,
                        name
                    )
                    {
                        ButtonParameter = (int)Buttons.Last + 1 + rightArea.Children.Count
                    }
                );

                databox.ReArrangeChildren();
                nb.IsSelected = true;

                _macroControl?.Dispose();

                _macroControl = new MacroControl(this, name)
                {
                    X = 270,
                    Y = 130
                };


                manager.PushToBack(_macroControl.Macro);
                Add(_macroControl, PAGE);


                nb.DragBegin += (sss, eee) =>
                {
                    if (UIManager.IsDragging || Math.Max(Math.Abs(Mouse.LDragOffset.X), Math.Abs(Mouse.LDragOffset.Y)) < 5 || nb.ScreenCoordinateX > Mouse.LClickPosition.X || nb.ScreenCoordinateX < Mouse.LClickPosition.X - nb.Width || nb.ScreenCoordinateY > Mouse.LClickPosition.Y || nb.ScreenCoordinateY + nb.Height < Mouse.LClickPosition.Y)
                    {
                        return;
                    }

                    MacroControl control = _macroControl.FindControls<MacroControl>().SingleOrDefault();

                    if (control == null)
                    {
                        return;
                    }

                    UIManager.Gumps.OfType<MacroButtonGump>().FirstOrDefault(s => s._macro == control.Macro)?.Dispose();
                    MacroButtonGump macroButtonGump = new MacroButtonGump(World, control.Macro, Mouse.Position.X, Mouse.Position.Y);
                    macroButtonGump.X = Mouse.LClickPosition.X + (macroButtonGump.Width >> 1);
                    macroButtonGump.Y = Mouse.LClickPosition.Y + (macroButtonGump.Height >> 1);

                    UIManager.Add(macroButtonGump);

                    UIManager.AttemptDragControl(macroButtonGump, true);
                };



                nb.MouseUp += (sss, eee) =>
                {
                    _macroControl?.Dispose();
                    _macroControl = new MacroControl(this, name)
                    {
                        X = 270,
                        Y = 130
                    };
                    Add(_macroControl, PAGE);
                };


            };

            _delButton.MouseUp += (ss, ee) =>
            {
                NiceButton nb = databox.FindControls<NiceButton>().SingleOrDefault(a => a.IsSelected);

                if (nb != null)
                {
                    if (_macroControl != null)
                    {
                        UIManager.Gumps.OfType<MacroButtonGump>().FirstOrDefault(s => s._macro == _macroControl.Macro)?.Dispose();

                        World.Macros.Remove(_macroControl.Macro);

                        _macroControl.Dispose();
                    }

                    nb.Dispose();
                    databox.ReArrangeChildren();
                }

            };

            _convertMacro.MouseUp += (ss, ee) =>
            {
                string username = Settings.GlobalSettings.Username;
                string servername = World.ServerName;
                string charactername = World.Player.Name;
                string rootpath;

                if (string.IsNullOrWhiteSpace(Settings.GlobalSettings.ProfilesPath))
                {
                    rootpath = Path.Combine(CUOEnviroment.ExecutablePath, "Data", "Profiles");
                }
                else
                {
                    rootpath = Settings.GlobalSettings.ProfilesPath;
                }

                string path = FileSystemHelper.CreateFolderIfNotExists(rootpath, username, servername, charactername);
                string fileToLoad = Path.Combine(path, "macros.txt");
                string fileToLoad2 = Path.Combine(path, "macros2d.txt");
                string fileToLoadXml = Path.Combine(path, "macros.xml");

                FileInfo fi = new FileInfo(fileToLoad2);
                if (fi.Exists)
                {
                    fi.MoveTo(path + "/macros.txt");
                    Console.WriteLine("macros2d.txt renamed macros.txt");
                }

                if (File.Exists(fileToLoad))
                {
                    Array.ForEach(UIManager.Gumps.OfType<MacroButtonGump>().ToArray(), m => m?.Dispose());
                    Array.ForEach(World.Macros.GetAllMacros().ToArray(), m => World.Macros.Remove(m));

                    MacroManager convManager = World.Macros;
                    //convMacro.Clear();
                    var file = new StreamReader(fileToLoad);
                    string line;
                    Macro macro = null;
                    int sira = 1;
                    var MacroSubTypeNameAlternate = MacroSubTypeName.Select(s => s.Replace(" ", "")).ToArray();

                    while ((line = file.ReadLine()) != null)
                    {
                        if (macro == null)
                        {
                            var ayirac = AllIndexesOf(line, " ");
                            var kodA = line.Substring(0, ayirac[ayirac.Length - 3]).ToUpper();
                            var kod = MacroToKeyCode(kodA);
                            sira++;
                            var tuslar = line.Substring(ayirac[ayirac.Length - 3] + 1).Split(' ');
                            macro = new Macro($"{kodA}", kod, tuslar[1] == "1", tuslar[0] == "1", tuslar[2] == "1");
                            //
                            Console.WriteLine(sira + "." + kodA + "-" + kod);
                            TusName = kodA;
                        }
                        else
                        {
                            if (line[0] != '#')
                            {
                                if (line[0] == '+')
                                {
                                    line = Encoding.Unicode.GetString(Encoding.UTF8.GetBytes(line.Substring(1)));

                                }
                                var sIndex = line.IndexOf(' ');
                                var mc = line.Substring(0, sIndex);

                                var msc = line.Substring(sIndex + 1);

                                var mci = (MacroType)Array.IndexOf(MacroTypeName, mc);
                                var msci = (MacroSubType)Array.IndexOf(MacroSubTypeName, msc);

                                if (msci == (MacroSubType)(-1))
                                    msci = (MacroSubType)Array.IndexOf(MacroSubTypeNameAlternate, msc);

                                if (msci == (MacroSubType)(-1))
                                    msci = 0;

                                var mObject = new MacroObject(mci, msci);

                                if (mObject.SubMenuType == 2)
                                {
                                    mObject = new MacroObjectString(mci, 0, msc);
                                }
                                //macro.Name = line;
                                macro.Name = TusName;
                                Console.WriteLine(sira + "." + mci + ">" + msc + ">" + msci);
                                macro.PushToBack(mObject);
                            }
                            else
                            {
                                convManager.PushToBack(macro);
                                macro = null;
                            }
                        }
                    }


                    convManager.Save();

                    _macroControl?.Dispose();

                    databox.ReArrangeChildren();

                    //UIManager.GetGump<OptionsGump>()?.Dispose();
                    UIManager.GetGump<OptionsGumpOld>()?.Dispose();
                    GameActions.OpenSettings(World, 4);

                    _convertMacro.Clear();

                    GameActions.Print(World, "Macrolar başarıyla aktarıldı!" + ":" + fileToLoad + " (" + sira + ")", 0x44, MessageType.Regular, 1, true);
                }
                else
                {
                    GameActions.Print(World, "macros.txt dosyası bulunamadı!", 0x20, MessageType.Regular, 1, true);
                }
            };


            _alldelButton.MouseUp += (ss, ee) =>
            {
                QuestionGump dialog = new QuestionGump
                (
                     World,
                    ResGumps.MacroDeleteConfirmation,
                    b =>
                    {
                        if (!b)
                        {
                            return;
                        }
                        Array.ForEach(UIManager.Gumps.OfType<MacroButtonGump>().ToArray(), m => m?.Dispose());
                        Array.ForEach(World.Macros.GetAllMacros().ToArray(), m => World.Macros.Remove(m));
                        GameActions.Print(World, "Tüm macrolar silindi.", 0x44, MessageType.Regular, 1, true);
                        databox.ReArrangeChildren();
                        UIManager.GetGump<OptionsGumpOld>()?.Dispose();

                        //UIManager.Gumps.OfType<MacroGump>().FirstOrDefault()?.Dispose();

                        GameActions.OpenSettings(World, 4);

                    }
                );

                UIManager.Add(dialog);

            };

            _openFolder.MouseUp += (ss, ee) =>
            {
                string username = Settings.GlobalSettings.Username;
                string servername = World.ServerName;
                string charactername = World.Player.Name;
                string rootpath;
                if (string.IsNullOrWhiteSpace(Settings.GlobalSettings.ProfilesPath))
                {
                    rootpath = Path.Combine(CUOEnviroment.ExecutablePath, "Data", "Profiles");
                }
                else
                {
                    rootpath = Settings.GlobalSettings.ProfilesPath;
                }

                string path = FileSystemHelper.CreateFolderIfNotExists(rootpath, username, servername, charactername);


                if (!Directory.Exists(path))
                {
                    GameActions.Print(World, "Klasör bulunamadı!", 0x20, MessageType.Regular, 1, true);
                }
                GameActions.Print(World, "Hesap, Sunucu ve karaktere ait macro klasörü açılıyor...", 946, MessageType.Regular, 1, true);
                System.Diagnostics.Process.Start("explorer.exe", path);

            };

            _prevButton.MouseUp += (ss, ee) =>
            {

                for (int i = 0; i <= databox.Children.Count - 1; i++)
                {

                    var Child = (NiceButton)databox.Children[i];

                    if (Child.IsSelected == true)
                    {
                        if (i > 0)
                        {
                            Child.IsSelected = false;
                            var TempChild = (NiceButton)databox.Children[i - 1];
                            TempChild.IsSelected = true;
                            TempChild.OnButtonClick(TempChild.ButtonParameter);
                            TempChild.InvokeMouseUp(new Point(TempChild.X, TempChild.Y), MouseButtonType.Left);
                        }
                    }
                }

            };

            _nextButton.MouseUp += (ss, ee) =>
            {
                var ModifyIndex = 0;
                for (int i = 0; i < databox.Children.Count - 1; i++)
                {
                    var Child = (NiceButton)databox.Children[i];
                    if (Child.IsSelected == true)
                    {
                        if (i < databox.Children.Count)
                        {
                            Child.IsSelected = false;
                            ModifyIndex = i + 1;
                        }
                    }
                }
                var TempChild = (NiceButton)databox.Children[ModifyIndex];
                TempChild.IsSelected = true;
                TempChild.OnButtonClick(TempChild.ButtonParameter);
                TempChild.InvokeMouseUp(new Point(TempChild.X, TempChild.Y), MouseButtonType.Left);
            };


            // macro listesi
            MacroManager macroManager = World.Macros;

            for (Macro macro = (Macro)macroManager.Items; macro != null; macro = (Macro)macro.Next)
            {
                NiceButton nb;

                databox.Add
                (
                    nb = new NiceButton
                    (
                        0,
                        0,
                        130,
                        25,
                        ButtonAction.Activate,
                        macro.Name
                    )
                    {
                        ButtonParameter = (int)Buttons.Last + 1 + rightArea.Children.Count,
                        Tag = macro
                    }
                );

                nb.IsSelected = true;

                nb.DragBegin += (sss, eee) =>
                {
                    NiceButton mupNiceButton = (NiceButton)sss;

                    Macro m = mupNiceButton.Tag as Macro;

                    if (m == null)
                    {
                        return;
                    }

                    if (UIManager.IsDragging || Math.Max(Math.Abs(Mouse.LDragOffset.X), Math.Abs(Mouse.LDragOffset.Y)) < 5 || nb.ScreenCoordinateX > Mouse.LClickPosition.X || nb.ScreenCoordinateX < Mouse.LClickPosition.X - nb.Width || nb.ScreenCoordinateY > Mouse.LClickPosition.Y || nb.ScreenCoordinateY + nb.Height < Mouse.LClickPosition.Y)
                    {
                        return;
                    }

                    UIManager.Gumps.OfType<MacroButtonGump>().FirstOrDefault(s => s._macro == m)?.Dispose();

                    MacroButtonGump macroButtonGump = new MacroButtonGump(World,m, Mouse.Position.X, Mouse.Position.Y);

                    macroButtonGump.X = Mouse.LClickPosition.X + (macroButtonGump.Width >> 1);
                    macroButtonGump.Y = Mouse.LClickPosition.Y + (macroButtonGump.Height >> 1);

                    UIManager.Add(macroButtonGump);

                    UIManager.AttemptDragControl(macroButtonGump, true);
                };

                nb.MouseUp += (sss, eee) =>
                {
                    NiceButton mupNiceButton = (NiceButton)sss;

                    Macro m = mupNiceButton.Tag as Macro;

                    if (m == null)
                    {
                        return;
                    }

                    _macroControl?.Dispose();

                    _macroControl = new MacroControl(this, m.Name)
                    {
                        X = 270,
                        Y = 130
                    };

                    Add(_macroControl, PAGE);
                };
            }


            databox.ReArrangeChildren();

            Add(rightArea, PAGE);
        }

        private void BuildTooltip()
        {
            const int PAGE = 5;

            ScrollArea rightArea = new ScrollArea
            (
                MAIN_X + 70,      // sağdan sola
                MAIN_Y + 50,        // yukardan aşağıya
                WIDTH - 35,     // genişlik
                340,            // yükseklik
                true
            );


            int startX = 5;
            int startY = 5;

            _use_tooltip = AddCheckBox
            (
                rightArea,
                ResGumps.UseTooltip,
                _currentProfile.UseTooltip,
                startX,
                startY
            );

            startY += _use_tooltip.Height + 2;

            startX += 40;

            Label text = AddLabel(rightArea, ResGumps.DelayBeforeDisplay, startX, startY);
            startX += text.Width + 5;

            _delay_before_display_tooltip = AddHSlider
            (
                rightArea,
                0,
                1000,
                _currentProfile.TooltipDelayBeforeDisplay,
                startX,
                startY,
                200
            );

            startX = 5 + 40;
            startY += text.Height + 2;

            text = AddLabel(rightArea, ResGumps.TooltipZoom, startX, startY);
            startX += text.Width + 5;

            _tooltip_zoom = AddHSlider
            (
                rightArea,
                100,
                200,
                _currentProfile.TooltipDisplayZoom,
                startX,
                startY,
                200
            );

            startX = 5 + 40;
            startY += text.Height + 2;

            text = AddLabel(rightArea, ResGumps.TooltipBackgroundOpacity, startX, startY);
            startX += text.Width + 5;

            _tooltip_background_opacity = AddHSlider
            (
                rightArea,
                0,
                100,
                _currentProfile.TooltipBackgroundOpacity,
                startX,
                startY,
                200
            );

            startX = 5 + 40;
            startY += text.Height + 2;

            _tooltip_font_hue = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.TooltipTextHue,
                ResGumps.TooltipFontHue
            );

            startY += _tooltip_font_hue.Height + 2;

            startY += 15;

            text = AddLabel(rightArea, ResGumps.TooltipFont, startX, startY);
            startY += text.Height + 2;
            startX += 40;

            _tooltip_font_selector = new FontSelector(7, _currentProfile.TooltipFont, ResGumps.TooltipFontSelect)
            {
                X = startX,
                Y = startY
            };

            rightArea.Add(_tooltip_font_selector);

            Add(rightArea, PAGE);
        }

        private void BuildFonts()
        {
            const int PAGE = 6;

            ScrollArea rightArea = new ScrollArea
            (
                MAIN_X + 70,      // sağdan sola
                MAIN_Y + 50,        // yukardan aşağıya
                WIDTH - 35,     // genişlik
                340,            // yükseklik
                true
            );


            int startX = 5;
            int startY = 5;

            _overrideAllFonts = AddCheckBox
            (
                rightArea,
                ResGumps.OverrideGameFont,
                _currentProfile.OverrideAllFonts,
                startX,
                startY,
                true
            );

            startX += _overrideAllFonts.Width + 5;

            _overrideAllFontsIsUnicodeCheckbox = AddCombobox
            (
                rightArea,
                new[]
                {
                    ResGumps.ASCII, ResGumps.Unicode
                },
                _currentProfile.OverrideAllFontsIsUnicode ? 1 : 0,
                startX,
                startY,
                100,
                false
            );

            startX = 5;
            startY += _overrideAllFonts.Height + 2;

            _forceUnicodeJournal = AddCheckBox
            (
                rightArea,
                ResGumps.ForceUnicodeInJournal,
                _currentProfile.ForceUnicodeJournal,
                startX,
                startY
            );

            startY += _forceUnicodeJournal.Height + 2;

            Label text = AddLabel(rightArea, ResGumps.SpeechFont, startX, startY);
            startX += 40;
            startY += text.Height + 2;

            _fontSelectorChat = new FontSelector(20, _currentProfile.ChatFont, ResGumps.ThatSClassicUO)
            {
                X = startX,
                Y = startY
            };

            rightArea.Add(_fontSelectorChat);

            Add(rightArea, PAGE);
        }

        private void BuildSpeech()
        {
            const int PAGE = 7;

            ScrollArea rightArea = new ScrollArea
            (
                MAIN_X + 70,      // sağdan sola
                MAIN_Y + 50,        // yukardan aşağıya
                WIDTH - 35,     // genişlik
                340,            // yükseklik
                true
            );


            int startX = 5;
            int startY = 5;

            _scaleSpeechDelay = AddCheckBox
            (
                rightArea,
                ResGumps.ScaleSpeechDelay,
                _currentProfile.ScaleSpeechDelay,
                startX,
                startY
            );

            startX += _scaleSpeechDelay.Width + 5;

            _sliderSpeechDelay = AddHSlider
            (
                rightArea,
                0,
                1000,
                _currentProfile.SpeechDelay,
                startX,
                startY,
                180
            );

            startX = 5;
            startY += _scaleSpeechDelay.Height + 2;

            _saveJournalCheckBox = AddCheckBox
            (
                rightArea,
                ResGumps.SaveJournalToFileInGameFolder,
                _currentProfile.SaveJournalToFile,
                startX,
                startY
            );

            startY += _saveJournalCheckBox.Height + 2;

            if (!_currentProfile.SaveJournalToFile)
            {
                World.Journal.CloseWriter();
            }

            _chatAfterEnter = AddCheckBox
            (
                rightArea,
                ResGumps.ActiveChatWhenPressingEnter,
                _currentProfile.ActivateChatAfterEnter,
                startX,
                startY
            );

            startX += 40;
            startY += _chatAfterEnter.Height + 2;

            _chatAdditionalButtonsCheckbox = AddCheckBox
            (
                rightArea,
                ResGumps.UseAdditionalButtonsToActivateChat,
                _currentProfile.ActivateChatAdditionalButtons,
                startX,
                startY
            );

            startY += _chatAdditionalButtonsCheckbox.Height + 2;

            _chatShiftEnterCheckbox = AddCheckBox
            (
                rightArea,
                ResGumps.UseShiftEnterToSendMessage,
                _currentProfile.ActivateChatShiftEnterSupport,
                startX,
                startY
            );

            startY += _chatShiftEnterCheckbox.Height + 2;
            startX = 5;

            _hideChatGradient = AddCheckBox
            (
                rightArea,
                ResGumps.HideChatGradient,
                _currentProfile.HideChatGradient,
                startX,
                startY
            );

            startY += _hideChatGradient.Height + 2;

            _ignoreGuildMessages = AddCheckBox
            (
                rightArea,
                ResGumps.IgnoreGuildMessages,
                _currentProfile.IgnoreGuildMessages,
                startX,
                startY
            );

            startY += _ignoreGuildMessages.Height + 2;

            _ignoreAllianceMessages = AddCheckBox
            (
                rightArea,
                ResGumps.IgnoreAllianceMessages,
                _currentProfile.IgnoreAllianceMessages,
                startX,
                startY
            );


            startY += 35;

            _randomizeColorsButton = new NiceButton
            (
                startX,
                startY,
                140,
                25,
                ButtonAction.Activate,
                ResGumps.RandomizeSpeechHues
            )
            { ButtonParameter = (int)Buttons.Disabled };

            _randomizeColorsButton.MouseUp += (sender, e) =>
            {
                if (e.Button != MouseButtonType.Left)
                {
                    return;
                }

                ushort speechHue = (ushort)RandomHelper.GetValue(2, 0x03b2); //this seems to be the acceptable hue range for chat messages,

                ushort emoteHue = (ushort)RandomHelper.GetValue(2, 0x03b2); //taken from POL source code.
                ushort yellHue = (ushort)RandomHelper.GetValue(2, 0x03b2);
                ushort whisperHue = (ushort)RandomHelper.GetValue(2, 0x03b2);
                _currentProfile.SpeechHue = speechHue;
                _speechColorPickerBox.Hue = speechHue;
                _currentProfile.EmoteHue = emoteHue;
                _emoteColorPickerBox.Hue = emoteHue;
                _currentProfile.YellHue = yellHue;
                _yellColorPickerBox.Hue = yellHue;
                _currentProfile.WhisperHue = whisperHue;
                _whisperColorPickerBox.Hue = whisperHue;
            };

            rightArea.Add(_randomizeColorsButton);
            startY += _randomizeColorsButton.Height + 2 + 20;


            _speechColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.SpeechHue,
                ResGumps.SpeechColor
            );

            startX += 200;

            _emoteColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.EmoteHue,
                ResGumps.EmoteColor
            );

            startY += _emoteColorPickerBox.Height + 2;
            startX = 5;

            _yellColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.YellHue,
                ResGumps.YellColor
            );

            startX += 200;

            _whisperColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.WhisperHue,
                ResGumps.WhisperColor
            );

            startY += _whisperColorPickerBox.Height + 2;
            startX = 5;

            _partyMessageColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.PartyMessageHue,
                ResGumps.PartyMessageColor
            );

            startX += 200;

            _guildMessageColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.GuildMessageHue,
                ResGumps.GuildMessageColor
            );

            startY += _guildMessageColorPickerBox.Height + 2;
            startX = 5;

            _allyMessageColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.AllyMessageHue,
                ResGumps.AllianceMessageColor
            );

            startX += 200;

            _chatMessageColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.ChatMessageHue,
                ResGumps.ChatMessageColor
            );

            startY += _chatMessageColorPickerBox.Height + 2;
            startX = 5;

            Add(rightArea, PAGE);
        }

        private void BuildCombat()
        {
            const int PAGE = 8;

            ScrollArea rightArea = new ScrollArea
            (
                MAIN_X + 70,      // sağdan sola
                MAIN_Y + 50,        // yukardan aşağıya
                WIDTH - 35,     // genişlik
                340,            // yükseklik
                true
            );


            int startX = 5;
            int startY = 5;

            _holdDownKeyTab = AddCheckBox
            (
                rightArea,
                ResGumps.TabCombat,
                _currentProfile.HoldDownKeyTab,
                startX,
                startY
            );

            startY += _holdDownKeyTab.Height + 2;

            _queryBeforAttackCheckbox = AddCheckBox
            (
                rightArea,
                ResGumps.QueryAttack,
                _currentProfile.EnabledCriminalActionQuery,
                startX,
                startY
            );

            startY += _queryBeforAttackCheckbox.Height + 2;

            _queryBeforeBeneficialCheckbox = AddCheckBox
            (
                rightArea,
                ResGumps.QueryBeneficialActs,
                _currentProfile.EnabledBeneficialCriminalActionQuery,
                startX,
                startY
            );

            startY += _queryBeforeBeneficialCheckbox.Height + 2;

            _spellFormatCheckbox = AddCheckBox
            (
                rightArea,
                ResGumps.EnableOverheadSpellFormat,
                _currentProfile.EnabledSpellFormat,
                startX,
                startY
            );

            startY += _spellFormatCheckbox.Height + 2;

            _spellColoringCheckbox = AddCheckBox
            (
                rightArea,
                ResGumps.EnableOverheadSpellHue,
                _currentProfile.EnabledSpellHue,
                startX,
                startY
            );

            startY += _spellColoringCheckbox.Height + 2;

            _uiButtonsSingleClick = AddCheckBox
            (
                rightArea,
                ResGumps.UIButtonsSingleClick,
                _currentProfile.CastSpellsByOneClick,
                startX,
                startY
            );

            startY += _uiButtonsSingleClick.Height + 2;

            _buffBarTime = AddCheckBox
            (
                rightArea,
                ResGumps.ShowBuffDuration,
                _currentProfile.BuffBarTime,
                startX,
                startY
            );

            startY += _buffBarTime.Height + 2;

            _enableFastSpellsAssign = AddCheckBox
            (
                rightArea,
                ResGumps.EnableFastSpellsAssign,
                _currentProfile.FastSpellsAssign,
                startX,
                startY
            );

            startY += 30;

            int initialY = startY;

            _innocentColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.InnocentHue,
                ResGumps.InnocentColor
            );

            startY += _innocentColorPickerBox.Height + 2;

            _friendColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.FriendHue,
                ResGumps.FriendColor
            );

            startY += _innocentColorPickerBox.Height + 2;

            _crimialColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.CriminalHue,
                ResGumps.CriminalColor
            );

            startY += _innocentColorPickerBox.Height + 2;

            _canAttackColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.CanAttackHue,
                ResGumps.CanAttackColor
            );

            startY += _innocentColorPickerBox.Height + 2;

            _murdererColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.MurdererHue,
                ResGumps.MurdererColor
            );

            startY += _innocentColorPickerBox.Height + 2;

            _enemyColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.EnemyHue,
                ResGumps.EnemyColor
            );

            startY += _innocentColorPickerBox.Height + 2;

            startY = initialY;
            startX += 200;

            _beneficColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.BeneficHue,
                ResGumps.BeneficSpellHue
            );

            startY += _beneficColorPickerBox.Height + 2;

            _harmfulColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.HarmfulHue,
                ResGumps.HarmfulSpellHue
            );

            startY += _harmfulColorPickerBox.Height + 2;

            _neutralColorPickerBox = AddColorBox
            (
                rightArea,
                startX,
                startY,
                _currentProfile.NeutralHue,
                ResGumps.NeutralSpellHue
            );

            startY += _neutralColorPickerBox.Height + 2;

            startX = 5;
            startY += (_neutralColorPickerBox.Height + 2) * 4;

            _spellFormatBox = AddInputField
            (
                rightArea,
                startX,
                startY,
                200,
                TEXTBOX_HEIGHT,
                ResGumps.SpellOverheadFormat,
                0,
                true,
                false,
                30
            );

            _spellFormatBox.SetText(_currentProfile.SpellDisplayFormat);

            Add(rightArea, PAGE);
        }


        private void BuildCounters()
        {
            const int PAGE = 9;

            ScrollArea rightArea = new ScrollArea
            (
                MAIN_X + 70,      // sağdan sola
                MAIN_Y + 50,        // yukardan aşağıya
                WIDTH - 35,     // genişlik
                340,            // yükseklik
                true
            );


            int startX = 5;
            int startY = 5;

            NiceButton infobarButton = new NiceButton
           (
               80,
               350,
               100,
               25,
               ButtonAction.SwitchPage,
               ResGumps.InfoBar
           )
            { ButtonParameter = 10 };

            Add(infobarButton, PAGE);

            NiceButton containerButton = new NiceButton
            (
                190,
                350,
                100,
                25,
                ButtonAction.SwitchPage,
                ResGumps.Containers
            )
            { ButtonParameter = 11 };

            Add(containerButton, PAGE);

            NiceButton ingnorelistButton = new NiceButton
            (
                300,
                350,
                100,
                25,
                ButtonAction.Activate,
                ResGumps.IgnoreListManager
            )
            { ButtonParameter = (int)Buttons.OpenIgnoreList };

            Add(ingnorelistButton, PAGE);


            _enableCounters = AddCheckBox
            (
                rightArea,
                ResGumps.EnableCounters,
                _currentProfile.CounterBarEnabled,
                startX,
                startY
            );

            startX += 40;
            startY += _enableCounters.Height + 2;

            _highlightOnUse = AddCheckBox
            (
                rightArea,
                ResGumps.HighlightOnUse,
                _currentProfile.CounterBarHighlightOnUse,
                startX,
                startY
            );

            startY += _highlightOnUse.Height + 2;

            _enableAbbreviatedAmount = AddCheckBox
            (
                rightArea,
                ResGumps.EnableAbbreviatedAmountCountrs,
                _currentProfile.CounterBarDisplayAbbreviatedAmount,
                startX,
                startY
            );

            startX += _enableAbbreviatedAmount.Width + 5;

            _abbreviatedAmount = AddInputField
            (
                rightArea,
                startX,
                startY,
                50,
                TEXTBOX_HEIGHT,
                null,
                50,
                false,
                true
            );

            _abbreviatedAmount.SetText(_currentProfile.CounterBarAbbreviatedAmount.ToString());

            startX = 5;
            startX += 40;
            startY += _enableAbbreviatedAmount.Height + 2;

            _highlightOnAmount = AddCheckBox
            (
                rightArea,
                ResGumps.HighlightRedWhenBelow,
                _currentProfile.CounterBarHighlightOnAmount,
                startX,
                startY
            );

            startX += _highlightOnAmount.Width + 5;

            _highlightAmount = AddInputField
            (
                rightArea,
                startX,
                startY,
                50,
                TEXTBOX_HEIGHT,
                null,
                50,
                false,
                true,
                999
            );

            _highlightAmount.SetText(_currentProfile.CounterBarHighlightAmount.ToString());

            startX = 5;
            startX += 40;
            startY += _highlightAmount.Height + 2 + 5;

            startY += 40;

            Label text = AddLabel(rightArea, ResGumps.CounterLayout, startX, startY);

            startX += 40;
            startY += text.Height + 2;
            text = AddLabel(rightArea, ResGumps.CellSize, startX, startY);

            int initialX = startX;
            startX += text.Width + 5;

            _cellSize = AddHSlider
            (
                rightArea,
                30,
                80,
                _currentProfile.CounterBarCellSize,
                startX,
                startY,
                80
            );


            startX = initialX;
            startY += text.Height + 2 + 15;

            _rows = AddInputField
            (
                rightArea,
                startX,
                startY,
                50,
                30,
                ResGumps.Counter_Rows,
                50,
                false,
                true,
                30
            );

            _rows.SetText(_currentProfile.CounterBarRows.ToString());


            startX += _rows.Width + 5 + 100;

            _columns = AddInputField
            (
                rightArea,
                startX,
                startY,
                50,
                30,
                ResGumps.Counter_Columns,
                50,
                false,
                true,
                30
            );

            _columns.SetText(_currentProfile.CounterBarColumns.ToString());


            Add(rightArea, PAGE);
        }

        private void BuildExperimental()
        {
            const int PAGE = 12;

            ScrollArea rightArea = new ScrollArea
            (
                MAIN_X + 70,      // sağdan sola
                MAIN_Y + 50,        // yukardan aşağıya
                WIDTH - 35,     // genişlik
                340,            // yükseklik
                true
            );


            int startX = 5;
            int startY = 5;

            _disableDefaultHotkeys = AddCheckBox
            (
                rightArea,
                ResGumps.DisableDefaultUOHotkeys,
                _currentProfile.DisableDefaultHotkeys,
                startX,
                startY
            );

            startX += 40;
            startY += _disableDefaultHotkeys.Height + 2;

            _disableArrowBtn = AddCheckBox
            (
                rightArea,
                ResGumps.DisableArrowsPlayerMovement,
                _currentProfile.DisableArrowBtn,
                startX,
                startY
            );

            startY += _disableArrowBtn.Height + 2;

            _disableTabBtn = AddCheckBox
            (
                rightArea,
                ResGumps.DisableTab,
                _currentProfile.DisableTabBtn,
                startX,
                startY
            );

            startY += _disableTabBtn.Height + 2;

            _disableCtrlQWBtn = AddCheckBox
            (
                rightArea,
                ResGumps.DisableMessageHistory,
                _currentProfile.DisableCtrlQWBtn,
                startX,
                startY
            );

            startY += _disableCtrlQWBtn.Height + 2;

            _disableAutoMove = AddCheckBox
            (
                rightArea,
                ResGumps.DisableClickAutomove,
                _currentProfile.DisableAutoMove,
                startX,
                startY
            );

            startY += _disableAutoMove.Height + 2;

            Add(rightArea, PAGE);
        }


        private void BuildInfoBar()
        {
            const int PAGE = 10;

            ScrollArea rightArea = new ScrollArea
            (
                MAIN_X + 70,      // sağdan sola
                MAIN_Y + 50,        // yukardan aşağıya
                WIDTH - 35,     // genişlik
                340,            // yükseklik
                true
            );


            int startX = 5;
            int startY = 5;

            _showInfoBar = AddCheckBox
            (
                rightArea,
                ResGumps.ShowInfoBar,
                _currentProfile.ShowInfoBar,
                startX,
                startY
            );

            startX += 40;
            startY += _showInfoBar.Height + 2;

            Label text = AddLabel(rightArea, ResGumps.DataHighlightType, startX, startY);

            startX += text.Width + 5;

            _infoBarHighlightType = AddCombobox
            (
                rightArea,
                new[] { ResGumps.TextColor, ResGumps.ColoredBars },
                _currentProfile.InfoBarHighlightType,
                startX,
                startY,
                150
            );

            startX = 5;
            startY += _infoBarHighlightType.Height + 5;

            NiceButton nb = new NiceButton
            (
                startX,
                startY,
                90,
                20,
                ButtonAction.Activate,
                ResGumps.AddItem,
                0,
                TEXT_ALIGN_TYPE.TS_LEFT
            )
            {
                ButtonParameter = -1,
                IsSelectable = true,
                IsSelected = true
            };

            nb.MouseUp += (sender, e) =>
            {
                InfoBarBuilderControl ibbc = new InfoBarBuilderControl(this, new InfoBarItem("", InfoBarVars.HP, 0x3B9));
                ibbc.X = 5;
                ibbc.Y = _databox.Children.Count * ibbc.Height;
                _infoBarBuilderControls.Add(ibbc);
                _databox.Add(ibbc);
                _databox.WantUpdateSize = true;
            };

            rightArea.Add(nb);


            startY += 40;

            text = AddLabel(rightArea, ResGumps.Label, startX, startY);

            startX += 150;

            text = AddLabel(rightArea, ResGumps.Color, startX, startY);

            startX += 55;
            text = AddLabel(rightArea, ResGumps.Data, startX, startY);

            startX = 5;
            startY += text.Height + 2;

            rightArea.Add
            (
                new Line
                (
                    startX,
                    startY,
                    rightArea.Width,
                    1,
                    Color.Gray.PackedValue
                )
            );

            startY += 20;


            List<InfoBarItem> _infoBarItems = World.InfoBars.GetInfoBars();

            _infoBarBuilderControls = new List<InfoBarBuilderControl>();

            _databox = new DataBox(startX, startY, 10, 10)
            {
                WantUpdateSize = true
            };


            for (int i = 0; i < _infoBarItems.Count; i++)
            {
                InfoBarBuilderControl ibbc = new InfoBarBuilderControl(this, _infoBarItems[i]);
                ibbc.X = 5;
                ibbc.Y = i * ibbc.Height;
                _infoBarBuilderControls.Add(ibbc);
                _databox.Add(ibbc);
            }

            rightArea.Add(_databox);

            Add(rightArea, PAGE);
        }

        private void BuildContainers()
        {
            const int PAGE = 11;

            ScrollArea rightArea = new ScrollArea
            (
                MAIN_X + 70,      // sağdan sola
                MAIN_Y + 50,        // yukardan aşağıya
                WIDTH - 35,     // genişlik
                340,            // yükseklik
                true
            );


            int startX = 5;
            int startY = 5;
            Label text;

            //bool hasBackpacks = Client.Game.UO.Version >= ClientVersion.CV_705301;
            bool hasBackpacks = true;

            if (hasBackpacks)
            {
                text = AddLabel(rightArea, ResGumps.BackpackStyle, startX, startY);
                startX += text.Width + 5;
            }

            _backpackStyle = AddCombobox
            (
                rightArea,
                new[]
                {
                    ResGumps.BackpackStyle_Default, 
                    ResGumps.BackpackStyle_Suede,
                    ResGumps.BackpackStyle_PolarBear, 
                    ResGumps.BackpackStyle_GhoulSkin
                },
                _currentProfile.BackpackStyle,
                startX,
                startY,
                200
            );

            _backpackStyle.IsVisible = hasBackpacks;

            if (hasBackpacks)
            {
                startX = 5;
                startY += _backpackStyle.Height + 2 + 10;
            }

            text = AddLabel(rightArea, ResGumps.ContainerScale, startX, startY);
            startX += text.Width + 5;

            _containersScale = AddHSlider
            (
                rightArea,
                Constants.MIN_CONTAINER_SIZE_PERC,
                Constants.MAX_CONTAINER_SIZE_PERC,
                _currentProfile.ContainersScale,
                startX,
                startY,
                200
            );

            startX = 5;
            startY += text.Height + 2;

            _containerScaleItems = AddCheckBox
            (
                rightArea,
                ResGumps.ScaleItemsInsideContainers,
                _currentProfile.ScaleItemsInsideContainers,
                startX,
                startY
            );

            startY += _containerScaleItems.Height + 2;

            _useLargeContianersGumps = AddCheckBox
            (
                rightArea,
                ResGumps.UseLargeContainersGump,
                _currentProfile.UseLargeContainerGumps,
                startX,
                startY
            );

            //_useLargeContianersGumps.IsVisible = Client.Game.UO.Version >= ClientVersion.CV_706000;
            _useLargeContianersGumps.IsVisible = true;

            if (_useLargeContianersGumps.IsVisible)
            {
                startY += _useLargeContianersGumps.Height + 2;
            }

            _containerDoubleClickToLoot = AddCheckBox
            (
                rightArea,
                ResGumps.DoubleClickLootContainers,
                _currentProfile.DoubleClickToLootInsideContainers,
                startX,
                startY,
                true
            );

            startY += _containerDoubleClickToLoot.Height + 2;

            _relativeDragAnDropItems = AddCheckBox
            (
                rightArea,
                ResGumps.RelativeDragAndDropContainers,
                _currentProfile.RelativeDragAndDropItems,
                startX,
                startY
            );

            startY += _relativeDragAnDropItems.Height + 2;

            _highlightContainersWhenMouseIsOver = AddCheckBox
            (
                rightArea,
                ResGumps.HighlightContainerWhenSelected,
                _currentProfile.HighlightContainerWhenSelected,
                startX,
                startY
            );

            startY += _highlightContainersWhenMouseIsOver.Height + 2;

            _hueContainerGumps = AddCheckBox
            (
                rightArea,
                ResGumps.HueContainerGumps,
                _currentProfile.HueContainerGumps,
                startX,
                startY
            );

            startY += _hueContainerGumps.Height + 2;

            _overrideContainerLocation = AddCheckBox
            (
                rightArea,
                ResGumps.OverrideContainerGumpLocation,
                _currentProfile.OverrideContainerLocation,
                startX,
                startY
            );

            startX += _overrideContainerLocation.Width + 5;

            _overrideContainerLocationSetting = AddCombobox
            (
                rightArea,
                new[]
                {
                    ResGumps.ContLoc_NearContainerPosition, ResGumps.ContLoc_TopRight,
                    ResGumps.ContLoc_LastDraggedPosition, ResGumps.ContLoc_RememberEveryContainer
                },
                _currentProfile.OverrideContainerLocationSetting,
                startX,
                startY,
                200
            );

            startX = 5;
            startY += _overrideContainerLocation.Height + 2 + 10;

            NiceButton button = new NiceButton
            (
                startX,
                startY,
                130,
                30,
                ButtonAction.Activate,
                ResGumps.RebuildContainers
            )
            {
                ButtonParameter = -1,
                IsSelectable = true,
                IsSelected = true
            };

            button.MouseUp += (sender, e) => { World.ContainerManager.BuildContainerFile(true); };
            rightArea.Add(button);

            Add(rightArea, PAGE);
        }


        public override void OnButtonClick(int buttonID)
        {
            if (buttonID == (int)Buttons.Last + 1)
            {
                // it's the macro buttonssss
                return;
            }

            switch ((Buttons)buttonID)
            {
                case Buttons.Disabled: break;

                case Buttons.Cancel:
                    Dispose();

                    break;

                case Buttons.Apply:
                    Apply();

                    break;

                case Buttons.Default:
                    SetDefault();

                    break;

                case Buttons.Ok:
                    Apply();
                    Dispose();

                    break;

                case Buttons.NewMacro: break;

                case Buttons.ConvertMacro: break;
                case Buttons.DeleteMacro: break;

                case Buttons.AllDeleteMacro: break;
                case Buttons.OpenMacrosFolder: break;

                case Buttons.PrevMacroButton: break;
                case Buttons.NextMacroButton: break;

                case Buttons.OpenIgnoreList:
                    // If other IgnoreManagerGump exist - Dispose it
                    UIManager.GetGump<IgnoreManagerGump>()?.Dispose();
                    // Open new
                    UIManager.Add(new IgnoreManagerGump(World));
                    break;
            }
        }

        private void SetDefault(int page)
        {
            if (page > 0)
            {
                GameActions.Print(World, "Sayfa: " + page, 946, MessageType.Regular, 1, true);
            }

            if (page == 1)
            {
                _sliderFPS.Value = 60;
                _reduceFPSWhenInactive.IsChecked = true;
                _clientnotifyballon.IsChecked = true;
                _inputautofocused.IsChecked = false;
                _highlightObjects.IsChecked = false;
                _enableTopbar.IsChecked = false;
                _holdDownKeyTab.IsChecked = true;
                _holdDownKeyAlt.IsChecked = true;
                _closeAllAnchoredGumpsWithRClick.IsChecked = false;
                _holdShiftForContext.IsChecked = false;
                _holdAltToMoveGumps.IsChecked = false;
                _holdShiftToSplitStack.IsChecked = false;
                _enablePathfind.IsChecked = false;
                _useShiftPathfind.IsChecked = false;
                _alwaysRun.IsChecked = false;
                _alwaysRunUnlessHidden.IsChecked = false;
                _showHpMobile.IsChecked = false;
                _hpComboBox.SelectedIndex = 0;
                _hpComboBoxShowWhen.SelectedIndex = 0;
                _highlightByPoisoned.IsChecked = true;
                _highlightByParalyzed.IsChecked = true;
                _highlightByInvul.IsChecked = true;
                _poisonColorPickerBox.Hue = 0x0044;
                _paralyzedColorPickerBox.Hue = 0x014C;
                _invulnerableColorPickerBox.Hue = 0x0030;

                //_drawRoofs.IsChecked = false;
                _drawRoofs.IsChecked = true;

                _enableCaveBorder.IsChecked = false;
                _treeToStumps.IsChecked = false;
                _hideVegetation.IsChecked = false;
                _noColorOutOfRangeObjects.IsChecked = false;
                _circleOfTranspRadius.Value = Constants.MIN_CIRCLE_OF_TRANSPARENCY_RADIUS;
                _cotType.SelectedIndex = 0;
                _useCircleOfTransparency.IsChecked = false;
                _healtbarType.SelectedIndex = 0;
                _fieldsType.SelectedIndex = 0;
                _useStandardSkillsGump.IsChecked = true;
                _showCorpseNameIncoming.IsChecked = true;
                _showMobileNameIncoming.IsChecked = true;
                _gridLoot.SelectedIndex = 0;
                _uiType.SelectedIndex = 0;
                _sallosEasyGrab.IsChecked = false;
                _partyInviteGump.IsChecked = false;
                _showHouseContent.IsChecked = false;
                _objectsFading.IsChecked = true;
                _objectsFading.IsChecked = false;
                _textFading.IsChecked = true;
                _textFading.IsChecked = false;
                _enableDragSelect.IsChecked = false;
                _dragSelectHumanoidsOnly.IsChecked = false;
                _showTargetRangeIndicator.IsChecked = false;
                _customBars.IsChecked = false;
                _customBarsBBG.IsChecked = false;
                _autoOpenCorpse.IsChecked = false;
                _autoOpenDoors.IsChecked = false;
                _smoothDoors.IsChecked = false;
                _skipEmptyCorpse.IsChecked = false;
                _saveHealthbars.IsChecked = false;
                _use_smooth_boat_movement.IsChecked = false;
                _hideScreenshotStoredInMessage.IsChecked = false;
                _use_old_status_gump.IsChecked = false;
                _use_old_status_gump.IsChecked = true;
                _auraType.SelectedIndex = 0;
                _fieldsType.SelectedIndex = 0;

                _showSkillsMessage.IsChecked = true;
                _showSkillsMessageDelta.Value = 1;
                _showStatsMessage.IsChecked = true;

                _dragSelectStartX.Value = 100;
                _dragSelectStartY.Value = 100;
                _dragSelectAsAnchor.IsChecked = false;
            }

            if (page == 2) // sounds
            {
                _enableSounds.IsChecked = true;
                _enableMusic.IsChecked = true;
                _combatMusic.IsChecked = true;
                _soundsVolume.Value = 100;
                _musicVolume.Value = 25;
                _musicInBackground.IsChecked = false;
                _footStepsSound.IsChecked = true;
                _loginMusicVolume.Value = 100;
                _loginMusic.IsChecked = true;
                _soundsVolume.IsVisible = _enableSounds.IsChecked;
                _musicVolume.IsVisible = _enableMusic.IsChecked;
            }

            if (page == 3) // video
            {
                _windowBorderless.IsChecked = false;
                _zoomCheckbox.IsChecked = false;
                _restorezoomCheckbox.IsChecked = false;

                //_gameWindowWidth.SetText("600");
                //_gameWindowHeight.SetText("480");

                _gameWindowWidth.SetText(Constants.CLIENT_DEF_SIZE_WIDTH.ToString());
                _gameWindowHeight.SetText(Constants.CLIENT_DEF_SIZE_HEIGHT.ToString());

                _gameWindowPositionX.SetText(Constants.CLIENT_START_POSX.ToString());
                _gameWindowPositionY.SetText(Constants.CLIENT_START_POSY.ToString());
                _gameWindowLock.IsChecked = false;
                _gameWindowFullsize.IsChecked = false;
                _enableDeathScreen.IsChecked = true;
                _enableBlackWhiteEffect.IsChecked = true;

                Client.Game.Scene.Camera.Zoom = 1f;
                _sliderZoom.Value = 5;
                _currentProfile.DefaultScale = 1f;

                _lightBar.Value = 0;
                _enableLight.IsChecked = false;
                _lightLevelType.SelectedIndex = 0;
                _useColoredLights.IsChecked = false;
                _darkNights.IsChecked = false;

                _enableShadows.IsChecked = true;
                _enableShadowsStatics.IsChecked = true;
                _terrainShadowLevel.Value = 15;
                _runMouseInSeparateThread.IsChecked = true;
                _auraMouse.IsChecked = true;
                _partyAura.IsChecked = true;
                _animatedWaterEffect.IsChecked = false;
                _partyAuraColorPickerBox.Hue = 0x0044;
            }

            if (page == 4) // macros
            {

            }


            if (page == 5) // Tooltip
            {
                _use_tooltip.IsChecked = true;
                _tooltip_font_hue.Hue = 0xFFFF;
                _delay_before_display_tooltip.Value = 200;
                _tooltip_background_opacity.Value = 70;
                _tooltip_zoom.Value = 100;
                _tooltip_font_selector.SetSelectedFont(1);
            }


            if (page == 6) // fonts
            {
                _fontSelectorChat.SetSelectedFont(0);
                _overrideAllFonts.IsChecked = false;
                _overrideAllFontsIsUnicodeCheckbox.SelectedIndex = 1;

            }

            if (page == 7) // speech
            {

                _scaleSpeechDelay.IsChecked = true;
                _sliderSpeechDelay.Value = 100;
                _speechColorPickerBox.Hue = 0x02B2;
                _emoteColorPickerBox.Hue = 0x0021;
                _yellColorPickerBox.Hue = 0x0021;
                _whisperColorPickerBox.Hue = 0x0033;
                _partyMessageColorPickerBox.Hue = 0x0044;
                _guildMessageColorPickerBox.Hue = 0x0044;
                _allyMessageColorPickerBox.Hue = 0x0057;
                _chatMessageColorPickerBox.Hue = 0x0256;
                _chatAfterEnter.IsChecked = false;
                UIManager.SystemChat.IsActive = !_chatAfterEnter.IsChecked;
                _chatAdditionalButtonsCheckbox.IsChecked = true;
                _chatShiftEnterCheckbox.IsChecked = true;
                _saveJournalCheckBox.IsChecked = false;
                _hideChatGradient.IsChecked = false;
                _hideChatGradient.IsChecked = true;
                _ignoreGuildMessages.IsChecked = false;
                _ignoreAllianceMessages.IsChecked = false;

            }


            if (page == 8) // combat
            {
                _innocentColorPickerBox.Hue = 0x005A;
                _friendColorPickerBox.Hue = 0x0044;
                _crimialColorPickerBox.Hue = 0x03b2;
                _canAttackColorPickerBox.Hue = 0x03b2;
                _murdererColorPickerBox.Hue = 0x0023;
                _enemyColorPickerBox.Hue = 0x0031;
                _queryBeforAttackCheckbox.IsChecked = true;
                _queryBeforeBeneficialCheckbox.IsChecked = false;
                _uiButtonsSingleClick.IsChecked = false;
                _buffBarTime.IsChecked = true;
                _enableFastSpellsAssign.IsChecked = false;
                _beneficColorPickerBox.Hue = 0x0059;
                _harmfulColorPickerBox.Hue = 0x0020;
                _neutralColorPickerBox.Hue = 0x03b2;
                _spellFormatBox.SetText(ResGumps.SpellFormat_Default);
                _spellColoringCheckbox.IsChecked = false;
                _spellFormatCheckbox.IsChecked = false;
            }

            if (page == 9) // counters
            {
                _enableCounters.IsChecked = false;
                _highlightOnUse.IsChecked = false;
                _enableAbbreviatedAmount.IsChecked = false;
                _columns.SetText("1");
                _rows.SetText("1");
                _cellSize.Value = 40;
                _highlightOnAmount.IsChecked = false;
                _highlightAmount.SetText("5");
                _abbreviatedAmount.SetText("1000");

            }

            if (page == 10) // info bar
            {

            }

            if (page == 11) // containers
            {
                _containersScale.Value = 100;
                _containerScaleItems.IsChecked = false;
                _useLargeContianersGumps.IsChecked = false;
                _containerDoubleClickToLoot.IsChecked = false;
                _relativeDragAnDropItems.IsChecked = false;
                _highlightContainersWhenMouseIsOver.IsChecked = false;
                _overrideContainerLocation.IsChecked = false;
                _overrideContainerLocationSetting.SelectedIndex = 0;
                _backpackStyle.SelectedIndex = 0;
                _hueContainerGumps.IsChecked = true;

            }

            if (page == 12) // experimental
            {

                _disableDefaultHotkeys.IsChecked = false;
                _disableArrowBtn.IsChecked = false;
                _disableTabBtn.IsChecked = false;
                _disableCtrlQWBtn.IsChecked = false;
                _disableAutoMove.IsChecked = false;
            }

        }

        private void SetDefault()
        {

            if (SetDefaultClickCount >= 3)
            {
                SetDefault(1);
                SetDefault(2);
                SetDefault(3);
                SetDefault(4);
                SetDefault(5);
                SetDefault(6);
                SetDefault(7);
                SetDefault(8);
                SetDefault(9);
                SetDefault(10);
                SetDefault(11);
                SetDefault(12);

                GameActions.Print(World, "[Tümü] Ayarlar varsayılan haline getirildi.", 0x44, MessageType.Regular, 1, true);
                SetDefaultClickCount = 0;
            }
            else
            {
                SetDefault(ActivePage);

                switch (ActivePage)
                {
                    case 1: // general
                        GameActions.Print(World, "[Genel] Ayarlar varsayılan haline getirildi.", 0x44, MessageType.System);
                        break;

                    case 2: // sounds
                        GameActions.Print(World, "[Sounds] Ayarlar varsayılan haline getirildi.", 0x44, MessageType.Regular, 1, true);
                        break;

                    case 3: // video
                        GameActions.Print(World, "[Video] Ayarlar varsayılan haline getirildi.", 0x44, MessageType.Regular, 1, true);
                        break;

                    case 4: // macros
                        break;

                    case 5: // tooltip
                        GameActions.Print(World, "[Tooltip] Ayarlar varsayılan haline getirildi.", 0x44, MessageType.Regular, 1, true);
                        break;

                    case 6: // fonts
                        GameActions.Print(World, "[Fonts] Ayarlar varsayılan haline getirildi.", 0x44, MessageType.Regular, 1, true);
                        break;

                    case 7: // speech
                        GameActions.Print(World, "[Speech] Ayarlar varsayılan haline getirildi.", 0x44, MessageType.Regular, 1, true);
                        break;

                    case 8: // combat
                        GameActions.Print(World, "[Combat] Ayarlar varsayılan haline getirildi.", 0x44, MessageType.Regular, 1, true);
                        break;

                    case 9: // counters
                        GameActions.Print(World, "[Counters] Ayarlar varsayılan haline getirildi.", 0x44, MessageType.Regular, 1, true);
                        break;

                    case 10: // info bar
                        GameActions.Print(World, "[Info Bar] Ayarlar varsayılan haline getirildi.", 0x44, MessageType.Regular, 1, true);
                        break;

                    case 11: // containers
                        GameActions.Print(World, "[Containers] Ayarlar varsayılan haline getirildi.", 0x44, MessageType.Regular, 1, true);
                        break;

                    case 12: // experimental
                        GameActions.Print(World, "[Experimental] Ayarlar varsayılan haline getirildi.", 0x44, MessageType.Regular, 1, true);
                        break;
                }

            }

            SetDefaultClickCount++;

            Apply();

        }

        private void Apply()
        {
            GameActions.Print(World, "Ayarlar uygulandı.", 0x44, MessageType.Regular, 1, true);
            WorldViewportGump vp = UIManager.GetGump<WorldViewportGump>();

            // general
            if (Settings.GlobalSettings.FPS != _sliderFPS.Value)
            {
                Client.Game.SetRefreshRate(_sliderFPS.Value);
            }

            _currentProfile.HighlightGameObjects = _highlightObjects.IsChecked;
            _currentProfile.ReduceFPSWhenInactive = _reduceFPSWhenInactive.IsChecked;
            _currentProfile.EnablePathfind = _enablePathfind.IsChecked;
            _currentProfile.UseShiftToPathfind = _useShiftPathfind.IsChecked;
            _currentProfile.AlwaysRun = _alwaysRun.IsChecked;
            _currentProfile.AlwaysRunUnlessHidden = _alwaysRunUnlessHidden.IsChecked;
            _currentProfile.ShowMobilesHP = _showHpMobile.IsChecked;
            _currentProfile.HighlightMobilesByPoisoned = _highlightByPoisoned.IsChecked;
            _currentProfile.HighlightMobilesByParalize = _highlightByParalyzed.IsChecked;
            _currentProfile.HighlightMobilesByInvul = _highlightByInvul.IsChecked;
            _currentProfile.PoisonHue = _poisonColorPickerBox.Hue;
            _currentProfile.ParalyzedHue = _paralyzedColorPickerBox.Hue;
            _currentProfile.InvulnerableHue = _invulnerableColorPickerBox.Hue;
            _currentProfile.MobileHPType = _hpComboBox.SelectedIndex;
            _currentProfile.MobileHPShowWhen = _hpComboBoxShowWhen.SelectedIndex;
            _currentProfile.HoldDownKeyTab = _holdDownKeyTab.IsChecked;
            _currentProfile.HoldDownKeyAltToCloseAnchored = _holdDownKeyAlt.IsChecked;

            _currentProfile.CloseAllAnchoredGumpsInGroupWithRightClick = _closeAllAnchoredGumpsWithRClick.IsChecked;

            _currentProfile.HoldShiftForContext = _holdShiftForContext.IsChecked;
            _currentProfile.HoldAltToMoveGumps = _holdAltToMoveGumps.IsChecked;
            _currentProfile.HoldShiftToSplitStack = _holdShiftToSplitStack.IsChecked;
            _currentProfile.CloseHealthBarType = _healtbarType.SelectedIndex;
            _currentProfile.HideScreenshotStoredInMessage = _hideScreenshotStoredInMessage.IsChecked;

            if (_currentProfile.DrawRoofs == _drawRoofs.IsChecked)
            {
                _currentProfile.DrawRoofs = !_drawRoofs.IsChecked;

                Client.Game.GetScene<GameScene>()?.UpdateMaxDrawZ(true);
            }

            if (_currentProfile.TopbarGumpIsDisabled != _enableTopbar.IsChecked)
            {
                if (_enableTopbar.IsChecked)
                {
                    UIManager.GetGump<TopBarGump>()?.Dispose();
                }
                else
                {
                    TopBarGump.Create(World);
                }

                _currentProfile.TopbarGumpIsDisabled = _enableTopbar.IsChecked;
            }

            if (_currentProfile.EnableCaveBorder != _enableCaveBorder.IsChecked)
            {
                StaticFilters.CleanCaveTextures();
                _currentProfile.EnableCaveBorder = _enableCaveBorder.IsChecked;
            }

            if (_currentProfile.TreeToStumps != _treeToStumps.IsChecked)
            {
                StaticFilters.CleanTreeTextures();
                _currentProfile.TreeToStumps = _treeToStumps.IsChecked;
            }

            _currentProfile.FieldsType = _fieldsType.SelectedIndex;
            _currentProfile.HideVegetation = _hideVegetation.IsChecked;
            _currentProfile.NoColorObjectsOutOfRange = _noColorOutOfRangeObjects.IsChecked;
            _currentProfile.UseCircleOfTransparency = _useCircleOfTransparency.IsChecked;
            _currentProfile.CircleOfTransparencyRadius = _circleOfTranspRadius.Value;
            _currentProfile.CircleOfTransparencyType = _cotType.SelectedIndex;
            _currentProfile.StandardSkillsGump = _useStandardSkillsGump.IsChecked;

            if (_useStandardSkillsGump.IsChecked)
            {
                SkillGumpAdvanced newGump = UIManager.GetGump<SkillGumpAdvanced>();

                if (newGump != null)
                {
                    UIManager.Add(new StandardSkillsGump(World) { X = newGump.X, Y = newGump.Y });

                    newGump.Dispose();
                }
            }
            else
            {
                StandardSkillsGump standardGump = UIManager.GetGump<StandardSkillsGump>();

                if (standardGump != null)
                {
                    UIManager.Add(new SkillGumpAdvanced(World) { X = standardGump.X, Y = standardGump.Y });

                    standardGump.Dispose();
                }
            }

            _currentProfile.ShowNewMobileNameIncoming = _showMobileNameIncoming.IsChecked;
            _currentProfile.ShowNewCorpseNameIncoming = _showCorpseNameIncoming.IsChecked;
            _currentProfile.GridLootType = _gridLoot.SelectedIndex;
            _currentProfile.SallosEasyGrab = _sallosEasyGrab.IsChecked;
            _currentProfile.PartyInviteGump = _partyInviteGump.IsChecked;
            _currentProfile.UseObjectsFading = _objectsFading.IsChecked;
            _currentProfile.TextFading = _textFading.IsChecked;
            _currentProfile.UseSmoothBoatMovement = _use_smooth_boat_movement.IsChecked;

            //if (_currentProfile.ShowHouseContent != _showHouseContent.IsChecked)
            //{
                //_currentProfile.ShowHouseContent = _showHouseContent.IsChecked;
                //NetClient.Socket.Send_ShowPublicHouseContent(_currentProfile.ShowHouseContent);
            //}


            // sounds
            _currentProfile.EnableSound = _enableSounds.IsChecked;
            _currentProfile.EnableMusic = _enableMusic.IsChecked;
            _currentProfile.EnableFootstepsSound = _footStepsSound.IsChecked;
            _currentProfile.EnableCombatMusic = _combatMusic.IsChecked;
            _currentProfile.ReproduceSoundsInBackground = _musicInBackground.IsChecked;
            _currentProfile.SoundVolume = _soundsVolume.Value;
            _currentProfile.MusicVolume = _musicVolume.Value;
            Settings.GlobalSettings.LoginMusicVolume = _loginMusicVolume.Value;
            Settings.GlobalSettings.LoginMusic = _loginMusic.IsChecked;

            Client.Game.Audio.UpdateCurrentMusicVolume();
            Client.Game.Audio.UpdateCurrentSoundsVolume();

            if (!_currentProfile.EnableMusic)
            {
                Client.Game.Audio.StopMusic();
            }

            if (!_currentProfile.EnableSound)
            {
                Client.Game.Audio.StopSounds();
            }

            // speech
            _currentProfile.ScaleSpeechDelay = _scaleSpeechDelay.IsChecked;
            _currentProfile.SpeechDelay = _sliderSpeechDelay.Value;
            _currentProfile.SpeechHue = _speechColorPickerBox.Hue;
            _currentProfile.EmoteHue = _emoteColorPickerBox.Hue;
            _currentProfile.YellHue = _yellColorPickerBox.Hue;
            _currentProfile.WhisperHue = _whisperColorPickerBox.Hue;
            _currentProfile.PartyMessageHue = _partyMessageColorPickerBox.Hue;
            _currentProfile.GuildMessageHue = _guildMessageColorPickerBox.Hue;
            _currentProfile.AllyMessageHue = _allyMessageColorPickerBox.Hue;
            _currentProfile.ChatMessageHue = _chatMessageColorPickerBox.Hue;

            if (_currentProfile.ActivateChatAfterEnter != _chatAfterEnter.IsChecked)
            {
                UIManager.SystemChat.IsActive = !_chatAfterEnter.IsChecked;
                _currentProfile.ActivateChatAfterEnter = _chatAfterEnter.IsChecked;
            }

            _currentProfile.ActivateChatAdditionalButtons = _chatAdditionalButtonsCheckbox.IsChecked;
            _currentProfile.ActivateChatShiftEnterSupport = _chatShiftEnterCheckbox.IsChecked;
            _currentProfile.SaveJournalToFile = _saveJournalCheckBox.IsChecked;

            // video
            _currentProfile.EnableDeathScreen = _enableDeathScreen.IsChecked;
            _currentProfile.EnableBlackWhiteEffect = _enableBlackWhiteEffect.IsChecked;

            var camera = Client.Game.Scene.Camera;
            _currentProfile.DefaultScale = camera.Zoom = (_sliderZoom.Value * camera.ZoomStep) + camera.ZoomMin;

            _currentProfile.EnableMousewheelScaleZoom = _zoomCheckbox.IsChecked;
            _currentProfile.RestoreScaleAfterUnpressCtrl = _restorezoomCheckbox.IsChecked;

            if (!CUOEnviroment.IsOutlands && _use_old_status_gump.IsChecked != _currentProfile.UseOldStatusGump)
            {
                StatusGumpBase status = StatusGumpBase.GetStatusGump();

                _currentProfile.UseOldStatusGump = _use_old_status_gump.IsChecked;

                if (status != null)
                {
                    status.Dispose();
                    UIManager.Add(StatusGumpBase.AddStatusGump(World, status.ScreenCoordinateX, status.ScreenCoordinateY));
                }
            }


            int.TryParse(_gameWindowWidth.Text, out int gameWindowSizeWidth);
            int.TryParse(_gameWindowHeight.Text, out int gameWindowSizeHeight);

            if (gameWindowSizeWidth != Client.Game.Scene.Camera.Bounds.Width || gameWindowSizeHeight != Client.Game.Scene.Camera.Bounds.Height)
            {
                if (vp != null)
                {
                    Point n = vp.ResizeGameWindow(new Point(gameWindowSizeWidth, gameWindowSizeHeight));

                    _gameWindowWidth.SetText(n.X.ToString());
                    _gameWindowHeight.SetText(n.Y.ToString());
                }
            }

            int.TryParse(_gameWindowPositionX.Text, out int gameWindowPositionX);
            int.TryParse(_gameWindowPositionY.Text, out int gameWindowPositionY);

            if (gameWindowPositionX != camera.Bounds.X || gameWindowPositionY != camera.Bounds.Y)
            {
                if (vp != null)
                {
                    vp.SetGameWindowPosition(new Point(gameWindowPositionX, gameWindowPositionY));
                    _currentProfile.GameWindowPosition = vp.Location;
                }
            }

            if (_currentProfile.GameWindowLock != _gameWindowLock.IsChecked)
            {
                if (vp != null)
                {
                    vp.CanMove = !_gameWindowLock.IsChecked;
                }

                _currentProfile.GameWindowLock = _gameWindowLock.IsChecked;
            }

            if (_currentProfile.GameWindowFullSize != _gameWindowFullsize.IsChecked)
            {
                Point n = Point.Zero, loc = Point.Zero;

                if (_gameWindowFullsize.IsChecked)
                {
                    if (vp != null)
                    {
                        n = vp.ResizeGameWindow(new Point(Client.Game.Window.ClientBounds.Width, Client.Game.Window.ClientBounds.Height));
                        vp.SetGameWindowPosition(new Point(-5, -5));
                        _currentProfile.GameWindowPosition = vp.Location;
                    }
                }
                else
                {
                    if (vp != null)
                    {
                        n = vp.ResizeGameWindow(new Point(600, 480));
                        vp.SetGameWindowPosition(new Point(20, 20));
                        _currentProfile.GameWindowPosition = vp.Location;
                    }
                }

                _gameWindowPositionX.SetText(loc.X.ToString());
                _gameWindowPositionY.SetText(loc.Y.ToString());
                _gameWindowWidth.SetText(n.X.ToString());
                _gameWindowHeight.SetText(n.Y.ToString());

                _currentProfile.GameWindowFullSize = _gameWindowFullsize.IsChecked;
            }

            if (_currentProfile.WindowBorderless != _windowBorderless.IsChecked)
            {
                _currentProfile.WindowBorderless = _windowBorderless.IsChecked;
                Client.Game.SetWindowBorderless(_windowBorderless.IsChecked);
            }

            _currentProfile.UseAlternativeLights = _altLights.IsChecked;
            _currentProfile.UseCustomLightLevel = _enableLight.IsChecked;
            _currentProfile.LightLevel = (byte)(_lightBar.MaxValue - _lightBar.Value);
            _currentProfile.LightLevelType = _lightLevelType.SelectedIndex;

            if (_enableLight.IsChecked)
            {
                World.Light.Overall = _currentProfile.LightLevelType == 1 ? Math.Min(World.Light.RealOverall, _currentProfile.LightLevel) : _currentProfile.LightLevel;
                World.Light.Personal = 0;
            }
            else
            {
                World.Light.Overall = World.Light.RealOverall;
                World.Light.Personal = World.Light.RealPersonal;
            }

            _currentProfile.UseColoredLights = _useColoredLights.IsChecked;
            _currentProfile.UseDarkNights = _darkNights.IsChecked;
            _currentProfile.ShadowsEnabled = _enableShadows.IsChecked;
            _currentProfile.ShadowsStatics = _enableShadowsStatics.IsChecked;
            _currentProfile.TerrainShadowsLevel = _terrainShadowLevel.Value;
            _currentProfile.AuraUnderFeetType = _auraType.SelectedIndex;

            Client.Game.IsMouseVisible = Settings.GlobalSettings.RunMouseInASeparateThread = _runMouseInSeparateThread.IsChecked;

            _currentProfile.AuraOnMouse = _auraMouse.IsChecked;
            _currentProfile.AnimatedWaterEffect = _animatedWaterEffect.IsChecked;
            _currentProfile.PartyAura = _partyAura.IsChecked;
            _currentProfile.PartyAuraHue = _partyAuraColorPickerBox.Hue;
            _currentProfile.HideChatGradient = _hideChatGradient.IsChecked;
            _currentProfile.IgnoreGuildMessages = _ignoreGuildMessages.IsChecked;
            _currentProfile.IgnoreAllianceMessages = _ignoreAllianceMessages.IsChecked;

            // fonts
            _currentProfile.ForceUnicodeJournal = _forceUnicodeJournal.IsChecked;
            byte _fontValue = _fontSelectorChat.GetSelectedFont();
            _currentProfile.OverrideAllFonts = _overrideAllFonts.IsChecked;
            _currentProfile.OverrideAllFontsIsUnicode = _overrideAllFontsIsUnicodeCheckbox.SelectedIndex == 1;

            if (_currentProfile.ChatFont != _fontValue)
            {
                _currentProfile.ChatFont = _fontValue;
                UIManager.SystemChat.TextBoxControl.Font = _fontValue;
            }

            // combat
            _currentProfile.InnocentHue = _innocentColorPickerBox.Hue;
            _currentProfile.FriendHue = _friendColorPickerBox.Hue;
            _currentProfile.CriminalHue = _crimialColorPickerBox.Hue;
            _currentProfile.CanAttackHue = _canAttackColorPickerBox.Hue;
            _currentProfile.EnemyHue = _enemyColorPickerBox.Hue;
            _currentProfile.MurdererHue = _murdererColorPickerBox.Hue;
            _currentProfile.EnabledCriminalActionQuery = _queryBeforAttackCheckbox.IsChecked;
            _currentProfile.EnabledBeneficialCriminalActionQuery = _queryBeforeBeneficialCheckbox.IsChecked;
            _currentProfile.CastSpellsByOneClick = _uiButtonsSingleClick.IsChecked;
            _currentProfile.BuffBarTime = _buffBarTime.IsChecked;
            _currentProfile.FastSpellsAssign = _enableFastSpellsAssign.IsChecked;

            _currentProfile.BeneficHue = _beneficColorPickerBox.Hue;
            _currentProfile.HarmfulHue = _harmfulColorPickerBox.Hue;
            _currentProfile.NeutralHue = _neutralColorPickerBox.Hue;
            _currentProfile.EnabledSpellHue = _spellColoringCheckbox.IsChecked;
            _currentProfile.EnabledSpellFormat = _spellFormatCheckbox.IsChecked;
            _currentProfile.SpellDisplayFormat = _spellFormatBox.Text;

            // macros
            World.Macros.Save();

            // counters

            bool before = _currentProfile.CounterBarEnabled;
            _currentProfile.CounterBarEnabled = _enableCounters.IsChecked;
            _currentProfile.CounterBarCellSize = _cellSize.Value;

            if (!int.TryParse(_rows.Text, out int v))
            {
                v = 1;
                _rows.SetText("1");
            }

            _currentProfile.CounterBarRows = v;

            if (!int.TryParse(_columns.Text, out v))
            {
                v = 1;
                _columns.SetText("1");
            }
            _currentProfile.CounterBarColumns = v;
            _currentProfile.CounterBarHighlightOnUse = _highlightOnUse.IsChecked;

            if (!int.TryParse(_highlightAmount.Text, out v))
            {
                v = 5;
                _highlightAmount.SetText("5");
            }
            _currentProfile.CounterBarHighlightAmount = v;

            if (!int.TryParse(_abbreviatedAmount.Text, out v))
            {
                v = 1000;
                _abbreviatedAmount.SetText("1000");
            }
            _currentProfile.CounterBarAbbreviatedAmount = v;
            _currentProfile.CounterBarHighlightOnAmount = _highlightOnAmount.IsChecked;
            _currentProfile.CounterBarDisplayAbbreviatedAmount = _enableAbbreviatedAmount.IsChecked;

            CounterBarGump counterGump = UIManager.GetGump<CounterBarGump>();

            counterGump?.SetLayout(_currentProfile.CounterBarCellSize, _currentProfile.CounterBarRows, _currentProfile.CounterBarColumns);


            if (before != _currentProfile.CounterBarEnabled)
            {
                if (counterGump == null)
                {
                    if (_currentProfile.CounterBarEnabled)
                    {
                        UIManager.Add
                        (
                            new CounterBarGump
                            (
                                World,
                                200,
                                200,
                                _currentProfile.CounterBarCellSize,
                                _currentProfile.CounterBarRows,
                                _currentProfile.CounterBarColumns
                            )
                        );
                    }
                }
                else
                {
                    counterGump.IsEnabled = counterGump.IsVisible = _currentProfile.CounterBarEnabled;
                }
            }

            // experimental
            // Reset nested checkboxes if parent checkbox is unchecked
            if (!_disableDefaultHotkeys.IsChecked)
            {
                _disableArrowBtn.IsChecked = false;
                _disableTabBtn.IsChecked = false;
                _disableCtrlQWBtn.IsChecked = false;
                _disableAutoMove.IsChecked = false;
            }

            // NOTE: Keep these assignments AFTER the code above that resets nested checkboxes if parent checkbox is unchecked
            _currentProfile.DisableDefaultHotkeys = _disableDefaultHotkeys.IsChecked;
            _currentProfile.DisableArrowBtn = _disableArrowBtn.IsChecked;
            _currentProfile.DisableTabBtn = _disableTabBtn.IsChecked;
            _currentProfile.DisableCtrlQWBtn = _disableCtrlQWBtn.IsChecked;
            _currentProfile.DisableAutoMove = _disableAutoMove.IsChecked;
            _currentProfile.AutoOpenDoors = _autoOpenDoors.IsChecked;
            _currentProfile.SmoothDoors = _smoothDoors.IsChecked;
            _currentProfile.AutoOpenCorpses = _autoOpenCorpse.IsChecked;
            _currentProfile.AutoOpenCorpseRange = int.Parse(_autoOpenCorpseRange.Text);
            _currentProfile.CorpseOpenOptions = _autoOpenCorpseOptions.SelectedIndex;
            _currentProfile.SkipEmptyCorpse = _skipEmptyCorpse.IsChecked;

            _currentProfile.EnableDragSelect = _enableDragSelect.IsChecked;
            _currentProfile.DragSelectModifierKey = _dragSelectModifierKey.SelectedIndex;
            _currentProfile.DragSelectHumanoidsOnly = _dragSelectHumanoidsOnly.IsChecked;
            _currentProfile.DragSelectStartX = _dragSelectStartX.Value;
            _currentProfile.DragSelectStartY = _dragSelectStartY.Value;
            _currentProfile.DragSelectAsAnchor = _dragSelectAsAnchor.IsChecked;

            _currentProfile.ShowSkillsChangedMessage = _showSkillsMessage.IsChecked;
            _currentProfile.ShowSkillsChangedDeltaValue = _showSkillsMessageDelta.Value;
            _currentProfile.ShowStatsChangedMessage = _showStatsMessage.IsChecked;

            _currentProfile.OverrideContainerLocation = _overrideContainerLocation.IsChecked;
            _currentProfile.OverrideContainerLocationSetting = _overrideContainerLocationSetting.SelectedIndex;

            _currentProfile.ShowTargetRangeIndicator = _showTargetRangeIndicator.IsChecked;


            bool updateHealthBars = _currentProfile.CustomBarsToggled != _customBars.IsChecked;
            _currentProfile.CustomBarsToggled = _customBars.IsChecked;

            if (updateHealthBars)
            {
                if (_currentProfile.CustomBarsToggled)
                {
                    List<HealthBarGump> hbgstandard = UIManager.Gumps.OfType<HealthBarGump>().ToList();

                    foreach (HealthBarGump healthbar in hbgstandard)
                    {
                        UIManager.Add(new HealthBarGumpCustom(World, healthbar.LocalSerial) { X = healthbar.X, Y = healthbar.Y });

                        healthbar.Dispose();
                    }
                }
                else
                {
                    List<HealthBarGumpCustom> hbgcustom = UIManager.Gumps.OfType<HealthBarGumpCustom>().ToList();

                    foreach (HealthBarGumpCustom customhealthbar in hbgcustom)
                    {
                        UIManager.Add(new HealthBarGump(World, customhealthbar.LocalSerial) { X = customhealthbar.X, Y = customhealthbar.Y });

                        customhealthbar.Dispose();
                    }
                }
            }

            _currentProfile.CBBlackBGToggled = _customBarsBBG.IsChecked;
            _currentProfile.SaveHealthbars = _saveHealthbars.IsChecked;


            // infobar
            _currentProfile.ShowInfoBar = _showInfoBar.IsChecked;
            _currentProfile.InfoBarHighlightType = _infoBarHighlightType.SelectedIndex;

            World.InfoBars.Clear();

            for (int i = 0; i < _infoBarBuilderControls.Count; i++)
            {
                if (!_infoBarBuilderControls[i].IsDisposed)
                {
                    World.InfoBars.AddItem(new InfoBarItem(_infoBarBuilderControls[i].LabelText, _infoBarBuilderControls[i].Var, _infoBarBuilderControls[i].Hue));
                }
            }

            World.InfoBars.Save();

            InfoBarGump infoBarGump = UIManager.GetGump<InfoBarGump>();

            if (_currentProfile.ShowInfoBar)
            {
                if (infoBarGump == null)
                {
                    UIManager.Add(new InfoBarGump(World) { X = 300, Y = 300 });
                }
                else
                {
                    infoBarGump.ResetItems();
                    infoBarGump.SetInScreen();
                }
            }
            else
            {
                if (infoBarGump != null)
                {
                    infoBarGump.Dispose();
                }
            }


            // containers
            int containerScale = _currentProfile.ContainersScale;

            if ((byte)_containersScale.Value != containerScale || _currentProfile.ScaleItemsInsideContainers != _containerScaleItems.IsChecked)
            {
                containerScale = _currentProfile.ContainersScale = (byte)_containersScale.Value;
                UIManager.ContainerScale = containerScale / 100f;
                _currentProfile.ScaleItemsInsideContainers = _containerScaleItems.IsChecked;

                foreach (ContainerGump resizableGump in UIManager.Gumps.OfType<ContainerGump>())
                {
                    resizableGump.RequestUpdateContents();
                }
            }

            _currentProfile.UseLargeContainerGumps = _useLargeContianersGumps.IsChecked;
            _currentProfile.DoubleClickToLootInsideContainers = _containerDoubleClickToLoot.IsChecked;
            _currentProfile.RelativeDragAndDropItems = _relativeDragAnDropItems.IsChecked;
            _currentProfile.HighlightContainerWhenSelected = _highlightContainersWhenMouseIsOver.IsChecked;
            _currentProfile.HueContainerGumps = _hueContainerGumps.IsChecked;

            if (_currentProfile.BackpackStyle != _backpackStyle.SelectedIndex)
            {
                _currentProfile.BackpackStyle = _backpackStyle.SelectedIndex;
                UIManager.GetGump<PaperDollGump>(World.Player.Serial)?.RequestUpdateContents();
                Item backpack = World.Player.FindItemByLayer(Layer.Backpack);
                GameActions.DoubleClick(World, backpack);
            }


            // tooltip
            _currentProfile.UseTooltip = _use_tooltip.IsChecked;
            _currentProfile.TooltipTextHue = _tooltip_font_hue.Hue;
            _currentProfile.TooltipDelayBeforeDisplay = _delay_before_display_tooltip.Value;
            _currentProfile.TooltipBackgroundOpacity = _tooltip_background_opacity.Value;
            _currentProfile.TooltipDisplayZoom = _tooltip_zoom.Value;
            _currentProfile.TooltipFont = _tooltip_font_selector.GetSelectedFont();
            _currentProfile.UIType = _uiType.SelectedIndex;                 // _uiType


            // Disabled Settings
            _currentProfile.ShowHouseContent = false;           // _showHouseContent
            _currentProfile.OverrideAllFonts = false;           // _overrideAllFonts
            _currentProfile.OverrideAllFontsIsUnicode = true;   // _overrideAllFontsIsUnicodeCheckbox
            _currentProfile.DrawRoofs = true;                   // _drawRoofs
            _currentProfile.TreeToStumps = false;               // _treeToStumps
            _currentProfile.CloseHealthBarType = 1;             // _healtbarType
            _currentProfile.GridLootType = 0;                   // _gridLoot
            _currentProfile.FieldsType = 0;                     // _fieldsType
            _currentProfile.UseDarkNights = true;               //_darkNights
            _currentProfile.SmoothDoors = false;                // _smoothDoors
            _currentProfile.AutoOpenCorpses = false;            // _autoOpenCorpse
            _currentProfile.UseCustomLightLevel = false;        // _enableLight
            _currentProfile.UseAlternativeLights = false;       // _altLights
            _currentProfile.DoubleClickToLootInsideContainers = false;
            _currentProfile.HideChatGradient = true;            // _hideChatGradient
            _currentProfile.SkipEmptyCorpse = false;            // _skipEmptyCorpse
            _currentProfile.EnableDeathScreen = true;           // _enableDeathScreen
            _currentProfile.EnableBlackWhiteEffect = true;      // _enableBlackWhiteEffect
            _currentProfile.UseOldStatusGump = true;            // _use_old_status_gump
            _currentProfile.SallosEasyGrab = false;             // _sallosEasyGrab
            _currentProfile.CustomBarsToggled = false;          // _customBars
            _currentProfile.CBBlackBGToggled = false;           // _customBarsBBG
            _currentProfile.LightLevel = 0;                     // _lightBar
            _currentProfile.LightLevelType = 0;                 // _lightLevelType
            _currentProfile.UseColoredLights = false;           // _useColoredLights
            _currentProfile.TreeToStumps = false;               // _treeToStumps
            _currentProfile.CloseHealthBarType = 0;             // _healtbarType

            _currentProfile?.Save(World, ProfileManager.ProfilePath);

        }

        internal void UpdateVideo()
        {
            var camera = Client.Game.Scene.Camera;

            _gameWindowPositionX.SetText(camera.Bounds.X.ToString());
            _gameWindowPositionY.SetText(camera.Bounds.Y.ToString());
            _gameWindowWidth.SetText(camera.Bounds.Width.ToString());
            _gameWindowHeight.SetText(camera.Bounds.Height.ToString());
        }

        public override bool Draw(UltimaBatcher2D batcher, int x, int y)
        {
            Vector3 hueVector = ShaderHueTranslator.GetHueVector(0);

            /*
            batcher.Draw
            (
                LogoTexture,
                new Rectangle
                (
                    x + 190,
                    y + 20,
                    WIDTH - 250,
                    400
                ),
                hueVector
            );

            batcher.DrawRectangle
            (
                SolidColorTextureCache.GetTexture(Color.Gray),
                x,
                y,
                Width,
                Height,
                hueVector
            );
            */

            return base.Draw(batcher, x, y);
        }



        public static int[] AllIndexesOf(string str, string substr, bool ignoreCase = false)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("String or substring is not specified.");
            }

            var indexes = new List<int>();
            int index = 0;

            while ((index = str.IndexOf(substr, index, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) != -1)
            {
                indexes.Add(index++);
            }

            return indexes.ToArray();
        }

        public SDL_Keycode MacroToKeyCode(string str)
        {
            SDL_Keycode key;

            if (str == "ESC")
                key = SDL_Keycode.SDLK_ESCAPE;
            else if (str == "BACKSPACE")
                key = SDL_Keycode.SDLK_AC_BACK;
            else if (str == "TAB")
                key = SDL_Keycode.SDLK_TAB;
            else if (str == "ENTER")
                key = SDL_Keycode.SDLK_RETURN;
            else if (str == "CTRL")
                key = SDL_Keycode.SDLK_LCTRL;
            else if (str == "ALT")
                key = SDL_Keycode.SDLK_LALT;
            else if (str == "SHIFT")
                key = SDL_Keycode.SDLK_LSHIFT;
            else if (str == "SPACE")
                key = SDL_Keycode.SDLK_SPACE;
            else if (str == "CAPS LOCK")
                key = SDL_Keycode.SDLK_CAPSLOCK;
            else if (str == "F1")
                key = SDL_Keycode.SDLK_F1;
            else if (str == "F2")
                key = SDL_Keycode.SDLK_F2;
            else if (str == "F3")
                key = SDL_Keycode.SDLK_F3;
            else if (str == "F4")
                key = SDL_Keycode.SDLK_F4;
            else if (str == "F5")
                key = SDL_Keycode.SDLK_F5;
            else if (str == "F6")
                key = SDL_Keycode.SDLK_F6;
            else if (str == "F7")
                key = SDL_Keycode.SDLK_F7;
            else if (str == "F8")
                key = SDL_Keycode.SDLK_F8;
            else if (str == "F9")
                key = SDL_Keycode.SDLK_F9;
            else if (str == "F10")
                key = SDL_Keycode.SDLK_F10;
            else if (str == "F11")
                key = SDL_Keycode.SDLK_F11;
            else if (str == "F12")
                key = SDL_Keycode.SDLK_F12;
            else if (str == "PAUSE")
                key = SDL_Keycode.SDLK_PAUSE;
            else if (str == "SCROLL LOCK")
                key = SDL_Keycode.SDLK_SCROLLLOCK;
            else if (str == "NUM 0")
                key = SDL_Keycode.SDLK_KP_0;
            else if (str == "NUM 1")
                key = SDL_Keycode.SDLK_KP_1;
            else if (str == "NUM 2")
                key = SDL_Keycode.SDLK_KP_2;
            else if (str == "NUM 3")
                key = SDL_Keycode.SDLK_KP_3;
            else if (str == "NUM 4")
                key = SDL_Keycode.SDLK_KP_4;
            else if (str == "NUM 5")
                key = SDL_Keycode.SDLK_KP_5;
            else if (str == "NUM 6")
                key = SDL_Keycode.SDLK_KP_6;
            else if (str == "NUM 7")
                key = SDL_Keycode.SDLK_KP_7;
            else if (str == "NUM 8")
                key = SDL_Keycode.SDLK_KP_8;
            else if (str == "NUM 9")
                key = SDL_Keycode.SDLK_KP_9;
            else if (str == "NUM *")
                key = SDL_Keycode.SDLK_KP_MULTIPLY;
            else if (str == "NUM -")
                key = SDL_Keycode.SDLK_KP_MINUS;
            else if (str == "NUM +")
                key = SDL_Keycode.SDLK_KP_PLUS;
            else if (str == "NUM DEL")
                key = SDL_Keycode.SDLK_DELETE;
            else
            {
                var lower = str.ToLower();
                key = (SDL_Keycode)lower[0];

            }

            return key;
        }

        public string[] MacroTypeName = new string[]
        {
            "(NONE)",
            "Say",
            "Emote",
            "Whisper",
            "Yell",
            "Walk",
            "War/Peace",
            "Paste",
            "Open",
            "Close",
            "Minimize",
            "Maximize",
            "OpenDoor",
            "UseSkill",
            "LastSkill",
            "CastSpell",
            "LastSpell",
            "LastObject",
            "Bow",
            "Salute",
            "QuitGame",
            "AllNames",
            "LastTarget",
            "TargetSelf",
            "Arm/Disarm",
            "WaitForTarg",
            "TargetNext",
            "AttackLast",
            "Delay",
            "CircleTrans",
            "CloseGumps",
            "AlwaysRun",
            "SaveDesktop",
            "KillGumpOpen",
            "PrimaryAbility",
            "SecondaryAbility",
            "EquipLastWeapon",
            "SetUpdateRange",
            "ModifyUpdateRange",
            "IncreaseUpdateRange",
            "DecreaseUpdateRange",
            "MaxUpdateRange",
            "MinUpdateRange",
            "DefaultUpdateRange",
            "EnableRangeColor",
            "DisableRangeColor",
            "ToggleRangeColor",
            "InvoreVirture",
            "SelectNext",
            "SelectPreveous",
            "SelectNearest",
            "AttackSelectedTarget",
            "UseSelectedTarget",
            "CurrentTarget",
            "TargetSystemOn/Off",
            "ToggleBuficonWindow",
            "BandageSelf",
            "BandageTarget",
            "ToggleGargoyleFlying"
        };


        public string[] MacroSubTypeName = new string[]
        {
            "?",
            "NW (top)", //Walk group
            "N (topright)",
            "NE (right)",
            "E (bottonright)",
            "SE (bottom)",
            "S (bottomleft)",
            "SW (left)",
            "W (topleft)",
            "Configuration", //Open/Close/Minimize/Maximize group
            "Paperdoll",
            "Status",
            "Journal",
            "Skills",
            "Mage Spellbook",
            "Chat",
            "Backpack",
            "Overview",
            "World Map",
            "Mail",
            "Party Manifest",
            "Party Chat",
            "Necro Spellbook",
            "Paladin Spellbook",
            "Combat Book",
            "Bushido Spellbook",
            "Ninjitsu Spellbook",
            "Guild",
            "Spell Weaving Spellbook",
            "Quest Log",
            "Mysticism Spellbook",
            "Racial Abilities Book",
            "Bard Spellbook",
            "Anatomy", //Skills group
            "Animal Lore",
            "Animal Taming",
            "Arms Lore",
            "Begging",
            "Cartograpy",
            "Detecting Hidden",
            "Enticement",
            "Evaluating Intelligence",
            "Forensic Evaluation",
            "Hiding",
            "Imbuing",
            "Inscription",
            "Item Identification",
            "Meditation",
            "Peacemaking",
            "Poisoning",
            "Provocation",
            "Remove Trap",
            "Spirit Speak",
            "Stealing",
            "Stealth",
            "Taste Identification",
            "Tracking",
            "Left Hand", ///Arm/Disarm group
            "Right Hand",
            "Honor", //Invoke Virture group
            "Sacrifice",
            "Valor",
            "Clumsy", //Cast Spell group
            "Create Food",
            "Feeblemind",
            "Heal",
            "Magic Arrow",
            "Night Sight",
            "Reactive Armor",
            "Weaken",
            "Agility",
            "Cunning",
            "Cure",
            "Harm",
            "Magic Trap",
            "Magic Untrap",
            "Protection",
            "Strength",
            "Bless",
            "Fireball",
            "Magic Lock",
            "Poison",
            "Telekinesis",
            "Teleport",
            "Unlock",
            "Wall of Stone",
            "Arch Cure",
            "Arch Protection",
            "Curse",
            "Fire Field",
            "Greater Heal",
            "Lightning",
            "Mana Drain",
            "Recall",
            "Blade Spirits",
            "Dispell Field",
            "Incognito",
            "Magic Reflection",
            "Mind Blast",
            "Paralyze",
            "Poison Field",
            "Summon Creature",
            "Dispel",
            "Energy Bolt",
            "Explosion",
            "Invisibility",
            "Mark",
            "Mass Curse",
            "Paralyze Field",
            "Reveal",
            "Chain Lightning",
            "Energy Field",
            "Flame Strike",
            "Gate Travel",
            "Mana Vampire",
            "Mass Dispel",
            "Meteor Swarm",
            "Polymorph",
            "Earthquake",
            "Energy Vortex",
            "Resurrection",
            "Air Elemental",
            "Summon Daemon",
            "Earth Elemental",
            "Fire Elemental",
            "Water Elemental",
            //"Animate Dead",
            //"Blood Oath",
            //"Corpse Skin",
            //"Curse Weapon",
            //"Evil Omen",
            //"Horrific Beast",
            //"Lich Form",
            //"Mind Rot",
            //"Pain Spike",
            //"Poison Strike",
            //"Strangle",
            //"Summon Familar",
            //"Vampiric Embrace",
            //"Vengeful Spirit",
            //"Wither",
            //"Wraith Form",
            //"Exorcism",
            //"Cleanse By Fire",
            //"Close Wounds",
            //"Concentrate Weapon",
            //"Dispel Evil",
            //"Divine Fury",
            //"Enemy Of One",
            //"Holy Light",
            //"Noble Sacrifice",
            //"Remove Curse",
            //"Sacred Journey",
            //"Honorable Execution",
            //"Confidence",
            //"Evasion",
            //"Counter Attack",
            //"Lightning Strike",
            //"Momentum Strike",
            //"Focus Attack",
            //"Death Strike",
            //"Animal Form",
            //"Ki Attack",
            //"Surprice Attack",
            //"Backstab",
            //"Shadowjump",
            //"Mirror Image",
            //"Arcane Circle",
            //"Gift Of Reneval",
            //"Immolating Weapon",
            //"Attunement",
            //"Thunderstorm",
            //"Natures Fury",
            //"Summon Fey",
            //"Summon Fiend",
            //"Reaper Form",
            //"Wildfire",
            //"Essence Of Wind",
            //"Dryad Allure",
            //"Ethereal Voyage",
            //"Word Of Death",
            //"Gift Of Life",
            //"Arcane Empowerment",
            //"Nether Bolt",
            //"Healing Stone",
            //"Purge Magic",
            //"Enchant",
            //"Sleep",
            //"Eagle Strike",
            //"Animated Weapon",
            //"Stone Form",
            //"Spell Trigger",
            //"Mass Sleep",
            //"Cleansing Winds",
            //"Bombard",
            //"Spell Plague",
            //"Hail Storm",
            //"Nether Cyclone",
            //"Rising Colossus",
            //"Inspire",
            //"Invigorate",
            //"Resilience",
            //"Perseverance",
            //"Tribulation",
            //"Despair",
            "Hostile", //Select Next/Previous/Nearest group
            "Party",
            "Follower",
            "Object",
            "Mobile"
        };



        private InputField AddInputField
        (
            ScrollArea area,
            int x,
            int y,
            int width,
            int height,
            string label = null,
            int maxWidth = 0,
            bool set_down = false,
            bool numbersOnly = false,
            int maxCharCount = -1
        )
        {
            InputField elem = new InputField
            (
                0x0BB8,
                FONT,
                HUE_FONT,
                true,
                width,
                height,
                maxWidth,
                maxCharCount
            )
            {
                NumbersOnly = numbersOnly,
                X = x,
                Y = y
            };


            if (area != null)
            {
                Label text = AddLabel(area, label, x, y);

                if (set_down)
                {
                    elem.Y = text.Bounds.Bottom + 2;
                }
                else
                {
                    elem.X = text.Bounds.Right + 2;
                }

                area.Add(elem);
            }

            return elem;
        }

        private Label AddLabel(ScrollArea area, string text, int x, int y)
        {
            Label label = new Label(text, true, HUE_FONT)
            {
                X = x,
                Y = y
            };

            area?.Add(label);

            return label;
        }

        private Checkbox AddCheckBox(ScrollArea area, string text, bool ischecked, int x, int y, bool disabled = false)
        {

            Checkbox box = new Checkbox
            (
                0x00D2,
                0x00D3,
                text,
                FONT,
                HUE_FONT
            )
            {
                IsChecked = ischecked,
                X = x,
                Y = y
            };

            if (disabled == true)
            {
                box = new Checkbox
               (
                   0x00D4,
                   0x00D4,
                   text,
                   FONT,
                   HUE_FONT
               )
                {
                    IsChecked = false,
                    IsEnabled = false,
                    X = x,
                    Y = y,
                };
            }

            area?.Add(box);

            return box;
        }

        private Combobox AddCombobox
        (
            ScrollArea area,
            string[] values,
            int currentIndex,
            int x,
            int y,
            int width,
            bool disabled = false
        )
        {
            Combobox combobox = new Combobox(x, y, width, values)
            {
                SelectedIndex = currentIndex,
                IsEditable = disabled == true ? false : true,
                IsEnabled = disabled == true ? false : true,
            };

            area?.Add(combobox);

            return combobox;
        }

        private HSliderBar AddHSlider
        (
            ScrollArea area,
            int min,
            int max,
            int value,
            int x,
            int y,
            int width
        )
        {
            HSliderBar slider = new HSliderBar
            (
                x,
                y,
                width,
                min,
                max,
                value,
                HSliderBarStyle.MetalWidgetRecessedBar,
                true,
                FONT,
                HUE_FONT
            );

            area?.Add(slider);

            return slider;
        }

        private ClickableColorBox AddColorBox(ScrollArea area, int x, int y, ushort hue, string text)
        {
            ClickableColorBox box = new ClickableColorBox
            (
                this.World,
                x,
                y,
                13,
                14,
                hue
            );

            area?.Add(box);

            area?.Add
            (
                new Label(text, true, HUE_FONT)
                {
                    X = x + box.Width + 10,
                    Y = y
                }
            );

            return box;
        }

        private SettingsSection AddSettingsSection(DataBox area, string label)
        {
            SettingsSection section = new SettingsSection(label, area.Width);
            area.Add(section);
            area.WantUpdateSize = true;
            //area.ReArrangeChildren();

            return section;
        }

        protected override void OnDragBegin(int x, int y)
        {
            if (UIManager.MouseOverControl?.RootParent == this)
            {
                UIManager.MouseOverControl.InvokeDragBegin(new Point(x, y));
            }

            base.OnDragBegin(x, y);
        }

        protected override void OnDragEnd(int x, int y)
        {
            if (UIManager.MouseOverControl?.RootParent == this)
            {
                UIManager.MouseOverControl.InvokeDragEnd(new Point(x, y));
            }

            base.OnDragEnd(x, y);
        }

        private enum Buttons
        {
            Disabled, //no action will be done on these buttons, at least not by OnButtonClick()
            Cancel,
            Apply,
            Default,
            Ok,
            SpeechColor,
            EmoteColor,
            PartyMessageColor,
            GuildMessageColor,
            AllyMessageColor,
            InnocentColor,
            FriendColor,
            CriminalColor,
            EnemyColor,
            MurdererColor,

            OpenIgnoreList,
            NewMacro,
            DeleteMacro,
            AllDeleteMacro,
            ConvertMacro,
            OpenMacrosFolder,
            PrevMacroButton,
            NextMacroButton,
            ProfileManagerButton,
            Last = NextMacroButton

            //Last = ConvertMacro

            //Last = DeleteMacro
        }


        private class SettingsSection : Control
        {
            private readonly DataBox _databox;
            private int _indent;

            public SettingsSection(string title, int width)
            {
                CanMove = true;
                AcceptMouseInput = true;
                WantUpdateSize = true;


                Label label = new Label(title, true, HUE_FONT, font: FONT);
                label.X = 5;
                base.Add(label);

                base.Add
                (
                    new Line
                    (
                        0,
                        label.Height,
                        width - 30,
                        1,
                        0xFFbabdc2
                    )
                );

                Width = width;
                Height = label.Height + 1;

                _databox = new DataBox(label.X + 10, label.Height + 4, 0, 0);

                base.Add(_databox);
            }

            public void PushIndent()
            {
                _indent += 40;
            }

            public void PopIndent()
            {
                _indent -= 40;
            }


            public void AddRight(Control c, int offset = 15)
            {
                int i = _databox.Children.Count - 1;

                for (; i >= 0; --i)
                {
                    if (_databox.Children[i].IsVisible)
                    {
                        break;
                    }
                }

                c.X = i >= 0 ? _databox.Children[i].Bounds.Right + offset : _indent;

                c.Y = i >= 0 ? _databox.Children[i].Bounds.Top : 0;

                _databox.Add(c);
                _databox.WantUpdateSize = true;
            }

            public override void Add(Control c, int page = 0)
            {
                int i = _databox.Children.Count - 1;
                int bottom = 0;

                for (; i >= 0; --i)
                {
                    if (_databox.Children[i].IsVisible)
                    {
                        if (bottom == 0 || bottom < _databox.Children[i].Bounds.Bottom + 2)
                        {
                            bottom = _databox.Children[i].Bounds.Bottom + 2;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                c.X = _indent;
                c.Y = bottom;

                _databox.Add(c, page);
                _databox.WantUpdateSize = true;

                Height += c.Height + 2;
            }
        }

        private class FontSelector : Control
        {
            private readonly RadioButton[] _buttons;

            public FontSelector(int max_font, int current_font_index, string markup)
            {
                CanMove = false;
                CanCloseWithRightClick = false;

                int y = 0;

                _buttons = new RadioButton[max_font];

                for (byte i = 0; i < max_font; i++)
                {
                    if (FontsLoader.Instance.UnicodeFontExists(i))
                    {
                        Add
                        (
                            _buttons[i] = new RadioButton
                            (
                                0,
                                0x00D0,
                                0x00D1,
                                markup,
                                i,
                                HUE_FONT
                            )
                            {
                                Y = y,
                                Tag = i,
                                IsChecked = current_font_index == i
                            }
                        );

                        y += 25;
                    }
                }
            }

            public byte GetSelectedFont()
            {
                for (byte i = 0; i < _buttons.Length; i++)
                {
                    RadioButton b = _buttons[i];

                    if (b != null && b.IsChecked)
                    {
                        return i;
                    }
                }

                return 0xFF;
            }

            public void SetSelectedFont(int index)
            {
                if (index >= 0 && index < _buttons.Length && _buttons[index] != null)
                {
                    _buttons[index].IsChecked = true;
                }
            }
        }

        private class InputField : Control
        {
            private readonly StbTextBox _textbox;

            public InputField
            (
                ushort backgroundGraphic,
                byte font,
                ushort hue,
                bool unicode,
                int width,
                int height,
                int maxWidthText = 0,
                int maxCharsCount = -1
            )
            {
                WantUpdateSize = false;

                Width = width;
                Height = height;

                ResizePic background = new ResizePic(backgroundGraphic)
                {
                    Width = width,
                    Height = height
                };

                _textbox = new StbTextBox
                (
                    font,
                    maxCharsCount,
                    maxWidthText,
                    unicode,
                    FontStyle.BlackBorder,
                    hue
                )
                {
                    X = 4,
                    Y = 4,
                    Width = width - 8,
                    Height = height - 8
                };


                Add(background);
                Add(_textbox);
            }

            public override bool Draw(UltimaBatcher2D batcher, int x, int y)
            {
                if (batcher.ClipBegin(x, y, Width, Height))
                {
                    base.Draw(batcher, x, y);

                    batcher.ClipEnd();
                }

                return true;
            }


            public string Text => _textbox.Text;

            public override bool AcceptKeyboardInput
            {
                get => _textbox.AcceptKeyboardInput;
                set => _textbox.AcceptKeyboardInput = value;
            }

            public bool NumbersOnly
            {
                get => _textbox.NumbersOnly;
                set => _textbox.NumbersOnly = value;
            }


            public void SetText(string text)
            {
                _textbox.SetText(text);
            }
        }
    }
}
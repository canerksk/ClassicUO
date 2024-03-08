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

namespace ClassicUO.Game.Data
{
    internal enum Layer : byte
    {
        Invalid = 0x00,
        OneHanded = 0x01,
        TwoHanded = 0x02,
        Shoes = 0x03,
        Pants = 0x04,
        Shirt = 0x05,
        Helmet = 0x06,
        Gloves = 0x07,
        Ring = 0x08,
        Talisman = 0x09,
        Necklace = 0x0A,
        Hair = 0x0B,
        Waist = 0x0C,
        Torso = 0x0D,
        Bracelet = 0x0E,
        Face = 0x0F,
        Beard = 0x10,
        Tunic = 0x11,
        Earrings = 0x12,
        Arms = 0x13,
        Cloak = 0x14,
        Backpack = 0x15,
        Robe = 0x16,
        Skirt = 0x17,
        Legs = 0x18,
        Mount = 0x19,
        ShopBuyRestock = 0x1A,
        ShopBuy = 0x1B,
        ShopSell = 0x1C,
        Bank = 0x1D,
        Special = 0x1E, // 30
        Dragging = 0x1F, // 31
        SpellStat = 0x20, // 32
        SpellReactive = 0x21, // 33
        SpellNightSight = 0x22, // 34
        SpellProtection = 0x23, // 35
        SpellIncognito = 0x24, // 36
        SpellMagicReflection = 0x25, // 37
        SpellParalyze = 0x26, // 38
        SpellInvis = 0x27, // 39
        SpellPolymorph = 0x28, // 40
        SpellSummon = 0x29, // 41
        FlagPoison = 0x2A, // 42
        FlagCriminal = 0x2B, // 43
        FlagPotion = 0x2C, // 44
        FlagSpiritSpeak = 0x2D, // 45 
        FlagWool = 0x2E, // 46
        FlagDrunk = 0x2F, // 47
        FlagClientLinger = 0x30, // 48
        FlagHallucination = 0x31, // 49
        FlagPotionUsed = 0x32, // 50
        FlagStuck = 0x33, // 51
        FlagMurders = 0x34, // 52
        FlagBandage = 0x35, // 53
        FlagAuction = 0x36, // 54
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Albo1125.Common.CommonLibrary
{
    public static class FriendlyKeys
    {
        public static string GetFriendlyNames(Keys modifier, Keys key, bool format = true, char format_color = 'b')
        {
            string output = "";
            if (modifier != Keys.None)
            {
                output += GetFriendlyName(modifier, format, format_color) + " + ";
            }
            output += GetFriendlyName(key, format, format_color);
            return output;
        }

        public static string GetFriendlyName(Keys key, bool format = true, char format_color = 'b')
        {
            string key_name = key.ToString();
            bool key_found = KeysToFriendlyName.TryGetValue(key, out key_name);
            if (format)
            {
                return "~" + format_color + "~~h~" + key_name + "~h~~w~";
            }
            else
            {
                return key_name;
            }
        }

        public static string FriendlyName(this Keys key, bool format = true, char format_color = 'b')
        {
            return GetFriendlyName(key, format, format_color);
        }

        /// <summary>
        /// A lookup dictionary to take you from a Keys enumeration to a printable friendly name for the key.
        /// </summary>
        internal static Dictionary<Keys, string> KeysToFriendlyName = new Dictionary<Keys, string>
    {
        // Credit to PeterU for creating this list
        {Keys.A,"A"},
        {Keys.Add,"+"},
        {Keys.Alt,"Alt"},
        {Keys.Apps,"Apps"},
        {Keys.Attn,"Attn"},
        {Keys.B,"B"},
        {Keys.Back,"Backspace"},
        {Keys.BrowserBack,"Browser Back"},
        {Keys.BrowserFavorites,"Browser Favorites"},
        {Keys.BrowserForward,"Browser Forward"},
        {Keys.BrowserHome,"Browser Home"},
        {Keys.BrowserRefresh,"Browser Refresh"},
        {Keys.BrowserSearch,"Browser Search"},
        {Keys.BrowserStop,"Browser Stop"},
        {Keys.C,"C"},
        {Keys.Cancel,"Cancel"},
        {Keys.Capital,"Caps Lock"},
        {Keys.Clear,"Clear"},
        {Keys.Control,"Control"},
        {Keys.ControlKey,"Control"},
        {Keys.Crsel,"Crsel"},
        {Keys.D,"D"},
        {Keys.D0,"0"},
        {Keys.D1,"1"},
        {Keys.D2,"2"},
        {Keys.D3,"3"},
        {Keys.D4,"4"},
        {Keys.D5,"5"},
        {Keys.D6,"6"},
        {Keys.D7,"7"},
        {Keys.D8,"8"},
        {Keys.D9,"9"},
        {Keys.Decimal,"Numpad ."},
        {Keys.Delete,"Delete"},
        {Keys.Divide,"/"},
        {Keys.Down,"Down"},
        {Keys.E,"E"},
        {Keys.End,"End"},
        {Keys.Enter,"Enter"},
        {Keys.EraseEof,"Erase EOF"},
        {Keys.Escape,"Esc"},
        {Keys.Execute,"Execute"},
        {Keys.Exsel,"Exsel"},
        {Keys.F,"F"},
        {Keys.F1,"F1"},
        {Keys.F10,"F10"},
        {Keys.F11,"F11"},
        {Keys.F12,"F12"},
        {Keys.F13,"F13"},
        {Keys.F14,"F14"},
        {Keys.F15,"F15"},
        {Keys.F16,"F16"},
        {Keys.F17,"F17"},
        {Keys.F18,"F18"},
        {Keys.F19,"F19"},
        {Keys.F2,"F2"},
        {Keys.F20,"F20"},
        {Keys.F21,"F21"},
        {Keys.F22,"F22"},
        {Keys.F23,"F23"},
        {Keys.F24,"F24"},
        {Keys.F3,"F3"},
        {Keys.F4,"F4"},
        {Keys.F5,"F5"},
        {Keys.F6,"F6"},
        {Keys.F7,"F7"},
        {Keys.F8,"F8"},
        {Keys.F9,"F9"},
        {Keys.FinalMode,"IME Final Mode"},
        {Keys.G,"G"},
        {Keys.H,"H"},
        {Keys.HangulMode,"Hangul Mode"},
        {Keys.HanjaMode,"Hanja Mode"},
        {Keys.Help,"Help"},
        {Keys.Home,"Home"},
        {Keys.I,"I"},
        {Keys.IMEAccept,"IME Accept"},
        {Keys.IMEConvert,"IME Convert"},
        {Keys.IMEModeChange,"IME Mode Change"},
        {Keys.IMENonconvert,"IME Non-convert"},
        {Keys.Insert,"Insert"},
        {Keys.J,"J"},
        {Keys.JunjaMode,"Junja Mode"},
        {Keys.K,"K"},
        {Keys.KeyCode,"Key Code"},
        {Keys.L,"L"},
        {Keys.LaunchApplication1,"Start Application 1"},
        {Keys.LaunchApplication2,"Start Application 2"},
        {Keys.LaunchMail,"Mail"},
        {Keys.LButton,"Left click"},
        {Keys.LControlKey,"Left Ctrl"},
        {Keys.Left,"Left"},
        {Keys.LineFeed,"Line Feed"},
        {Keys.LMenu,"Left Alt"},
        {Keys.LShiftKey,"Left Shift"},
        {Keys.LWin,"Left Windows key"},
        {Keys.M,"M"},
        {Keys.MButton,"Middle click"},
        {Keys.MediaNextTrack,"Next Track"},
        {Keys.MediaPlayPause,"Play Pause"},
        {Keys.MediaPreviousTrack,"Previous Track"},
        {Keys.MediaStop,"Stop"},
        {Keys.Menu,"Alt"},
        {Keys.Modifiers,"Modifiers"},
        {Keys.Multiply,"*"},
        {Keys.N,"N"},
        {Keys.NoName,"NoName"},
        {Keys.None,"[none]"},
        {Keys.NumLock,"Num Lock"},
        {Keys.NumPad0,"Numpad 0"},
        {Keys.NumPad1,"Numpad 1"},
        {Keys.NumPad2,"Numpad 2"},
        {Keys.NumPad3,"Numpad 3"},
        {Keys.NumPad4,"Numpad 4"},
        {Keys.NumPad5,"Numpad 5"},
        {Keys.NumPad6,"Numpad 6"},
        {Keys.NumPad7,"Numpad 7"},
        {Keys.NumPad8,"Numpad 8"},
        {Keys.NumPad9,"Numpad 9"},
        {Keys.O,"O"},
        {Keys.Oem1,"Oem 1"},
        {Keys.Oem102,"Oem 102"},
        {Keys.Oem2,"/"},
        {Keys.Oem3,"'"},
        {Keys.Oem4,"Oem 4"},
        {Keys.Oem5,"\\"},
        {Keys.Oem6,"]"},
        {Keys.Oem7,"Oem 7"},
        {Keys.Oem8,"Oem 8"},
        {Keys.OemClear,"Clear"},
        {Keys.Oemcomma,","},
        {Keys.OemMinus,"-"},
        {Keys.OemPeriod,"."},
        {Keys.Oemplus,"+"},
        {Keys.P,"P"},
        {Keys.Pa1,"Pa1"},
        {Keys.Packet,"Packet"},
        {Keys.PageDown,"Page Down"},
        {Keys.PageUp,"Page Up"},
        {Keys.Pause,"Pause"},
        {Keys.Play,"Play"},
        {Keys.Print,"Print"},
        {Keys.PrintScreen,"Print Screen"},
        {Keys.ProcessKey,"Process Key"},
        {Keys.Q,"Q"},
        {Keys.R,"R"},
        {Keys.RButton,"Right click"},
        {Keys.RControlKey,"Right Ctrl"},
        {Keys.Right,"Right"},
        {Keys.RMenu,"Right Alt"},
        {Keys.RShiftKey,"Right Shift"},
        {Keys.RWin,"Right Windows key"},
        {Keys.S,"S"},
        {Keys.Scroll,"Scroll Lock"},
        {Keys.Select,"Select"},
        {Keys.SelectMedia,"Select Media"},
        {Keys.Separator,"Separator"},
        {Keys.Shift,"Shift"},
        {Keys.ShiftKey,"Shift"},
        {Keys.Sleep,"Sleep"},
        {Keys.Space,"Space"},
        {Keys.Subtract,"-"},
        {Keys.T,"T"},
        {Keys.Tab,"Tab"},
        {Keys.U,"U"},
        {Keys.Up,"Up"},
        {Keys.V,"V"},
        {Keys.VolumeDown,"Volume Down"},
        {Keys.VolumeMute,"Volume Mute"},
        {Keys.VolumeUp,"Volume Up"},
        {Keys.W,"W"},
        {Keys.X,"X"},
        {Keys.XButton1,"Mouse X1"},
        {Keys.XButton2,"Mouse X2"},
        {Keys.Y,"Y"},
        {Keys.Z,"Z"},
        {Keys.Zoom,"Zoom"},
    };
    }
}

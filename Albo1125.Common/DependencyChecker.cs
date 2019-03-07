using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Rage;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using System.Threading;
using Albo1125.Common.CommonLibrary;
using Rage.Native;
using System.Net;
using Microsoft.Win32;

namespace Albo1125.Common
{

    public static class DependencyChecker
    {
        private static List<string> RegisteredPluginsForDependencyChecks = new List<string>();
        public static void RegisterPluginForDependencyChecks(string CallingPlugin)
        {
            if (!RegisteredPluginsForDependencyChecks.Contains(CallingPlugin))
            {
                RegisteredPluginsForDependencyChecks.Add(CallingPlugin);
            }
        }

        private static TupleList<string, string, string> Plugins_URL_Errors = new TupleList<string, string, string>();
        private static string scid;

        public static bool DependencyCheckMain(string CallingPlugin, Version Albo1125CommonVer, float MinimumRPHVersion, Version MadeForGTAVersion = null, Version MadeForLSPDFRVersion = null, Version RAGENativeUIVersion = null, string[] AudioFilesToCheckFor = null, string[] OtherRequiredFilesToCheckFor = null)
        {
            return DependencyCheckMain(CallingPlugin, Albo1125CommonVer, MinimumRPHVersion, "https://youtu.be/af434m72rIo?list=PLEKypmos74W8PMP4k6xmVxpTKdebvJpFb", MadeForGTAVersion, MadeForLSPDFRVersion, RAGENativeUIVersion, AudioFilesToCheckFor, OtherRequiredFilesToCheckFor);
        }

        public static bool DependencyCheckMain(string CallingPlugin, Version Albo1125CommonVer, float MinimumRPHVersion, string installationVideoURL, Version MadeForGTAVersion = null, Version MadeForLSPDFRVersion = null, Version RAGENativeUIVersion = null, string[] AudioFilesToCheckFor = null, string[] OtherRequiredFilesToCheckFor = null)
        {
            bool CheckPassedSuccessfully = true;
            Game.LogTrivial("Albo1125.Common.dll " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + " starting standard dependency check for " + CallingPlugin);
            if (scid == null)
            {
                scid = NativeFunction.Natives.GET_PLAYER_NAME<string>(Game.LocalPlayer);
                Game.LogTrivial("SCID:/" + scid + "/");
            }
            if (File.Exists("Albo1125.Common.dll"))
            {
                Version InstalledCommonVer = new Version(FileVersionInfo.GetVersionInfo("Albo1125.Common.dll").ProductVersion);
                if (InstalledCommonVer.CompareTo(Albo1125CommonVer) >= 0)
                {
                    if (MadeForGTAVersion != null)
                    {
                        Game.LogTrivial("GAME VERSION: " + Game.ProductVersion.ToString());
                        int compare = Game.ProductVersion.CompareTo(MadeForGTAVersion);
                        if (compare > 0)
                        {
                            Game.LogTrivial(CallingPlugin + " compatibility warning: The current game version is newer than " + MadeForGTAVersion.ToString() + " and may or may not be incompatible due to RPH changes. Use at own risk.");
                        }
                    }

                    if (MadeForLSPDFRVersion != null)
                    {
                        if (File.Exists("Plugins/LSPD First Response.dll"))
                        {
                            Version InstalledLSPDFRVer = new Version(FileVersionInfo.GetVersionInfo("Plugins/LSPD First Response.dll").ProductVersion);
                            if (InstalledLSPDFRVer.CompareTo(MadeForLSPDFRVersion) != 0)
                            {
                                Game.LogTrivial(CallingPlugin + " compatibility warning: Different LSPD First Response.dll version detected, use at your own risk! This mod was made for LSPDFR " + MadeForLSPDFRVersion.ToString());
                                Game.DisplayNotification(CallingPlugin + " compatibility warning: Different LSPD First Response.dll version detected, use at your own risk! This mod was made for LSPDFR " + MadeForLSPDFRVersion.ToString());
                                //Plugins_URL_Errors.Add(CallingPlugin, installationVideoURL, "Detected invalid LSPD First Response.dll version. To run this mod, you need LSPDFR " + MadeForLSPDFRVersion.ToString());
                                //CheckPassedSuccessfully = false;
                            }
                        }
                        else
                        {
                            Game.LogTrivial("LSPD First Response.dll not installed.");
                            Plugins_URL_Errors.Add(CallingPlugin, installationVideoURL, "Couldn't detect required LSPD First Response.dll. You must install it.");
                            CheckPassedSuccessfully = false;
                        }

                    }
                    if (RAGENativeUIVersion != null)
                    {
                        if (File.Exists("RAGENativeUI.dll"))
                        {
                            Version InstalledNativeUIVer = new Version(FileVersionInfo.GetVersionInfo("RAGENativeUI.dll").ProductVersion);
                            if (InstalledNativeUIVer.CompareTo(RAGENativeUIVersion) < 0)
                            {
                                Game.LogTrivial("RAGENativeUI.dll out of date. Required version of RAGENativeUI to run this mod: " + RAGENativeUIVersion);
                                Plugins_URL_Errors.Add(CallingPlugin, installationVideoURL, "RAGENativeUI.dll out of date. Required version of RAGENativeUI to run this mod: " + RAGENativeUIVersion);
                                CheckPassedSuccessfully = false;
                            }
                        }
                        else
                        {
                            Game.LogTrivial("RAGENativeUI.dll is not installed. You must install it to run this mod.");
                            Plugins_URL_Errors.Add(CallingPlugin, installationVideoURL, "RAGENativeUI.dll is not installed. You must install it to run this mod.");
                            CheckPassedSuccessfully = false;
                        }
                    }
                    if (AudioFilesToCheckFor != null)
                    {
                        foreach (string s in AudioFilesToCheckFor)
                        {
                            if (!File.Exists(s))
                            {
                                Game.LogTrivial("Couldn't find the required audio file at " + s);
                                Plugins_URL_Errors.Add(CallingPlugin, installationVideoURL, "You are missing required (new) audio files. Path is: " + s);

                                CheckPassedSuccessfully = false;
                            }
                        }
                    }
                    if (OtherRequiredFilesToCheckFor != null)
                    {
                        foreach (string s in OtherRequiredFilesToCheckFor)
                        {
                            if (!File.Exists(s))
                            {
                                Game.LogTrivial("Couldn't find the required file at " + s);
                                Plugins_URL_Errors.Add(CallingPlugin, installationVideoURL, "You are missing required (new) files. Path is: " + s);
                                CheckPassedSuccessfully = false;
                            }
                        }

                    }
                    if (!CheckForRageVersion(MinimumRPHVersion))
                    {
                        CheckPassedSuccessfully = false;
                        Plugins_URL_Errors.Add(CallingPlugin, installationVideoURL, "RAGEPluginHook is out of date. This mod requires RPH " + MinimumRPHVersion);
                    }

                }
                else
                {
                    Game.LogTrivial("Albo1125.Common.dll is out of date. This mod requires Albo1125.Common " + Albo1125CommonVer);
                    Plugins_URL_Errors.Add(CallingPlugin, installationVideoURL, "Albo1125.Common.dll is out of date. This mod requires Albo1125.Common " + Albo1125CommonVer);
                    CheckPassedSuccessfully = false;
                }
            }
            else
            {
                CheckPassedSuccessfully = false;
                Game.LogTrivial("Albo1125.Common.dll is not installed. This mod requires Albo1125.Common to be installed. You've successfully run this without actually having it on your PC...spooky.");
                Plugins_URL_Errors.Add(CallingPlugin, installationVideoURL, "Albo1125.Common.dll is not installed. This mod requires Albo1125.Common to be installed. You've successfully run this without actually having it on your PC...spooky.");
            }
            if (RegisteredPluginsForDependencyChecks.Contains(CallingPlugin)) { RegisteredPluginsForDependencyChecks.Remove(CallingPlugin); }
            if (RegisteredPluginsForDependencyChecks.Count == 0 && Plugins_URL_Errors.Count > 0) { DisplayDependencyErrors(); }

            Game.LogTrivial("Dependency check for " + CallingPlugin + " successful: " + CheckPassedSuccessfully);
            return CheckPassedSuccessfully;

        }

        public static bool CheckIfThereAreNoConflictingFiles(string CallingMod, string[] FilesToCheckFor)
        {
            bool noconflicts = true;
            foreach (string file in FilesToCheckFor)
            {
                if (File.Exists(file))
                {
                    ExtensionMethods.DisplayPopupTextBoxWithConfirmation(CallingMod + " detected a conflicting file",
                        "The following file can cause conflicts with " + CallingMod + ": " + file + ". You are advised to remove it when running " + CallingMod + ".", true);
                    noconflicts = false;
                }
            }
            return noconflicts;
        }
        public static bool CheckIfFileExists(string file, Version MinVer = null)
        {
            if (File.Exists(file))
            {
                Version InstalledVer = new Version(FileVersionInfo.GetVersionInfo(file).ProductVersion);
                if (MinVer == null || InstalledVer.CompareTo(MinVer) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static int Index = -1;
        public static void DisplayDependencyErrors()
        {
            string ModsWithErrors = "";

            foreach (string s in Plugins_URL_Errors.Select(x => x.Item1).Distinct().ToList())
            {
                ModsWithErrors += s + ",";
            }
            Popup pop = new Popup("Albo1125.Common detected errors", "Errors were detected in your installation of the following of my modifications, so they will not load: " + ModsWithErrors, new List<string>() { "Continue" }, false, false, NextDependencyErrorCallback);
            pop.Display();
        }

        private static void NextDependencyErrorCallback(Popup p)
        {
            if (p.IndexOfGivenAnswer == 0)
            {
                Game.LogTrivial("Continue pressed");
                Index++;
                if (Plugins_URL_Errors.Count > Index)
                {
                    Popup pop = new Popup("Error " + (Index + 1) + ": " + Plugins_URL_Errors[Index].Item1, Plugins_URL_Errors[Index].Item3, new List<string>() { "Continue", "Open installation video tutorial for this plugin" },
                        false, false, NextDependencyErrorCallback);
                    pop.Display();
                }
                else
                {
                    Popup pop = new Popup("Albo1125.Common detected errors", "To fix these installation errors, you should read the appropriate ReadMe or documentation files, watch the installation video tutorial or use my Troubleshooter (link in video description).",
                        new List<string>() { "Continue", "Open installation/troubleshooting video tutorial", "Exit" }, false, false, NextDependencyErrorCallback);
                    pop.Display();
                }
            }
            else if (p.IndexOfGivenAnswer == 1)
            {
                Game.LogTrivial("GoToVideo pressed.");
                if (Plugins_URL_Errors.Count > Index)
                {
                    Process.Start(Plugins_URL_Errors[Index].Item2);
                }
                else
                {
                    Process.Start("https://youtu.be/af434m72rIo?list=PLEKypmos74W8PMP4k6xmVxpTKdebvJpFb"); 
                }
                p.Display();
            }
            else if (p.IndexOfGivenAnswer == 2)
            {
                Game.LogTrivial("ExitButton pressed.");
            }
            
        }

        private static bool CorrectRPHVersion;

        private static bool CheckForRageVersion(float MinimumVersion)
        {
            string RPHFile = "RAGEPluginHook.exe";
            string[] files = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "*.exe", SearchOption.TopDirectoryOnly);
            foreach (string file in files)
            {
                if (Path.GetFileName(file).ToLower() == "ragepluginhook.exe")
                {
                    RPHFile = file;
                    break;
                }
            }

            var versionInfo = FileVersionInfo.GetVersionInfo(RPHFile);
            float Rageversion;
            try
            {
                Rageversion = float.Parse(versionInfo.ProductVersion.Substring(0, 4), CultureInfo.InvariantCulture);
                Game.LogTrivial("Albo1125.Common detected RAGEPluginHook version: " + Rageversion.ToString());

                //If user's RPH version is older than the minimum
                if (Rageversion < MinimumVersion)
                {
                    CorrectRPHVersion = false;
                }
                //If user's RPH version is (above) the specified minimum
                else
                {
                    CorrectRPHVersion = true;
                }
            }
            catch (Exception e)
            {
                //If for whatever reason the version couldn't be found.
                Game.LogTrivial(e.ToString());
                Game.LogTrivial("Unable to detect your Rage installation.");
                if (File.Exists("RAGEPluginHook.exe"))
                {
                    Game.LogTrivial("RAGEPluginHook.exe exists");
                }
                else { Game.LogTrivial("RAGEPluginHook doesn't exist."); }
                Game.LogTrivial("Rage Version: " + versionInfo.ProductVersion.ToString());
                Game.DisplayNotification("Albo1125.Common was unable to detect RPH installation. Please send me your logfile.");
                CorrectRPHVersion = false;

            }

            return CorrectRPHVersion;
        }
    }
}

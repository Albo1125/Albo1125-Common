using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rage;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using System.Net;
using System.Drawing;
using System.Threading;
using Albo1125.Common.CommonLibrary;
using System.Xml.Linq;
using static Albo1125.Common.UpdateEntry;

namespace Albo1125.Common
{
    public class UpdateChecker
    {

        private static int Index = -1;
        public static void DisplayUpdates()
        {
            string ModsWithErrors = "";
            foreach (string s in PluginsDownloadLink.Select(x => x.Item1).Distinct().ToList())
            {
                ModsWithErrors += s + ",";
            }
            Popup pop = new Popup("Albo1125.Common Update Check", "Updates are available for the following modifications: " + ModsWithErrors, new List<string>() { "Continue" }, false, false, NextUpdateCallback);
            pop.Display();
        }

        private static void NextUpdateCallback(Popup p)
        {
            if (p.IndexOfGivenAnswer == 0)
            {
                Game.LogTrivial("Continue pressed");
                Index++;
                if (PluginsDownloadLink.Count > Index)
                {
                    Popup pop = new Popup("Albo1125.Common Update Check", "Update " + (Index + 1) + ": " + PluginsDownloadLink[Index].Item1, new List<string>() { "Continue", "Go to download page" },
                        false, false, NextUpdateCallback);
                    pop.Display();
                }
                else
                {
                    Popup pop = new Popup("Albo1125.Common Update Check", "Please install updates to maintain stability and don't request support for old versions.",
                        new List<string>() { "-", "Open installation/troubleshooting video tutorial", "Continue to game.", "Delay next update check by a week.", "Delay next update check by a month.",
                            "Fully disable update checks (not recommended)." }, false, false, NextUpdateCallback);
                    pop.Display();
                }
            }
            else if (p.IndexOfGivenAnswer == 1)
            {
                Game.LogTrivial("GoToDownload pressed.");
                if (PluginsDownloadLink.Count > Index && Index >= 0)
                {
                    System.Diagnostics.Process.Start(PluginsDownloadLink[Index].Item2);
                }
                else
                {
                    System.Diagnostics.Process.Start("https://youtu.be/af434m72rIo?list=PLEKypmos74W8PMP4k6xmVxpTKdebvJpFb");
                }
                p.Display();
            }
            else if (p.IndexOfGivenAnswer == 2)
            {
                Game.LogTrivial("ExitButton pressed.");
            }
            else if (p.IndexOfGivenAnswer == 3)
            {
                Game.LogTrivial("Delay by week pressed");
                DateTime NextUpdateCheckDT = DateTime.Now.AddDays(6);
                XDocument CommonVariablesDoc = XDocument.Load("Albo1125.Common/CommonVariables.xml");
                if (CommonVariablesDoc.Root.Element("NextUpdateCheckDT") == null) { CommonVariablesDoc.Root.Add(new XElement("NextUpdateCheckDT")); }
                CommonVariablesDoc.Root.Element("NextUpdateCheckDT").Value = NextUpdateCheckDT.ToBinary().ToString();
                CommonVariablesDoc.Save("Albo1125.Common/CommonVariables.xml");
                CommonVariablesDoc = null;
            }
            else if (p.IndexOfGivenAnswer == 4)
            {
                Game.LogTrivial("Delay by month pressed");
                DateTime NextUpdateCheckDT = DateTime.Now.AddMonths(1);
                XDocument CommonVariablesDoc = XDocument.Load("Albo1125.Common/CommonVariables.xml");
                if (CommonVariablesDoc.Root.Element("NextUpdateCheckDT") == null) { CommonVariablesDoc.Root.Add(new XElement("NextUpdateCheckDT")); }
                CommonVariablesDoc.Root.Element("NextUpdateCheckDT").Value = NextUpdateCheckDT.ToBinary().ToString();
                CommonVariablesDoc.Save("Albo1125.Common/CommonVariables.xml");
                CommonVariablesDoc = null;
            }
            else if (p.IndexOfGivenAnswer == 5)
            {
                Game.LogTrivial("Disable Update Checks pressed.");
                XDocument CommonVariablesDoc = XDocument.Load("Albo1125.Common/CommonVariables.xml");
                if (CommonVariablesDoc.Root.Element("NextUpdateCheckDT") == null) { CommonVariablesDoc.Root.Add(new XElement("NextUpdateCheckDT")); }
                CommonVariablesDoc.Root.Element("NextUpdateCheckDT").Value = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                CommonVariablesDoc.Save("Albo1125.Common/CommonVariables.xml");
                CommonVariablesDoc = null;
                Popup pop = new Popup("Albo1125.Common Update Check", "Update checking has been disabled for this version of Albo1125.Common." +
                    "To re-enable it, delete the Albo1125.Common folder from your Grand Theft Auto V folder. Please do not request support for old versions.", false, true);
                pop.Display();
            }
        }
     
        private static TupleList<string, string> PluginsDownloadLink = new TupleList<string, string>();


        private static void CheckForModificationUpdates(string ModificationName, Version curVersion, string VersionCheckLink, string DownloadLink)
        {
            if (LSPDFRUpdateAPIRunning)
            {
                new UpdateChecker(ModificationName, curVersion, VersionCheckLink, DownloadLink);
                if (!Albo1125CommonCheckedForUpdates)
                {
                    Albo1125CommonCheckedForUpdates = true;
                    new UpdateChecker("Albo1125.Common", Assembly.GetExecutingAssembly().GetName().Version, "10294", "https://www.lcpdfr.com/files/file/10294-albo1125common/");
                }
            }
            else
            {
                Game.LogTrivial("LSPDFR Update API down. Not starting checks.");
            }
        }
        private Version NewVersion = new Version();
        private static bool LSPDFRUpdateAPIRunning = true;
        private UpdateChecker(string ModificationName, Version curVersion, string FileID, string DownloadLink)
        {

            try
            {
                Game.LogTrivial("Albo1125.Common " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ", developed by Albo1125. Checking for " + ModificationName + " updates.");

                Thread FetchVersionThread = new Thread(() =>
                {

                    using (WebClient client = new WebClient())
                    {
                        try
                        {
                            string s = client.DownloadString("http://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=" + FileID + "&textOnly=1");

                            NewVersion = new Version(s);
                        }
                        catch (Exception e) { LSPDFRUpdateAPIRunning = false; Game.LogTrivial("LSPDFR Update API down. Aborting checks."); }
                    }
                });
                FetchVersionThread.Start();
                while (FetchVersionThread.ThreadState != System.Threading.ThreadState.Stopped)
                {
                    GameFiber.Yield();
                }

                // compare the versions  
                if (curVersion.CompareTo(NewVersion) < 0)
                {
                    // ask the user if he would like  
                    // to download the new version  
                    PluginsDownloadLink.Add(ModificationName, DownloadLink);
                    Game.LogTrivial("Update available for " + ModificationName);
                }

            }
            catch (System.Threading.ThreadAbortException e)
            {

            }
            catch (Exception e)
            {
                Game.LogTrivial("Error while checking " + ModificationName + " for updates.");
            }
        }


        private static bool Albo1125CommonCheckedForUpdates = false;

        public static void InitialiseUpdateCheckingProcess()
        {
            Game.LogTrivial("Albo1125.Common " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ", developed by Albo1125. Starting update checks.");
            Directory.CreateDirectory("Albo1125.Common/UpdateInfo");
            if (!File.Exists("Albo1125.Common/CommonVariables.xml"))
            {
                new XDocument(
                        new XElement("CommonVariables")
                    )
                    .Save("Albo1125.Common/CommonVariables.xml");
            }
            try
            {
                XDocument CommonVariablesDoc = XDocument.Load("Albo1125.Common/CommonVariables.xml");
                if (CommonVariablesDoc.Root.Element("NextUpdateCheckDT") == null) { CommonVariablesDoc.Root.Add(new XElement("NextUpdateCheckDT")); }
                if (!string.IsNullOrWhiteSpace((string)CommonVariablesDoc.Root.Element("NextUpdateCheckDT")))
                {

                    try
                    {
                        if (CommonVariablesDoc.Root.Element("NextUpdateCheckDT").Value == Assembly.GetExecutingAssembly().GetName().Version.ToString())
                        {
                            Game.LogTrivial("Albo1125.Common update checking has been disabled. Skipping checks.");
                            Game.LogTrivial("Albo1125.Common note: please do not request support for old versions.");
                            return;
                        }
                        DateTime UpdateCheckDT = DateTime.FromBinary(long.Parse(CommonVariablesDoc.Root.Element("NextUpdateCheckDT").Value));
                        if (DateTime.Now < UpdateCheckDT)
                        {

                            Game.LogTrivial("Albo1125.Common " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ", developed by Albo1125. Not checking for updates until " + UpdateCheckDT.ToString());
                            return;
                        }
                    }
                    catch (Exception e) { Game.LogTrivial(e.ToString()); Game.LogTrivial("Albo1125.Common handled exception. #1"); }

                }

                
                DateTime NextUpdateCheckDT = DateTime.Now.AddDays(1);
                if (CommonVariablesDoc.Root.Element("NextUpdateCheckDT") == null) { CommonVariablesDoc.Root.Add(new XElement("NextUpdateCheckDT")); }
                CommonVariablesDoc.Root.Element("NextUpdateCheckDT").Value = NextUpdateCheckDT.ToBinary().ToString();
                CommonVariablesDoc.Save("Albo1125.Common/CommonVariables.xml");
                CommonVariablesDoc = null;
                GameFiber.StartNew(delegate
                {


                    GetUpdateNodes();
                    foreach (UpdateEntry entry in AllUpdateEntries.ToArray())
                    {
                        CheckForModificationUpdates(entry.Name, new Version(FileVersionInfo.GetVersionInfo(entry.Path).FileVersion), entry.FileID, entry.DownloadLink);
                    }
                    if (PluginsDownloadLink.Count > 0) { DisplayUpdates(); }
                    Game.LogTrivial("Albo1125.Common " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ", developed by Albo1125. Update checks complete.");
                });
            }
            catch (System.Xml.XmlException e)
            {
                Game.LogTrivial(e.ToString());
                Game.DisplayNotification("Error while processing XML files. To fix this, please delete the following folder and its contents: Grand Theft Auto V/Albo1125.Common");
                Albo1125.Common.CommonLibrary.ExtensionMethods.DisplayPopupTextBoxWithConfirmation("Albo1125.Common", "Error while processing XML files. To fix this, please delete the following folder and its contents: Grand Theft Auto V/Albo1125.Common", false);
                throw e;
            }

            



        }

        public static void VerifyXmlNodeExists(string Name, string FileID, string DownloadLink, string Path)
        {        
            Game.LogTrivial("Albo1125.Common verifying update entry for " + Name);
            XDocument xdoc =

                    new XDocument(
                            new XElement("UpdateEntry")
                        );
                        
            try
            {
                Directory.CreateDirectory("Albo1125.Common/UpdateInfo");

                xdoc.Root.Add(new XElement("Name"));
                xdoc.Root.Element("Name").Value = XmlConvert.EncodeName(Name);

                xdoc.Root.Add(new XElement("FileID"));
                xdoc.Root.Element("FileID").Value = FileID;

                xdoc.Root.Add(new XElement("DownloadLink"));
                xdoc.Root.Element("DownloadLink").Value = XmlConvert.EncodeName(DownloadLink);

                xdoc.Root.Add(new XElement("Path"));

                xdoc.Root.Element("Path").Value = XmlConvert.EncodeName(Path);

                xdoc.Save("Albo1125.Common/UpdateInfo/" + Name + ".xml");
                
            }
            catch (System.Xml.XmlException e)
            {
                Game.LogTrivial(e.ToString());
                Game.DisplayNotification("Error while processing XML files. To fix this, please delete the following folder and its contents: Grand Theft Auto V/Albo1125.Common");
                ExtensionMethods.DisplayPopupTextBoxWithConfirmation("Albo1125.Common", "Error while processing XML files. To fix this, please delete the following folder and its contents: Grand Theft Auto V/Albo1125.Common", false);
                throw e;
            }

        }

        private static void GetUpdateNodes()
        {
            IEnumerable<string> XMLUpdateFiles = Directory.EnumerateFiles("Albo1125.Common/UpdateInfo", "*.xml");
            foreach (string xmlnode in XMLUpdateFiles)
            {
                XDocument xdoc = XDocument.Load(xmlnode);
                if (IsUpdateNodeValid(xdoc.Root))
                {
                    if (File.Exists(XmlConvert.DecodeName(xdoc.Root.Element("Path").Value.Trim())))
                    {
                        AllUpdateEntries.Add(new UpdateEntry()
                        {
                            Name = XmlConvert.DecodeName(xdoc.Root.Element("Name").Value.Trim()),
                            FileID = xdoc.Root.Element("FileID").Value.Trim(),
                            DownloadLink = XmlConvert.DecodeName(xdoc.Root.Element("DownloadLink").Value.Trim()),
                            Path = XmlConvert.DecodeName(xdoc.Root.Element("Path").Value.Trim()),

                        });
                    }
                }
                xdoc = null;


            }
            

        }

        private static bool IsUpdateNodeValid(XElement root)
        {
            return new string[] { "Name", "FileID", "DownloadLink", "Path" }.All(s => root.Elements().Select(x => x.Name.ToString()).Contains(s)) && root.Elements().All(s => !string.IsNullOrWhiteSpace(s.Value));
        }
    }

    internal class UpdateEntry
    {
        public static List<UpdateEntry> AllUpdateEntries = new List<UpdateEntry>();

        public string Name;
        public string FileID;
        public string DownloadLink;
        public string Path;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Rage;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Net;
using System.Threading;
using Belgs.Common.CommonLibrary;
using System.Xml.Linq;

namespace Belgs.Common
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
            Popup pop = new Popup("Belgs.Common Update Check", "Updates are available for the following modifications: " + ModsWithErrors, new List<string>() { "Continue" }, false, false, NextUpdateCallback);
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
                    Popup pop = new Popup("Belgs.Common Update Check", "Update " + (Index + 1) + ": " + PluginsDownloadLink[Index].Item1, new List<string>() { "Continue", "Go to download page" },
                        false, false, NextUpdateCallback);
                    pop.Display();
                }
                else
                {
                    Popup pop = new Popup("Belgs.Common Update Check", "Please install updates to maintain stability and don't request support for old versions.",
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
                XDocument CommonVariablesDoc = XDocument.Load("Belgs.Common/CommonVariables.xml");
                if (CommonVariablesDoc.Root.Element("NextUpdateCheckDT") == null) { CommonVariablesDoc.Root.Add(new XElement("NextUpdateCheckDT")); }
                CommonVariablesDoc.Root.Element("NextUpdateCheckDT").Value = NextUpdateCheckDT.ToBinary().ToString();
                CommonVariablesDoc.Save("Belgs.Common/CommonVariables.xml");
                CommonVariablesDoc = null;
            }
            else if (p.IndexOfGivenAnswer == 4)
            {
                Game.LogTrivial("Delay by month pressed");
                DateTime NextUpdateCheckDT = DateTime.Now.AddMonths(1);
                XDocument CommonVariablesDoc = XDocument.Load("Belgs.Common/CommonVariables.xml");
                if (CommonVariablesDoc.Root.Element("NextUpdateCheckDT") == null) { CommonVariablesDoc.Root.Add(new XElement("NextUpdateCheckDT")); }
                CommonVariablesDoc.Root.Element("NextUpdateCheckDT").Value = NextUpdateCheckDT.ToBinary().ToString();
                CommonVariablesDoc.Save("Belgs.Common/CommonVariables.xml");
                CommonVariablesDoc = null;
            }
            else if (p.IndexOfGivenAnswer == 5)
            {
                Game.LogTrivial("Disable Update Checks pressed.");
                XDocument CommonVariablesDoc = XDocument.Load("Belgs.Common/CommonVariables.xml");
                if (CommonVariablesDoc.Root.Element("NextUpdateCheckDT") == null) { CommonVariablesDoc.Root.Add(new XElement("NextUpdateCheckDT")); }
                CommonVariablesDoc.Root.Element("NextUpdateCheckDT").Value = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                CommonVariablesDoc.Save("Belgs.Common/CommonVariables.xml");
                CommonVariablesDoc = null;
                Popup pop = new Popup("Belgs.Common Update Check", "Update checking has been disabled for this version of Belgs.Common." +
                    "To re-enable it, delete the Belgs.Common folder from your Grand Theft Auto V folder. Please do not request support for old versions.", false, true);
                pop.Display();
            }
        }
     
        private static TupleList<string, string> PluginsDownloadLink = new TupleList<string, string>();


        private static void CheckForModificationUpdates(string ModificationName, Version curVersion, string VersionCheckLink, string DownloadLink)
        {
            if (LSPDFRUpdateAPIRunning)
            {
                new UpdateChecker(ModificationName, curVersion, VersionCheckLink, DownloadLink);
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
                Game.LogTrivial("Belgs.Common " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ". Checking for " + ModificationName + " updates.");

                Thread FetchVersionThread = new Thread(() =>
                {

                    using (WebClient client = new WebClient())
                    {
                        try
                        {
                            string s = client.DownloadString("https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=" + FileID + "&textOnly=1");

                            NewVersion = new Version(s);
                        }
                        catch (Exception) { LSPDFRUpdateAPIRunning = false; Game.LogTrivial("LSPDFR Update API down. Aborting checks."); }
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
            catch (ThreadAbortException)
            {

            }
            catch (Exception)
            {
                Game.LogTrivial("Error while checking " + ModificationName + " for updates.");
            }
        }


        public static void InitialiseUpdateCheckingProcess()
        {
            Game.LogTrivial("Belgs.Common " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ". Starting update checks.");
            Directory.CreateDirectory("Belgs.Common/UpdateInfo");
            if (!File.Exists("Belgs.Common/CommonVariables.xml"))
            {
                new XDocument(
                        new XElement("CommonVariables")
                    )
                    .Save("Belgs.Common/CommonVariables.xml");
            }
            try
            {
                XDocument CommonVariablesDoc = XDocument.Load("Belgs.Common/CommonVariables.xml");
                if (CommonVariablesDoc.Root.Element("NextUpdateCheckDT") == null) { CommonVariablesDoc.Root.Add(new XElement("NextUpdateCheckDT")); }
                if (!string.IsNullOrWhiteSpace((string)CommonVariablesDoc.Root.Element("NextUpdateCheckDT")))
                {

                    try
                    {
                        if (CommonVariablesDoc.Root.Element("NextUpdateCheckDT").Value == Assembly.GetExecutingAssembly().GetName().Version.ToString())
                        {
                            Game.LogTrivial("Belgs.Common update checking has been disabled. Skipping checks.");
                            Game.LogTrivial("Belgs.Common note: please do not request support for old versions.");
                            return;
                        }
                        DateTime UpdateCheckDT = DateTime.FromBinary(long.Parse(CommonVariablesDoc.Root.Element("NextUpdateCheckDT").Value));
                        if (DateTime.Now < UpdateCheckDT)
                        {

                            Game.LogTrivial("Belgs.Common " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ". Not checking for updates until " + UpdateCheckDT.ToString());
                            return;
                        }
                    }
                    catch (Exception e) { Game.LogTrivial(e.ToString()); Game.LogTrivial("Belgs.Common handled exception. #1"); }

                }

                
                DateTime NextUpdateCheckDT = DateTime.Now.AddDays(1);
                if (CommonVariablesDoc.Root.Element("NextUpdateCheckDT") == null) { CommonVariablesDoc.Root.Add(new XElement("NextUpdateCheckDT")); }
                CommonVariablesDoc.Root.Element("NextUpdateCheckDT").Value = NextUpdateCheckDT.ToBinary().ToString();
                CommonVariablesDoc.Save("Belgs.Common/CommonVariables.xml");
                CommonVariablesDoc = null;
                GameFiber.StartNew(delegate
                {


                    GetUpdateNodes();
                    foreach (UpdateEntry entry in UpdateEntry.AllUpdateEntries.ToArray())
                    {
                        CheckForModificationUpdates(entry.Name, new Version(FileVersionInfo.GetVersionInfo(entry.Path).FileVersion), entry.FileID, entry.DownloadLink);
                    }
                    if (PluginsDownloadLink.Count > 0) { DisplayUpdates(); }
                    Game.LogTrivial("Belgs.Common " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ". Update checks complete.");
                });
            }
            catch (System.Xml.XmlException e)
            {
                Game.LogTrivial(e.ToString());
                Game.DisplayNotification("Error while processing XML files. To fix this, please delete the following folder and its contents: Grand Theft Auto V/Belgs.Common");
                Belgs.Common.CommonLibrary.ExtensionMethods.DisplayPopupTextBoxWithConfirmation("Belgs.Common", "Error while processing XML files. To fix this, please delete the following folder and its contents: Grand Theft Auto V/Belgs.Common", false);
                throw;
            }

            



        }

        public static void VerifyXmlNodeExists(string Name, string FileID, string DownloadLink, string Path)
        {        
            Game.LogTrivial("Belgs.Common verifying update entry for " + Name);
            XDocument xdoc =

                    new XDocument(
                            new XElement("UpdateEntry")
                        );
                        
            try
            {
                Directory.CreateDirectory("Belgs.Common/UpdateInfo");

                xdoc.Root.Add(new XElement("Name"));
                xdoc.Root.Element("Name").Value = XmlConvert.EncodeName(Name);

                xdoc.Root.Add(new XElement("FileID"));
                xdoc.Root.Element("FileID").Value = FileID;

                xdoc.Root.Add(new XElement("DownloadLink"));
                xdoc.Root.Element("DownloadLink").Value = XmlConvert.EncodeName(DownloadLink);

                xdoc.Root.Add(new XElement("Path"));

                xdoc.Root.Element("Path").Value = XmlConvert.EncodeName(Path);

                xdoc.Save("Belgs.Common/UpdateInfo/" + Name + ".xml");
                
            }
            catch (System.Xml.XmlException e)
            {
                Game.LogTrivial(e.ToString());
                Game.DisplayNotification("Error while processing XML files. To fix this, please delete the following folder and its contents: Grand Theft Auto V/Belgs.Common");
                ExtensionMethods.DisplayPopupTextBoxWithConfirmation("Belgs.Common", "Error while processing XML files. To fix this, please delete the following folder and its contents: Grand Theft Auto V/Belgs.Common", false);
                throw;
            }

        }

        private static void GetUpdateNodes()
        {
            IEnumerable<string> XMLUpdateFiles = Directory.EnumerateFiles("Belgs.Common/UpdateInfo", "*.xml");
            foreach (string xmlnode in XMLUpdateFiles)
            {
                XDocument xdoc = XDocument.Load(xmlnode);
                if (IsUpdateNodeValid(xdoc.Root))
                {
                    if (File.Exists(XmlConvert.DecodeName(xdoc.Root.Element("Path").Value.Trim())))
                    {
                        UpdateEntry.AllUpdateEntries.Add(new UpdateEntry()
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

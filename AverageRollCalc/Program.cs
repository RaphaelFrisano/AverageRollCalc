using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AverageRollCalc
{
    

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Create firstime folder and files for programm
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\" + "averageRollCalc";
            System.IO.Directory.CreateDirectory(folderPath);
            if (!File.Exists(folderPath + "\\charList.txt")){File.Create(folderPath + "\\charList.txt").Dispose();}

            // Check if a new version is available
            string currentVersion = "1.0";
            string pathToVersionFile = "https://github.com/RaphaelFrisano/AverageRollCalc/tree/main/AverageRollCalc/bin/Debug/currentVersion.txt";
            bool versionDif = versionDifferent(currentVersion, pathToVersionFile);

            if (versionDif)
            {
                // Update Programm
                if (MessageBox.Show("You are currently running version " + currentVersion + "\na new Version is available to download. Take you there?", "AverageRollCalc Updater", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/RaphaelFrisano/AverageRollCalc/tree/main/AverageRollCalc/bin/release/AverageRollCalc.exe");
                }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            
        }

        public static bool versionDifferent(string currentVersion, string pathToVersionFile)
        {
            WebClient myBotNewVersionClient = new WebClient();
            Stream stream = myBotNewVersionClient.OpenRead(pathToVersionFile);
            StreamReader reader = new StreamReader(stream);
            string onlineVersion = reader.ReadToEnd();

            return onlineVersion == currentVersion;
        }
    }
    
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AverageRollCalc
{
    public class dice
    {
        public int sides = 20;
        Random rnd = new Random();

        public int Roll()
        {
            return rnd.Next(1, sides+1);
        }

    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\" + "averageRollCalc";
            System.IO.Directory.CreateDirectory(folderPath);
            if (!File.Exists(folderPath + "\\charList.txt")){File.Create(folderPath + "\\charList.txt").Dispose();}

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            /*
            dice D4 = new dice();
            D4.sides = 4;
            dice D6 = new dice();
            D6.sides = 6;
            dice D8 = new dice();
            D8.sides = 8;
            dice D10 = new dice();
            D8.sides = 10;
            dice D12 = new dice();
            D12.sides = 12;
            dice D20 = new dice();
            dice D100 = new dice();
            D100.sides = 100;

            MessageBox.Show("On the D4 you rolled: " + D4.Roll().ToString());
            MessageBox.Show("On the D6 you rolled: " + D6.Roll().ToString());
            MessageBox.Show("On the D8 you rolled: " + D8.Roll().ToString());
            MessageBox.Show("On the D10 you rolled: " + D10.Roll().ToString());
            MessageBox.Show("On the D12 you rolled: " + D12.Roll().ToString());
            MessageBox.Show("On the D20 you rolled: " + D20.Roll().ToString());
            MessageBox.Show("On the D100 you rolled: " + D100.Roll().ToString());
            */
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using MadMilkman.Ini;

namespace AverageRollCalc
{
    public partial class Form1 : Form
    {
        // Innit functions
        public Form1()
        {
            InitializeComponent();
            int i = 0;
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\" + "averageRollCalc";
            foreach (string line in File.ReadAllLines(folderPath + "\\charList.txt"))
            {
                cbCharacter.Items.Insert(i, line);
                i++;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        // Utility functions
        public IniFile ReadData(string character, string dice)
        {
            IniFile ini = new IniFile();
            string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\" + "averageRollCalc" + "\\" + character + "\\" + dice + ".ini";
            ini.Load(iniPath);
            return ini;
        }

        public void add_roll(string dice, string character, string roll, string modType, string modValue)
        {
            IniFile ini = ReadData(character, dice);
            string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\" + "averageRollCalc" + "\\" + character + "\\" + dice + ".ini";

            // If no modifier was added, make it 0
            if(modValue == ""){modValue = "0";}

            // Add roll under a new section (name based on linecount)
            ini.Sections.Add(
                new IniSection(ini, Guid.NewGuid().ToString(),
                    new IniKey(ini, "roll", roll),
                    new IniKey(ini, "modifier", modType),
                    new IniKey(ini, "value", modValue)));
            ini.Save(iniPath);
        }

        public void delete_roll(string dice, string character, DataGridView rollsBox)
        {
            IniFile ini = ReadData(character, dice);
            string iniPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\" + "averageRollCalc" + "\\" + character + "\\" + dice + ".ini";

            foreach (DataGridViewRow row in rollsBox.SelectedRows)
            {
                string secID = row.Cells[0].Value.ToString();
                rollsBox.Rows.RemoveAt(row.Index);
                ini.Sections.Remove(secID);
            }
            ini.Save(iniPath);
        }

        public void refresh(DataGridView rollsBox, Label lblAverageWith, Label lblAverageWithout, string charName, string dice)
        {
            //Read data from file
            IniFile ini = ReadData(charName, dice);

            //Refresh character list with all Data
            int i = 0;
            float averageWith = 0;
            float averageWithout = 0;
            rollsBox.Rows.Clear();

            foreach (IniSection section in ini.Sections)
            {
                //Get and format data
                string sectName = section.Name;
                float roll = float.Parse(section.Keys["roll"].Value);
                string modifier = section.Keys["modifier"].Value;
                float value = float.Parse(section.Keys["value"].Value);
                float result = 0;
                if (modifier == "+")
                {
                    result = roll + value;
                }
                else
                {
                    result = roll - value;
                }

                //Add data to box

                rollsBox.Rows.Add(sectName, roll, modifier, value, result);

                //Do math for average
                averageWithout += roll;
                if (modifier == "+") 
                {
                    averageWith += roll;
                    averageWith += value;
                }
                else
                {
                    averageWith += roll;
                    averageWith -= value;
                }
                i++;
            }

            // Add averages to labels
            if (averageWith == 0 || averageWithout == 0)
            {
                //No data for diceroll, ignore and continue
            }
            else
            {
                averageWith /= i;
                averageWithout /= i;
                lblAverageWith.Text = averageWith.ToString();
                lblAverageWithout.Text = averageWithout.ToString();
            }
        }


        // Other onclick events
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (cbCharacter.SelectedIndex != -1)
            {
                refresh(gvD4, lblWithResultD4, lblWithoutResultD4, cbCharacter.SelectedItem.ToString(), "D4");
                refresh(gvD6, lblWithResultD6, lblWithoutResultD6, cbCharacter.SelectedItem.ToString(), "D6");
                refresh(gvD8, lblWithResultD8, lblWithoutResultD8, cbCharacter.SelectedItem.ToString(), "D8");
                refresh(gvD10, lblWithResultD10, lblWithoutResultD10, cbCharacter.SelectedItem.ToString(), "D10");
                refresh(gvD12, lblWithResultD12, lblWithoutResultD12, cbCharacter.SelectedItem.ToString(), "D12");
                refresh(gvD20, lblWithResultD20, lblWithoutResultD20, cbCharacter.SelectedItem.ToString(), "D20");
                refresh(gvD100, lblWithResultD100, lblWithoutResultD100, cbCharacter.SelectedItem.ToString(), "D100");
            }
            else
            {
                MessageBox.Show("Please select a character first!");
            }
        }

        private void cbCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            refresh(gvD4, lblWithResultD4, lblWithoutResultD4, cbCharacter.SelectedItem.ToString(), "D4");
            refresh(gvD6, lblWithResultD6, lblWithoutResultD6, cbCharacter.SelectedItem.ToString(), "D6");
            refresh(gvD8, lblWithResultD8, lblWithoutResultD8, cbCharacter.SelectedItem.ToString(), "D8");
            refresh(gvD10, lblWithResultD10, lblWithoutResultD10, cbCharacter.SelectedItem.ToString(), "D10");
            refresh(gvD12, lblWithResultD12, lblWithoutResultD12, cbCharacter.SelectedItem.ToString(), "D12");
            refresh(gvD20, lblWithResultD20, lblWithoutResultD20, cbCharacter.SelectedItem.ToString(), "D20");
            refresh(gvD100, lblWithResultD100, lblWithoutResultD100, cbCharacter.SelectedItem.ToString(), "D100");
        }

        private void btnNewChar_Click(object sender, EventArgs e)
        {
            string charName = Interaction.InputBox("Give your Character a name,\ndont use any special characters!", "Creating Character", "Default Text");
            Regex rgx = new Regex("[^a-zA-Z ]");
            bool hasSpecialChars = rgx.IsMatch(charName);
            if (!hasSpecialChars && charName != "" && !cbCharacter.Items.Contains(charName))
            {
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\" + "averageRollCalc";
                File.AppendAllText(folderPath + "\\charList.txt", charName + "\r\n");
                System.IO.Directory.CreateDirectory(folderPath + "\\" + charName);
                File.Create(folderPath + "\\" + charName + "\\d4.ini").Dispose();
                File.Create(folderPath + "\\" + charName + "\\d6.ini").Dispose();
                File.Create(folderPath + "\\" + charName + "\\d8.ini").Dispose();
                File.Create(folderPath + "\\" + charName + "\\d10.ini").Dispose();
                File.Create(folderPath + "\\" + charName + "\\d12.ini").Dispose();
                File.Create(folderPath + "\\" + charName + "\\d20.ini").Dispose();
                File.Create(folderPath + "\\" + charName + "\\d100.ini").Dispose();
                cbCharacter.Items.Add(charName);
            }
            else
            {
                MessageBox.Show("Name invalid!\nPlease remove special characters!");
            }
        }


        // Buttons to add rolls
        private void btnAddD4_Click(object sender, EventArgs e)
        {
            if (cbCharacter.SelectedIndex != -1)
            {
                var isNumeric = int.TryParse(boxRolledD4.Text, out int n);
                if (isNumeric && n <= 4)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    add_roll("D4", character, boxRolledD4.Text, cbModD4.SelectedItem.ToString(), boxModifierD4.Text);
                    refresh(gvD4, lblWithResultD4, lblWithoutResultD4, character, "D4");
                }
                else
                {
                    MessageBox.Show("Invalid number!");
                }
            }
            else
            {
                MessageBox.Show("Please select a character first!");
            }
        }

        private void btnAddD6_Click(object sender, EventArgs e)
        {
            if (cbCharacter.SelectedIndex != -1)
            {
                var isNumeric = int.TryParse(boxRolledD6.Text, out int n);
                if (isNumeric && n <= 6)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    add_roll("D6", character, boxRolledD6.Text, cbModD6.SelectedItem.ToString(), boxModifierD6.Text);
                    refresh(gvD6, lblWithResultD6, lblWithoutResultD6, character, "D6");
                }
                else
                {
                    MessageBox.Show("Invalid number!");
                }
            }
            else
            {
                MessageBox.Show("Please select a character first!");
            }
        }

        private void btnAddD8_Click(object sender, EventArgs e)
        {
            if (cbCharacter.SelectedIndex != -1)
            {
                var isNumeric = int.TryParse(boxRolledD8.Text, out int n);
                if (isNumeric && n <= 8)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    add_roll("D8", character, boxRolledD8.Text, cbModD8.SelectedItem.ToString(), boxModifierD8.Text);
                    refresh(gvD8, lblWithResultD8, lblWithoutResultD8, character, "D8");
                }
                else
                {
                    MessageBox.Show("Invalid number!");
                }
            }
            else
            {
                MessageBox.Show("Please select a character first!");
            }
        }

        private void btnAddD10_Click(object sender, EventArgs e)
        {
            if (cbCharacter.SelectedIndex != -1)
            {
                var isNumeric = int.TryParse(boxRolledD10.Text, out int n);
                if (isNumeric && n <= 10)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    add_roll("D10", character, boxRolledD10.Text, cbModD10.SelectedItem.ToString(), boxModifierD10.Text);
                    refresh(gvD10, lblWithResultD10, lblWithoutResultD10, character, "D10");
                }
                else
                {
                    MessageBox.Show("Invalid number!");
                }
            }
            else
            {
                MessageBox.Show("Please select a character first!");
            }
        }

        private void btnAddD12_Click(object sender, EventArgs e)
        {
            if (cbCharacter.SelectedIndex != -1)
            {
                var isNumeric = int.TryParse(boxRolledD12.Text, out int n);
                if (isNumeric && n <= 12)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    add_roll("D12", character, boxRolledD12.Text, cbModD12.SelectedItem.ToString(), boxModifierD12.Text);
                    refresh(gvD12, lblWithResultD12, lblWithoutResultD12, character, "D12");
                }
                else
                {
                    MessageBox.Show("Invalid number!");
                }
            }
            else
            {
                MessageBox.Show("Please select a character first!");
            }
        }

        private void btnAddD20_Click(object sender, EventArgs e)
        {
            if (cbCharacter.SelectedIndex != -1)
            {
                var isNumeric = int.TryParse(boxRolledD20.Text, out int n);
                if (isNumeric && n <= 20)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    add_roll("D20", character, boxRolledD20.Text, cbModD20.SelectedItem.ToString(), boxModifierD20.Text);
                    refresh(gvD20, lblWithResultD20, lblWithoutResultD20, character, "D20");
                }
                else
                {
                    MessageBox.Show("Invalid number!");
                }
            }
            else
            {
                MessageBox.Show("Please select a character first!");
            }
        }

        private void btnAddD100_Click(object sender, EventArgs e)
        {
            if (cbCharacter.SelectedIndex != -1)
            {
                var isNumeric = int.TryParse(boxRolledD100.Text, out int n);
                if (isNumeric && n <= 100)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    add_roll("D100", character, boxRolledD100.Text, cbModD100.SelectedItem.ToString(), boxModifierD100.Text);
                    refresh(gvD100, lblWithResultD100, lblWithoutResultD100, character, "D100");
                }
                else
                {
                    MessageBox.Show("Invalid number!");
                }
            }
            else
            {
                MessageBox.Show("Please select a character first!");
            }
        }


        // Buttons to delete rolls
        private void btnDelD4_Click(object sender, EventArgs e)
        {
            if (gvD4.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to delete the selected roll?", "Delete roll", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    delete_roll("D4", character, gvD4);
                    refresh(gvD4, lblWithResultD4, lblWithoutResultD4, character, "D4");
                }
            }
            else
            {
                MessageBox.Show("Please select a diceroll first!");
            }
        }

        private void btnDelD6_Click(object sender, EventArgs e)
        {
            if (gvD6.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to delete the selected roll?", "Delete roll", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    delete_roll("D6", character, gvD6);
                    refresh(gvD6, lblWithResultD6, lblWithoutResultD6, character, "D6");
                }
            }
            else
            {
                MessageBox.Show("Please select a diceroll first!");
            }
        }

        private void btnDelD8_Click(object sender, EventArgs e)
        {
            if (gvD8.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to delete the selected roll?", "Delete roll", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    delete_roll("D8", character, gvD8);
                    refresh(gvD8, lblWithResultD8, lblWithoutResultD8, character, "D8");
                }
            }
            else
            {
                MessageBox.Show("Please select a diceroll first!");
            }
        }

        private void btnDelD10_Click(object sender, EventArgs e)
        {
            if (gvD10.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to delete the selected roll?", "Delete roll", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    delete_roll("D10", character, gvD10);
                    refresh(gvD10, lblWithResultD10, lblWithoutResultD10, character, "D10");
                }
            }
            else
            {
                MessageBox.Show("Please select a diceroll first!");
            }
        }

        private void btnDelD12_Click(object sender, EventArgs e)
        {
            if (gvD12.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to delete the selected roll?", "Delete roll", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    delete_roll("D12", character, gvD12);
                    refresh(gvD12, lblWithResultD12, lblWithoutResultD12, character, "D12");
                }
            }
            else
            {
                MessageBox.Show("Please select a diceroll first!");
            }
        }

        private void btnDelD20_Click(object sender, EventArgs e)
        {
            if (gvD20.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to delete the selected roll?", "Delete roll", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    delete_roll("D20", character, gvD20);
                    refresh(gvD20, lblWithResultD20, lblWithoutResultD20, character, "D20");
                }
            }
            else
            {
                MessageBox.Show("Please select a diceroll first!");
            }
        }

        private void btnDelD100_Click(object sender, EventArgs e)
        {
            if (gvD100.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to delete the selected roll?", "Delete roll", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string character = cbCharacter.SelectedItem.ToString();
                    delete_roll("D100", character, gvD100);
                    refresh(gvD100, lblWithResultD100, lblWithoutResultD100, character, "D100");
                }
            }
            else
            {
                MessageBox.Show("Please select a diceroll first!");
            }
        }
    }
}
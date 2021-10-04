using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTA_GUI
{
    public partial class Form1 : Form
    {
        private void HandlePanelSwitch(Panel SelectedPanel)
        {
            HomePanel.Hide();
            PubChemPanel.Hide();
            BiotransformerPanel.Hide();
            SkippedCompoundPanel.Hide();

            SelectedPanel.Show();
        }

        private void HandleButtonPress(Button SelectedButton)
        {
            HomeButton.ForeColor = Color.FromArgb(255, 255, 255);
            PreBiotransformerButton.ForeColor = Color.FromArgb(255, 255, 255);
            PubChemButton.ForeColor = Color.FromArgb(255, 255, 255);
            SkippedCompoundsButton.ForeColor = Color.FromArgb(255, 255, 255);

            SelectedButton.ForeColor = Color.FromArgb(206, 212, 218);
        }

        public Form1()
        {
            InitializeComponent();

            SkippedCompoundsButton.ForeColor = Color.FromArgb(108, 117, 125);
            SkippedCompoundsButton.Enabled = false;

            Saving.Setup();

        }

        private void PreBiotransformerButton_Click(object sender, EventArgs e)
        {
            HandlePanelSwitch(BiotransformerPanel);
            HandleButtonPress(PreBiotransformerButton);
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            HandlePanelSwitch(HomePanel);
            HandleButtonPress(HomeButton);
        }
        private void PubChemButton_Click(object sender, EventArgs e)
        {
            HandlePanelSwitch(PubChemPanel);
            HandleButtonPress(PubChemButton);
        }
        private void SkippedCompoundsButton_Click(object sender, EventArgs e)
        {
            HandlePanelSwitch(SkippedCompoundPanel);
            HandleButtonPress(SkippedCompoundsButton);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => System.Diagnostics.Process.Start("https://github.com/KuebV/Biotransformer-Assistant-GUI");
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => System.Diagnostics.Process.Start("https://github.com/KuebV/Biotransformer-Assistant-GUI/releases");
        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => System.Diagnostics.Process.Start("https://pubchem.ncbi.nlm.nih.gov/idexchange/idexchange.cgi");
        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => System.Diagnostics.Process.Start("https://bitbucket.org/djoumbou/biotransformerjar/src/master/");

        private void ParseDataButton_Click(object sender, EventArgs e)
        {
            foreach (string line in RawCompoundsTextBox.Lines)
                PubChem.ParsedCompounds.Add(PubChem.ParseCompounds(line));

            ParsedCompoundTextBox.Text = string.Empty;
            foreach (string compound in PubChem.ParsedCompounds)
                ParsedCompoundTextBox.Text += string.Format("{0}\n", compound);

            Saving.SaveFile(Saving.PubChemCompounds, ParsedCompoundTextBox.Lines);
            PubChem.ParsedCompounds.Clear();
        }

        private void ClearRawCompoundButton_Click(object sender, EventArgs e) => RawCompoundsTextBox.Clear();
        private void CompoundCopyButton_Click(object sender, EventArgs e) => Clipboard.SetText(ParsedCompoundTextBox.Text);
        private void PubChemLinkButton_Click(object sender, EventArgs e) => System.Diagnostics.Process.Start("https://pubchem.ncbi.nlm.nih.gov/idexchange/idexchange.cgi");

        private void BiotransformerClearButton_Click(object sender, EventArgs e)
        {
            BiotransformerInputTextBox.Clear();
            SkippedCompoundsButton.ForeColor = Color.FromArgb(108, 117, 125);
            SkippedCompoundsButton.Enabled = false;

            BiotransformerCMDS.Clear();
            SkippedSMILES.Clear();
            BiotransformerInput.Text = string.Empty;
            SkippedCompoundTextBox.Text = string.Empty;
            SMILES_Spreadsheet_Key.Text = string.Empty;
            Biotransformer.PubChemSMILES.Clear();
        }

        private List<string> BiotransformerCMDS = new List<string>();
        public static List<string> SkippedSMILES = new List<string>();

        private void BiotransformerParseDataButton_Click(object sender, EventArgs e)
        {
            BiotransformerCMDS.Clear();
            SkippedSMILES.Clear();
            BiotransformerInput.Text = string.Empty;
            SkippedCompoundTextBox.Text = string.Empty;
            SMILES_Spreadsheet_Key.Text = string.Empty;
            Biotransformer.PubChemSMILES.Clear();


            Biotransformer.LoadChemFile(BiotransformerInputTextBox.Lines);

            SkippedCompoundsButton.ForeColor = Color.FromArgb(255, 255, 255);
            SkippedCompoundsButton.Enabled = true;

            int i = 0;
            foreach (var value in Biotransformer.PubChemSMILES)
            {
                i = i + 1;
                if (!string.IsNullOrEmpty(value.Value))
                {
                    string name = ($"{ListNameTextBox.Text}" + ".cmpnd" + i + "." + FileTypeDropDown.Text.ToLower());
                    string bio = "java -jar \"biotransformer-1.1.5 (1).jar\" -k pred -b " + MetabolismDropDown.Text + " -ismi \"" + value.Value + "\" -o" + FileTypeDropDown.Text.ToLower() + " " + name + " -s 1";

                    BiotransformerCMDS.Add(bio);
                }
                else
                {
                    string compoundFormatted = $"[{i}] {value.Key}";
                    SkippedSMILES.Add(compoundFormatted);
                }
            }

            foreach (string input in BiotransformerCMDS) { BiotransformerInput.Text += string.Format("{0}\n", input); }

            foreach (var smiles in Biotransformer.PubChemSMILES)
            {

                if (smiles.Key != null)
                    SMILES_Spreadsheet_Key.Text += string.Format("{0}\n", smiles.Value);
                else
                    SMILES_Spreadsheet_Key.Text += "\n";
            }

            foreach (string skippedSMILE in SkippedSMILES) { SkippedCompoundTextBox.Text += string.Format("{0}\n", skippedSMILE); }

            Saving.SaveFile(Saving.SMILES_SpreadsheetKey, SMILES_Spreadsheet_Key.Lines);
            Saving.SaveFile(Saving.BiotransformerInput, BiotransformerInput.Lines);
        }

        private void SetToDefaultButton_Click(object sender, EventArgs e)
        {
            MetabolismDropDown.SelectedIndex = 4;
            FileTypeDropDown.SelectedIndex = 0;
            ListNameTextBox.Text = "list1";
        }

        private void BiotransformerInputCopyButton_Click(object sender, EventArgs e) => Clipboard.SetText(BiotransformerInput.Text);

        private void HelpSkippedCompoundsButton_Click(object sender, EventArgs e) => System.Diagnostics.Process.Start("https://github.com/KuebV/Biotransformer-Assistant-GUI/wiki");

        private void SkippedCompoundsSetToDefaultButton_Click(object sender, EventArgs e)
        {
            SkippedCompoundsFileTypeDropdown.SelectedIndex = 0;
            SkippedCompoundsMetabolismTypeDropdown.SelectedIndex = 4;
            labelSomething.Text = "list1";
        }

        public Dictionary<int, string> SkippedCompoundDictionary = new Dictionary<int, string>();

        private void SkippedCompoundParseDataButton_Click(object sender, EventArgs e)
        {
            SkippedCompoundDictionary.Clear();
            SkippedCompoundsBiotransformerInputTextBox.Text = string.Empty;

            foreach (string line in SkippedCompoundTextBox.Lines)
            {
                if (line.Contains("|||"))
                {
                    List<string> CmdElementSplit = new List<string>(line.Split(new string[] { "|||" }, StringSplitOptions.None));

                    int NumberCompound = Convert.ToInt32(CmdElementSplit.ElementAt(0).Split(']')[0].Replace("[", ""));

                    // Parse the spaces outta there, we dont need them
                    string ParsedSMILE = CmdElementSplit.ElementAt(1).Replace(" ", "");
                    SkippedCompoundDictionary.Add(NumberCompound, ParsedSMILE);
                }
            }

            if (!SkippedCompoundTextBox.Text.Contains("|||"))
                SkippedCompoundsBiotransformerInputTextBox.Text = "Expecting Something? Press the \"Help\" button in the top right";

            foreach (var keyValue in SkippedCompoundDictionary)
            {
                string name = ($"{SkippedCompoundsListNameTextBox.Text}" + ".cmpnd" + keyValue.Key + "." + SkippedCompoundsFileTypeDropdown.Text.ToLower());
                string bio = "java -jar \"biotransformer-1.1.5 (1).jar\" -k pred -b " + SkippedCompoundsMetabolismTypeDropdown.Text + " -ismi \"" + keyValue.Value + "\" -o" + SkippedCompoundsFileTypeDropdown.Text.ToLower() + " " + name + " -s 1";

                SkippedCompoundsBiotransformerInputTextBox.Text += string.Format("{0}\n", bio);
            }

            Saving.SaveFile(Saving.SkippedCompounds, SkippedCompoundsBiotransformerInputTextBox.Lines);

        }
    }
}

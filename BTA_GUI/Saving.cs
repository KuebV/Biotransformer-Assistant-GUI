using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BTA_GUI
{
    public class Saving
    {

        public static string BTADirectory = Path.Combine(Directory.GetCurrentDirectory(), "BTAFiles");

        public static string PubChemCompounds = Path.Combine(BTADirectory, "PubChemCompounds.txt");
        public static string BiotransformerInput = Path.Combine(BTADirectory, "BiotransformerInput.txt");

        public static string SMILES_SpreadsheetKey = Path.Combine(BTADirectory, "SMILES_Spreadsheet_Key.txt");
        public static string SkippedCompounds = Path.Combine(BTADirectory, "SkippedCompounds.txt");
        public static void Setup()
        {
            if (!Directory.Exists(BTADirectory))
                Directory.CreateDirectory(BTADirectory);

            InitalizeFile(PubChemCompounds);
            InitalizeFile(BiotransformerInput);
            InitalizeFile(SMILES_SpreadsheetKey);
            InitalizeFile(SkippedCompounds);
        }

        public static void SaveFile(string fileName, string[] stringArray)
        {
            File.Delete(fileName);
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (string item in stringArray)
                    sw.WriteLine(item);
                sw.Close();
            }
        }

        private static void InitalizeFile(string fileName)
        {
            if (!File.Exists(fileName))
                using (StreamWriter sw = new StreamWriter(fileName))
                    sw.Close();
        }
    }
}

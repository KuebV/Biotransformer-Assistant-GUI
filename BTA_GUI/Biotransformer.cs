using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BTA_GUI
{
    public class Biotransformer
    {
        public static IEnumerable<string> PubChemOutputFile;

        public static List<string> PubChemOutputFinalized;
        public static Dictionary<string, string> PubChemSMILES = new Dictionary<string, string>();

        // This is typically not very good, especially for larger datasets
        // Although this is the easiest and cleanest implementation
        public static string ReplaceWhitespaces(string comp)
        {
            string pattern = "\\s+";
            string replacement = " ";
            Regex rx = new Regex(pattern);

            string result = rx.Replace(comp, replacement);
            return result;
        }

        public static void LoadChemFile(string[] BiotransformerInput)
        {
            foreach (string chem in BiotransformerInput)
            {
                IsSmiles(chem);

            }
        }


        /// <summary>
        /// There are a lot of ways to go about this,
        /// One of them being taking the 6 most prominent characters in the SMILES format, those being c, p, o, h, n, s
        /// 
        /// In our case, what may work is just colliding and testing the string against what SMILES may have, and what it may not have
        /// Things to watch out for:
        /// As example:
        /// Cholic acid	C[C@H](CCC(=O)O)[C@H]1CC[C@@H]2[C@@]1([C@H](C[C@H]3[C@H]2[C@@H](C[C@H]4[C@@]3(CC[C@H](C4)O)C)O)O)C
        /// 
        /// The way BTA would typically handle this, is it would split it by spaces, unfortuneatly, that is no longer viable
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void IsSmiles(string obj)
        {
            // Define the characters that are typically found in SMILES
            // This is oddly never used, since only 1 character is needed, that being C
            //List<char> SMILESCharacters = new List<char> { 'C', 'P', 'O', 'H', 'N', 'S' };

            // This is still very helpful in removing useless whitespaces
            string possibleSmiles = ReplaceWhitespaces(obj);

            // We get the amount of words in that possible SMILES
            // We can follow this chart as an example of outcomes:
            // 1 : No SMILES found in String
            // 2 : The previous method of splitting at the space is viable, but it may not be what we want
            // 3+ : If the Compound Name is more than 1 word, which although unlikely, is still possible
            int wordAmount = possibleSmiles.Split(' ').Length;

            switch (wordAmount)
            {
                // If there is only 1 word found in the line, then we most likely know it doesnt have a SMILES Compound
                case 0:
                case 1:
                    PubChemSMILES.Add(possibleSmiles, null);
                    break;

                // If there is 2 words found in the line, the previous method is viable, but first we have to test for things that are unlikely, but may happen
                case 2:
                    #region If the possible smiles has any character in here, then it automatically nulls it
                    if (possibleSmiles.Contains('*'))
                        PubChemSMILES.Add(string.Format("{0}({1})", possibleSmiles, PubChemSMILES.Count + 1), null);
                    else if (possibleSmiles.Contains("iso3") || possibleSmiles.Contains("iso6"))
                        PubChemSMILES.Add(string.Format("{0}({1})", possibleSmiles, PubChemSMILES.Count + 1), null);
                    #endregion
                    else
                    {
                        string[] splitChem = possibleSmiles.Split(' ');
                        if (!PubChemSMILES.ContainsKey(splitChem[0]))
                            PubChemSMILES.Add(splitChem[0], splitChem[1]);
                        //else
                        //{
                        //    PubChemSMILES.Add(string.Format("{0}({1})", splitChem[0], PubChemSMILES.Count + 1), null);
                        //}

                    }

                    break;

                // We can have this be set to default, I dont think we're expecting any other value
                default:
                    string[] possibleCompounds = possibleSmiles.Split(' ');
                    foreach (string str in possibleCompounds)
                    {

                        // After 20 minutes of tinkering, this is the best that works
                        var checkForSpecial = new string(str.Where(c => Char.IsLetterOrDigit(c)
                                            || Char.IsWhiteSpace(c)).ToArray());

                        // Let's break down this if statement
                        // 1. If the divided string contains an Uppercase C
                        // 2. If the divided string contains an Comma Sign
                        // 3. Suprisingly, I have yet to find a colon in the SMILES, it may not exist. Lets use that to our advantage
                        if (str.Contains('C') && !str.Contains(',') && !str.Contains(':') && str != checkForSpecial)
                        {
                            // Gets the index of the SMILE
                            int indexOfSMILE = Array.IndexOf(possibleCompounds, str);
                            string compoundName = string.Format("{0}_{1}", possibleCompounds[0], possibleCompounds[1]);

                            // On the rare occasion that PubChem spits out a compound that has both 3 letters, and is a duplicate
                            // If it exists already, enter it in as null
                            if (!PubChemSMILES.ContainsKey(compoundName))
                                PubChemSMILES.Add(compoundName, str);
                            //else
                            //    PubChemSMILES.Add(string.Format("{0}({1})", compoundName, PubChemSMILES.Count + 1), null);
                        }
                    }
                    break;
            }

        }
    }
}

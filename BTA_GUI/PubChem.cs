using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTA_GUI
{
    public class PubChem
    {
        public static string ParseCompounds(string Compound)
        {
            if (Compound.Contains("Esi+"))
            {
                int indexOfPlus = Compound.IndexOf("Esi+");
                if (indexOfPlus >= 0)
                    Compound = Compound.Substring(0, indexOfPlus);
            }

            if (Compound.Contains('+'))
            {
                int indexOfPlus = Compound.IndexOf('+');
                if (indexOfPlus >= 0)
                    Compound = Compound.Substring(0, indexOfPlus);
            }

            return Compound;
        }

        public static List<string> ParsedCompounds = new List<string>();

    }
}

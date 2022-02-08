using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PubChem : MonoBehaviour
{
    public InputField inputField;
    public InputField outputField;

    public void CopyToClipboard(InputField field) => GUIUtility.systemCopyBuffer = field.text;
    public void OpenPubChem() => Application.OpenURL("https://pubchem.ncbi.nlm.nih.gov/idexchange/idexchange.cgi");

    public void ParseData()
    {
        string pubcheminput = Path.Combine(Directory.GetCurrentDirectory(), "PubChem.txt");
        if (File.Exists(pubcheminput))
            File.Delete(pubcheminput);
        outputField.text = "";
        string[] data = inputField.text.Split('\n');
        foreach (string str in data)
            outputField.text += $"{ParseCompounds(str)}\n";

        StreamWriter sw = new StreamWriter(pubcheminput);
        sw.WriteLine(outputField.text.Replace("\n", ""));
        sw.Close();
    }
    public void ClearData() 
    {
        inputField.text = "";
        outputField.text = "";
    }

    private string ParseCompounds(string Compound)
    {
        if (Compound.Contains("Esi+"))
        {
            int indexOfPlus = Compound.IndexOf("Esi+");
            if (indexOfPlus >= 0)
                Compound = Compound.Substring(0, indexOfPlus);
        }

        if (Compound.Contains("+"))
        {
            int indexOfPlus = Compound.IndexOf('+');
            if (indexOfPlus >= 0)
                Compound = Compound.Substring(0, indexOfPlus);
        }

        if (Compound.Contains("*"))
        {
            int indexOfPlus = Compound.IndexOf('*');
            if (indexOfPlus >= 0)
                Compound = Compound.Substring(0, indexOfPlus);
        }

        return Compound;
    }
}

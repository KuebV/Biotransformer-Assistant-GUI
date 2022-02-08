using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class Biotransformer : MonoBehaviour
{
    public TMP_InputField PubChemInput;

    public TMP_Dropdown Metabolites;
    public TMP_Dropdown FileType;
    public TMP_InputField ListName;
    public TMP_InputField BiotransformerFile;

    public InputField PubChemOutput;

    public Button SkippedCompoundButton;

    private SkippedCompounds skippedCompounds;
    public GameObject SkippedCompoundPanel;

    public TMP_Text BiotransformerStatus;

    public string BiotransformerData;

    private ButtonHeaders header;
    public GameObject ButtonHeader;

    public int TotalCompounds;

    public void Awake() => BiotransformerStatus.text = "Status: <color=#FF4612> Waiting </color>";

    public void ParseCompounds()
    {
        if (string.IsNullOrEmpty(ListName.text))
            ListName.text = "list1";

        if (string.IsNullOrEmpty(BiotransformerFile.text))
            BiotransformerFile.text = "biotransformer-1.1.5.jar";

        skippedCompounds = SkippedCompoundPanel.GetComponent<SkippedCompounds>();

        CompoundData.Clear();
        CompoundNames.Clear();
        BiotransformerData = "";

        skippedCompounds.SpreadsheetKey = "";
        foreach (SkippedCompoundsListElement element in skippedCompounds._compounds)
            Destroy(element.gameObject);

        Arguments = new BiotransformerInput
        {
            BiotransformerFileName = BiotransformerFile.text,
            FileType = FileType.options[FileType.value].text,
            ListName = ListName.text,
            MetabolismType = Metabolites.options[Metabolites.value].text
        };

        string mainDirectory = Directory.GetCurrentDirectory();
        string biotransformerInputFile = BiotransformerOutputFile;

        File.Delete(biotransformerInputFile);
        using (StreamWriter sw = new StreamWriter(biotransformerInputFile))
            sw.Close();

        string[] data = PubChemInput.text.Split('\n');
        SkippedCompoundButton.gameObject.SetActive(true);

        List<string> listData = new List<string>();
        List<string> duplicateChecker = new List<string>();
        foreach (string d in data){
            string compoundName = d.Split('\t')[0];
            if (!duplicateChecker.Contains(compoundName))
            {
                listData.Add(d);
                duplicateChecker.Add(compoundName);
                Debug.Log("Added: " + compoundName);
            }
        }

        Debug.Log("Data : " + data.Length + "\tList Data: " + listData.Count);


        BiotransformerStatus.text = "Status: <color=#509ED8> Parsing </color>";

        TotalCompounds = listData.Count;

        for (int i = 0; i < listData.Count; i++) {
            Debug.Log("Starting Index: " + i);
            StartCoroutine(FindSMILES(listData[i], i));
        }
            

        skippedCompounds = SkippedCompoundPanel.GetComponent<SkippedCompounds>();
        skippedCompounds.InitalizeCompoundData(CompoundData);
        skippedCompounds.PopulateList();

        BiotransformerStatus.text = "Status: <color=#006E62> Completed </color>";
    }

    public List<CompoundData> CompoundData = new List<CompoundData>();
    public List<string> CompoundNames = new List<string>();
    private BiotransformerInput Arguments;
    public List<string> PubChemMasterList = new List<string>();
    public List<string> AllowedCompounds = new List<string>();

    public void CopyToClipboard() => GUIUtility.systemCopyBuffer = File.ReadAllText(BiotransformerOutputFile);
    public void AssistanceDocument() => Application.OpenURL("https://docs.google.com/document/d/1FbvyxIwNhrWRuiSscL4cNdbIBZxMrf3fKLy6LvbpqRQ/edit?usp=sharing");

    private string BiotransformerOutputFile = Path.Combine(Directory.GetCurrentDirectory(), "BiotransformerInput.txt");

    public string ReturnBiotransformerInput(int Position, string SMILE)
    {
        //Seralize String
        SMILE = SMILE.Replace("\n", "");
        string name = $"{Arguments.ListName}" + ".cmpnd" + Position + "." + Arguments.FileType.ToLower();
        string bio = "java -jar \"" + Arguments.BiotransformerFileName + "\" -k pred -b " + Arguments.MetabolismType + " -ismi \"" + SMILE + "\" -o" + Arguments.FileType.ToLower() + " " + name + " -s 1";
        return bio;
    }

    public string[] RemoveWhitespacedPubChem(string[] UnparsedInput)
    {
        List<string> output = new List<string>();
        foreach (string line in UnparsedInput)
        {
            string compoundName = line;
            char LastCharacter = compoundName.ElementAt(compoundName.Length - 1);
            if (char.IsWhiteSpace(LastCharacter)) {
                compoundName = compoundName.Remove(compoundName.Length - 1);
            }
            output.Add(compoundName);
        }

        return output.ToArray();
    }

    public IEnumerator FindSMILES(string compound, int position)
    {
        position++;
        string endingElement = compound.Split('\t')[1];

        string biotransformerFileInput = BiotransformerOutputFile;
        if (compound.Contains("\t") && endingElement.Length > 1)
        {
            compound = compound.Replace("\t", " ");
            string[] CompoundComponents = compound.Split(' ');
            string SMILES = CompoundComponents.Last();
            SMILES = SMILES.Remove(SMILES.Length - 1);

            string CompoundName = compound.Replace(SMILES, "");
            if (SMILES.Length > 3 && !CompoundNames.Contains(CompoundName))
            {
                CompoundData compoundInfo = new CompoundData
                {
                    CompoundName = CompoundName,
                    SMILESFormat = SMILES,
                    Index = position,
                    Duplicate = false
                };
                CompoundData.Add(compoundInfo);
                CompoundNames.Add(CompoundName);

                string biotransformerinput = ReturnBiotransformerInput(position, SMILES);
                BiotransformerData += biotransformerinput + "\n";

                using (StreamWriter sw = new StreamWriter(biotransformerFileInput, append: true))
                {
                    sw.WriteLine(biotransformerinput);
                    sw.Close();
                }
                skippedCompounds.SpreadsheetKey += SMILES + "\n";
            }
            else if (CompoundNames.Contains(CompoundName))
            {
                CompoundData compoundInfo = new CompoundData
                {
                    CompoundName = CompoundName,
                    SMILESFormat = SMILES,
                    Index = position,
                    Duplicate = true
                };
                CompoundData.Add(compoundInfo);
            }

        }
        else
        {
            CompoundData compoundInfo = new CompoundData
            {
                CompoundName = compound,
                SMILESFormat = null,
                Index = position,
                Duplicate = false
                
            };
            CompoundData.Add(compoundInfo);
            skippedCompounds.SpreadsheetKey += "\n";
        }

        yield return new WaitForEndOfFrame();
    }


    public void ClearButton()
    {
        BiotransformerStatus.text = "Status: <color=#FF4612> Waiting </color>";
        PubChemInput.text = "";
        CompoundData.Clear();
        CompoundNames.Clear();
        BiotransformerData = "";

        skippedCompounds.SpreadsheetKey = "";
        foreach (SkippedCompoundsListElement element in skippedCompounds._compounds)
            Destroy(element.gameObject);

        header = ButtonHeader.GetComponent<ButtonHeaders>();
        header.SkippedCompoundButton.gameObject.SetActive(false);

    }



    }

public class CompoundData
{
    public string CompoundName { get; set; }
    public string SMILESFormat { get; set; }
    public int Index { get; set; }
    public bool Duplicate { get; set; }

}
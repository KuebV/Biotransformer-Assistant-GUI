using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Biotransformer : MonoBehaviour
{
    public TMP_InputField PubChemInput;

    public TMP_Dropdown Metabolites;
    public TMP_Dropdown FileType;
    public TMP_InputField ListName;
    public TMP_InputField BiotransformerFile;

    public Button SkippedCompoundButton;

    private SkippedCompounds skippedCompounds;
    public GameObject SkippedCompoundPanel;

    public TMP_Text BiotransformerStatus;

    public string BiotransformerData;

    private ButtonHeaders header;
    public GameObject ButtonHeader;

    public void Awake() => BiotransformerStatus.text = "Status: <color=#FF4612> Waiting </color>";

    public void ParseCompounds()
    {
        if (string.IsNullOrEmpty(ListName.text))
            ListName.text = "list1";

        if (string.IsNullOrEmpty(BiotransformerFile.text))
            BiotransformerFile.text = "biotransformer-1.1.5.jar";

        skippedCompounds = SkippedCompoundPanel.GetComponent<SkippedCompounds>();
        skippedCompounds.SpreadsheetKey.textComponent.enableWordWrapping = false;

        CompoundData.Clear();
        CompoundNames.Clear();
        BiotransformerData = "";

        skippedCompounds.SpreadsheetKey.text = "";
        foreach (SkippedCompoundsListElement element in skippedCompounds._compounds)
            Destroy(element.gameObject);

        Arguments = new BiotransformerInput
        {
            BiotransformerFileName = BiotransformerFile.text,
            FileType = FileType.options[FileType.value].text,
            ListName = ListName.text,
            MetabolismType = Metabolites.options[Metabolites.value].text
        };

        string[] data = PubChemInput.text.Split('\n');
        SkippedCompoundButton.gameObject.SetActive(true);

        BiotransformerStatus.text = "Status: <color=#509ED8> Parsing </color>";

        for (int i = 0; i < data.Length; i++)
            StartCoroutine(FindSMILES(data[i], i));

        skippedCompounds = SkippedCompoundPanel.GetComponent<SkippedCompounds>();
        skippedCompounds.InitalizeCompoundData(CompoundData);
        skippedCompounds.PopulateList();

        BiotransformerStatus.text = "Status: <color=#006E62> Completed </color>";

    }

    public List<CompoundData> CompoundData = new List<CompoundData>();
    public List<string> CompoundNames = new List<string>();
    private BiotransformerInput Arguments;

    public void CopyToClipboard() => GUIUtility.systemCopyBuffer = BiotransformerData;
    public void AssistanceDocument() => Application.OpenURL("https://docs.google.com/document/d/1FbvyxIwNhrWRuiSscL4cNdbIBZxMrf3fKLy6LvbpqRQ/edit?usp=sharing");

    public string ReturnBiotransformerInput(int Position, string SMILE)
    {
        //Seralize String
        SMILE = SMILE.Replace("\n", "");
        string name = $"{Arguments.ListName}" + ".cmpnd" + Position + "." + Arguments.FileType.ToLower();
        string bio = "java -jar \"" + Arguments.BiotransformerFileName + "\" -k pred -b " + Arguments.MetabolismType + " -ismi \"" + SMILE + "\" -o" + Arguments.FileType.ToLower() + " " + name + " -s 1";
        return bio;
    }

    public IEnumerator FindSMILES(string compound, int position)
    {
        position++;
        string endingElement = compound.Split('\t')[1];
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

                BiotransformerData += ReturnBiotransformerInput(position, SMILES) + "\n";
                skippedCompounds.SpreadsheetKey.text += SMILES + "\n";
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
            skippedCompounds.SpreadsheetKey.text += "\n";
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

        skippedCompounds.SpreadsheetKey.text = "";
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
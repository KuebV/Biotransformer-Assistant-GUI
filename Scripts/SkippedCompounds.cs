using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class SkippedCompounds : MonoBehaviour
{
    public ScrollView SkippedCompoundView;
    public GameObject Prefab;

    public GameObject BiotransformerPanel;
    private Biotransformer biotransformer;

    public List<CompoundData> CompoundData;

    public GameObject SkippedCompoundList;
    public string SpreadsheetKey;

    public Button CopyClipboardButton;
    private string BiotransformerInput;

    public TMP_Dropdown FileType;
    public TMP_Dropdown Metabolites;
    public TMP_InputField ListName;
    public TMP_InputField BiotransformerFile;

    public TMP_Text SkippedCompoundCount;

    public APIData api;

    public void Awake()
    {
        biotransformer = BiotransformerPanel.GetComponent<Biotransformer>();
    }
    public void InitalizeCompoundData(List<CompoundData> data) => CompoundData = data;

    public List<SkippedCompoundsListElement> _compounds;

    public void PopulateList()
    {
        _compounds = new List<SkippedCompoundsListElement>();
        foreach (CompoundData cmpd in CompoundData)
        {
            if (!cmpd.Duplicate && cmpd.SMILESFormat == null)
            {
                SkippedCompoundsListElement compoundListElement = Instantiate(Prefab, SkippedCompoundList.transform).GetComponent<SkippedCompoundsListElement>();
                compoundListElement.Index.text = $"[{cmpd.Index}]";
                compoundListElement.CompoundName.text = cmpd.CompoundName;
                compoundListElement.gameObject.SetActive(true);
                compoundListElement.IndexInt = cmpd.Index;
                _compounds.Add(compoundListElement);
            }

        }

        SkippedCompoundCount.text = _compounds.Count.ToString();

    }

    public void ParseCompounds()
    {
        LoadArguments();
        foreach (SkippedCompoundsListElement element in _compounds){
            if (element.SMILE.text.Length > 1) {
                if (element.SMILE.text.Contains("LM"))
                {
                    api = GetComponent<APIData>();
                    BiotransformerInput += ReturnBiotransformerInput(element.IndexInt, api.LipidID_SMILES(element.SMILE.text));
                }
                else
                    BiotransformerInput += ReturnBiotransformerInput(element.IndexInt, element.SMILE.text) + "\n";
            }
                
        }
    }

    private BiotransformerInput Arguments;

    private void LoadArguments()
    {
        if (string.IsNullOrEmpty(ListName.text))
            ListName.text = "list1";

        if (string.IsNullOrEmpty(BiotransformerFile.text))
            BiotransformerFile.text = "biotransformer-1.1.5.jar";

        Arguments = new BiotransformerInput
        {
            BiotransformerFileName = BiotransformerFile.text,
            FileType = FileType.options[FileType.value].text,
            ListName = ListName.text,
            MetabolismType = Metabolites.options[Metabolites.value].text
        };
    }

    public void CopyCompoundList() => GUIUtility.systemCopyBuffer = SpreadsheetKey;


    private string ReturnBiotransformerInput(int Position, string SMILE)
    {
        //Seralize String
        SMILE = SMILE.Replace("\n", "");
        string name = $"{Arguments.ListName}" + ".cmpnd" + Position + "." + Arguments.FileType.ToLower();
        string bio = "java -jar \"" + Arguments.BiotransformerFileName + "\" -k pred -b " + Arguments.MetabolismType + " -ismi \"" + SMILE + "\" -o" + Arguments.FileType.ToLower() + " " + name + " -s 1";
        Debug.Log(bio);
        return bio;
    }

    public void CopyToClipboard() => GUIUtility.systemCopyBuffer = BiotransformerInput;


}

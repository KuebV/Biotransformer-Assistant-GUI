using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrefabButtonHandler : MonoBehaviour
{
    public TMP_InputField CompoundName;
    public void CopyCompoundName() => GUIUtility.systemCopyBuffer = CompoundName.text;

    public void GotoLIPIDMaps(TMP_InputField CompoundName) => Application.OpenURL($"https://www.lipidmaps.org/search/quicksearch.php?Name={CompoundName.text.Replace("\t", "")}");
    public void GotoHMDB(TMP_InputField CompoundName) => Application.OpenURL($"https://hmdb.ca/unearth/q?utf8=%E2%9C%93&query={CompoundName.text.Replace("\t", "")}%29&searcher=metabolites&button=");
}

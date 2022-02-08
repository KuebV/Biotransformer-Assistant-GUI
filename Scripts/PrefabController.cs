using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Xml.Linq;
using System.Xml;

public class PrefabController : MonoBehaviour
{
    public void OnInputChange(TMP_InputField smilesField)
    {
        string text = smilesField.text;
        if (text.StartsWith("LM"))  
            StartCoroutine(GetRequest($"https://www.lipidmaps.org/rest/compound/lm_id/{text}/smiles", smilesField));
        if (text.StartsWith("HMDB") && text.Length == 9)
            StartCoroutine(SendToHMDB($"https://hmdb.ca/metabolites/{text}", smilesField));
        if (text.StartsWith("C") && text.Length < 10)
            StartCoroutine(GetRequest($"https://www.lipidmaps.org/rest/compound/kegg_id/{text}/smiles", smilesField));

    }

    public TMP_Text Result;

    IEnumerator GetRequest(string url, TMP_InputField inputField)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    break;
                case UnityWebRequest.Result.Success:
                    if (webRequest.downloadHandler.text.Length < 5 && Result.text.Length != 26)
                        Result.text = "<color=red>No Results</color>";
                    else
                    {
                        LipidMapsAPI api = JsonUtility.FromJson<LipidMapsAPI>(webRequest.downloadHandler.text);
                        inputField.text = api.smiles;
                        Result.text = "<color=green>Found</color>";
                    }
                    
                    break;
            }

            webRequest.Dispose();

            yield return new WaitForSeconds(5f);
            Result.text = "";
        }
    }

    IEnumerator SendToHMDB(string url, TMP_InputField input)
    {
        Application.OpenURL(url);
        yield return new WaitForSeconds(1f);
        input.text = "";
    }
}

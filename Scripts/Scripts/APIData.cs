using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class APIData : MonoBehaviour
{
    public string LipidID_SMILES(string LipidMapsID)
    {
        StartCoroutine(GetRequest($"https://www.lipidmaps.org/rest/compound/lm_id/{LipidMapsID}/smiles"));
        while (string.IsNullOrEmpty(JsonData))
        {
            LipidMapsAPI api = JsonUtility.FromJson<LipidMapsAPI>(JsonData);
            return api.smiles;
        }

        return null;
        
    }

    private string JsonData;

    IEnumerator GetRequest(string url)
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
                    JsonData = webRequest.downloadHandler.text;
                    break;
            }

            webRequest.Dispose();
        }
    }
}

[Serializable]
public class LipidMapsAPI
{
    public string input;
    public string smiles;
}

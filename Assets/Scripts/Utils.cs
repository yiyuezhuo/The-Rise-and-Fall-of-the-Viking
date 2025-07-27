using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public static class Utils
{
    public static IEnumerator FetchStreamingAssetFile(string subPath, Action<string> callback)
    {
        var root = Application.streamingAssetsPath;
        var path = root + "/" + subPath;
        var request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Success: {path}");
            callback(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"failed to fetch and setup: {subPath} from {path}");
        }
    }
}
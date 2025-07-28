using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Collections;

public class ImageFetchTask
{
    public enum State
    {
        Downloading,
        Fail,
        Downloaded
    }

    public string path;
    // public UnityWebRequest request;
    public Texture2D texture;
    public State state;

    // StyleBackground _styleBackground;
    // public StyleBackground styleBackground
    // {
    //     get
    //     {
    //         if (state != State.Downloaded)
    //         {
    //             return null;
    //         }

    //         if (_styleBackground == null)
    //         {
    //             texture = DownloadHandlerTexture.GetContent(request);
    //             _styleBackground = new StyleBackground(texture);
    //         }
    //         return _styleBackground;
    //     }
    // }
    public StyleBackground styleBackground;
}

public class StreaingImageReader
{
    Dictionary<string, ImageFetchTask> taskMap = new();

    public StyleBackground FetchStyleBackground(string path)
    {
        if (!taskMap.TryGetValue(path, out var task))
        {
            task = taskMap[path] = new()
            {
                path = path,
                state = ImageFetchTask.State.Downloading,
            };
            GameManager.Instance.StartCoroutine(Request(task));
        }
        return task.styleBackground;
    }

    public IEnumerator Request(ImageFetchTask task)
    {
        using (var webRequest = UnityWebRequest.Get(task.path))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // textLoaded?.Invoke(null, webRequest.downloadHandler.text);
                task.texture = DownloadHandlerTexture.GetContent(webRequest);
                task.state = ImageFetchTask.State.Downloaded;
            }
            else
            {
                Debug.LogError("UnityWebRequest failed to get");
                task.state = ImageFetchTask.State.Fail;
            }
        }
    }
}
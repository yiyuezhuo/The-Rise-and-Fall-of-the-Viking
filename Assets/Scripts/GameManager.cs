using GameCore;
using UnityEngine;
using Unity.Properties;
using System.Collections;
using UnityEngine.Networking;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public string selectedObjectId; // May be a card or area at this point
    public bool isInEditMode = true;

    string initGameStatePath = "gameState.xml";

    [CreateProperty]
    public GameState state => CoreManager.Instance.state; // Data binding helper

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Utils.FetchStreamingAssetFile(initGameStatePath, OnInitGameStateLoaded));
    }

    void OnInitGameStateLoaded(string xml)
    {
        CoreManager.Instance.LoadFromXml(xml);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using GameCore;
using UnityEngine;
using Unity.Properties;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public string selectedObjectId; // May be a card or area at this point

    [CreateProperty]
    public Area selectedArea => EntityManager.current.Get<Area>(selectedObjectId);

    [CreateProperty]
    public bool isSelectedAreaAvailable => selectedArea != null;

    [CreateProperty]
    public Card selectedCard => EntityManager.current.Get<Card>(selectedObjectId);

    [CreateProperty]
    public bool isSelectedCardAvailable => selectedCard != null;


    string initGameStatePath = "gameState.xml";

    public enum State
    {
        Idle,
        SelectingAreaForCallback
    }

    public State state = State.Idle;

    public bool editMode = false;

    [CreateProperty]
    public bool isNotEditMode => !editMode;

    public Action<Area> oneShotAreaSelectingCallback;

    [CreateProperty]
    public GameState gameState => CoreManager.Instance.state; // Data binding helper

    [CreateProperty]
    public bool showTopMessage => topMessage != null && topMessage != "";

    [CreateProperty]
    public string topMessage
    {
        get
        {
            if (state == State.SelectingAreaForCallback)
                return "Select an area";

            if (state == State.Idle)
            {
                var gameState = GameState.current;
                if (gameState.phase == GamePhase.PlayingCard)
                {
                    if (gameState.availableCardPlay > 0)
                    {
                        return "Available card to play";
                    }
                    else
                    {
                        return "No cand to play, proceed to next phase";
                    }
                }
                if (gameState.phase == GamePhase.DoingAction)
                {
                    if (gameState.availableActionPoints > 0)
                    {
                        return "Available AP to do action";
                    }
                    else if (gameState.availableCardPlay == 0 && gameState.availableActionPoints == 0)
                    {
                        return "No AP remained, Proceed to next phase";
                    }
                }
            }
            return "";
        }
    }

    [CreateProperty]
    public GamePhase managedGamePhase
    {
        get => gameState.phase;
        set
        {
            if (editMode && value != managedGamePhase)
            {
                gameState.phase = value;
            }
        }
    }

    [CreateProperty]
    public bool managedIsFreeTransfer
    {
        get => gameState.isFreeTransfer;
        set
        {
            if (editMode)
                gameState.isFreeTransfer = value;
        }
    }

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
    public void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                var cam = PlaneCameraController.Instance.cam;
                var worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);

                var hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                if (hit.collider != null)
                {
                    Debug.Log($"Hit: {hit.collider} {hit.point}");

                    var areaViewer = hit.collider.GetComponent<AreaViewer>();
                    if (areaViewer != null)
                    {
                        Debug.Log($"Clicked areaViewer={areaViewer}");

                        if (state == State.Idle)
                        {
                            if (areaViewer.areaObjectId == selectedObjectId) // double click to enter command
                            {
                                Debug.Log("Popuping...");
                                DialogRoot.Instance.PopupAreaContextMenu();
                            }
                            else // re-select
                            {
                                selectedObjectId = areaViewer.areaObjectId;
                            }
                        }
                        else if (state == State.SelectingAreaForCallback)
                        {
                            state = State.Idle;
                            var callback = oneShotAreaSelectingCallback;
                            oneShotAreaSelectingCallback = null;
                            if (callback != null)
                            {
                                callback(EntityManager.current.Get<Area>(areaViewer.areaObjectId));
                            }
                            
                        }
                    }
                }
            }
        }
    }

    public void PrepareSelectingAreaCallback(Action<Area> callback)
    {
        oneShotAreaSelectingCallback = callback;
        state = State.SelectingAreaForCallback;
    }
}

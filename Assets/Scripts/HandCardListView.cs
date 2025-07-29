using UnityEngine;
using GameCore;
using System.Linq;

public class HandCardListView : MonoBehaviour
{
    public Transform cardContainer;
    public GameObject cardViewerPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Bind(GameState.current); // TOOD: Performance issues?
    }

    public void Bind(GameState gameState)
    {
        var handCards = gameState.handCardReferences.Select(r => r.GetCard()).ToList();
        var viewers = cardContainer.GetComponentsInChildren<CardViewer>();

        var uiLength = viewers.Length;
        var targetLength = handCards.Count;
        var diff = targetLength - uiLength;

        if (diff > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                Instantiate(cardViewerPrefab, cardContainer);
            }
        }
        else if (diff < 0)
        {
            for (int i = 0; i < -diff; i++)
            {
                Destroy(viewers[uiLength - i - 1].gameObject);
            }
        }

        viewers = cardContainer.GetComponentsInChildren<CardViewer>();
        for (int i = 0; i < targetLength; i++)
        {
            var card = handCards[i];
            var viewer = viewers[i];
            viewer.Bind(card);
        }
    }
}

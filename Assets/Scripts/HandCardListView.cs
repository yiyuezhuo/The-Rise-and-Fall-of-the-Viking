using UnityEngine;
using GameCore;
using System.Linq;
using System.Collections.Generic;

public class AbstractCardListView : MonoBehaviour
{
    public Transform cardContainer;
    public GameObject cardViewerPrefab;

    public void BindCardReferences(List<CardReference> cardReferences)
    {
        var cards = cardReferences.Select(r => r.GetCard()).ToList();
        var viewers = cardContainer.GetComponentsInChildren<CardViewer>();

        var uiLength = viewers.Length;
        var targetLength = cards.Count;
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
            var card = cards[i];
            var viewer = viewers[i];
            viewer.Bind(card);
        }
    }

}

public class HandCardListView : AbstractCardListView
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        BindCardReferences(GameState.current.handCardReferences);
        // Bind(GameState.current); // TOOD: Performance issues?
    }

    // public void Bind(GameState gameState)
    // {
    //     var cards = gameState.handCardReferences.Select(r => r.GetCard()).ToList();
    //     var viewers = cardContainer.GetComponentsInChildren<CardViewer>();

    //     var uiLength = viewers.Length;
    //     var targetLength = cards.Count;
    //     var diff = targetLength - uiLength;

    //     if (diff > 0)
    //     {
    //         for (int i = 0; i < diff; i++)
    //         {
    //             Instantiate(cardViewerPrefab, cardContainer);
    //         }
    //     }
    //     else if (diff < 0)
    //     {
    //         for (int i = 0; i < -diff; i++)
    //         {
    //             Destroy(viewers[uiLength - i - 1].gameObject);
    //         }
    //     }

    //     viewers = cardContainer.GetComponentsInChildren<CardViewer>();
    //     for (int i = 0; i < targetLength; i++)
    //     {
    //         var card = cards[i];
    //         var viewer = viewers[i];
    //         viewer.Bind(card);
    //     }
    // }
}

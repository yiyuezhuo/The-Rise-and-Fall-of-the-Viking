using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameCore;
using UnityEngine.EventSystems;

public class CardViewer : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text cardName;
    public TMP_Text actionPoint;
    public Image cardImage;
    public TMP_Text cardDescription;

    public Card currentCard;

    public void Bind(Card card)
    {
        currentCard = card;
        Refresh();
    }

    public void Refresh()
    {
        var cls = currentCard.cardClass;

        cardName.text = cls.name;
        actionPoint.text = cls.actionPoints.ToString();
        cardImage.sprite = UnityWebRequestImageReader.Instance.FetchSprite(Application.streamingAssetsPath + "/" + cls.imagePath);
        cardDescription.text = cls.cardDescription; // TODO: Switch to Card Event Description?
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{currentCard} was clicked!");

        if (GameManager.Instance.selectedObjectId != currentCard.objectId) // First click => select
        {
            GameManager.Instance.selectedObjectId = currentCard.objectId;
        }
        else //  Double click => open the context menu
        {
            DialogRoot.Instance.PopupCardContextMenu();
        }
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

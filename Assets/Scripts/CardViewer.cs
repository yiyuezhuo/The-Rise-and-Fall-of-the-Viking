using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GameCore;

public class CardViewer : MonoBehaviour
{
    public TMP_Text cardName;
    public TMP_Text actionPoint;
    public Image cardImage;
    public TMP_Text cardDescription;

    public void Bind(Card card)
    {
        var cls = card.cardClass;
        cardName.text = cls.name;
        actionPoint.text = cls.actionPoints.ToString();
        cardImage.sprite = UnityWebRequestImageReader.Instance.FetchSprite(Application.streamingAssetsPath + "/" + cls.imagePath);
        cardDescription.text = cls.cardDescription; // TODO: Switch to Card Event Description?
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

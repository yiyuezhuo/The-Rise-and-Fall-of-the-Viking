using UnityEngine;
using TMPro;
using GameCore;
using System;

public class AreaViewer : MonoBehaviour
{
    public string areaObjectId;

    public TMP_Text vikingResourceText;
    public TMP_Text hostResourceText;
    public TMP_Text occupationText;
    public TMP_Text christianizationText;

    public GameObject vikingResourceField;
    public GameObject hostResourceField;
    public GameObject occupationField;
    public GameObject christianizationField;

    public SpriteRenderer boxSpriteRenderer;

    public void Update()
    {
        var area = EntityManager.current.Get<Area>(areaObjectId);

        if (area != null)
        {
            vikingResourceText.text = area.vikingResources.ToString();
            hostResourceText.text = area.hostResources.ToString();
            occupationText.text = area.vikingOccupyingPercent.ToString("P0");
            christianizationText.text = area.vikingChristianization.ToString("P0");

            hostResourceField.SetActive(!area.isVikingHomeland);
            vikingResourceField.SetActive(area.vikingZoneCreated);
            occupationField.SetActive(area.vikingZoneCreated && !area.isVikingHomeland && !area.isColony);
            christianizationField.SetActive(area.vikingZoneCreated);

            var occupyingPercent = area.vikingOccupyingPercent <= 0  ? 0 : Mathf.Lerp(0.2f, 1f, area.vikingOccupyingPercent);
            if (area.isColony && area.vikingResources > 0)
                occupyingPercent = 1;
            
            var christianization = (1 - occupyingPercent) * 1 + occupyingPercent * area.vikingChristianization;

            boxSpriteRenderer.color = new Color(1, christianization, 1 - occupyingPercent);
        }
    }
}
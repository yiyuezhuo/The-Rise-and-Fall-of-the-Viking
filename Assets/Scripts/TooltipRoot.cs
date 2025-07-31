using UnityEngine.UIElements;
using System.Collections.Generic;

public class TooltipRoot : SingletonDocument<TooltipRoot>
{
    Label tooltipLabel;
    List<VisualElement> processingElements = new();

    protected override void Awake()
    {
        base.Awake();

        tooltipLabel = root.Q<Label>("TooltipLabel");
        tooltipLabel.style.display = DisplayStyle.None;
    }

    public void ShowTooltip(VisualElement target)
    {
        processingElements.Add(target);
        Sync();
    }

    public void HideTooltip(VisualElement target)
    {
        processingElements.Remove(target);
        Sync();
    }

    void Sync()
    {
        if (processingElements.Count == 0)
        {
            tooltipLabel.style.display = DisplayStyle.None;
        }
        else
        {
            var target = processingElements[^1];

            tooltipLabel.style.position = Position.Absolute;
            tooltipLabel.style.left = target.worldBound.center.x;
            tooltipLabel.style.top = target.worldBound.yMin;
            tooltipLabel.text = target.tooltip;

            tooltipLabel.style.display = DisplayStyle.Flex;
        }
    }
}
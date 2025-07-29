using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine.UIElements;
using UnityEngine;

public class TempDialog
{
    public VisualElement root;
    public VisualTreeAsset template;
    public object templateDataSource;
    public event EventHandler<VisualElement> onCreated;
    public event EventHandler<VisualElement> onConfirmed;
    public event EventHandler<VisualElement> onCancelled;

    static Dictionary<VisualTreeAsset, (StyleLength, StyleLength)> memorizedLeftTop = new();
    // static Dictionary<VisualTreeAsset, IStyle> memorizedStyle = new();

    public VisualElement el;

    public enum PositionMode
    {
        Centering,
        MousePosition,
        Memorizing
    }

    public bool draggable = false;

    public PositionMode positionMode = PositionMode.Centering;

    public bool singleton = false;
    static Dictionary<VisualTreeAsset, TempDialog> singletonMap = new();

    public enum SingletonTag
    {
        None,
        ContextMenu
    }

    public SingletonTag singletonTag = SingletonTag.None;
    static Dictionary<SingletonTag, TempDialog> singletonTagMap = new();

    public void Popup()
    {
        el = template.CloneTree();
        el.dataSource = templateDataSource;

        onCreated?.Invoke(this, el);

        var confirmButton = el.Q<Button>("ConfirmButton");
        var cancelButton = el.Q<Button>("CancelButton");

        Utils.BindItemsSourceRecursive(el);

        root.Add(el);

        if (confirmButton != null)
        {
            confirmButton.clicked += () =>
            {
                RemoveSelf();

                onConfirmed?.Invoke(this, el);
            };
        }

        if (cancelButton != null)
        {
            cancelButton.clicked += () =>
            {
                RemoveSelf();

                onCancelled?.Invoke(this, el);
            };
        }

        if (positionMode == PositionMode.Centering)
        {
            Utils.AbsoluteCenteringElement(el);
        }
        else if (positionMode == PositionMode.MousePosition)
        {
            Utils.SetMousePosition(el);
        }
        else if (positionMode == PositionMode.Memorizing)
        {
            if (memorizedLeftTop.TryGetValue(template, out var leftTop))
            {
                var (left, top) = leftTop;
                // Utils.SetAbsoluteXY(el, px, py, true);
                el.style.position = Position.Absolute;
                el.style.left = left;
                el.style.top = top;
                el.style.translate = new StyleTranslate(
                    new Translate(
                        new Length(-50, LengthUnit.Percent),
                        new Length(-50, LengthUnit.Percent)
                    )
                );
            }
            else
            {
                Utils.AbsoluteCenteringElement(el);
            }
        }

        if (draggable)
        {
            el.AddManipulator(new MyDragger());
        }

        if (singleton)
        {
            if (singletonMap.TryGetValue(template, out var oldDialog))
            {
                oldDialog.RemoveSelf();
            }
            singletonMap[template] = this;
        }

        if (singletonTag != SingletonTag.None)
        {
            if (singletonTagMap.TryGetValue(singletonTag, out var oldDialog))
            {
                oldDialog.RemoveSelf();
            }
            singletonTagMap[singletonTag] = this;
        }
    }

    public void RemoveSelf()
    {
        if (positionMode == PositionMode.Memorizing)
        {
            // var px = el.style.left.value.value / Screen.width;
            // var py = el.style.top.value.value / Screen.height;
            // Debug.Log($"px={px},py={py},el.style.left.value.value={el.style.left.value.value},el.style.top.value.value={el.style.top.value.value}");
            memorizedLeftTop[template] = (el.style.left, el.style.top);
        }

        if (singleton && singletonMap.ContainsKey(template))
        {
            singletonMap.Remove(template);
        }

        if (singletonTag != SingletonTag.None && singletonTagMap.ContainsKey(singletonTag))
        {
            singletonTagMap.Remove(singletonTag);
        }

        root.Remove(el);
    }
}
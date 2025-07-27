using System;
using System.Numerics;
using UnityEngine.UIElements;

public class TempDialog
{
    public VisualElement root;
    public VisualTreeAsset template;
    public object templateDataSource;
    public event EventHandler<VisualElement> onCreated;
    public event EventHandler<VisualElement> onConfirmed;
    public event EventHandler<VisualElement> onCancelled;

    public VisualElement el;

    public enum PositionMode
    {
        Centering,
        MousePosition
    }

    public bool draggable = false;

    public PositionMode positionMode = PositionMode.Centering;

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

        if (draggable)
        {
            root.AddManipulator(new MyDragger());
        }
    }

    public void RemoveSelf()
    {
        root.Remove(el);
    }
}
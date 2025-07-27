using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using GameCore;


public class DialogRoot : SingletonDocument<DialogRoot>
{
    public VisualTreeAsset areaContextMenuTemplate;
    public VisualTreeAsset areaEditorTemplate;

    public void PopupAreaContextMenu()
    {
        var tempDialog = new TempDialog()
        {
            root = root,
            template = areaContextMenuTemplate,
            templateDataSource = GameManager.Instance, // GameManager.Instance
            positionMode = TempDialog.PositionMode.MousePosition
        };

        tempDialog.onCreated += (sender, el) =>
        {
            el.Q<Button>("EditButton").clicked += () =>
            {
                PopupAreaEditor();
                ((TempDialog)sender).RemoveSelf();
            };
        };

        tempDialog.Popup();
    }

    public void PopupAreaEditor()
    {
        var area = EntityManager.current.Get<Area>(GameManager.Instance.selectedObjectId);
        var tempDialog = new TempDialog()
        {
            root = root,
            template = areaEditorTemplate,
            templateDataSource = area, // GameManager.Instance
            positionMode = TempDialog.PositionMode.Centering,
            draggable = true
        };

        tempDialog.Popup();

    }
}
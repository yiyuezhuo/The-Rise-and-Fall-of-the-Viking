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
            positionMode = TempDialog.PositionMode.MousePosition,
            singleton = true
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
            // positionMode = TempDialog.PositionMode.Centering,
            positionMode = TempDialog.PositionMode.Memorizing,
            draggable = true
        };

        tempDialog.onCreated += (sender, el) =>
        {
            var neighborsMultiColumnListView = el.Q<MultiColumnListView>("NeighborsMultiColumnListView");
            Utils.BindItemsAddedRemoved<AreaReference>(neighborsMultiColumnListView, () => null);
            var setNeighborButtonCol = neighborsMultiColumnListView.columns["setNeighborButton"];
            setNeighborButtonCol.makeCell = () =>
            {
                var el = setNeighborButtonCol.cellTemplate.CloneTree();
                el.Q<Button>().clicked += () =>
                {
                    if (Utils.TryResolveCurrentValueForBinding(el, out AreaReference areaReference))
                    {
                        GameManager.Instance.PrepareSelectingAreaCallback(selectingArea =>
                        {
                            areaReference.objectId = selectingArea.objectId;
                        });
                    }
                };
                return el;
            };

            el.Q<Button>("SetLordButton").clicked += () =>
            {
                GameManager.Instance.PrepareSelectingAreaCallback(selectingArea =>
                {
                    if (Utils.TryResolveCurrentValueForBinding(el, out Area currentArea))
                    {
                        currentArea.lord.objectId = selectingArea.objectId;
                    }
                });
            };
        };

        tempDialog.Popup();

    }
}
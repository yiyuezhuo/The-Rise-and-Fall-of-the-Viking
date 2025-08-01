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
    public VisualTreeAsset cardContextMenuTemplate;
    public VisualTreeAsset areaEditorTemplate;
    public VisualTreeAsset messageDialogTemplate;
    public VisualTreeAsset confirmDialogTemplate;
    public VisualTreeAsset fromToAreaReferenceParameterTemplate;
    public VisualTreeAsset resourceAssignParameterTemplate;

    public void PopupFromToAreaReferenceParameterDialog(FromToAreaReferenceParameter p, string title, Action<FromToAreaReferenceParameter> callback)
    {
        var tempDialog = new TempDialog()
        {
            root = root,
            template = fromToAreaReferenceParameterTemplate,
            templateDataSource = p,
            draggable = true
        };

        tempDialog.onCreated += (sender, el) =>
        {
            BindSetButtons(el);
            el.Q<Label>("TitleLabel").text = title;
        };

        tempDialog.onConfirmed += (sender, el) => callback(p);

        tempDialog.Popup();
    }

    void BindSetButtons(VisualElement el)
    {
        foreach (var setButton in el.Query<Button>("SetButton").ToList())
        {
            setButton.clicked += () =>
            {
                GameManager.Instance.PrepareSelectingAreaCallback(area =>
                {
                    if (Utils.TryResolveCurrentValueForBinding(setButton, out AreaReference areaReference))
                    {
                        areaReference.objectId = area.objectId;
                    }
                });
            };
        }
    }

    public void PopupResourceAssignParameterDialog(ResourceAssignParameter p, string title, Action<ResourceAssignParameter> callback)
    {
        var tempDialog = new TempDialog()
        {
            root = root,
            template = resourceAssignParameterTemplate,
            templateDataSource = p,
            draggable = true
        };

        // TODO: Bind "Set" buttons

        tempDialog.onCreated += (sender, el) =>
        {
            BindSetButtons(el);
            el.Q<Label>("TitleLabel").text = title;
        };

        tempDialog.onConfirmed += (sender, el) => callback(p);

        tempDialog.Popup();
    }

    public void PopupMessageDialog(string message, string title = "Message")
    {
        var tempDialog = new TempDialog()
        {
            root = root,
            template = messageDialogTemplate,
            draggable = true
        };

        tempDialog.onCreated += (sender, el) =>
        {
            el.Q<Label>("TitleLabel").text = title;
            el.Q<TextField>("ContentTextField").SetValueWithoutNotify(message);
        };

        tempDialog.Popup();
    }

    public void PopupConfirmDialog(string message, Action confirmCallback, string title = "Confirm")
    {
        var tempDialog = new TempDialog()
        {
            root = root,
            template = confirmDialogTemplate,
            draggable = true,
        };

        tempDialog.onCreated += (sender, el) =>
        {
            el.Q<Label>("TitleLabel").text = title;
            el.Q<TextField>("ContentTextField").SetValueWithoutNotify(message);
        };

        tempDialog.onConfirmed += (sender, el) => confirmCallback();

        tempDialog.Popup();
    }

    public void PopupAreaContextMenu()
    {
        var tempDialog = new TempDialog()
        {
            root = root,
            template = areaContextMenuTemplate,
            templateDataSource = GameManager.Instance, // GameManager.Instance
            positionMode = TempDialog.PositionMode.MousePosition,
            singletonTag = TempDialog.SingletonTag.ContextMenu
        };

        tempDialog.onCreated += (sender, el) =>
        {
            el.Q<Button>("EditButton").clicked += () =>
            {
                ((TempDialog)sender).RemoveSelf();

                PopupAreaEditor();
            };

            el.Q<Button>("CounterInfluenceButton").clicked += () =>
            {
                ((TempDialog)sender).RemoveSelf();

                var selectedArea = GameManager.Instance.selectedArea;
                if (selectedArea != null)
                {
                    GameState.current.DoCounterInfluence(selectedArea);
                }
            };

            el.Q<Button>("TradeButton").clicked += () =>
            {
                ((TempDialog)sender).RemoveSelf();

                GameManager.Instance.PrepareSelectingAreaCallback(area =>
                {
                    var selectedArea = GameManager.Instance.selectedArea;
                    if (selectedArea == null)
                        return;

                    var p = new FromToAreaReferenceParameter()
                    {
                        from = new AreaReference() { objectId = selectedArea.objectId },
                        to = new AreaReference() { objectId = area.objectId },
                    };

                    PopupFromToAreaReferenceParameterDialog(p, "Trade", p =>
                    {
                        GameState.current.DoTrade(p);
                    });
                });
            };

            el.Q<Button>("ColonizationButton").clicked += () =>
            {
                ((TempDialog)sender).RemoveSelf();

                GameManager.Instance.PrepareSelectingAreaCallback(area =>
                {
                    var selectedArea = GameManager.Instance.selectedArea;
                    if (selectedArea == null)
                        return;

                    var p = new FromToAreaReferenceParameter()
                    {
                        from = new AreaReference() { objectId = selectedArea.objectId },
                        to = new AreaReference() { objectId = area.objectId },
                    };

                    PopupFromToAreaReferenceParameterDialog(p, "Colonization", p =>
                    {
                        GameState.current.DoColonization(p);
                    });
                });
            };

            el.Q<Button>("RaidButton").clicked += () =>
            {
                ((TempDialog)sender).RemoveSelf();

                GameManager.Instance.PrepareSelectingAreaCallback(area =>
                {
                    var selectedArea = GameManager.Instance.selectedArea;
                    if (selectedArea == null)
                        return;

                    var p = new ResourceAssignParameter()
                    {
                        from = new AreaReference() { objectId = selectedArea.objectId },
                        to = new AreaReference() { objectId = area.objectId },
                        assignResourceLimit = selectedArea.GetRaidAssignedResourceLimit(),
                        assignResource = selectedArea.GetRaidAssignedResourceLimit()
                    };

                    PopupResourceAssignParameterDialog(p, "Raid", p =>
                    {
                        GameState.current.DoRaid(p);
                    });
                });
            };

            el.Q<Button>("ConquerButton").clicked += () =>
            {
                ((TempDialog)sender).RemoveSelf();

                GameManager.Instance.PrepareSelectingAreaCallback(area =>
                {
                    var selectedArea = GameManager.Instance.selectedArea;
                    if (selectedArea == null)
                        return;

                    var p = new ResourceAssignParameter()
                    {
                        from = new AreaReference() { objectId = selectedArea.objectId },
                        to = new AreaReference() { objectId = area.objectId },
                        assignResourceLimit = selectedArea.GetConquerAssignedResourceLimit(),
                        assignResource = selectedArea.GetConquerAssignedResourceLimit()
                    };

                    PopupResourceAssignParameterDialog(p, "Conquer", p =>
                    {
                        GameState.current.DoConquer(p);
                    });
                });
            };
        };


        tempDialog.Popup();
    }

    public void PopupCardContextMenu()
    {
        var tempDialog = new TempDialog()
        {
            root = root,
            template = cardContextMenuTemplate,
            templateDataSource = GameManager.Instance, // GameManager.Instance
            positionMode = TempDialog.PositionMode.MousePosition,
            singletonTag = TempDialog.SingletonTag.ContextMenu
        };

        tempDialog.onCreated += (sender, el) =>
        {
            el.Q<Button>("PlayActionPointButton").clicked += () =>
            {
                ((TempDialog)sender).RemoveSelf();

                var selectedCard = GameManager.Instance.selectedCard;
                var gameState = GameState.current;
                if (selectedCard != null)
                {
                    gameState.PlayCardForActionPoint(selectedCard);
                }
            };

            el.Q<Button>("PlayEventEffectButton").clicked += () =>
            {
                ((TempDialog)sender).RemoveSelf();

                var selectedCard = GameManager.Instance.selectedCard;
                var gameState = GameState.current;
                if (selectedCard != null)
                {
                    gameState.PlayCardForEventEffect(selectedCard);
                }
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
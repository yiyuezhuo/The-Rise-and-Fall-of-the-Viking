using UnityEngine;
using UnityEngine.UIElements;

using GameCore;


public class Overlay : SingletonDocument<Overlay>
{
    public void OnEnable()
    {
        root.dataSource = GameManager.Instance;

        Utils.BindItemsSourceRecursive(root);

        root.Q<Button>("SaveButton").clicked += () =>
        {
            Debug.Log("SaveButton clicked");

            var state = CoreManager.Instance.state;
            var xml = XmlUtils.ToXML(state);
            IOManager.Instance.SaveTextFile(xml, "gameState", "xml");
        };

        root.Q<Button>("LoadButton").clicked += () =>
        {
            Debug.Log("LoadButton clicked");

            IOManager.Instance.LoadTextFile(CoreManager.Instance.LoadFromXml, "xml");
        };

        root.Q<Button>("ExitButton").clicked += Application.Quit;

        root.Q<Button>("NextPhaseButton").clicked += () =>
        {
            Debug.Log("NextPhaseButton clicked");

            var gameState = GameState.current;

            if (gameState.phase == GamePhase.PlayingCard && gameState.availableCardPlay > 0)
            {
                DialogRoot.Instance.PopupConfirmDialog("It's possible to play more card, confirm to continue?", gameState.NextPhase);
            }
            else if (gameState.phase == GamePhase.DoingAction && gameState.availableActionPoints > 0)
            {
                DialogRoot.Instance.PopupConfirmDialog("There're unused action point, confirm to continue?", gameState.NextPhase);
            }
            else
            {
                gameState.NextPhase();
            }
        };

        root.Q<Button>("OpenSourceRepoButton").clicked += () =>
        {
            Application.OpenURL("https://github.com/yiyuezhuo/The-Rise-and-Fall-of-the-Viking");
        };

        root.Q<Button>("HelpButton").clicked += () =>
        {
            DialogRoot.Instance.PopupMessageDialog("Help WIP \n 114514");
        };

        var userLogListView = root.Q<ListView>("UserLogListView");

        root.Q<Button>("ExpandCollapseUserLogButton").clicked += () =>
        {
            var displayStyle = userLogListView.style.display;
            if (displayStyle == DisplayStyle.None)
            {
                userLogListView.style.display = DisplayStyle.Flex;
            }
            else
            {
                userLogListView.style.display = DisplayStyle.None;
            }
        };

        // userLogListView.bindItem = (item, index) =>
        // {
        //     var userLogs = GameState.current.userLogs;
        //     var indexReversed = userLogs.Count - index - 1;
        //     Debug.Log($"bindItem: {item}, {index}, {indexReversed}");
        //     item.dataSource = userLogs[indexReversed]; // reverse order binding
        // };
    }
}
using UnityEngine;
using UnityEngine.UIElements;

using GameCore;


public class Overlay : SingletonDocument<Overlay>
{
    public void OnEnable()
    {
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

        root.Q<Button>("NextPhaseButton").clicked += () =>
        {
            Debug.Log("NextPhaseButton clicked");

            CoreManager.Instance.state.NextPhase();
        };
    }
}
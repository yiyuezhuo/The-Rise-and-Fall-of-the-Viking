using UnityEngine;
using UnityEditor;

using GameCore;
using System;
using System.IO;

public static class MenuItems
{
    [MenuItem("Custom/Build Default Default Data And Bind Unity Viewer")]
    public static void BuildDefaultDefaultDataAndBindUnityViewer()
    {
        var state = new GameState();
        foreach (var areaViewer in UnityEngine.Object.FindObjectsByType<AreaViewer>(FindObjectsSortMode.None))
        {
            Debug.Log($"areaViewer={areaViewer}");

            var guid = System.Guid.NewGuid().ToString();
            areaViewer.areaObjectId = guid;

            EditorUtility.SetDirty(areaViewer);
            var area = new Area()
            {
                objectId = guid,
                name = areaViewer.name
            };
            state.areas.Add(area);
        }

        var xml = XmlUtils.ToXML(state);
        var path = EditorUtility.SaveFilePanel("Save built game state", "streamingAsset", "gameState", "xml");
        File.WriteAllText(path, xml);
    }
} 
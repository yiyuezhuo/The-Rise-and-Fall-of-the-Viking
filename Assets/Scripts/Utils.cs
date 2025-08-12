using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.Properties;

using GameCore;
using UnityEngine.InputSystem;

public static class Utils
{
    public static IEnumerator FetchStreamingAssetFile(string subPath, Action<string> callback)
    {
        var root = Application.streamingAssetsPath;
        var path = root + "/" + subPath;
        var request = UnityWebRequest.Get(path);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"Success: {path}");
            callback(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"failed to fetch and setup: {subPath} from {path}");
        }
    }

    public static Action<IEnumerable<int>> MakeCallbackForItemsAdded<T>(BaseListView listView, Func<object> parentProvider) where T : new()
    {
        return (IEnumerable<int> index) =>
        {
            foreach (var i in index)
            {
                var v = listView.itemsSource[i];
                if (v == null)
                {
                    var obj = new T();
                    listView.itemsSource[i] = obj;

                    if (obj is IObjectIdLabeled labeledObj)
                    {
                        EntityManager.current.Register(labeledObj, parentProvider());
                    }
                }
            }
        };
    }

    public static Action<IEnumerable<int>> MakeCallbackForItemsRemoved(BaseListView listView)
    {
        return (IEnumerable<int> index) =>
        {
            foreach (var i in index)
            {
                var v = listView.itemsSource[i];
                if (v is IObjectIdLabeled labeledObj)
                {
                    EntityManager.current.Unregister(labeledObj);
                }
            }
        };
    }

    public static void BindItemsAddedRemoved<T>(BaseListView listView, Func<object> parentProvider) where T : new()
    {
        listView.itemsAdded += MakeCallbackForItemsAdded<T>(listView, parentProvider);
        listView.itemsRemoved += MakeCallbackForItemsRemoved(listView);
    }

    public static void BindItemsSourceRecursive(VisualElement root)
    {
        foreach (var listView in root.Query<BaseListView>().ToList())
        {
            listView.SetBinding("itemsSource", new DataBinding());
        }
    }

    public static void AbsoluteCenteringElement(VisualElement el)
    {
        el.style.position = Position.Absolute;
        el.style.left = new Length(50, LengthUnit.Percent);
        el.style.top = new Length(50, LengthUnit.Percent);
        el.style.translate = new StyleTranslate(
            new Translate(
                new Length(-50, LengthUnit.Percent),
                new Length(-50, LengthUnit.Percent)
            )
        );
    }

    public static void SetAbsoluteXY(VisualElement el, float px, float py)
    {
        el.style.position = Position.Absolute;
        el.style.left = new Length(px * 100, LengthUnit.Percent); // Pixel mode is subject to scale mode
        el.style.top = new Length(py * 100, LengthUnit.Percent);
        // if (centering)
        // {
        //     el.style.translate = new StyleTranslate(
        //         new Translate(
        //             new Length(-50, LengthUnit.Percent),
        //             new Length(-50, LengthUnit.Percent)
        //         )
        //     );
        // }
    }

    public static void SetMousePosition(VisualElement el)
    {
        // var pos = DialogRoot.Instance.root.WorldToLocal(Input.mousePosition);
        // var pos = new Vector3(Input.mousePosition.x / Screen.width, (Screen.height - Input.mousePosition.y) / Screen.height, 0);
        var mousePosition = Utils.mousePosition;
        var pos = new Vector3(mousePosition.x / Screen.width, (Screen.height - mousePosition.y) / Screen.height, 0);
        // var pos = Input.mousePosition;
        // var pos = PlaneCameraController.Instance.cam.WorldToViewportPoint(Input.mousePosition);
        // var pos = PlaneCameraController.Instance.cam.WorldToScreenPoint(Input.mousePosition);
        // Debug.Log($"pos={pos}");
        SetAbsoluteXY(el, pos.x, pos.y);
        // SetAbsoluteXY(el, 100, 100);
        // SetAbsoluteXY(el, Input.mousePosition.x, Screen.height - Input.mousePosition.y);
    }

    public static bool TryResolveCurrentValueForBinding<T>(VisualElement el, out T ret) where T : class
    {
        var ctx = el.GetHierarchicalDataSourceContext();
        var succ = PropertyContainer.TryGetValue(ctx.dataSource, ctx.dataSourcePath, out ret);
        if (!succ && ctx.dataSourcePath.Length == 0)
        {
            ret = (T)ctx.dataSource;
            succ = true;
        }
        return succ;
    }

    public static Vector3 mousePosition
    {
        get
        {
            return Input.mousePosition;

            // return Mouse.current.position.ReadValue();

            // if (Mouse.current != null)
            // {
            //     return Mouse.current.position.ReadValue();
            // }
            // else if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            // {
            //     return Touchscreen.current.primaryTouch.position.ReadValue();
            // }
            // Debug.LogError("mousePosition is not available.");
            // return Vector2.zero;
        }
    }
}
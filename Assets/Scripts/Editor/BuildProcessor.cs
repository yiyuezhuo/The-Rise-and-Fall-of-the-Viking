using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UIElements;


public class BuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 100; } }


    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Checking MultiColumnListView integrity...");
        CheckMultiColumnListViewBlockOnly();
    }

    public void CheckMultiColumnListViewBlockOnly()
    {
        string[] uxmlGuids = AssetDatabase.FindAssets("t:VisualTreeAsset");
        foreach (var uxmlGuid in uxmlGuids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(uxmlGuid);

            if (!assetPath.StartsWith("Assets/"))
                continue;

            VisualTreeAsset vta = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);

            var el = vta.CloneTree();
            var multiColumnListViews = el.Query<MultiColumnListView>().ToList();
            if (multiColumnListViews.Count > 0)
            {
                foreach (var mclv in multiColumnListViews)
                {
                    foreach (var col in mclv.columns)
                    {
                        if (col.cellTemplate == null)
                        {
                            throw new BuildFailedException($"cellTemplate missing: col.title={col.title}, mclv.name={mclv.name}, vta.name={vta.name} (assetPath={assetPath}, uxmlGuid={uxmlGuid})");
                        }
                    }
                }
            }
        }
    }

    [MenuItem("Custom/Check MultiColumnListView cellTemplate missing")]
    public static void CheckMultiColumnListView()
    {
        string[] uxmlGuids = AssetDatabase.FindAssets("t:VisualTreeAsset");
        foreach (var uxmlGuid in uxmlGuids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(uxmlGuid);

            if (!assetPath.StartsWith("Assets/"))
                continue;

            VisualTreeAsset vta = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(assetPath);

            // Debug.Log($"vta.name={vta.name}");

            var el = vta.CloneTree();
            var multiColumnListViews = el.Query<MultiColumnListView>().ToList();
            if (multiColumnListViews.Count > 0)
            {
                Debug.Log($"vta.name={vta.name} (assetPath={assetPath}, uxmlGuid={uxmlGuid})");
                foreach (var mclv in multiColumnListViews)
                {
                    Debug.Log($"mclv.name={mclv.name}");
                    var hasMissing = false;
                    foreach (var col in mclv.columns)
                    {
                        if (col.cellTemplate != null)
                        {
                            Debug.Log($"col.title={col.title}, col.cellTemplate.name={col.cellTemplate.name}");
                        }
                        else
                        {
                            Debug.LogWarning($"cellTemplate missing: col.title={col.title}, col.cellTemplate={col.cellTemplate}");
                            hasMissing = true;
                        }
                    }
                    if (hasMissing)
                    {
                        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.Default);
                        Debug.LogWarning("Reimporting...");
                    }
                }
            }
        }
    }
    
}
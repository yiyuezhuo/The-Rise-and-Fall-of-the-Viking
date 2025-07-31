using UnityEngine;
using UnityEngine.UIElements;

public class TooltipManager : MonoBehaviour
{
    public Transform docRoot;

    void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var sum = 0;
        foreach(var doc in docRoot.GetComponentsInChildren<UIDocument>())
        {
            var root = doc.rootVisualElement;
            sum += RecursiveApply(root);
        }
        Debug.Log($"Tooltip count: {sum}");
    }

    int RecursiveApply(VisualElement el)
    {
        var sum = 0;
        if(el.tooltip != null && el.tooltip != "")
        {
            el.AddManipulator(new TooltipManipulator());
            sum += 1;
        }
        foreach(var child in el.Children())
        {
            sum += RecursiveApply(child);
        }
        return sum;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

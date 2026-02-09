using System.Linq;

using UnityEditor;

using UnityEngine;

public class Anchor
{
    [MenuItem("GameObject/Adjust RectTransform Anchors %l")]
    public static void Adjust()
    {
        var rects = Selection.gameObjects.Select(obj => obj.GetComponent<RectTransform>()).ToArray();
        Undo.RecordObjects(rects, "Adjust RectTransform Anchors");

        foreach (GameObject gameObject in Selection.gameObjects)
            AdjustRectTransform(gameObject);
    }

    private static void AdjustRectTransform(GameObject gameObject)
    {
        var transform = gameObject.GetComponent<RectTransform>();
        if (transform == null || transform.parent == null)
            return;

        var parentRect = transform.parent.GetComponent<RectTransform>().rect;
        var parentSize = new Vector2(parentRect.width, parentRect.height);

        var originalOffsetMin = transform.offsetMin;
        var originalOffsetMax = transform.offsetMax;

        var posMin = new Vector2(parentSize.x * transform.anchorMin.x, parentSize.y * transform.anchorMin.y) + originalOffsetMin;
        var posMax = new Vector2(parentSize.x * transform.anchorMax.x, parentSize.y * transform.anchorMax.y) + originalOffsetMax;

        posMin = new Vector2(Round(posMin.x / parentSize.x, 3), Round(posMin.y / parentSize.y, 3));
        posMax = new Vector2(Round(posMax.x / parentSize.x, 3), Round(posMax.y / parentSize.y, 3));

        transform.anchorMin = posMin;
        transform.anchorMax = posMax;

        transform.offsetMin = new Vector2(0f, 0f);
        transform.offsetMax = new Vector2(0f, 0f);
    }

    private static float Round(float number, int factor)
    {
        return Mathf.Round(number * Mathf.Pow(10, factor)) / Mathf.Pow(10, factor);
    }
}
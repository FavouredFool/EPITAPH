using UnityEngine;

public static class LayerUtil
{
    public static bool MaskContainsLayer(LayerMask mask, int layer)
    {
        return (mask & (1 << layer)) != 0;
    }

    public static int ExtractLayerFromMask(LayerMask mask)
    {
        // this will offer wrong results when you put in a mask with multiple layers
        return Mathf.RoundToInt(Mathf.Log(mask.value, 2));
    }
    
    public static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}

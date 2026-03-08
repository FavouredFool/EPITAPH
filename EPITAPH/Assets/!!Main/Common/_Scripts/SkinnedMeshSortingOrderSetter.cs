using System;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class SkinnedMeshSortingOrderSetter : MonoBehaviour
{
    [SerializeField] int _sortingLayerOrder;


    void Awake()
    {
        GetComponent<SkinnedMeshRenderer>().sortingOrder = _sortingLayerOrder;
    }
}

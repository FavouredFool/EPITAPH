using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class MeshSortingOrderSetter : MonoBehaviour
{
    [SerializeField] int _sortingLayerOrder;


    void Awake()
    {
        GetComponent<MeshRenderer>().sortingOrder = _sortingLayerOrder;
    }
}

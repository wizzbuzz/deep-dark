using System;
using UnityEngine;

public class IgnoreLayer : MonoBehaviour
{
    [SerializeField]
    private LayerMask mask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Hide()
    {
        gameObject.GetComponent<Camera>().cullingMask = ~mask;
    }

}

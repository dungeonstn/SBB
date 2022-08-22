using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePalletColor : MonoBehaviour
{
    [SerializeField] private Vector2 offset;

    private void Awake()
    {
        GetComponent<MeshRenderer>().material.mainTextureOffset = offset;
    }
}

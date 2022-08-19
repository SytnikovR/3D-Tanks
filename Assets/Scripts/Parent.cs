﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parent : MonoBehaviour
{
    [SerializeField] private Transform parent;

    private void Awake()
    {
        transform.SetParent(parent);
    }
}

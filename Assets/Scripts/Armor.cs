using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmorType
{
    Vehicle,
    Module
}

public class Armor : MonoBehaviour
{
    [SerializeField] private ArmorType type;
    [SerializeField] private Destructible destructible;
    [SerializeField] private int thickness;

    public ArmorType Type => type;
    public Destructible Destructible => destructible;
    public int Thickness => thickness;

    public void SetDestructible(Destructible destructible)
    {
        this.destructible = destructible;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ElementTypes
{
    Fire,
    Earth,
    Water,
    Air,
    Default
}
public abstract class SpellObject : ScriptableObject
{
    public ElementTypes type;
    public GameObject prefab;
    public float speed;
    public float damage;
    [TextArea(5, 10)]
    public string description;
}

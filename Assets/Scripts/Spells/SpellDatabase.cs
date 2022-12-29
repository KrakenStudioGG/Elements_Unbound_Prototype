using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable Object housing all individual Spell Scriptable Objects
[CreateAssetMenu(menuName = "Create Spell Database")]
[Serializable]
public class SpellDatabase : ScriptableObject
{
    public SpellObject[] spells;
}

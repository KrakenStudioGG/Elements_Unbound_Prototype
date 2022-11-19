using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpellSO", menuName = "CreateSO/Spells/Default")]
public class DefaultSpellObject : SpellObject
{
    //When new SO created, selected type from SO is set automatically
    private void Awake()
    {
        type = ElementTypes.Default;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Create New Inventory", menuName = "CreateSO/Inventory/Spells")]
public class SpellInventory : ScriptableObject
{
    public List<InventorySlot> Inventory = new List<InventorySlot>();

    public void AddSpell(SpellObject _spell, int _amount)
    {
        bool hasSpell = false;
        for (int i = 0; i < Inventory.Count; i++)
        {
            if (Inventory[i].spell == _spell)
            {
                Inventory[i].AddSpellAmount(_amount);
                hasSpell = true;
                break;
            }
        }

        if (!hasSpell)
        {
            Inventory.Add(new InventorySlot(_spell, _amount));
        }
    }
}

[System.Serializable]
public class InventorySlot
{
    public SpellObject spell;
    public int amount;

    public InventorySlot(SpellObject _spell, int _amount)
    {
        spell = _spell;
        amount = _amount;
    }

    public void AddSpellAmount(int spellAmount)
    {
        amount += spellAmount;
    }
}
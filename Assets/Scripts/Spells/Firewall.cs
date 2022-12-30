using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Firewall : SpellBaseScript
{
    
    private float timer = 3f;

    private void Update()
    {
        Countdown();
    }

    private void Countdown()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;
    }

    protected override IEnumerator MoveSpell()
    {
        transform.position = spellEndPos;
        while (timer > 0f)
        {
            yield return null;
            if (timer <= 0f)
                break;
        }
        timer = 3f;
        gameObject.SetActive(false);
        player.spellIsActive = false;
    }
}

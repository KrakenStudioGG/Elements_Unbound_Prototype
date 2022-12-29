using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firewall : SpellBaseScript
{
    private float timer;

    private void Start()
    {
        timer = 3f;
    }

    private void Update()
    {
        CountdownTimer();
    }

    private void CountdownTimer()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
        }
    }
}

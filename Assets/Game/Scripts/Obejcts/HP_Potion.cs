using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP_Potion : Object
{
    public float hpAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPlayer iPlayer = collision.GetComponent<IPlayer>();

        if (iPlayer == null)
        {
            return;
        }

        iPlayer.RestoreHP(hpAmount);

        Destroy(gameObject);
    }
}

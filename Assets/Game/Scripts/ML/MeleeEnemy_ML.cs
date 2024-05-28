using System;
using UnityEngine;

public class MeleeEnemy_ML : Enemy_ML
{
    

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        if (!isDead)
        {
            LookAtPlayer();
            MoveToPlayer();
        }

        base.FixedUpdate();
    }

    
}
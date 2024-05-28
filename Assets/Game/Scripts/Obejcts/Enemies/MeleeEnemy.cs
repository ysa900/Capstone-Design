using System;
using UnityEngine;

public class MeleeEnemy : Enemy
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
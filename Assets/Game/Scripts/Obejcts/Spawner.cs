using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    //BossManager bossManager;
    PoolManager poolManager;
    Boss boss;
    // Start is called before the first frame update
    void Start()
    {
        //bossManager = GetComponent<BossManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Spawn();
        
    }

    public void Spawn()
    {
        GameObject Boss_Bullet = poolManager.Get(0);
        Boss_Bullet.transform.position = new Vector2(boss.X + 1f, boss.Y - 1.2f);
    }
}

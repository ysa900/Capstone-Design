using System;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    // 利 弥家 积己 芭府
    //private float minBossSpawnRange = 20;

    Boss boss;
    public Boss bossPrefab;

    Boss_Bullet bossBullet;
    public Boss_Bullet bossBulletPrefab;

    private void Start()
    {
        CreateBoss(bossPrefab);
    }

    public void CreateBoss(Boss bossPrefab)
    {
        SetBossInfoNSummon(bossPrefab);

    }

    private void OnBossWasKilled(Boss killedBoss)
    {
        //
    }

    private void SetBossInfoNSummon(Boss bossPrefab)
    {
        boss = Instantiate(bossPrefab);

        boss.onBossTryAttack = OnBossTryAttack;

        boss.X = 0; boss.Y = 0;
    }

    private void OnBossTryAttack()
    {
        bossBullet = Instantiate(bossBulletPrefab);
        bossBullet.X = boss.X + 1f;
        bossBullet.Y = boss.Y - 1.2f;
    }
}

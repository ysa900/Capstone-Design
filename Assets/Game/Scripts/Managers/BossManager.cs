using System;
using UnityEngine;
using static Boss;

public class BossManager : MonoBehaviour
{
    // 利 弥家 积己 芭府
    //private float minBossSpawnRange = 20;

    public Boss boss;
    public Boss bossPrefab;

    public Player player;

    Boss_Bullet bossBullet;
    public Boss_Bullet bossBulletPrefab;

    Boss_Lazer bossLaser;
    public Boss_Lazer bossLaserPrefab;

    private void Start()
    {
    }

    public void CreateBoss()
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

        boss.onBossTryBulletAttack = onBossTryBulletAttack;
        boss.onBossTryLaserAttack = onBossTryLaserAttack;

        boss.player = player;
    }

    private void onBossTryBulletAttack()
    {
        bossBullet = Instantiate(bossBulletPrefab);
        bossBullet.X = boss.X + 1f;
        bossBullet.Y = boss.Y - 1.2f;
    }

    private void onBossTryLaserAttack()
    {
        bossLaser = Instantiate(bossLaserPrefab);

        bossLaser.boss = boss;
        bossLaser.player = player;
    }
}

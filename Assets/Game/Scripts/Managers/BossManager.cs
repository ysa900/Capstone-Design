using System;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    // 利 弥家 积己 芭府
    //private float minBossSpawnRange = 20;

    public Boss boss;
    public Boss bossPrefab;

    public Player player;

    Boss_Bullet bossBullet;
    public Boss_Bullet bossBulletPrefab;

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

        boss.onBossTryAttack = OnBossTryAttack;

        boss.X = player.transform.position.x + 10;
        boss.Y = player.transform.position.y + 8;

        boss.player = player;
    }

    private void OnBossTryAttack()
    {
        bossBullet = Instantiate(bossBulletPrefab);
        bossBullet.X = boss.X + 1f;
        bossBullet.Y = boss.Y - 1.2f;
    }
}

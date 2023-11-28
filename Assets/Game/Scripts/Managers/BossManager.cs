using System.Collections;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    // 적 최소 생성 거리
    //private float minBossSpawnRange = 20;

    public Boss boss;
    public Boss bossPrefab;

    public Player player;

    Boss_Bullet bossBullet;
    public Boss_Bullet bossBulletPrefab;

    Boss_Lazer bossLaser;
    public Boss_Lazer bossLaserPrefab;

    Boss_Genesis bossGenesis;
    public Boss_Genesis bossGenesisPrefab;

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
        boss.onBossTryGenesisAttack = onBossTryGenesisAttack;

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

    private void onBossTryGenesisAttack()
    {
        CastWithDelay(30);
    }

    // bossGenesis 스킬 쓸 때 일정 딜레이로 스킬 cast하기 위함
    private void CastWithDelay(int num)
    {
        for (int i = 0; i < num; i++)
        {
            bossGenesis = Instantiate(bossGenesisPrefab);

            float tmpX = boss.transform.position.x;
            float tmpY = boss.transform.position.y;

            float ranNum = UnityEngine.Random.Range(-20f, 20f);
            float ranNum2 = UnityEngine.Random.Range(-16f, 8f);

            tmpX += ranNum;
            tmpY += ranNum2;

            bossGenesis.X = tmpX;
            bossGenesis.Y = tmpY;

            bossGenesis.boss = boss;
            bossGenesis.player = player;

            bossGenesis.aliveTime = 2.0f;
        }

    }
}

using System.Collections;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    // �� �ּ� ���� �Ÿ�
    //private float minBossSpawnRange = 20;

    public Boss boss;
    public Boss bossPrefab;

    public Player player;

    Boss_Bullet_ML bossBullet;
    public Boss_Bullet_ML bossBulletPrefab;
    Boss_Bullet bossBullet2;
    public Boss_Bullet bossBulletPrefab2;

    Boss_Lazer bossLaser;
    public Boss_Lazer bossLaserPrefab;

    Boss_Grid_Lazer bossGridLaser;
    public Boss_Grid_Lazer bossGridLaserPrefab;

    Boss_Genesis bossGenesis;
    public Boss_Genesis bossGenesisPrefab;

    // GameManger���� ������ �׾��ٰ� �˷��ֱ� ���� delegate
    public delegate void OnBossHasKilled();
    public OnBossHasKilled onBossHasKilled;

    private void Start()
    {
    }

    public void CreateBoss()
    {
        SetBossInfoNSummon(bossPrefab);
        GameManager.instance.boss = boss;
    }

    private void SetBossInfoNSummon(Boss bossPrefab)
    {
        boss = Instantiate(bossPrefab);

        boss.onBossTryBulletAttack = onBossTryBulletAttack;
        boss.onBossTryLaserAttack = onBossTryLaserAttack;
        boss.onBossTryGenesisAttack = onBossTryGenesisAttack;
        boss.onBossTryGridLaserAttack = onBossTryGridLaserAttack;
        boss.onbossDead = OnbossDead;

        boss.player = player;
    }

    private void OnbossDead()
    {
        onBossHasKilled();
    }

    private void onBossTryBulletAttack()
    {
        bossBullet2 = Instantiate(bossBulletPrefab2);
  
    }

    private void onBossTryLaserAttack(float num)
    {
        bossLaser = Instantiate(bossLaserPrefab);

        bossLaser.boss = boss;
        bossLaser.player = player;
        bossLaser.laserTurnNum = num;
    }

    private void onBossTryGridLaserAttack(float x, float y, bool isRightTop)
    {
        bossGridLaser = Instantiate(bossGridLaserPrefab);

        bossGridLaser.boss = boss;
        bossGridLaser.player = player;

        bossGridLaser.X = x;
        bossGridLaser.Y = y;
        bossGridLaser.isRightTop = isRightTop;
        bossGridLaser.isRightBottom = !isRightTop;
    }

    private void onBossTryGenesisAttack()
    {
        CastWithDelay(30);
    }

    // bossGenesis ��ų �� �� ���� �����̷� ��ų cast�ϱ� ����
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

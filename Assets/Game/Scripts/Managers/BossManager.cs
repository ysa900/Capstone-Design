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

    Boss_Bullet_ML bossBullet_ML;
    public Boss_Bullet_ML bossBulletPrefab_ML;

    Boss_Laser bossLaser;

    Boss_Grid_Laser bossGridLaser;

    Boss_Genesis bossGenesis;

    // GameManger에게 보스가 죽었다고 알려주기 위한 delegate
    public delegate void OnBossHasKilled();
    public OnBossHasKilled onBossHasKilled;

    private void Start()
    {
        bossBullet_ML = Instantiate(bossBulletPrefab_ML); // 모델 적용을 위해 야매로 함
        Destroy(bossBullet_ML.gameObject);
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
<<<<<<< Updated upstream
        bossBullet = Instantiate(bossBulletPrefab);
        //GameManager.instance.poolManager.Get(0);
=======
        bossBullet = GameManager.instance.poolManager.GetBossSkill(0) as Boss_Bullet;

        bossBullet.boss = boss;
        bossBullet.player = player;
>>>>>>> Stashed changes
    }

    private void onBossTryLaserAttack(float num)
    {
        bossLaser = GameManager.instance.poolManager.GetBossSkill(1, num) as Boss_Laser;

        bossLaser.boss = boss;
        bossLaser.player = player;
        //bossLaser.laserTurnNum = num;
    }

    private void onBossTryGridLaserAttack(float x, float y, bool isRightTop)
    {
        bossGridLaser = GameManager.instance.poolManager.GetBossSkill(2, x, y, isRightTop) as Boss_Grid_Laser;

        bossGridLaser.boss = boss;
        bossGridLaser.player = player;
        /*
        bossGridLaser.X = x;
        bossGridLaser.Y = y;
        bossGridLaser.isRightTop = isRightTop;
        bossGridLaser.isRightBottom = !isRightTop;*/
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
            bossGenesis = GameManager.instance.poolManager.GetBossSkill(3) as Boss_Genesis;

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

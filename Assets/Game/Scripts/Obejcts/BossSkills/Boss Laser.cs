using System;
using UnityEngine;

public class Boss_Lazer : Object
{
    public Player player;
    public Boss boss;
    private float damage = 0.25f;

    float laserHalf = 29 / 2;

    public float aliveTime = 3f; // 스킬 생존 시간을 체크할 변수
    private float aliveTimer; // 스킬 생존 시간 타이머

    private float safeTime = 0.4f; // 플레이어가 피할 수 있게 아주 살짝 데미지 안들어가는 시간
    private float safeTimer; // 데미지 안들어가는 시간 타이머

    private void Start()
    {
        AttachBoss();
    }

    private void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime;

        if (destroySkill)
        {
            Destroy(gameObject);
            return;
        }

        aliveTimer += Time.fixedDeltaTime;
        safeTimer += Time.fixedDeltaTime;
    }

    // 보스에 붙어다니는 스킬
    private void AttachBoss()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 bossPosition = boss.transform.position;

        float compensateNum;

        if (boss.isBossLookLeft)
            compensateNum = -2.5f;
        else
            compensateNum = 2.5f;

        Vector2 direction = new Vector2(bossPosition.x + compensateNum - playerPosition.x, bossPosition.y - 3f - playerPosition.y + 0.1f);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
        transform.rotation = rotation;

        float zDegree = transform.rotation.eulerAngles.z;
        float tmpX = (float)Mathf.Cos(zDegree * Mathf.Deg2Rad) * laserHalf;
        float tmpY = (float)Mathf.Sin(zDegree * Mathf.Deg2Rad) * laserHalf;

        X = boss.X + compensateNum; Y = boss.Y - 3f;
        X -= tmpX; Y -= tmpY;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPlayer iPlayer = collision.GetComponent<IPlayer>();

        if (iPlayer == null)
        {
            return;
        }

        if(safeTimer >= safeTime)
        {
            iPlayer.TakeDamage(damage);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        IPlayer iPlayer = collision.GetComponent<IPlayer>();
        
        if (iPlayer == null)
        {
            return;
        }

        if (safeTimer >= safeTime)
        {
            iPlayer.TakeDamage(damage);
        }
    }
}


using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Player player;

    private float fireAttackRange = 12.5f;
    private float electricAttackRange = 12.5f;
    //private float waterAttackRange = 12.5f;

    public List<Enemy> enemies; // Enemy들을 담을 리스트

    // 스킬 클래스 객체들
    private PlayerAttachSkill playerAttachSkill;
    private EnemyOnSkill enemyOnSkill;
    private EnemyTrackingSkill enemyTrackingSkill;

    // 플레이어가 해당 스킬을 획득했는지를 판별하는 변수
    private bool isFireSkillSlected;
    private bool isElectricSkillSlected;
    private bool isWaterSkillSlected;

    // 각 스킬 별 delay 관련 변수
    private float attackDelayTimer_fire;
    private float attackDelayTime_fire = 1f;

    private float attackDelayTimer_electric;
    private float attackDelayTime_electric = 1f;

    private float attackDelayTimer_water;
    private float attackDelayTime_water = 1f;

    // 스킬 프리팹들
    public EnemyTrackingSkill fireBasicSkillPrefab;
    public EnemyOnSkill electricBasicSkillPrefab;
    public PlayerAttachSkill waterBasicSkillPrefab;

    private void Update()
    {
        if (isFireSkillSlected)
        {
            bool shouldBeAttack = 0 >= attackDelayTimer_fire; // 공격 쿨타임이 됐는지 확인
            if (shouldBeAttack)
            {
                attackDelayTimer_fire = attackDelayTime_fire;
                
                TryAttack(0); // 스킬 쿨타임이 다 됐으면 공격을 시도한다
            }
            else
            {
                attackDelayTimer_fire -= Time.deltaTime;
            }
        }

        if (isElectricSkillSlected)
        {
            bool shouldBeAttack = 0 >= attackDelayTimer_electric; // 공격 쿨타임이 됐는지 확인
            if (shouldBeAttack)
            {
                attackDelayTimer_electric = attackDelayTime_electric;

                TryAttack(1); // 스킬 쿨타임이 다 됐으면 공격을 시도한다
            }
            else
            {
                attackDelayTimer_electric -= Time.deltaTime;
            }
        }

        if (isWaterSkillSlected)
        {
            bool shouldBeAttack = 0 >= attackDelayTimer_water; // 공격 쿨타임이 됐는지 확인
            if (shouldBeAttack)
            {
                attackDelayTimer_water = attackDelayTime_water;

                TryAttack(2); // 스킬 쿨타임이 다 됐으면 공격을 시도한다
            }
            else
            {
                attackDelayTimer_water -= Time.deltaTime;
            }
        }
    }

    // 시작 스킬을 선택하는 함수
    // num : 스킬 번호 (불 - 0 , 전기 - 1, 물 - 2)
    public void ChooseStartSkill(int num)
    {
        switch (num)
        {
            case 0:
                {
                    isFireSkillSlected = true;
                    break;
                }
            case 1:
                {
                    isElectricSkillSlected = true;
                    break;
                }
            case 2:
                {
                    isWaterSkillSlected = true;
                    break;
                }
        }
    }

    // 공격을 시도하는 함수
    public void TryAttack(int num)
    {
        switch (num)
        {
            case 0:
                {
                    // 불 공격은 가장 가까운 적이 사거리 내에 있어야지만 나간다
                    Enemy enemy = FindNearestEnemy(); // 가장 가까운 적을 찾는다

                    float distance = Vector2.Distance(enemy.transform.position, player.transform.position);

                    bool isInAttackRange = distance <= fireAttackRange; // 적이 사거리 내에 있을때만 공격이 나간다

                    if (isInAttackRange)
                    {
                        CastSkill(enemy, num); // 스킬을 시전
                    }
                    break; 
                }
            case 1:
                {
                    // 전기 공격은 사거리 내 랜덤한 적에게 시전된다
                    Enemy enemy;

                    int breakNum = 0; // while문 탈출을 위한 num

                    while (true)
                    {
                        int ranNum = UnityEngine.Random.Range(0, enemies.Count);

                        enemy = enemies[ranNum];

                        float distance = Vector2.Distance(enemy.transform.position, player.transform.position);

                        bool isInAttackRange = distance <= electricAttackRange; // 적이 사거리 내에 있을때만 공격이 나간다

                        if (isInAttackRange)
                            break;

                        breakNum++;
                        if (breakNum >= 1000) // 1000회 반복 내에 마땅한 적을 찾지 못했다면 그냥 break;
                            break;
                    }
                    
                    CastSkill(enemy, num);
                    break;
                }
            case 2:
                {
                    // 물 스킬은 적에 상관없이 항상 나간다
                    Enemy enemy = enemies[0]; // 가짜로 일단 줌
                    CastSkill(enemy, num);

                    break;
                }

        }
        
        
    }

    // 가장 가까운 Enemy를 찾는 함수
    public Enemy FindNearestEnemy()
    {
        Enemy nearEnemy = null;

        float minDistance = float.MaxValue;

        for (int i = 0, ii = enemies.Count; i < ii; i++)
        {
            float distance = Vector2.Distance(enemies[i].transform.position, player.transform.position);
            if (distance <= minDistance)
            {
                minDistance = distance;
                nearEnemy = enemies[i];
            }
        }

        return nearEnemy;
    }

    // 스킬을 시전하는 함수
    // num : 스킬 번호 (불 - 0 , 전기 - 1, 물 - 2)
    private void CastSkill(Enemy enemy, int num)
    {
        switch(num)
        {
            case 0:
                {
                    enemyTrackingSkill = Instantiate(fireBasicSkillPrefab);
                    
                    Vector2 playerPosition = player.transform.position;
                    Vector2 enemyPosition = enemy.transform.position;

                    // 파이퍼볼 방향 보정 (적 바라보게)
                    Vector2 direction = new Vector2(playerPosition.x - enemyPosition.x, playerPosition.y - enemyPosition.y);
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    Quaternion angleAxis = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
                    Quaternion rotation = Quaternion.Slerp(enemyTrackingSkill.transform.rotation, angleAxis, 5f);
                    enemyTrackingSkill.transform.rotation = rotation;

                    enemyTrackingSkill.X = playerPosition.x;
                    enemyTrackingSkill.Y = playerPosition.y;

                    enemyTrackingSkill.enemy = enemy;

                    enemyTrackingSkill.speed = 10;
                    enemyTrackingSkill.damage = 20;

                    break;
                }
            case 1:
                {
                    enemyOnSkill = Instantiate(electricBasicSkillPrefab);

                    Vector2 enemyPosition = enemy.transform.position;

                    // 스킬 위치를 적 실제 위치로 변경
                    if(enemy.isEnemyLookLeft)
                        enemyOnSkill.X = enemyPosition.x - enemy.capsuleCollider.size.x * 6;
                    else
                        enemyOnSkill.X = enemyPosition.x + enemy.capsuleCollider.size.x * 6;
                    enemyOnSkill.Y = enemyPosition.y + enemy.capsuleCollider.size.y * 8;

                    enemyOnSkill.enemy = enemy;
                    enemyOnSkill.damage = 10;

                    break;
                }
            case 2:
                {
                    playerAttachSkill = Instantiate(waterBasicSkillPrefab);

                    playerAttachSkill.player = player;

                    playerAttachSkill.X = 999;
                    playerAttachSkill.Y = 999;

                    playerAttachSkill.damage = 20;
                    playerAttachSkill.enemy = enemy;
                    break;
                }
        }
    }
}


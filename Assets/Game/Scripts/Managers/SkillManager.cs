using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Player player;

    [SerializeField]
    private float attackRange = 30;

    public List<Enemy> enemies; // Enemy들을 담을 리스트

    private Skill skill; // 스킬 클래스 객체

    // 플레이어가 해당 스킬을 획득했는지를 판별하는 변수
    private bool isFireSkillSlected;
    private bool isElectricSkillSlected;
    private bool isWaterSkillSlected;

    // 각 스킬 별 delay 관련 변수
    private float attackDelayTimer_fire;
    private float attackDelayTime_fire = 2;

    private float attackDelayTimer_electric;
    private float attackDelayTime_electric = 5;

    private float attackDelayTimer_water;
    private float attackDelayTime_water = 5;

    // 스킬 프리팹들
    public Skill fireBasicSkillPrefab;
    public Skill electricBasicSkillPrefab;
    public Skill waterBasicSkillPrefab;

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
        Enemy enemy = FindNearestEnemy(); // 가장 가까운 적을 찾는다
        
        float distance = Vector2.Distance(enemy.transform.position, player.transform.position);

        bool isInAttackRange = distance <= attackRange; // 적이 사거리 내에 있을때만 공격이 나간다

        if (isInAttackRange)
        {
            CastSkill(enemy, num); // 스킬을 시전
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
                    skill = Instantiate(fireBasicSkillPrefab);
                    
                    Vector2 playerPosition = player.transform.position;
                    Vector2 enemyPosition = enemy.transform.position;

                    Vector2 direction = new Vector2(playerPosition.x - enemyPosition.x, playerPosition.y - enemyPosition.y);
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    Quaternion angleAxis = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
                    Quaternion rotation = Quaternion.Slerp(skill.transform.rotation, angleAxis, 5f);
                    skill.transform.rotation = rotation;

                    skill.X = playerPosition.x;
                    skill.Y = playerPosition.y;

                    skill.enemy = enemy;

                    break;
                }
            case 1:
                {
                    skill = Instantiate(electricBasicSkillPrefab);

                    skill.enemy = enemy;
                    break;
                }
            case 2:
                {
                    skill = Instantiate(waterBasicSkillPrefab);

                    skill.enemy = enemy;
                    break;
                }
        }
    }
}


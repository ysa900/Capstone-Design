using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public Player player;

    private float attackRange = 12.5f; // 플레이어 공격 사거리

    public List<Enemy> enemies; // Enemy들을 담을 리스트

    // 스킬 클래스 객체들
    private PlayerAttachSkill playerAttachSkill;
    private EnemyOnSkill enemyOnSkill;
    private EnemyTrackingSkill enemyTrackingSkill;
    private RandomSkill randomSkill;
    private GameObject skillObject;

    // 스킬 관련 배열들 공통 사항
    // 첫 번째 index - 0: 불, 1: 전기, 2: 물
    // 두 번째 index - 0: 기본 스킬(시작 스킬), 1, 2, 3 : 일반 스킬

<<<<<<< Updated upstream
    // 각 스킬 별 damage를 저장 리스트
    private float[,] skillDamages = new float[3, 4];

    // 플레이어가 해당 스킬을 획득했는지를 판별하는 bool 변수 배열
    private bool[,] isSkillSlected = new bool[3, 4];

    // 각 스킬 별 delay 관련 변수 리스트
    private float[,] attackDelayTimer = new float[3, 4];
    private float[,] attackDelayTime = new float[3, 4];
=======
    float[] attackDelayTimer = new float[7];
>>>>>>> Stashed changes

    // 스킬 프리팹들
    public EnemyTrackingSkill fireBasicSkillPrefab;
    public EnemyOnSkill electricBasicSkillPrefab;
    public PlayerAttachSkill waterBasicSkillPrefab;

    public PlayerAttachSkill fireNormalSkillPrefab1;
    public PlayerAttachSkill electricNormalSkillPrefab1;
    public PlayerAttachSkill waterNormalSkillPrefab1;

    public RandomSkill fireNormal2MeteorPrefab;
    public RandomSkill fireNormal2ExplodePrefab;
    public GameObject fireNormal2ShadowPrefab;

    public delegate void OnShiledSkillActivated(); // 쉴드 스킬이 켜 질때
    public OnShiledSkillActivated onShiledSkillActivated;

    public delegate void OnShiledSkillUnActivated(); // 쉴드 스킬이 꺼 질때
    public OnShiledSkillUnActivated onShiledSkillUnActivated;

    private void Start()
    {
        // 공격 딜레이 초기화
        for (int i = 0; i < 4; i++) { attackDelayTime[0, i] = 1f; }
        for (int i = 0; i < 4; i++) { attackDelayTime[1, i] = 1f; }
        for (int i = 0; i < 4; i++) { attackDelayTime[2, i] = 1f; }

        // 스킬 공격력 초기화
        for (int i = 0; i < 4; i++) { skillDamages[0, i] = 30f; }
        for (int i = 0; i < 4; i++) { skillDamages[1, i] = 10f; }
        for (int i = 0; i < 4; i++) { skillDamages[2, i] = 20f; }

        skillDamages[0, 1] = 10f;

        attackDelayTime[1, 1] = 6f;

        skillDamages[2, 1] = 0f;
        attackDelayTime[2, 1] = 10f;
    }

    private void Update()
    {
        for(int index1 = 0; index1 < 3; index1++)
        {
            for(int index2  = 0; index2 < 3; index2++)
            {
                if (isSkillSlected[index1, index2]) // 활성화(선택)된 스킬만 실행
                {
                    bool shouldBeAttack = 0 >= attackDelayTimer[index1, index2]; // 공격 쿨타임이 됐는지 확인
                    if (shouldBeAttack)
                    {
                        attackDelayTimer[index1, index2] = attackDelayTime[index1, index2];

                        TryAttack(index1, index2); // 스킬 쿨타임이 다 됐으면 공격을 시도한다
                    }
                    else
                    {
                        attackDelayTimer[index1, index2] -= Time.deltaTime;
                    }
                }
            }
        }
    }

<<<<<<< Updated upstream
    // 시작 스킬을 선택하는 함수
=======
    // skilldata를 초기화
    private void Init()
    {
        for (int i = 0; i < skillData.level.Length; i++)
        {
            skillData.level[i] = 0;
        }

        skillData.Damage[0] = 30;
        skillData.Damage[1] = 10;
        skillData.Damage[2] = 20;
        skillData.Damage[3] = 10;
        skillData.Damage[4] = 20;
        skillData.Damage[5] = 0;
        skillData.Damage[6] = 20;

        skillData.Delay[0] = 0.8f;
        skillData.Delay[1] = 1;
        skillData.Delay[2] = 2;
        skillData.Delay[3] = 3;
        skillData.Delay[4] = 8;
        skillData.Delay[5] = 10;
        skillData.Delay[6] = 2;

        for (int i = 0; i < skillData.skillSelected.Length; i++)
        {
            skillData.skillSelected[i] = false;
        }
    }

    // 시작 스킬을 선택하는 함수 (개발용)
>>>>>>> Stashed changes
    // num : 스킬 번호 (불 - 0 , 전기 - 1, 물 - 2)
    public void ChooseStartSkill(string type, int num)
    {
        switch (type)
        {
            case "불":
                {
                    isSkillSlected[0, num] = true;
                    break;
                }
            case "전기":
                {
                    isSkillSlected[1, num] = true;
                    break;
                }
            case "물":
                {
                    isSkillSlected[2, num] = true;
                    break;
                }
        }
    }

    // 공격을 시도하는 함수 (사거리 판단)
    // index1 : 스킬 종류 (불 - 0 , 전기 - 1, 물 - 2)
    // index2 : 스킬 번호 (기본 - 0, 일반 - 1, 2, 3)
    public void TryAttack(int index1, int index2)
    {
        switch (index1, index2)
        {
            case (0, 0):
                {
                    // 불 공격은 가장 가까운 적이 사거리 내에 있어야지만 나간다
                    Enemy enemy = FindNearestEnemy(); // 가장 가까운 적을 찾는다
                    Vector2 enemyPos = enemy.transform.position;
                    Vector2 playerPos = player.transform.position;
                    float distance = Vector2.Distance(enemyPos, playerPos);
                    
                    bool isInAttackRange = distance <= attackRange; // 적이 사거리 내에 있을때만 공격이 나간다

                    if (isInAttackRange)
                    {
                        CastSkill(enemy, index1, index2); // 스킬을 시전
                    }
                    break; 
                }
            case (1, 0):
                {
                    // 전기 공격은 사거리 내 랜덤한 적에게 시전된다
                    Enemy enemy;
                    
                    int breakNum = 0; // while문 탈출을 위한 num

                    while (true)
                    {
                        int ranNum = UnityEngine.Random.Range(0, enemies.Count);

                        enemy = enemies[ranNum];

                        float distance = Vector2.Distance(enemy.transform.position, player.transform.position);

                        bool isInAttackRange = distance <= attackRange; // 적이 사거리 내에 있을때만 공격이 나간다

                        if (isInAttackRange)
                            break;

                        breakNum++;
                        if (breakNum >= 1000) // 1000회 반복 내에 마땅한 적을 찾지 못했다면 그냥 break;
                            break;
                    }
                    
                    CastSkill(enemy, index1, index2);
                    break;
                }
            case (2, 0):
                {
                    // 물 기본 스킬은 적에 상관없이 항상 나간다 (플레이어에 붙어다님)
                    Enemy enemy = enemies[0]; // 가짜로 일단 줌
                    CastSkill(enemy, index1, index2);

                    break;
                }
            case (0, 1):
                {
                    // 불 일반1 스킬은 적에 상관없이 항상 나간다 (플레이어에 붙어다님)
                    Enemy enemy = enemies[0]; // 가짜로 일단 줌
                    CastSkill(enemy, index1, index2);
                    break;
                }
            case (1, 1):
                {
                    // 전기 일반1 스킬은 적에 상관없이 항상 나간다 (플레이어에 붙어다님)
                    Enemy enemy = enemies[0]; // 가짜로 일단 줌
                    CastSkill(enemy, index1, index2);
                    break;
                }
<<<<<<< Updated upstream
            case (2, 1):
            {
                // 물 일반1 스킬은 적에 상관없이 항상 나간다 (플레이어에 붙어다님)
                Enemy enemy = enemies[0]; // 가짜로 일단 줌
                CastSkill(enemy, index1, index2);
                break; 
            }
=======
            case 6: 
                {
                    // 불 일반2 스킬은 적에 상관없이 항상 나간다 (랜덤 위치에 떨어짐)
                    Enemy enemy = enemies[0]; // 가짜로 일단 줌
                    CastSkill(enemy, index);
                    break;
                }
>>>>>>> Stashed changes

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
    // index1 : 스킬 종류 (불 - 0 , 전기 - 1, 물 - 2)
    // index2 : 스킬 번호 (기본 - 0, 일반 - 1, 2, 3)
    private void CastSkill(Enemy enemy, int index1, int index2)
    {
        switch(index1, index2)
        {
            case (0, 0):
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
                    enemyTrackingSkill.damage = skillDamages[index1, index2];

                    break;
                }
            case (1, 0):
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
                    enemyOnSkill.damage = skillDamages[index1, index2];

                    break;
                }
            case (2, 0):
                {
                    playerAttachSkill = Instantiate(waterBasicSkillPrefab);

                    playerAttachSkill.player = player;

                    playerAttachSkill.xPositionNum = 3f;
                    playerAttachSkill.yPositionNum = 0.2f;

                    playerAttachSkill.isAttachSkill = true;

                    playerAttachSkill.damage = skillDamages[index1, index2];
                    break;
                }
            case (0, 1):
                {
                    playerAttachSkill = Instantiate(fireNormalSkillPrefab1);

                    playerAttachSkill.player = player;

                    playerAttachSkill.xPositionNum = 0;
                    playerAttachSkill.yPositionNum = 0.2f;

                    playerAttachSkill.isAttachSkill = true;

                    playerAttachSkill.damage = skillDamages[index1, index2];
                    break;
                }
            case (1, 1):
                {
                    playerAttachSkill = Instantiate(electricNormalSkillPrefab1);

                    playerAttachSkill.player = player;

                    playerAttachSkill.X = player.transform.position.x + 3f;
                    playerAttachSkill.Y = player.transform.position.y;

                    playerAttachSkill.xPositionNum = 3f;
                    playerAttachSkill.yPositionNum = 0f;

                    playerAttachSkill.isCircleSkill = true;

                    playerAttachSkill.damage = skillDamages[index1, index2];
                    break;
                }
                case (2, 1):
                {
                    playerAttachSkill = Instantiate(waterNormalSkillPrefab1);

                    playerAttachSkill.player = player;

                    playerAttachSkill.X = player.transform.position.x;
                    playerAttachSkill.Y = player.transform.position.y;

                    playerAttachSkill.isShieldSkill = true;

                    playerAttachSkill.damage = skillDamages[index1, index2];

                    playerAttachSkill.onShieldSkillDestroyed = OnShieldSkillDestryed;

                    onShiledSkillActivated();
                    break;
                }
            case 6:
                {
                    randomSkill = Instantiate(fireNormal2MeteorPrefab);

                    float tmpX = player.transform.position.x;
                    float tmpY = player.transform.position.y;

                    float ranNum = Random.Range(-14f, 10f);
                    float ranNum2 = Random.Range(-8f, 3f);

                    tmpX += ranNum;
                    tmpY += ranNum2;

                    randomSkill.impactPonitX = tmpX;
                    randomSkill.impactPonitY = tmpY;

                    // 메테오 방향 보정 (충돌 지점 바라보게)
                    Vector2 direction = new Vector2(-5f, -11f);
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    Quaternion angleAxis = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
                    Quaternion rotation = Quaternion.Slerp(randomSkill.transform.rotation, angleAxis, 5f);
                    randomSkill.transform.rotation = rotation;

                    randomSkill.X = tmpX + 5f;
                    randomSkill.Y = tmpY + 11f;

                    randomSkill.player = player;
                    randomSkill.isMeteor = true;

                    randomSkill.aliveTime = 0.5f;
                    randomSkill.damage = skillData.Damage[6];

                    randomSkill.fireNormal2ExplodePrefab = fireNormal2ExplodePrefab;

                    StartCoroutine(DisplayNDestroy(tmpX + 1.6f, tmpY + 3)); // 그림자 나타내고 지우기
                    StopCoroutine(DisplayNDestroy(tmpX + 1.6f, tmpY + 3));

                    break;
                }
        }
    }

    private void OnShieldSkillDestryed()
    {
        onShiledSkillUnActivated();
    }

    IEnumerator DisplayNDestroy(float x, float y)
    {
        skillObject = Instantiate(fireNormal2ShadowPrefab);

        skillObject.transform.position = new Vector2(x, y);

        yield return new WaitForSeconds(0.5f); // 지정한 초 만큼 쉬기

        Destroy(skillObject);
    }
}


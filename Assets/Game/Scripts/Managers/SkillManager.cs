using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class SkillManager : MonoBehaviour
{
    public Player player;
    private GameAudioManager gameAudioManager;

    private float attackRange = 12.5f; // 플레이어 공격 사거리

    public bool isBossAppear; // 보스가 소환됐는지를 판단하는 변수

    public List<Enemy> enemies; // Enemy들을 담을 리스트
    public Boss boss;

    public SkillData2 skillData;

    private bool isShadowAlive; // 그림자가 살아있으면 알파값을 조정하기 위함
    private float alpha = 0;

    private bool isFire3SkillLeftRight; // 불 일반3 스킬은 좌우 / 위아래로 번갈아 나가므로 설정한 변수

    // 스킬 클래스 객체들
    private PlayerAttachSkill playerAttachSkill;
    private EnemyOnSkill enemyOnSkill;
    private EnemyTrackingSkill enemyTrackingSkill;
    private RandomSkill randomSkill;
    private GameObject skillObject;

    // 스킬 관련 배열들 공통 사항
    // 스킬들 index: 불 - 3n, 전기 - 3n + 1, 물 - 3n + 2
    // 불 - 0, 3, 6, 9 / 전기 - 1, 4, 7, 10 / 물 - 2, 5, 8 ,11
    float[] attackDelayTimer = new float[12];

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
    public PlayerAttachSkill electricNormalSkillPrefab2;
    public RandomSkill waterNormalSkillPrefab2;

    public PlayerAttachSkill fireNormalSkillPrefab3_1;
    public PlayerAttachSkill fireNormalSkillPrefab3_2;
    public RandomSkill electricNormalSkillPrefab3;
    public PlayerAttachSkill waterNormalSkillPrefab3;

    // delegate들
    public delegate void OnShiledSkillActivated(); // 쉴드 스킬이 켜 질때
    public OnShiledSkillActivated onShiledSkillActivated;

    public delegate void OnShiledSkillUnActivated(); // 쉴드 스킬이 꺼 질때
    public OnShiledSkillUnActivated onShiledSkillUnActivated;

    private void Awake()
    {
        Init(); // skillData 초기화
    }

    private void Update()
    {
        for(int i = 0; i < skillData.Damage.Length; i++)
        {
            if (skillData.skillSelected[i]) // 활성화(선택)된 스킬만 실행
            {
                bool shouldBeAttack = 0 >= attackDelayTimer[i]; // 공격 쿨타임이 됐는지 확인
                if (shouldBeAttack)
                {
                    attackDelayTimer[i] = skillData.Delay[i];

                    if(i == 0 || i == 1)
                        TryAttack(i); // 스킬 쿨타임이 다 됐으면 공격을 시도한다
                    else
                        CastSkill(i); // 스킬 쿨타임이 다 됐으면 공격한다
                }
                else
                {
                    attackDelayTimer[i] -= Time.deltaTime;
                }
            }
        }

        if (isShadowAlive)
        {
            if (alpha < 1f)
                alpha += 0.008f;
            skillObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);
        }
        else
            alpha = 0f;
    }

    // skilldata를 초기화
    private void Init()
    {
        gameAudioManager = FindAnyObjectByType<GameAudioManager>();

        for (int i = 0; i < skillData.level.Length; i++)
        {
            skillData.level[i] = 0;
        }

        skillData.Damage[0] = 30f;
        skillData.Damage[1] = 20f;
        skillData.Damage[2] = 2.0f;
        skillData.Damage[3] = 20f;
        skillData.Damage[4] = 15f;
        skillData.Damage[5] = 0f;
        skillData.Damage[6] = 60f;
        skillData.Damage[7] = 3.0f;
        skillData.Damage[8] = 2.0f;
        skillData.Damage[9] = 40f;
        skillData.Damage[10] = 40f;
        skillData.Damage[11] = 2.0f;

        skillData.Delay[0] = 0.8f;
        skillData.Delay[1] = 1;
        skillData.Delay[2] = 1.5f;
        skillData.Delay[3] = 3;
        skillData.Delay[4] = 14;
        skillData.Delay[5] = 10;
        skillData.Delay[6] = 3;
        skillData.Delay[7] = 4;
        skillData.Delay[8] = 2;
        skillData.Delay[9] = 3;
        skillData.Delay[10] = 7.5f;
        skillData.Delay[11] = 8f;

        skillData.scale[0] = 1.5f;
        skillData.scale[1] = 1f;
        skillData.scale[2] = 2f;
        skillData.scale[3] = 1.5f;
        skillData.scale[4] = 1f;
        skillData.scale[5] = 1f;
        skillData.scale[6] = 1.5f;
        skillData.scale[7] = 1f;
        skillData.scale[8] = 1.8f;
        skillData.scale[9] = 1.75f;
        skillData.scale[10] = 1.5f;
        skillData.scale[11] = 1.5f;

        for (int i = 0; i < skillData.skillSelected.Length; i++)
        {
            skillData.skillSelected[i] = false;
        }
    }

    // 시작 스킬을 선택하는 함수 (개발용)
    // num : 스킬 번호 (불 - 0 , 전기 - 1, 물 - 2)
    public void ChooseStartSkill(int num)
    {
        skillData.skillSelected[num] = true;
    }

    // 공격을 시도하는 함수 (사거리 판단)
    // 스킬 관련 배열들 공통 사항
    // 스킬들 index: 불 - 3n, 전기 - 3n + 1, 물 - 3n + 2
    // 불 - 0, 3, 6, 9 / 전기 - 1, 4, 7, 10 / 물 - 2, 5, 8 ,11
    public void TryAttack(int index)
    {
        if (!isBossAppear)
        {
            switch (index)
            {
                case 0:
                    {
                        // 불 공격은 가장 가까운 적이 사거리 내에 있어야지만 나간다
                        Enemy enemy = FindNearestEnemy(); // 가장 가까운 적을 찾는다
                        if (enemy == null) return; // 적이 없으면 공격 X
                        Vector2 enemyPos = enemy.transform.position;
                        Vector2 playerPos = player.transform.position;
                        float distance = Vector2.Distance(enemyPos, playerPos);
                        
                        bool isInAttackRange = distance <= attackRange; // 적이 사거리 내에 있을때만 공격이 나간다

                        if (isInAttackRange)
                        {
                            CastSkill(enemy, index); // 스킬을 시전
                        }

                        break;
                    }
                case 1:
                    {
                        // 전기 공격은 사거리 내 랜덤한 적에게 시전된다
                        Enemy enemy;

                        int breakNum = 0; // while문 탈출을 위한 num
                        bool isInAttackRange = false;

                        while (true)
                        {
                            int ranNum = UnityEngine.Random.Range(0, enemies.Count);

                            enemy = enemies[ranNum];
                            
                            if (!(enemy == null))
                            {
                                float distance = Vector2.Distance(enemy.transform.position, player.transform.position);
                                
                                isInAttackRange = distance <= attackRange; // 적이 사거리 내에 있을때만 공격이 나간다

                                if (isInAttackRange)
                                    break;
                            }

                            breakNum++;
                            if (breakNum >= 1000) // 1000회 반복 내에 마땅한 적을 찾지 못했다면 그냥 break;
                                break;
                        }
                        if (enemy == null) return; // 적이 없으면 공격 X

                        if(isInAttackRange)
                            CastSkill(enemy, index);

                        break;
                    }
            }
        }
        else
        {
            if(boss == null) return;
            switch (index)
            {
                case 0:
                    {
                        // 불 공격은 보스가 사거리 내에 있어야지만 나간다
                        Vector2 bossPos = boss.transform.position;
                        Vector2 playerPos = player.transform.position;
                        float distance = Vector2.Distance(bossPos, playerPos);

                        bool isInAttackRange = distance <= attackRange; // 적이 사거리 내에 있을때만 공격이 나간다

                        if (isInAttackRange)
                        {
                            CastSkill(boss, index); // 스킬을 시전
                        }
                        break;
                    }
                case 1:
                    {
                        CastSkill(boss, index);
                        
                        break;
                    }
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

    // 스킬을 시전하는 함수 (Enemy)
    // 스킬 관련 배열들 공통 사항
    // 스킬들 index: 불 - 3n, 전기 - 3n + 1, 물 - 3n + 2
    // 불 - 0, 3, 6, 9 / 전기 - 1, 4, 7, 10 / 물 - 2, 5, 8 ,11
    private void CastSkill(Enemy enemy, int index)
    {
        switch (index)
        {
            case 0:
                {
                    enemyTrackingSkill = Instantiate(fireBasicSkillPrefab);
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

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

                    enemyTrackingSkill.speed = 20;
                    enemyTrackingSkill.damage = skillData.Damage[index];

                    SetScale(enemyTrackingSkill.gameObject, index);

                    break;
                }
            case 1:
                {
                    enemyOnSkill = Instantiate(electricBasicSkillPrefab);
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

                    Vector2 enemyPosition = enemy.transform.position;

                    // 스킬 위치를 적 실제 위치로 변경
                    if (enemy.isEnemyLookLeft)
                        enemyOnSkill.X = enemyPosition.x - enemy.capsuleCollider.size.x * 6;
                    else
                        enemyOnSkill.X = enemyPosition.x + enemy.capsuleCollider.size.x * 6;
                    enemyOnSkill.Y = enemyPosition.y + enemy.capsuleCollider.size.y * 8;

                    enemyOnSkill.enemy = enemy;
                    enemyOnSkill.damage = skillData.Damage[index];

                    SetScale(enemyOnSkill.gameObject, index);

                    break;
                }
        }
    }

    // 스킬을 시전하는 함수 (Boss)
    private void CastSkill(Boss boss, int index)
    {
        switch (index)
        {
            case 0:
                {
                    enemyTrackingSkill = Instantiate(fireBasicSkillPrefab);
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

                    Vector2 playerPosition = player.transform.position;
                    Vector2 bossPosition = boss.transform.position;

                    // 파이퍼볼 방향 보정 (적 바라보게)
                    Vector2 direction = new Vector2(playerPosition.x - bossPosition.x, playerPosition.y - bossPosition.y);
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    Quaternion angleAxis = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
                    Quaternion rotation = Quaternion.Slerp(enemyTrackingSkill.transform.rotation, angleAxis, 5f);
                    enemyTrackingSkill.transform.rotation = rotation;

                    enemyTrackingSkill.X = playerPosition.x;
                    enemyTrackingSkill.Y = playerPosition.y;

                    enemyTrackingSkill.boss = boss;
                    enemyTrackingSkill.isBossAppear = true;

                    enemyTrackingSkill.speed = 20;
                    enemyTrackingSkill.damage = skillData.Damage[index];

                    SetScale(enemyTrackingSkill.gameObject, index);

                    break;
                }
            case 1:
                {
                    enemyOnSkill = Instantiate(electricBasicSkillPrefab);
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

                    Vector2 bossPosition = boss.transform.position;

                    // 스킬 위치를 보스 실제 위치로 변경
                    enemyOnSkill.X = bossPosition.x;
                    enemyOnSkill.Y = bossPosition.y - boss.capsuleCollider.size.y * 4;
                    
                    enemyOnSkill.isBossAppear = true;

                    enemyOnSkill.damage = skillData.Damage[index];

                    SetScale(enemyOnSkill.gameObject, index);

                    break;
                }
        }
    }

    // 스킬을 시전하는 함수
    private void CastSkill(int index)
    {
        switch (index)
        {
            case 2:
                {
                    playerAttachSkill = Instantiate(waterBasicSkillPrefab);
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

                    playerAttachSkill.player = player;

                    if (skillData.level[index] == 5)
                    {
                        playerAttachSkill.xPositionNum = 8f;
                    }
                    else if (skillData.level[index] >= 3)
                    {
                        playerAttachSkill.xPositionNum = 6f;
                    }
                    else
                    {
                        playerAttachSkill.xPositionNum = 4f;
                    }
                    playerAttachSkill.yPositionNum = 0.2f;

                    playerAttachSkill.X = 999f;
                    playerAttachSkill.Y = 999f;

                    playerAttachSkill.isAttachSkill = true;
                    playerAttachSkill.isFlipped = true;

                    playerAttachSkill.isStaySkill = true;

                    playerAttachSkill.aliveTime = 0.5f;

                    playerAttachSkill.damage = skillData.Damage[index];

                    SetScale(playerAttachSkill.gameObject, index);
                    break;
                }
            case 3:
                {
                    playerAttachSkill = Instantiate(fireNormalSkillPrefab1);
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

                    playerAttachSkill.player = player;

                    playerAttachSkill.xPositionNum = 0;
                    playerAttachSkill.yPositionNum = 0.2f;

                    playerAttachSkill.isAttachSkill = true;

                    playerAttachSkill.X = player.transform.position.x;
                    playerAttachSkill.Y = player.transform.position.y;

                    playerAttachSkill.aliveTime = 0.5f;

                    playerAttachSkill.damage = skillData.Damage[index];

                    SetScale(playerAttachSkill.gameObject, index);
                    break;
                }
            case 4:
                {
                    playerAttachSkill = Instantiate(electricNormalSkillPrefab1);
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

                    playerAttachSkill.player = player;

                    playerAttachSkill.X = player.transform.position.x + 3f;
                    playerAttachSkill.Y = player.transform.position.y;

                    if (skillData.level[index] == 5)
                    {
                        playerAttachSkill.xPositionNum = 6f;
                    }
                    else if(skillData.level[index] >= 3)
                    {
                        playerAttachSkill.xPositionNum = 4.5f;
                    }
                    else
                    {
                        playerAttachSkill.xPositionNum = 3f;
                    }
                    playerAttachSkill.yPositionNum = 0f;

                    playerAttachSkill.isCircleSkill = true;

                    playerAttachSkill.aliveTime = 5f;

                    playerAttachSkill.damage = skillData.Damage[index];

                    SetScale(playerAttachSkill.gameObject, index);
                    break;
                }
            case 5:
                {
                    playerAttachSkill = Instantiate(waterNormalSkillPrefab1);
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

                    playerAttachSkill.player = player;

                    playerAttachSkill.X = player.transform.position.x;
                    playerAttachSkill.Y = player.transform.position.y;

                    playerAttachSkill.isShieldSkill = true;

                    playerAttachSkill.damage = skillData.Damage[index];

                    playerAttachSkill.aliveTime = 3f;

                    playerAttachSkill.onShieldSkillDestroyed = OnShieldSkillDestroyed;

                    onShiledSkillActivated();
                    break;
                }
            case 6:
                {
                    randomSkill = Instantiate(fireNormal2MeteorPrefab);
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

                    float tmpX = player.transform.position.x;
                    float tmpY = player.transform.position.y;

                    float ranNum = UnityEngine.Random.Range(-14f, 10f);
                    float ranNum2 = UnityEngine.Random.Range(-8f, 3f);

                    tmpX += ranNum;
                    tmpY += ranNum2;

                    randomSkill.impactPonitX = tmpX;
                    randomSkill.impactPonitY = tmpY;

                    // 메테오 방향 보정 (충돌 지점 바라보게)
                    Vector2 direction = new Vector2(-7f, -11f);
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                    Quaternion angleAxis = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
                    Quaternion rotation = Quaternion.Slerp(randomSkill.transform.rotation, angleAxis, 5f);
                    randomSkill.transform.rotation = rotation;

                    randomSkill.X = tmpX + 7f;
                    randomSkill.Y = tmpY + 11f;

                    randomSkill.player = player;
                    randomSkill.isMeteor = true;

                    randomSkill.aliveTime = 0.5f;
                    randomSkill.damage = skillData.Damage[index];
                    randomSkill.scale = skillData.scale[index];

                    randomSkill.fireNormal2ExplodePrefab = fireNormal2ExplodePrefab;

                    SetScale(randomSkill.gameObject, index);

                    StartCoroutine(DisplayShadowNDestroy(tmpX + 2.6f, tmpY + 3.4f)); // 그림자 나타내고 지우기

                    break;
                }
            case 7:
                {
                    playerAttachSkill = Instantiate(electricNormalSkillPrefab2);
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

                    playerAttachSkill.player = player;

                    
                    playerAttachSkill.xPositionNum = 8.5f;
                    playerAttachSkill.yPositionNum = -0.2f;

                    playerAttachSkill.X = 999f;
                    playerAttachSkill.Y = 999f;

                    playerAttachSkill.isAttachSkill = true;
                    playerAttachSkill.isDelaySkill = true;

                    playerAttachSkill.isStaySkill = true;

                    playerAttachSkill.aliveTime = 0.8f;

                    playerAttachSkill.damage = skillData.Damage[index];

                    Transform parent = playerAttachSkill.transform.parent;

                    playerAttachSkill.transform.parent = null;
                    playerAttachSkill.transform.localScale = new Vector3(3, skillData.scale[index], 0);
                    playerAttachSkill.transform.parent = parent;

                    break;
                }
            case 8:
                {
                    randomSkill = Instantiate(waterNormalSkillPrefab2);
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

                    float tmpX = player.transform.position.x;
                    float tmpY = player.transform.position.y;

                    float ranNum = UnityEngine.Random.Range(-14f, 14f);
                    float ranNum2 = UnityEngine.Random.Range(-6f, 6f);

                    tmpX += ranNum;
                    tmpY += ranNum2;

                    randomSkill.X = tmpX;
                    randomSkill.Y = tmpY;

                    randomSkill.player = player;

                    randomSkill.isStaySkill = true;
                    randomSkill.isIceSpike = true;

                    randomSkill.aliveTime = 3f;
                    randomSkill.damage = skillData.Damage[index];

                    SetScale(randomSkill.gameObject, index);
                    break;
                }
            case 9:
                {
                    if (isFire3SkillLeftRight)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            playerAttachSkill = Instantiate(fireNormalSkillPrefab3_2);
                            gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

                            if (i == 0)
                            {
                                playerAttachSkill.X = 999f;
                                playerAttachSkill.Y = 999f;

                                if (skillData.level[index] == 5)
                                {
                                    playerAttachSkill.xPositionNum = -8f;
                                }
                                else if (skillData.level[index] >= 3)
                                {
                                    playerAttachSkill.xPositionNum = -6f;
                                }
                                else
                                {
                                    playerAttachSkill.xPositionNum = -4f;
                                }
                                playerAttachSkill.yPositionNum = 0f;

                                playerAttachSkill.player = player;

                                playerAttachSkill.isAttachSkill = true;
                                playerAttachSkill.isFlipped = false;

                                playerAttachSkill.aliveTime = 0.8f;
                                playerAttachSkill.damage = skillData.Damage[index];

                                SetScale(playerAttachSkill.gameObject, index);
                            }
                            else
                            {
                                playerAttachSkill.X = 999f;
                                playerAttachSkill.Y = 999f;

                                if (skillData.level[index] == 5)
                                {
                                    playerAttachSkill.xPositionNum = 8f;
                                }
                                else if (skillData.level[index] >= 3)
                                {
                                    playerAttachSkill.xPositionNum = 6f;
                                }
                                else
                                {
                                    playerAttachSkill.xPositionNum = 4f;
                                }
                                playerAttachSkill.yPositionNum = 0f;

                                playerAttachSkill.player = player;

                                playerAttachSkill.isAttachSkill = true;
                                playerAttachSkill.isFlipped = true;

                                playerAttachSkill.aliveTime = 0.8f;
                                playerAttachSkill.damage = skillData.Damage[index];

                                SetScale(playerAttachSkill.gameObject, index);
                            }
                        }
                        isFire3SkillLeftRight = false;
                    }
                    else
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            playerAttachSkill = Instantiate(fireNormalSkillPrefab3_1);
                            gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

                            if (i == 0)
                            {
                                playerAttachSkill.X = 999f;
                                playerAttachSkill.Y = 999f;

                                playerAttachSkill.xPositionNum = 0f;
                                if (skillData.level[index] == 5)
                                {
                                    playerAttachSkill.yPositionNum = 4.5f;
                                }
                                else if (skillData.level[index] >= 3)
                                {
                                    playerAttachSkill.yPositionNum = 3f;
                                }
                                else
                                {
                                    playerAttachSkill.yPositionNum = 1.5f;
                                }

                                playerAttachSkill.GetComponent<SpriteRenderer>().flipY = true;

                                playerAttachSkill.player = player;

                                playerAttachSkill.isAttachSkill = true;

                                playerAttachSkill.aliveTime = 1f;
                                playerAttachSkill.damage = skillData.Damage[index];

                                SetScale(playerAttachSkill.gameObject, index);
                            }
                            else
                            {
                                playerAttachSkill.X = 999f;
                                playerAttachSkill.Y = 999f;

                                playerAttachSkill.xPositionNum = 0f;
                                if (skillData.level[index] == 5)
                                {
                                    playerAttachSkill.yPositionNum = -5f;
                                }
                                else if (skillData.level[index] >= 3)
                                {
                                    playerAttachSkill.yPositionNum = -3.75f;
                                }
                                else
                                {
                                    playerAttachSkill.yPositionNum = -2.5f;
                                }

                                playerAttachSkill.player = player;

                                playerAttachSkill.isAttachSkill = true;

                                playerAttachSkill.aliveTime = 1f;
                                playerAttachSkill.damage = skillData.Damage[index];

                                SetScale(playerAttachSkill.gameObject, index);
                            }

                        }
                        isFire3SkillLeftRight = true;
                    }

                    break;
                }
            case 10:
                {
                    StartCoroutine(CastWithDelay(index, 10));

                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음
                    break;
                }
            case 11:
                {
                    playerAttachSkill = Instantiate(waterNormalSkillPrefab3);
                    gameAudioManager.PlaySfx(GameAudioManager.Sfx.Range); // 스킬 사용 효과음

                    playerAttachSkill.player = player;

                    if (skillData.level[index] == 5)
                    {
                        playerAttachSkill.xPositionNum = 9f;
                    }
                    else if (skillData.level[index] >= 3)
                    {
                        playerAttachSkill.xPositionNum = 6.75f;
                    }
                    else
                    {
                        playerAttachSkill.xPositionNum = 4.5f;
                    }
                    playerAttachSkill.yPositionNum = -0.2f;

                    playerAttachSkill.X = 999f;
                    playerAttachSkill.Y = 999f;

                    playerAttachSkill.isAttachSkill = true;
                    playerAttachSkill.isStaySkill = true;
                    playerAttachSkill.isYFlipped = true;

                    playerAttachSkill.aliveTime = 3f;

                    playerAttachSkill.damage = skillData.Damage[index];

                    SetScale(playerAttachSkill.gameObject, index);
                    break;
                }
        }
    }

    // 스킬 스케일 설정
    private void SetScale(GameObject gameObject, int index)
    {
        Transform parent = gameObject.transform.parent;

        gameObject.transform.parent = null;
        gameObject.transform.localScale = new Vector3(skillData.scale[index], skillData.scale[index], 0);
        gameObject.transform.parent = parent;
    }

    // 쉴드 스킬이 꺼질 때 신호를 받아서 GameManager에게 전달
    private void OnShieldSkillDestroyed()
    {
        onShiledSkillUnActivated();
    }

    // 메테오 떨어질 때 그림자 오브젝트 생성 후 제거
    IEnumerator DisplayShadowNDestroy(float x, float y)
    {
        skillObject = Instantiate(fireNormal2ShadowPrefab);

        skillObject.transform.position = new Vector2(x, y);

        SetScale(skillObject.gameObject, 6);

        isShadowAlive = true;

        yield return new WaitForSeconds(0.5f); // 지정한 초 만큼 쉬기

        isShadowAlive = false;

        Destroy(skillObject);
    }

    // Judgment 스킬 쓸 때 일정 딜레이로 스킬 cast하기 위함
    IEnumerator CastWithDelay(int index, int num)
    {
        for (int i = 0; i < num; i++)
        {
            randomSkill = Instantiate(electricNormalSkillPrefab3);

            float tmpX = player.transform.position.x;
            float tmpY = player.transform.position.y;

            float ranNum = UnityEngine.Random.Range(-10f, 10f);
            float ranNum2 = UnityEngine.Random.Range(-5f, 5f);

            tmpX += ranNum;
            tmpY += ranNum2;

            randomSkill.X = tmpX;
            randomSkill.Y = tmpY;

            randomSkill.player = player;

            randomSkill.aliveTime = 0.8f;
            randomSkill.damage = skillData.Damage[index];

            SetScale(randomSkill.gameObject, index);

            yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기
        }
        
    }
}


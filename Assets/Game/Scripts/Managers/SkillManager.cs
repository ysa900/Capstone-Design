using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillManager : MonoBehaviour
{
    // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
    private static SkillManager _instance;

    // 인스턴스에 접근하기 위한 프로퍼티
    public static SkillManager instance
    {
        get
        {
            // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
            if (!_instance)
            {
                _instance = FindAnyObjectByType(typeof(SkillManager)) as SkillManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    public Player player;

    private float attackRange = 15f; // 플레이어 공격 사거리
    private float nearAttackRange = 10f; // 플레이어 근접 우선 공격 사거리

    public bool isBossAppear; // 보스가 소환됐는지를 판단하는 변수

    [SerializeField]
    [ReadOnly]
    public List<Enemy> enemies; // Enemy들을 담을 리스트
    public Boss boss;

    public SkillData2 skillData;
    public SkillData2 passiveSkillData;

    bool isFire3SkillLeftRight; // 불 일반3 스킬은 좌우 / 위아래로 번갈아 나가므로 설정한 변수
    int skyFallQuadrantNum = -1; // Skyfall스킬 랜덤 사분면 번호

    // 스킬 클래스 객체들
    private Skill skill;
    private PlayerAttachSkill playerAttachSkill;
    private EnemyOnSkill enemyOnSkill;
    private EnemyTrackingSkill enemyTrackingSkill;
    private RandomSkill randomSkill;

    // 스킬 관련 배열들 공통 사항
    // 스킬들 index: 불 - 3n, 전기 - 3n + 1, 물 - 3n + 2
    // 불 - 0, 3, 6, 9 / 전기 - 1, 4, 7, 10 / 물 - 2, 5, 8 ,11
    [SerializeField] float[] attackDelayTimer = new float[18];

    /* 
     * 0번 스킬(fire ball), 1번 스킬(lightning), 8번 스킬(ice spike), 16번 스킬(Sky Fall), 16번 스킬(Frozen Spike)
     * 들은 스킬 시전 중에 시전 가능, 따라서 스킬 시전 할 때 isSkillsCasted[index] = true로 안함
    */
    [SerializeField] bool[] isSkillsCasted = new bool[18];

    // delegate들
    public delegate void OnShiledSkillActivated(); // 쉴드 스킬이 켜 질때
    public OnShiledSkillActivated onShiledSkillActivated;

    public delegate void OnShiledSkillUnActivated(); // 쉴드 스킬이 꺼 질때
    public OnShiledSkillUnActivated onShiledSkillUnActivated;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제한다.
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        // 아래의 함수를 사용하여 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않는다.
        DontDestroyOnLoad(gameObject);

        Init(); // skillData 초기화
    }
    void OnEnable()
    {
        // 씬 매니저의 sceneLoaded에 체인을 건다.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 쿨타임 초기화
        for (int i = 0; i < isSkillsCasted.Length; i++) 
        {
            isSkillsCasted[i] = false;
            attackDelayTimer[i] = skillData.Delay[i];
        }
    }

    private void Update()
    {
        bool isSplashScene =
            SceneManager.GetActiveScene().name == "Lobby" ||
            SceneManager.GetActiveScene().name == "Splash1" || 
            SceneManager.GetActiveScene().name == "Splash2" ||
            SceneManager.GetActiveScene().name == "Splash3";

        if (isSplashScene) return;

        for (int i = 0; i < skillData.Damage.Length; i++)
        {
            if (skillData.skillSelected[i]) // 활성화(선택)된 스킬만 실행
            {
                bool shouldBeAttack = 0 >= attackDelayTimer[i] && !isSkillsCasted[i]; // 쿨타임이 됐는지 확인
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
                    if(!isSkillsCasted[i])
                        attackDelayTimer[i] -= Time.deltaTime;
                }
            }
        }
    }

    // skilldata를 초기화
    public void Init()
    {
        // Skill Data 초기화
        for (int i = 0; i < skillData.level.Length; i++) { skillData.level[i] = 0; }

        skillData.Damage[0] = 10f;
        skillData.Damage[1] = 5f;
        skillData.Damage[2] = 3f; // dot damage skill
        skillData.Damage[3] = 20f;
        skillData.Damage[4] = 10f;
        skillData.Damage[5] = 0f;
        skillData.Damage[6] = 40f;
        skillData.Damage[7] = 12f;  // dot damage skill
        skillData.Damage[8] = 12f;  // dot damage skill
        skillData.Damage[9] = 80f;
        skillData.Damage[10] = 80f;
        skillData.Damage[11] = 24f; // dot damage skill
        // 여기부터 공명 스킬
        skillData.Damage[12] = 5f;    // dot damage skill
        skillData.Damage[13] = 60f;  // dot damage skill
        skillData.Damage[14] = 40f;  // dot damage skill
        skillData.Damage[15] = 12f;   // dot damage skill
        skillData.Damage[16] = 40f;  // dot damage skill
        skillData.Damage[17] = 200f;

        skillData.Delay[0] = 2f;
        skillData.Delay[1] = 1.5f;
        skillData.Delay[2] = 2f;
        skillData.Delay[3] = 2f;
        skillData.Delay[4] = 5f;
        skillData.Delay[5] = 6f;
        skillData.Delay[6] = 2f;
        skillData.Delay[7] = 3f;
        skillData.Delay[8] = 2f;
        skillData.Delay[9] = 3;
        skillData.Delay[10] = 3f;
        skillData.Delay[11] = 3f;
        // 여기부터 공명 스킬
        skillData.Delay[12] = 2.6f;
        skillData.Delay[13] = 2.5f;
        skillData.Delay[14] = 4f;
        skillData.Delay[15] = 4f;
        skillData.Delay[16] = 2.55f;
        skillData.Delay[17] = 0.4f;

        skillData.scale[0] = 1.5f;
        skillData.scale[1] = 1f;
        skillData.scale[2] = 2f;
        skillData.scale[3] = 1.5f;
        skillData.scale[4] = 1.5f;
        skillData.scale[5] = 1f;
        skillData.scale[6] = 1.5f;
        skillData.scale[7] = 1.5f;
        skillData.scale[8] = 1.8f;
        skillData.scale[9] = 1.75f;
        skillData.scale[10] = 1.5f;
        skillData.scale[11] = 1.5f;

        for (int i = 0; i < skillData.skillSelected.Length; i++) { skillData.skillSelected[i] = false; }

        // Passive Skill Data 초기화
        for (int i = 0; i < passiveSkillData.level.Length; i++) { passiveSkillData.level[i] = 0; }

        passiveSkillData.Damage[0] = 1f;
        passiveSkillData.Damage[1] = 1f;
        passiveSkillData.Damage[2] = 1f;
        passiveSkillData.Damage[3] = 1f;
        passiveSkillData.Damage[4] = 1f;
        passiveSkillData.Damage[5] = 0.25f;
        for (int i = 0; i < passiveSkillData.skillSelected.Length; i++) { passiveSkillData.skillSelected[i] = false; }

        isBossAppear = false;
        isFire3SkillLeftRight = false;
        skyFallQuadrantNum = -1;
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
                        Enemy enemy = new Enemy();

                        bool isInAttackRange = false;

                        List<Enemy> findEnemies = enemies.ToList();
                        while (findEnemies.Count > 0)
                        {
                            int ranNum = UnityEngine.Random.Range(0, findEnemies.Count);
                            
                            enemy = findEnemies[ranNum];
                            
                            if (!(enemy == null))
                            {
                                float distance = Vector2.Distance(enemy.transform.position, player.transform.position);
                                
                                isInAttackRange = distance <= nearAttackRange; // 적이 사거리 내에 있을때만 공격이 나간다

                                if (isInAttackRange)
                                    break;
                            }
                            findEnemies.Remove(enemy);
                        }
                        if (enemies.IndexOf(enemy) == -1) return; // 적이 없으면 공격 X

                        if (isInAttackRange) { 
                            CastSkill(enemy, index);
                        } else { // 근접 우선 사거리 내에 적이 없으면 원거리 범위 적을 찾는다

                            findEnemies = enemies.ToList();

                            isInAttackRange = false;

                            while (findEnemies.Count > 0)
                            {
                                int ranNum = UnityEngine.Random.Range(0, findEnemies.Count);
                                
                                enemy = findEnemies[ranNum];

                                if (!(enemy == null))
                                {
                                    float distance = Vector2.Distance(enemy.transform.position, player.transform.position);

                                    isInAttackRange = distance <= attackRange; // 적이 사거리 내에 있을때만 공격이 나간다

                                    if (isInAttackRange)
                                        break;
                                }
                                findEnemies.Remove(enemy);
                            }
                        }

                        if (enemies.IndexOf(enemy) == -1) return; // 적이 없으면 공격 X

                        if (isInAttackRange)
                            CastSkill(enemy, index);

                        break; // switch 문의 break
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
                        bool isInAttackRange;

                        float distance = Vector2.Distance(boss.transform.position, player.transform.position);

                        isInAttackRange = distance <= attackRange; // 적이 사거리 내에 있을때만 공격이 나간다

                        if (isInAttackRange)
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
                    enemyTrackingSkill = GameManager.instance.poolManager.GetSkill(0, enemy) as Fireball;
                    

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
                    
                    enemyTrackingSkill.aliveTime = 1f;

                    enemyTrackingSkill.speed = 25;
                    enemyTrackingSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[0];

                    enemyTrackingSkill.skillIndex = index;

                    SetScale(enemyTrackingSkill.gameObject, index);

                    break;
                }
            case 1:
                {
                    enemyOnSkill = GameManager.instance.poolManager.GetSkill(7, enemy) as Lightning;
                    

                    enemyOnSkill.enemy = enemy;
                    enemyOnSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[1];

                    enemyOnSkill.skillIndex = index;

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
                    enemyTrackingSkill = GameManager.instance.poolManager.GetSkill(0, boss) as Fireball;
                    

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

                    enemyTrackingSkill.aliveTime = 1f;

                    // enemyTrackingSkill.boss = boss;
                    enemyTrackingSkill.isBossAppear = true;

                    enemyTrackingSkill.speed = 25;
                    enemyTrackingSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[0];

                    enemyTrackingSkill.skillIndex = index;

                    SetScale(enemyTrackingSkill.gameObject, index);

                    break;
                }
            case 1:
                {
                    enemyOnSkill = GameManager.instance.poolManager.GetSkill(7, boss) as Lightning;
                    

                    enemyOnSkill.aliveTime = 0.5f;

                    enemyOnSkill.isBossAppear = true;
                    enemyOnSkill.boss = boss;

                    enemyOnSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[1];

                    enemyOnSkill.skillIndex = index;

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
                    playerAttachSkill = GameManager.instance.poolManager.GetSkill(11) as Water_Shot;
                    

                    //playerAttachSkill.player = player; player는 현재 PoolManager에서 할당중

                    if (skillData.level[index] == 5)
                    {
                        playerAttachSkill.xPositionNum = 4 * 1.5f;
                    }
                    else if (skillData.level[index] >= 3)
                    {
                        playerAttachSkill.xPositionNum = 4 * 1.25f;
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
                    playerAttachSkill.isDotDamageSkill = true;

                    playerAttachSkill.aliveTime = 0.5f;

                    playerAttachSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[2];

                    playerAttachSkill.skillIndex = index;

                    SetScale(playerAttachSkill.gameObject, index);

                    playerAttachSkill.onSkillFinished = OnSkillFinished;
                    isSkillsCasted[index] = true;

                    break;
                }
            case 3:
                {
                    playerAttachSkill = GameManager.instance.poolManager.GetSkill(1) as Explosion;
                    
                    playerAttachSkill.player = player;

                    playerAttachSkill.xPositionNum = 0;
                    playerAttachSkill.yPositionNum = 0.2f;

                    playerAttachSkill.isAttachSkill = true;

                    playerAttachSkill.X = player.transform.position.x;
                    playerAttachSkill.Y = player.transform.position.y;

                    playerAttachSkill.aliveTime = 0.5f;

                    playerAttachSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[0];

                    playerAttachSkill.skillIndex = index;

                    SetScale(playerAttachSkill.gameObject, index);

                    playerAttachSkill.onSkillFinished = OnSkillFinished;
                    isSkillsCasted[index] = true;

                    break;
                }
            case 4:
                {
                    CastElectricBall();
                    break;
                }
            case 5:
                {
                    playerAttachSkill = GameManager.instance.poolManager.GetSkill(12) as Water_Shield;
                    

                    //playerAttachSkill.player = player;

                    playerAttachSkill.X = player.transform.position.x;
                    playerAttachSkill.Y = player.transform.position.y;

                    playerAttachSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[2];

                    playerAttachSkill.aliveTime = 3f;

                    ((Water_Shield)playerAttachSkill).onShieldSkillDestroyed = OnShieldSkillDestroyed;

                    playerAttachSkill.skillIndex = index;

                    onShiledSkillActivated();

                    playerAttachSkill.onSkillFinished = OnSkillFinished;
                    isSkillsCasted[index] = true;

                    break;
                }
            case 6:
                {
                    randomSkill = GameManager.instance.poolManager.GetSkill(3) as Meteor;
                    

                    randomSkill.player = player;

                    randomSkill.aliveTime = 0.5f;

                    randomSkill.speed = 25f;

                    randomSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[0];
                    randomSkill.scale = skillData.scale[index];

                    randomSkill.skillIndex = index;

                    SetScale(randomSkill.gameObject, index);

                    randomSkill.onSkillFinished = OnSkillFinished;
                    isSkillsCasted[index] = true;

                    break;
                }
            case 7:
                {
                    playerAttachSkill = GameManager.instance.poolManager.GetSkill(9) as Energy_Blast;
                    
                    //playerAttachSkill.player = player;

                    playerAttachSkill.xPositionNum = 11f;
                    playerAttachSkill.yPositionNum = -0.2f;

                    playerAttachSkill.X = 999f;
                    playerAttachSkill.Y = 999f;

                    playerAttachSkill.isAttachSkill = true;
                    playerAttachSkill.isDotDamageSkill = true;

                    ((Energy_Blast)playerAttachSkill).delay = 0.45f;
                    playerAttachSkill.aliveTime = 0.8f;

                    playerAttachSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[1];

                    playerAttachSkill.skillIndex = index;

                    Transform parent = playerAttachSkill.transform.parent;

                    playerAttachSkill.transform.parent = null;
                    playerAttachSkill.transform.localScale = new Vector3(playerAttachSkill.transform.localScale.x, skillData.scale[index], 0);
                    playerAttachSkill.transform.parent = parent;

                    playerAttachSkill.onSkillFinished = OnSkillFinished;
                    isSkillsCasted[index] = true;

                    break;
                }
            case 8:
                {
                    randomSkill = GameManager.instance.poolManager.GetSkill(13) as Ice_Spike;
                    
                    float tmpX = player.transform.position.x;
                    float tmpY = player.transform.position.y;

                    float ranNum = UnityEngine.Random.Range(-14f, 14f);
                    float ranNum2 = UnityEngine.Random.Range(-6f, 6f);

                    tmpX += ranNum;
                    tmpY += ranNum2;

                    randomSkill.X = tmpX;
                    randomSkill.Y = tmpY;

                    randomSkill.isDotDamageSkill = true;

                    randomSkill.aliveTime = 3f;
                    randomSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[2];

                    randomSkill.skillIndex = index;

                    SetScale(randomSkill.gameObject, index);

                    break;
                }
            case 9:
                {
                    if (isFire3SkillLeftRight)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            playerAttachSkill = GameManager.instance.poolManager.GetSkill(6) as Twin_Flame;

                            if (i == 0)
                            {
                                playerAttachSkill.X = 999f;
                                playerAttachSkill.Y = 999f;

                                if (skillData.level[index] == 5)
                                {
                                    playerAttachSkill.xPositionNum = - 4 * 1.5f;
                                }
                                else if (skillData.level[index] >= 3)
                                {
                                    playerAttachSkill.xPositionNum = -4 * 1.25f;
                                }
                                else
                                {
                                    playerAttachSkill.xPositionNum = -4f;
                                }
                                playerAttachSkill.yPositionNum = 0f;

                                //playerAttachSkill.player = player;

                                playerAttachSkill.isAttachSkill = true;
                                playerAttachSkill.isFlipped = false;

                                playerAttachSkill.aliveTime = 0.8f;
                                playerAttachSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[0];

                                playerAttachSkill.skillIndex = index;

                                SetScale(playerAttachSkill.gameObject, index);

                                playerAttachSkill.onSkillFinished = OnSkillFinished;
                                isSkillsCasted[index] = true;
                            }
                            else
                            {
                                playerAttachSkill.X = 999f;
                                playerAttachSkill.Y = 999f;

                                if (skillData.level[index] == 5)
                                {
                                    playerAttachSkill.xPositionNum = 4 * 1.5f;
                                }
                                else if (skillData.level[index] >= 3)
                                {
                                    playerAttachSkill.xPositionNum =  4 * 1.25f;
                                }
                                else
                                {
                                    playerAttachSkill.xPositionNum = 4f;
                                }
                                playerAttachSkill.yPositionNum = 0f;

                                //playerAttachSkill.player = player;

                                playerAttachSkill.isAttachSkill = true;
                                playerAttachSkill.isFlipped = true;

                                playerAttachSkill.aliveTime = 0.8f;
                                playerAttachSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[0];

                                playerAttachSkill.skillIndex = index;

                                SetScale(playerAttachSkill.gameObject, index);

                                playerAttachSkill.onSkillFinished = OnSkillFinished;
                                isSkillsCasted[index] = true;
                            }
                        }
                        isFire3SkillLeftRight = false;
                    }
                    else
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            playerAttachSkill = GameManager.instance.poolManager.GetSkill(5) as Twin_Flame;

                            if (i == 0)
                            {
                                playerAttachSkill.X = 999f;
                                playerAttachSkill.Y = 999f;

                                playerAttachSkill.xPositionNum = 0f;
                                if (skillData.level[index] == 5)
                                {
                                    playerAttachSkill.yPositionNum = 1.5f * 1.5f;
                                }
                                else if (skillData.level[index] >= 3)
                                {
                                    playerAttachSkill.yPositionNum = 1.5f * 1.25f;
                                }
                                else
                                {
                                    playerAttachSkill.yPositionNum = 1.5f;
                                }

                                playerAttachSkill.GetComponent<SpriteRenderer>().flipY = true;

                                //playerAttachSkill.player = player;

                                playerAttachSkill.isAttachSkill = true;

                                playerAttachSkill.aliveTime = 1f;
                                playerAttachSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[0];

                                playerAttachSkill.skillIndex = index;

                                SetScale(playerAttachSkill.gameObject, index);

                                playerAttachSkill.onSkillFinished = OnSkillFinished;
                                isSkillsCasted[index] = true;
                            }
                            else
                            {
                                playerAttachSkill.X = 999f;
                                playerAttachSkill.Y = 999f;

                                playerAttachSkill.xPositionNum = 0f;
                                if (skillData.level[index] == 5)
                                {
                                    playerAttachSkill.yPositionNum = -2.5f * 1.5f;
                                }
                                else if (skillData.level[index] >= 3)
                                {
                                    playerAttachSkill.yPositionNum = -2.5f * 1.25f;
                                }
                                else
                                {
                                    playerAttachSkill.yPositionNum = -2.5f;
                                }

                                //playerAttachSkill.player = player;

                                playerAttachSkill.isAttachSkill = true;

                                playerAttachSkill.aliveTime = 1f;
                                playerAttachSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[0];

                                playerAttachSkill.skillIndex = index;

                                SetScale(playerAttachSkill.gameObject, index);

                                playerAttachSkill.onSkillFinished = OnSkillFinished;
                                isSkillsCasted[index] = true;
                            }

                        }
                        isFire3SkillLeftRight = true;
                    }

                    break;
                }
            case 10:
                {
                    StartCoroutine(CastJudgement(index, 10));

                    break;
                }
            case 11:
                {
                    playerAttachSkill = GameManager.instance.poolManager.GetSkill(14) as Ice_Blast;

                    //playerAttachSkill.player = player;

                    if (skillData.level[index] == 5)
                    {
                        playerAttachSkill.xPositionNum = 4.5f * 1.5f;
                    }
                    else if (skillData.level[index] >= 3)
                    {
                        playerAttachSkill.xPositionNum = 4.5f * 1.25f;
                    }
                    else
                    {
                        playerAttachSkill.xPositionNum = 4.5f;
                    }
                    playerAttachSkill.yPositionNum = -0.2f;

                    playerAttachSkill.X = 999f;
                    playerAttachSkill.Y = 999f;

                    playerAttachSkill.isAttachSkill = true;
                    playerAttachSkill.isYFlipped = true;
                    playerAttachSkill.isDotDamageSkill = true;

                    playerAttachSkill.aliveTime = 3f;

                    playerAttachSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[2];

                    playerAttachSkill.skillIndex = index;

                    SetScale(playerAttachSkill.gameObject, index);

                    playerAttachSkill.onSkillFinished = OnSkillFinished;
                    isSkillsCasted[index] = true;

                    break;
                }
            case 12:
                {
                    skill = GameManager.instance.poolManager.GetSkill(15) as HeavensEclipse;

                    skill.aliveTime = 2.6f;
                    skill.isDotDamageSkill = true;

                    skill.damage = skillData.Damage[index] * passiveSkillData.Damage[0] * passiveSkillData.Damage[2];
                    ((HeavensEclipse)skill).burstDamage = 60f * passiveSkillData.Damage[0] * passiveSkillData.Damage[2];

                    skill.skillIndex = index;

                    skill.onSkillFinished = OnSkillFinished;
                    isSkillsCasted[index] = true;

                    break;
                }
            case 13:
                {
                    playerAttachSkill = GameManager.instance.poolManager.GetSkill(16) as BeamLaser;

                    playerAttachSkill.xPositionNum = 11f;
                    playerAttachSkill.yPositionNum = -0.2f;

                    playerAttachSkill.isAttachSkill = true;
                    playerAttachSkill.isDotDamageSkill = true;
                    playerAttachSkill.isYFlipped = true;

                    ((BeamLaser)playerAttachSkill).delay = 1f;
                    playerAttachSkill.aliveTime = 2.5f;

                    playerAttachSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[0] * passiveSkillData.Damage[1];

                    playerAttachSkill.skillIndex = index;

                    Transform parent = playerAttachSkill.transform.parent;

                    playerAttachSkill.transform.parent = null;
                    playerAttachSkill.transform.parent = parent;

                    playerAttachSkill.onSkillFinished = OnSkillFinished;

                    playerAttachSkill.skillIndex = index;

                    isFire3SkillLeftRight = false;

                    isSkillsCasted[index] = true;
                    
                    break;
                }
            case 14:
                {
                    playerAttachSkill = GameManager.instance.poolManager.GetSkill(17) as HydroFlame;

                    playerAttachSkill.isAttachSkill = true;
                    playerAttachSkill.isDotDamageSkill = true;

                    playerAttachSkill.aliveTime = 4f;

                    playerAttachSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[1] * passiveSkillData.Damage[2];

                    playerAttachSkill.skillIndex = index;

                    Transform parent = playerAttachSkill.transform.parent;

                    playerAttachSkill.transform.parent = null;
                    playerAttachSkill.transform.parent = parent;

                    playerAttachSkill.onSkillFinished = OnSkillFinished;

                    isSkillsCasted[index] = true;

                    break;
                }
            case 15:
                {
                    playerAttachSkill = GameManager.instance.poolManager.GetSkill(18) as Shield_Flame;

                    playerAttachSkill.X = player.transform.position.x;
                    playerAttachSkill.Y = player.transform.position.y;

                    playerAttachSkill.xPositionNum = 0.08f * 8;

                    playerAttachSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[0] * passiveSkillData.Damage[2];

                    playerAttachSkill.aliveTime = 4f;
                    playerAttachSkill.isDotDamageSkill = true;

                    ((Shield_Flame)playerAttachSkill).onShieldSkillDestroyed = OnShieldSkillDestroyed;

                    playerAttachSkill.skillIndex = index;

                    onShiledSkillActivated();

                    playerAttachSkill.onSkillFinished = OnSkillFinished;
                    isSkillsCasted[index] = true;

                    break;
                }
            case 16:
                {
                    randomSkill = GameManager.instance.poolManager.GetSkill(19) as Sky_Fall;

                    float tmpX = player.transform.position.x;
                    float tmpY = player.transform.position.y;

                    // 스킬이 플레이어 기준 1 ~ 4분면 중 어디에 시전될 까
                    // 이전에 떨어졌던 사분면에는 떨어지지 않음
                    int quadrantNum;
                    do
                    {
                        quadrantNum = UnityEngine.Random.Range(1, 5);
                    }
                    while (skyFallQuadrantNum == quadrantNum);

                    skyFallQuadrantNum = quadrantNum;

                    float ranNumX = 0;
                    float ranNumY = 0;
                    switch (quadrantNum)
                    {
                        case 1:
                            ranNumX = UnityEngine.Random.Range(2f, 10f);
                            ranNumY = UnityEngine.Random.Range(1f, 5f);
                            break;

                        case 2:
                            ranNumX = UnityEngine.Random.Range(-10f, -2f);
                            ranNumY = UnityEngine.Random.Range(1f, 5f);
                            break;

                        case 3:
                            ranNumX = UnityEngine.Random.Range(-10f, -2f);
                            ranNumY = UnityEngine.Random.Range(-5f, -1f);
                            break;

                        case 4:
                            ranNumX = UnityEngine.Random.Range(2f, 10f);
                            ranNumY = UnityEngine.Random.Range(-5f, -1f);
                            break;
                    }
                    
                    tmpX += ranNumX;
                    tmpY += ranNumY;
                    
                    randomSkill.X = tmpX;
                    randomSkill.Y = tmpY;

                    randomSkill.aliveTime = 2.55f;
                    randomSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[0] * passiveSkillData.Damage[1];

                    ((Sky_Fall)randomSkill).delay = 0.65f;
                    randomSkill.isDotDamageSkill = true;

                    randomSkill.skillIndex = index;

                    break;
                }
            case 17:
                {
                    randomSkill = GameManager.instance.poolManager.GetSkill(20) as Frozen_Spike;

                    float tmpX = player.transform.position.x;
                    float tmpY = player.transform.position.y;

                    // 스킬이 플레이어 기준 1 ~ 4분면 중 어디에 시전될 까
                    // 이전에 떨어졌던 사분면에는 떨어지지 않음
                    int quadrantNum = -1;

                    List<int> qudrantNums = new List<int> { 1, 2, 3, 4 };

                    for(int i = 0; i < qudrantNums.Count; i++)
                    {
                        quadrantNum = UnityEngine.Random.Range(1, 5);
                        if (skyFallQuadrantNum == quadrantNum)
                            qudrantNums.Remove(quadrantNum);
                        else
                        {
                            skyFallQuadrantNum = quadrantNum;
                            break;
                        }
                    }

                    float ranNumX = 0;
                    float ranNumY = 0;
                    switch (quadrantNum)
                    {
                        case 1:
                            ranNumX = UnityEngine.Random.Range(0, 16f);
                            ranNumY = UnityEngine.Random.Range(0, 7.5f);
                            break;

                        case 2:
                            ranNumX = UnityEngine.Random.Range(-16f, 0);
                            ranNumY = UnityEngine.Random.Range(0, 7.5f);
                            break;

                        case 3:
                            ranNumX = UnityEngine.Random.Range(-16f, 0);
                            ranNumY = UnityEngine.Random.Range(0, -7.5f);
                            break;

                        case 4:
                            ranNumX = UnityEngine.Random.Range(0, 16f);
                            ranNumY = UnityEngine.Random.Range(-7.5f, 0);
                            break;
                        default:
                            Debug.Log("사분면을 찾지 못했습니다.");
                            break;
                    }

                    tmpX += ranNumX;
                    tmpY += ranNumY;

                    randomSkill.X = tmpX;
                    randomSkill.Y = tmpY;

                    randomSkill.aliveTime = 2f;
                    randomSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[1] * passiveSkillData.Damage[2];

                    ((Frozen_Spike)randomSkill).delay = 0.55f;

                    randomSkill.skillIndex = index;

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

    private void CastElectricBall()
    {
        float tmpDegree = 0f;
        float circleSpeed = 6f;

        if (skillData.level[4] == 5)
        {
            for(int i = 0; i < 3; i++)
            {
                playerAttachSkill = GameManager.instance.poolManager.GetSkill(8) as Electric_Ball;
                
                ((Electric_Ball)playerAttachSkill).degree = tmpDegree;
                tmpDegree -= 120f;

                playerAttachSkill.xPositionNum = 4f;
                playerAttachSkill.aliveTime = 5f * 1.5f;

                playerAttachSkill.X = player.transform.position.x + 3f;
                playerAttachSkill.Y = player.transform.position.y;

                playerAttachSkill.yPositionNum = 0f;

                playerAttachSkill.speed = circleSpeed;

                playerAttachSkill.damage = skillData.Damage[4] * passiveSkillData.Damage[1];

                playerAttachSkill.skillIndex = 4;

                SetScale(playerAttachSkill.gameObject, 4);

                playerAttachSkill.onSkillFinished = OnSkillFinished;
                isSkillsCasted[4] = true;
            }
        }
        else if (skillData.level[4] >= 3)
        {
            for(int i = 0; i < 2; i++)
            {
                playerAttachSkill = GameManager.instance.poolManager.GetSkill(8) as Electric_Ball;

                ((Electric_Ball)playerAttachSkill).degree = tmpDegree;
                tmpDegree -= 180f;

                playerAttachSkill.xPositionNum = 3.5f;
                playerAttachSkill.aliveTime = 5f * 1.25f;

                //playerAttachSkill.player = player;

                playerAttachSkill.X = player.transform.position.x + 3f;
                playerAttachSkill.Y = player.transform.position.y;

                playerAttachSkill.yPositionNum = 0f;

                playerAttachSkill.speed = circleSpeed;

                playerAttachSkill.damage = skillData.Damage[4] * passiveSkillData.Damage[1];

                playerAttachSkill.skillIndex = 4;

                SetScale(playerAttachSkill.gameObject, 4);

                playerAttachSkill.onSkillFinished = OnSkillFinished;
                isSkillsCasted[4] = true;
            }
        }
        else
        {
            playerAttachSkill = GameManager.instance.poolManager.GetSkill(8) as Electric_Ball;

            ((Electric_Ball)playerAttachSkill).degree = tmpDegree;

            playerAttachSkill.xPositionNum = 3f;
            playerAttachSkill.aliveTime = 5f;

            playerAttachSkill.X = player.transform.position.x + 3f;
            playerAttachSkill.Y = player.transform.position.y;

            playerAttachSkill.yPositionNum = 0f;

            playerAttachSkill.speed = circleSpeed;

            playerAttachSkill.damage = skillData.Damage[4] * passiveSkillData.Damage[1];

            playerAttachSkill.skillIndex = 4;

            SetScale(playerAttachSkill.gameObject, 4);

            playerAttachSkill.onSkillFinished = OnSkillFinished;
            isSkillsCasted[4] = true;
        }
    }

    // Judgment 스킬 쓸 때 일정 딜레이로 스킬 cast하기 위함
    IEnumerator CastJudgement(int index, int num)
    {
        for (int i = 0; i < num; i++)
        {
            randomSkill = GameManager.instance.poolManager.GetSkill(10) as Judgement;

            float tmpX = player.transform.position.x;
            float tmpY = player.transform.position.y;

            float ranNum = UnityEngine.Random.Range(-10f, 10f);
            float ranNum2 = UnityEngine.Random.Range(-5f, 5f);

            tmpX += ranNum;
            tmpY += ranNum2;

            randomSkill.X = tmpX;
            randomSkill.Y = tmpY;

            //randomSkill.player = player;

            randomSkill.aliveTime = 0.8f;
            randomSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[1];

            randomSkill.skillIndex = 10;

            SetScale(randomSkill.gameObject, index);

            randomSkill.onSkillFinished = OnSkillFinished;
            isSkillsCasted[10] = true;

            yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기
        }
    }

    // Laser 스킬 앞으로 다다다다 나가게 하기 위함
    // 현재 안쓰고 있음
    IEnumerator CastLaser(int index, int num)
    {
        int plusMinus;

        if (player.isPlayerLookLeft)
            plusMinus = -1;
        else
            plusMinus = 1;

        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;

        for (int i = 0; i < num; i++)
        {
            playerAttachSkill = GameManager.instance.poolManager.GetSkill(17) as BeamLaser;

            int xNum = (i + 1) * 5 * plusMinus;

            playerAttachSkill.X = playerX + xNum;
            playerAttachSkill.Y = playerY - 2f;
            
            playerAttachSkill.isAttachSkill = true;
            playerAttachSkill.isDotDamageSkill = true;

            ((BeamLaser)playerAttachSkill).delay = 1.2f;
            playerAttachSkill.aliveTime = 2.5f;

            //((BeamLaser)playerAttachSkill).isBeam = false;

            playerAttachSkill.damage = skillData.Damage[index] * passiveSkillData.Damage[0] * passiveSkillData.Damage[1];

            playerAttachSkill.skillIndex = index;

            Transform parent = playerAttachSkill.transform.parent;

            playerAttachSkill.transform.parent = null;
            playerAttachSkill.transform.parent = parent;

            playerAttachSkill.onSkillFinished = OnSkillFinished;
            isSkillsCasted[index] = true;

            yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기
        }

        isSkillsCasted[index] = true;

        isFire3SkillLeftRight = true;
    }

    // 쿨타임 초기화 함수
    public void ResetDelayTimer(int index)
    {
        attackDelayTimer[index] = 0;
    }

    // 스킬이 꺼질 때 스킬이 delegate를 통해 호출 할 함수
    void OnSkillFinished(int index)
    {
        isSkillsCasted[index] = false;
    }
}


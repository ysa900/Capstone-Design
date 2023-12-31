using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pause 걸면 이전에는 인게임 속 UI들(피통, 스킬 패널, 프로필)이 안사라져서
// 사라지게 하려고 gameObject로 선언한거랑
// OnPauseButtonClicked() ,onPlayButtonClicked() 메소드 수정했음

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager를 instance화

    // 게임 시간
    public float gameTime;
    public float maxGameTime = 5 * 60f;

    // 적 스폰 쿨타임
    private float CoolTime = 2f;
    private float CoolTimer = 0f;

    // 적 최대 생성 거리 (최소는 20, EnemyManager에 있음)
    private float maxEnemySpawnRange = 30;

    // GameOver가 됐는지 판별하는 변수
    public bool isGameOver;

    // 보스가 이미 스폰 됐는지 판별하는 변수
    private bool isBossSpawned;

    // 적이 너무 많은지(300마리 이상) 판별하는 변수
    private bool isEnemiesTooMany;

    // Enemy들을 담을 리스트
    private List<Enemy> enemies = new List<Enemy>();

    // 사용할 클래스 객체들
    public Player player;
    public Boss boss;
    private EnemyManager enemyManager;
    private GameAudioManager gameAudioManager;
    private FollowCam followCam;
    private InputManager inputManager;
    private SkillManager skillManager;
    private SkillSelectManager skillSelectManager;
    private EXP exp;
    private BossManager bossManager;
    public PoolManager poolManager;

    // GameObject에서 프리팹을 넣어주기 위해 public으로 설정
    public Player playerPrefab;

    // EXP 프리팹
    public EXP expPrefab1;
    public EXP expPrefab2;
    public EXP expPrefab3;

    // GameOver 오브젝트
    public GameObject gameOverObject;
    // GameClear 오브젝트
    public GameObject gameClearObject;

    // Pause 오브젝트
    public GameObject pauseObject;

    // HpBar
    public GameObject HpBarObject;
    // HpStatus
    public GameObject HpStatusObject;
    // HpStatusLettering
    public GameObject HpStatusLetteringObject;
    // SkillPenel
    public GameObject SkillPanelObject;
    // CharacterProfile
    public GameObject CharacterProfileObject;
    // Boss HP
    public GameObject BossHPObject;

    private void Awake()
    {
        instance = this; // GameManager를 인스턴스화

        // 시작 시 비활성화
        gameOverObject.SetActive(false);
        gameClearObject.SetActive(false);
        pauseObject.SetActive(false);
        HpBarObject.SetActive(true);
        BossHPObject.SetActive(false);

        // 클래스 객체들 초기화
        CreatePlayer();
        enemyManager = FindAnyObjectByType<EnemyManager>();
        inputManager = FindAnyObjectByType<InputManager>();
        gameAudioManager = FindAnyObjectByType<GameAudioManager>();
        followCam = FindAnyObjectByType<FollowCam>();
        skillManager = FindAnyObjectByType<SkillManager>();
        skillSelectManager = FindAnyObjectByType<SkillSelectManager>();
        bossManager = FindAnyObjectByType<BossManager>();
        poolManager = FindAnyObjectByType<PoolManager>();

        // inputManger Delegate 할당
        inputManager.onPauseButtonClicked = OnPauseButtonClicked;
        inputManager.onPlayButtonClicked = onPlayButtonClicked;

        // followCam 플레이어 객체 할당
        followCam.player = player;

        // skillManager에 객체 할당
        skillManager.player = player;

        // skillManager Delegate 할당
        skillManager.onShiledSkillActivated = OnShieldSkillActivated;
        skillManager.onShiledSkillUnActivated = OnShieldSkillUnActivated;

        // delegate 할당
        enemyManager.onEnemiesChanged = OnEnemiesChanged;
        enemyManager.onEnemyKilled = OnEnemyKilled;

        // delegate 할당
        skillSelectManager.onSkillSelectObjectDisplayed = OnSkillSelectObjectDisplayed;
        skillSelectManager.onSkillSelectObjectHided = OnSkillSelectObjectHided;
        skillSelectManager.onPlayerHealed = OnPlayerHealed;

        // delegate 할당
        bossManager.onBossHasKilled = OnBossHasKilled;

        //gameTime = 60 * 5f;
        //player.isPlayerShielded = true;
    }

    void Start()
    {
        gameAudioManager.PlaySfx(GameAudioManager.Sfx.Select); // GameStart 선택 효과음
        gameAudioManager.PlayBGM(0, true); // 배경음 시작
        enemyManager.CreateEnemies(50, player, 0, maxEnemySpawnRange); // 몬스터 소환
        skillSelectManager.ChooseStartSkill(); // 시작 스킬 선택
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameOver) {
            gameTime += Time.deltaTime; // 게임 시간 증가
            CoolTimer += Time.deltaTime;

            if(!isBossSpawned) {
                isEnemiesTooMany = enemies.Count > 300;
                SpawnBoss();
                if (!isEnemiesTooMany)
                    CalculateEnemySpawnTimeNSpawn(); // 소환할 적을 지정하고 스폰
            }
        }

        skillManager.enemies = enemies;
    }

    // Player 생성 함수
    private void CreatePlayer(){
        player = Instantiate(playerPrefab);
        player.onPlayerWasKilled = OnPlayerHasKilled;
        player.onPlayerLevelUP = OnPlayerLevelUP;
    }

    // Enemy 스폰 시간을 계산해 소환할 적을 지정하는 함수
    private void CalculateEnemySpawnTimeNSpawn()
    {
        if (gameTime <= 60 * 1 && CoolTimer >= CoolTime)
        {
            enemyManager.CreateEnemies(10, player, 0, maxEnemySpawnRange); // Ghoul 몬스터 소환
            CoolTimer = 0f;
        }
        else if (gameTime <= 60 * 2 && CoolTimer >= CoolTime)
        {
            enemyManager.CreateEnemies(5, player, 0, maxEnemySpawnRange); // Ghoul 몬스터 소환
            enemyManager.CreateEnemies(10, player, 1, maxEnemySpawnRange); // Spitter 몬스터 소환
            CoolTimer = 0f;
        }
        else if (gameTime <= 60 * 3 && CoolTimer >= CoolTime)
        {
            enemyManager.CreateEnemies(2, player, 0, maxEnemySpawnRange); // Ghoul 몬스터 소환
            enemyManager.CreateEnemies(5, player, 1, maxEnemySpawnRange); // Spitter 몬스터 소환
            enemyManager.CreateEnemies(10, player, 2, maxEnemySpawnRange); //Summoner 몬스터 소환
            CoolTime = 1.5f;
            CoolTimer = 0f;
        }
        else if (gameTime < 60 * 4 && CoolTimer >= CoolTime)
        {
            enemyManager.CreateEnemies(2, player, 0, maxEnemySpawnRange); // Ghoul 몬스터 소환
            enemyManager.CreateEnemies(5, player, 1, maxEnemySpawnRange); // Spitter 몬스터 소환
            enemyManager.CreateEnemies(8, player, 2, maxEnemySpawnRange); //Summoner 몬스터 소환
            enemyManager.CreateEnemies(15, player, 3, maxEnemySpawnRange); //BloodKing 몬스터 소환
            CoolTime = 1f;
            CoolTimer = 0f;
        }
        else if (gameTime < 60 * 5 && CoolTimer >= CoolTime)
        {
            enemyManager.CreateEnemies(2, player, 0, maxEnemySpawnRange); // Ghoul 몬스터 소환
            enemyManager.CreateEnemies(5, player, 1, maxEnemySpawnRange); // Spitter 몬스터 소환
            enemyManager.CreateEnemies(8, player, 2, maxEnemySpawnRange); //Summoner 몬스터 소환
            enemyManager.CreateEnemies(20, player, 3, maxEnemySpawnRange); //BloodKing 몬스터 소환
            CoolTime = 1f;
            CoolTimer = 0f;
        }
    }

    void SpawnBoss()
    {
        if (gameTime >= maxGameTime)
        {
            // 보스 등장
            bossManager.player = player;
            bossManager.CreateBoss();

            skillManager.isBossAppear = true;
            skillManager.boss = bossManager.boss;

            // 보스 HP바 active
            BossHPObject.SetActive(true);

            // 보스 BGM ON
            gameAudioManager.PlayBGM(0, false); // 기본 배경음 종료
            gameAudioManager.PlayBGM(1, true); // 보스 배경음 실행

            isBossSpawned = true;
        }
    }

    // 플레이어가 죽었을 시 실행됨
    private void OnPlayerHasKilled(Player player)
    {
        StartCoroutine(PlayerHasKilled()); // 효과음 넣기 위한 코루틴 생성 및 사용
    }
    IEnumerator PlayerHasKilled()
    {
        isGameOver = true;
        gameOverObject.SetActive(true);

        yield return new WaitForSeconds(0.5f); // 0.5초 이후 시간 차 두기
        gameAudioManager.PlaySfx(GameAudioManager.Sfx.Lose); // 캐릭터 사망 시 효과음
        inputManager.PauseButtonObject.interactable = false; // Pause버튼 비활성화
        gameAudioManager.PlayBGM(0, false); // 배경음 종료
        Time.timeScale = 0; // 화면 멈추기
    }

    // 보스가 죽었을 시 실행됨
    private void OnBossHasKilled()
    {
        StartCoroutine(BossHasKilled()); // 효과음 넣기 위한 코루틴 생성 및 사용
    }
    IEnumerator BossHasKilled()
    {
        player.isPlayerShielded = true;
        isGameOver = true;
        gameClearObject.SetActive(true);

        yield return new WaitForSeconds(0.5f); // 0.5초 이후 시간 차 두기
        gameAudioManager.PlaySfx(GameAudioManager.Sfx.Win); // 승리시 효과음
        inputManager.PauseButtonObject.interactable = false; // Pause버튼 비활성화
        gameAudioManager.PlayBGM(0, false); // 배경음 종료
        Time.timeScale = 0; // 화면 멈추기
    }

    // Pause버튼이 클릭됐을 시 실행됨
    private void OnPauseButtonClicked()
    {
        pauseObject.SetActive(true);
        gameAudioManager.PlaySfx(GameAudioManager.Sfx.Select); // 버튼 클릭 효과음
        // UI 비활성화
        HpBarObject.SetActive(false);
        HpStatusLetteringObject.SetActive(false);
        HpStatusObject.SetActive(false);
        SkillPanelObject.SetActive(false);
        CharacterProfileObject.SetActive(false);
    }

    private void onPlayButtonClicked()
    {
        pauseObject.SetActive(false);
        gameAudioManager.PlaySfx(GameAudioManager.Sfx.Select); // 버튼 클릭 효과음
        // UI 활성화
        HpBarObject.SetActive(true);
        HpStatusLetteringObject.SetActive(true);
        HpStatusObject.SetActive(true);
        SkillPanelObject.SetActive(true);
        CharacterProfileObject.SetActive(true);
    }

    private void OnEnemiesChanged(List<Enemy> enemies)
    {
        this.enemies = enemies;
    }

    // 쉴드 켜질 때 delegate에 할당해줄 함수
    private void OnShieldSkillActivated()
    {
        player.isPlayerShielded = true;
    }

    // 쉴드 꺼질 때 delegate에 할당해줄 함수
    private void OnShieldSkillUnActivated()
    {
        player.isPlayerShielded = false;
    }

    // 적이 죽었을 때 실행하는 함수
    private void OnEnemyKilled(Enemy killedEnemy)
    {
        if (!player.isPlayerDead)
        {
            player.kill++;
        }

        int ranNum = UnityEngine.Random.Range(0, 11);

        if(ranNum >= 6)
        {
            if (killedEnemy.tag == "Ghoul")
            {
                exp = Instantiate(expPrefab1);

                exp.expAmount = 1;

                exp.X = killedEnemy.X;
                exp.Y = killedEnemy.Y + 1f;
            }
            else if (killedEnemy.tag == "Spitter")
            {
                exp = Instantiate(expPrefab2);

                exp.expAmount = 2;

                exp.X = killedEnemy.X;
                exp.Y = killedEnemy.Y + 1f;
            }
            else if (killedEnemy.tag == "Summoner")
            {
                exp = Instantiate(expPrefab2);

                exp.expAmount = 3;

                exp.X = killedEnemy.X;
                exp.Y = killedEnemy.Y + 1f;
            }
            else if (killedEnemy.tag == "BloodKing")
            {
                exp = Instantiate(expPrefab3);

                exp.expAmount = 4;

                exp.X = killedEnemy.X;
                exp.Y = killedEnemy.Y + 1f;
            }
        }
    }

    // 플레이어가 레벨 업 했을 시 실행
    private void OnPlayerLevelUP()
    {
        skillSelectManager.DisplayLevelupPanel();
        gameAudioManager.PlaySfx(GameAudioManager.Sfx.LevelUp); // 레벨업 시 효과음
        gameAudioManager.EffectBGM(true); // AudioFilter 켜기
    }

    private void OnSkillSelectObjectDisplayed()
    {
        // UI 비활성화
        HpBarObject.SetActive(false);
        HpStatusLetteringObject.SetActive(false);
        HpStatusObject.SetActive(false);
        SkillPanelObject.SetActive(false);
        CharacterProfileObject.SetActive(false);
        inputManager.PauseButtonObject.interactable = false;
    }

    private void OnSkillSelectObjectHided()
    {
        // UI 활성화
        HpBarObject.SetActive(true);
        HpStatusLetteringObject.SetActive(true);
        HpStatusObject.SetActive(true);
        SkillPanelObject.SetActive(true);
        CharacterProfileObject.SetActive(true);
        inputManager.PauseButtonObject.interactable = true;
    }

    private void OnPlayerHealed()
    {
        player.hp += 10;
        if(player.hp > 100) { player.hp = 100; }
    }
}

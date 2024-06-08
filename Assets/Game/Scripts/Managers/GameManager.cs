using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameAudioManager;

// Pause 걸면 이전에는 인게임 속 UI들(피통, 스킬 패널, 프로필)이 안사라져서
// 사라지게 하려고 gameObject로 선언한거랑
// OnPauseButtonClicked() ,onPlayButtonClicked() 메소드 수정했음

public class GameManager : MonoBehaviour
{
    // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
    private static GameManager _instance;

    // 인스턴스에 접근하기 위한 프로퍼티
    public static GameManager instance
    {
        get
        {
            // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
            if (!_instance)
            {
                _instance = FindAnyObjectByType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    // 게임 시간
    public float gameTime;
    float sceneGameTime;
    public float maxGameTime = 5 * 60f; // 초기(Stage1) maxGameTime

    // 씬 번호
    // 0: splash, 1: Lobby, 2: Game, 3: Stage2
    public string sceneName;

    // 적 스폰 쿨타임
    private float coolTime = 2f;
    private float coolTimer = 0f;

    // player Area Size
    public int playerAreaSize;

    // GameOver가 됐는지 판별하는 변수
    public bool isGameOver;

    // 보스가 이미 스폰 됐는지 판별하는 변수
    private bool isBossSpawned;

    // 적이 너무 많은지(300마리 이상) 판별하는 변수
    private bool isEnemiesTooMany;

    // Enemy들을 담을 리스트
    [SerializeField]
    public List<Enemy> enemies = new List<Enemy>();

    // 사용할 클래스 객체들
    public Player player;
    public Boss boss;
    public FollowCam followCam;
    private InputManager inputManager;
    private SkillManager skillManager;
    private SkillSelectManager skillSelectManager;
    private EXP exp;
    private BossManager bossManager;
    public PoolManager poolManager;
    private TilemapManager tilemapManager;

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
    // GameOption 오브젝트
    public GameObject optionObject;

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
    // SettingPage
    public GameObject SettingPageObject;

    public PlayerData playerData; // 플레이어 데이터 객체

    public NavMeshControl navMeshControl; //Nav mesh AI 객체

    public bool isSettingPageOn = false;
    public bool isPausePageOn = false;
    public bool isClearPageOn = false;
    public bool isDeadPageOn = false;
    public bool isSkillSelectPageOn = false;
    public bool isStageClear = false;
    public bool isPauseReClicked = false;

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

        // 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않게 함
        DontDestroyOnLoad(gameObject);

        sceneName = SceneManager.GetActiveScene().name;
        // 시작 시 비활성화
        gameOverObject.SetActive(false);
        gameClearObject.SetActive(false);
        pauseObject.SetActive(false);
        optionObject.SetActive(true);
        BossHPObject.SetActive(false);
        SettingPageObject.SetActive(false);

        inputManager = FindAnyObjectByType<InputManager>();
        skillManager = FindAnyObjectByType<SkillManager>();
        skillSelectManager = FindAnyObjectByType<SkillSelectManager>();

        player = Instantiate(playerPrefab);
        player.playerData = playerData; // player에 playerData 할당

        // 클래스 객체들 초기화
        if (sceneName == "Stage1")
        {
            PlayerInit();
        }

        // inputManger Delegate 할당
        inputManager.onPauseButtonClicked = OnPauseButtonClicked;
        inputManager.onPlayButtonClicked = onPlayButtonClicked;

        // skillManager에 객체 할당
        skillManager.player = player;

        // skillManager Delegate 할당
        skillManager.onShiledSkillActivated = OnShieldSkillActivated;
        skillManager.onShiledSkillUnActivated = OnShieldSkillUnActivated;

        // SkillSelectManager delegate 할당
        skillSelectManager.onSkillSelectObjectDisplayed = OnSkillSelectObjectDisplayed;
        skillSelectManager.onSkillSelectObjectHided = OnSkillSelectObjectHided;
        skillSelectManager.onPlayerHealed = OnPlayerHealed;
        skillSelectManager.onPassiveSkillSelected = OnPassiveSkillSelected;
        skillSelectManager.onSkillSelected = OnSkillSelected;
        skillSelectManager.playerData = playerData;// skillSelectManager에 playerData 할당

        //gameTime = maxGameTime - 2f;
        //player.isPlayerShielded = true;
        //player.level = 20;
    }
    void OnEnable()
    {
        // 씬 매니저의 sceneLoaded에 체인을 건다.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        coolTimer = 0;
        coolTime = 2f;
        sceneGameTime = gameTime;

        sceneName = scene.name;

        enemies.Clear();

        SetPlayerInfo(); // player 위치 설정

        switch (sceneName)
        {
            case "Stage1":
                tilemapManager = FindAnyObjectByType<TilemapManager>();
                StageSetting();
                skillSelectManager.ChooseStartSkill(); // 시작 스킬 선택

                // Stage1 배경음 플레이
                GameAudioManager.instance.bgmPlayer.clip = GameAudioManager.instance.bgmClips[(int)Bgm.Stage1];
                GameAudioManager.instance.bgmPlayer.Play();

                SpawnStartEnemies();
                break;

            case "Stage2":
                maxGameTime += gameTime;
                tilemapManager = FindAnyObjectByType<TilemapManager>();
                StageSetting();
                // Stage2 배경음 플레이
                GameAudioManager.instance.bgmPlayer.clip = GameAudioManager.instance.bgmClips[(int)Bgm.Stage2];
                GameAudioManager.instance.bgmPlayer.Play();

                SpawnStartEnemies();
                break;
            case "Stage3":
                maxGameTime += gameTime;
                StageSetting();
                bossManager = FindAnyObjectByType<BossManager>();

                // BossManager delegate 할당
                bossManager.onBossHasKilled = OnBossHasKilled;

                // Stage3 배경음 플레이
                GameAudioManager.instance.bgmPlayer.clip = GameAudioManager.instance.bgmClips[(int)Bgm.Stage3];
                GameAudioManager.instance.bgmPlayer.Play();

                SpawnStartEnemies();
                break;
        }
    }

    void StageSetting()
    {
        followCam = FindAnyObjectByType<FollowCam>();
        poolManager = FindAnyObjectByType<PoolManager>();
        navMeshControl = FindAnyObjectByType<NavMeshControl>();

        // followCam 플레이어 객체 할당
        followCam.player = player;

        // PoolManager Player 할당
        poolManager.player = player;

        // EnemyManager delegate 할당
        poolManager.enemyManager.onEnemiesChanged = OnEnemiesChanged;
        poolManager.enemyManager.onEnemyKilled = OnEnemyKilled;

        if(sceneName == "Stage1")
            isSkillSelectPageOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            // Stage Clear가 안되면(5분 경과 안되면) 시간 흐르게
            if (!isStageClear)
            {
                gameTime += Time.deltaTime; // 게임 시간 증가
            }
            coolTimer += Time.deltaTime;

            if (!isBossSpawned)
            {
                isEnemiesTooMany = enemies.Count > 300;
                SpawnBoss(); 
                if (!isEnemiesTooMany && coolTimer >= coolTime) {
                    CalculateEnemySpawnTimeNSpawn(); // 소환할 적을 지정하고 스폰
                    coolTimer = 0;
                }
            }
        }

        skillManager.enemies = enemies;

        // Stage1, 2 Clear하면 게임시간 안가게 하기
        switch(sceneName)
        {
            case "Stage1":
                if (gameTime >= 300f) // 5분
                {   
                    isStageClear = true;
                }
                break;

            case "Stage2":
                if (gameTime >= 600f) // 10분
                {   
                    isStageClear = true;
                }
                break;
        }

    }

    private void SpawnStartEnemies()
    {
        switch (sceneName)
        {
            case "Stage1":
                SpawnEnemies(0, 50); // 시작 적 소환
                break;
            case "Stage2":
                SpawnEnemies(3, 50); // 시작 적 소환
                break;
            case "Stage3":
                SpawnEnemies(6, 50); // 시작 적 소환
                break;

        }

    }

    // BGM 바꿔야 될 때 실행하는 함수
    private void SwitchBGM(int clipIndex)
    {
        GameAudioManager.instance.bgmPlayer.Stop(); // 기존 배경음 종료
        GameAudioManager.instance.bgmPlayer.clip = GameAudioManager.instance.bgmClips[clipIndex];
        GameAudioManager.instance.bgmPlayer.Play(); // 원하는 배경음 시작
    }

    // Player 생성 함수
    private void SetPlayerInfo()
    {
        switch (sceneName)
        {
            case "Stage1":
                playerAreaSize = 140;
                Vector2 PlayerPos = new Vector2(0, 0);
                player.transform.position = PlayerPos;

                Vector2 AreaSize = new Vector2(playerAreaSize, playerAreaSize);
                player.gameObject.GetComponentInChildren<BoxCollider2D>().size = AreaSize;

                break;
            case "Stage2":
                playerAreaSize = 40;
                PlayerPos = new Vector2(0, 0);
                player.transform.position = PlayerPos;

                AreaSize = new Vector2(playerAreaSize, playerAreaSize);
                player.gameObject.GetComponentInChildren<BoxCollider2D>().size = AreaSize;
                break;
            case "Stage3":
                PlayerPos = new Vector2(0, 0);
                player.transform.position = PlayerPos;
                break;
        }

        player.onPlayerWasKilled = OnPlayerHasKilled;
        player.onPlayerLevelUP = OnPlayerLevelUP;
    }

    // Enemy 스폰 시간을 계산해 소환할 적을 지정하는 함수
    private void CalculateEnemySpawnTimeNSpawn()
    {
        switch (sceneName)
        {
            case "Stage1":
                Stage1Spawn();
                break;
            case "Stage2":
                Stage2Spawn();
                break;
            case "Stage3":
                Stage3Spawn();
                break;
        }
    }

    void Stage1Spawn()
    {
        if (gameTime <= 60 * 1)
        {
            SpawnEnemies(0, 10); // EvilTree 몬스터 소환
        }
        else if (gameTime <= 60 * 2)
        {
            SpawnEnemies(0, 5); // EvilTree 몬스터 소환
            SpawnEnemies(1, 10); // Pumpkin 몬스터 소환
        }
        else if (gameTime <= 60 * 3)
        {
            SpawnEnemies(0, 2); // EvilTree 몬스터 소환
            SpawnEnemies(1, 5); // Pumpkin 몬스터 소환
            SpawnEnemies(2, 10); // Warlock 몬스터 소환
            coolTime = 1.5f;
        }
        else if (gameTime < 60 * 4)
        {
            SpawnEnemies(0, 2); // EvilTree 몬스터 소환
            SpawnEnemies(1, 5); // Pumpkin 몬스터 소환
            SpawnEnemies(1, 8); // Pumpkin 몬스터 소환
            SpawnEnemies(2, 15); // Warlock 몬스터 소환
            coolTime = 1f;
        }
        else if (gameTime < 60 * 5)
        {
            SpawnEnemies(0, 2); // EvilTree 몬스터 소환
            SpawnEnemies(1, 4); // Pumpkin 몬스터 소환
            SpawnEnemies(2, 6); // Warlock 몬스터 소환
            SpawnEnemies(2, 20); // Warlock 몬스터 소환
            coolTime = 0.5f;
        }
    }
    void Stage2Spawn()
    {
        if (gameTime <= sceneGameTime + 60 * 1)
        {
            SpawnEnemies(3, 10); // Skeleton_Sword 몬스터 소환
        }
        else if (gameTime <= sceneGameTime + 60 * 2)
        {
            SpawnEnemies(3, 5); // Skeleton_Sword 몬스터 소환
            SpawnEnemies(4, 10); // Skeleton_Archor 몬스터 소환
        }
        else if (gameTime <= sceneGameTime + 60 * 3)
        {
            SpawnEnemies(3, 2); // Skeleton_Sword 몬스터 소환
            SpawnEnemies(4, 5); // Skeleton_Archor 몬스터 소환
            SpawnEnemies(5, 10); // Skeleton_Horse 몬스터 소환
            coolTime = 1.5f;
        }
        else if (gameTime < sceneGameTime + 60 * 4)
        {
            SpawnEnemies(3, 4); // Skeleton_Sword 몬스터 소환
            SpawnEnemies(4, 8); // Skeleton_Archor 몬스터 소환
            SpawnEnemies(5, 15); // Skeleton_Horse 몬스터 소환
            coolTime = 1f;
        }
        else if (gameTime < sceneGameTime + 60 * 5)
        {
            SpawnEnemies(3, 4); // Skeleton_Sword 몬스터 소환
            SpawnEnemies(4, 12); // Skeleton_Archor 몬스터 소환
            SpawnEnemies(5, 20); // Skeleton_Horse 몬스터 소환
            coolTime = 0.5f;
        }
    }

    void Stage3Spawn()
    {
        if (gameTime <= sceneGameTime + 60 * 1)
        {
            SpawnEnemies(6, 10); // Ghoul 몬스터 소환
        }
        else if (gameTime <= sceneGameTime + 60 * 2)
        {
            SpawnEnemies(6, 5); // Ghoul 몬스터 소환
            SpawnEnemies(7, 10); // Spitter 몬스터 소환
        }
        else if (gameTime <= sceneGameTime + 60 * 3)
        {
            SpawnEnemies(6, 2); // Ghoul 몬스터 소환
            SpawnEnemies(7, 5); // Spitter 몬스터 소환
            SpawnEnemies(8, 10); // Summoner 몬스터 소환
            coolTime = 1.5f;
        }
        else if (gameTime < sceneGameTime + 60 * 4)
        {
            SpawnEnemies(6, 2); // Ghoul 몬스터 소환
            SpawnEnemies(7, 5); // Spitter 몬스터 소환
            SpawnEnemies(8, 8); // Summoner 몬스터 소환
            SpawnEnemies(9, 15); // BloodKing 몬스터 소환
            coolTime = 1f;
        }
        else if (gameTime < sceneGameTime + 60 * 5)
        {
            SpawnEnemies(6, 2); // Ghoul 몬스터 소환
            SpawnEnemies(7, 4); // Spitter 몬스터 소환
            SpawnEnemies(8, 6); // Summoner 몬스터 소환
            SpawnEnemies(9, 20); // BloodKing 몬스터 소환
            coolTime = 0.5f;
        }
    }

    // Enemy 소환 함수
    void SpawnEnemies(int index, int num)
    {
        for (int i = 0; i < num; i++)
        {
            poolManager.GetEnemy(index); // 몬스터 소환
        }
    }

    // Boss 소환 함수
    void SpawnBoss()
    {
        bool isBossStage = sceneName == "Stage3";

        if (gameTime >= maxGameTime && isBossStage)
        {
            // 보스 등장
            bossManager.player = player;
            bossManager.CreateBoss();

            skillManager.isBossAppear = true;
            skillManager.boss = bossManager.boss;

            // 보스 HP바 active
            BossHPObject.SetActive(true);

            // Stage2 BGM 종료 후 보스 BGM ON
            SwitchBGM((int)Bgm.Boss);

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
        isDeadPageOn = true;
        gameOverObject.SetActive(true);

        yield return new WaitForSeconds(0.5f); // 0.5초 이후 시간 차 두기

        GameAudioManager.instance.PlaySfx(GameAudioManager.Sfx.Dead); // 캐릭터 사망 시 효과음
        inputManager.PauseButtonObject.interactable = false; // Pause버튼 비활성화
        GameAudioManager.instance.bgmPlayer.Stop(); // 배경음 멈추기
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
        isClearPageOn = true;
        gameClearObject.SetActive(true);

        yield return new WaitForSeconds(0.5f); // 0.5초 이후 시간 차 두기
        GameAudioManager.instance.PlaySfx(GameAudioManager.Sfx.Win); // 승리시 효과음
        inputManager.PauseButtonObject.interactable = false; // Pause버튼 비활성화
        SwitchBGM((int)GameAudioManager.Bgm.Clear);
        Time.timeScale = 0; // 화면 멈추기
    }

    // Pause 버튼이 클릭됐을 시 실행됨
    private void OnPauseButtonClicked()
    {
        pauseObject.SetActive(true);
        isPausePageOn = true;
        
        if (isPauseReClicked)
        {
            isPausePageOn = false;
            pauseObject.SetActive(false);
            Time.timeScale = 1;
            isPauseReClicked = false;
            return;
        }

        isPauseReClicked = true;
        // UI 비활성화
        //HpBarObject.SetActive(false);
        HpStatusLetteringObject.SetActive(false);
        HpStatusObject.SetActive(false);
        SkillPanelObject.SetActive(false);
        CharacterProfileObject.SetActive(false);
    }

    // Pause화면에서의 Play 버튼이 클릭됐을 시 실행됨
    private void onPlayButtonClicked()
    {
        pauseObject.SetActive(false);
        isPausePageOn = false;
        Time.timeScale = 1;

        // UI 활성화
        //HpBarObject.SetActive(true);
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
            player.playerData.kill++;
        }

        int ranNum = UnityEngine.Random.Range(0, 11);

        if (ranNum >= 6)
        {
            switch (killedEnemy.tag)
            {
                case "EvilTree":
                    ExpSpawn(0, 100, killedEnemy);
                    break;
                case "Pumpkin":
                    ExpSpawn(0, 2, killedEnemy);
                    break;
                case "WarLock":
                    ExpSpawn(1, 3, killedEnemy);
                    break;
                case "Skeleton_Sword":
                    ExpSpawn(0, 2, killedEnemy);
                    break;
                case "Skeleton_Archer":
                    ExpSpawn(0, 2, killedEnemy);
                    break;
                case "Skeleton_Horse":
                    ExpSpawn(1, 4, killedEnemy);
                    break;
                case "Ghoul":
                    ExpSpawn(0, 3, killedEnemy);
                    break;
                case "Spitter":
                    ExpSpawn(1, 3, killedEnemy);
                    break;
                case "Summoner":
                    ExpSpawn(2, 4, killedEnemy);
                    break;
                case "BloodKing":
                    ExpSpawn(2, 6, killedEnemy);
                    break;
            }

        }
    }

    private void ExpSpawn(int index, int expAmount, Enemy killedEnemy)
    {
        exp = poolManager.GetExp(index);

        exp.expAmount = expAmount;
        exp.index = index;
        exp.player = player;

        exp.X = killedEnemy.transform.position.x;
        exp.Y = killedEnemy.transform.position.y + 0.5f;
    }

    // 플레이어가 레벨 업 했을 시 실행
    private void OnPlayerLevelUP()
    {
        skillSelectManager.DisplayLevelupPanel();
        GameAudioManager.instance.PlaySfx(GameAudioManager.Sfx.LevelUp); // 레벨업 시 효과음
    }

    private void OnSkillSelectObjectDisplayed()
    {
        isSkillSelectPageOn = true;
        inputManager.PauseButtonObject.interactable = false;
    }

    private void OnSkillSelectObjectHided()
    {
        isSkillSelectPageOn = false;
        inputManager.PauseButtonObject.interactable = true;

        player.isSkillSelectComplete = true;
    }

    private void OnPlayerHealed()
    {
        player.playerData.hp += 10;
        if (player.playerData.hp > 100) { player.playerData.hp = 100; }
    }

    // 스킬이 선택되면 즉시 스킬 쿨타임을 초기화 시킨다
    private void OnSkillSelected(int index)
    {
        skillManager.ResetDelayTimer(index);
    }

    // 3 - 뎀감, 4 - 이속증가, 5 - 자석 효과
    private void OnPassiveSkillSelected(int num, float value)
    {
        switch (num)
        {
            case 3: { player.playerData.damageReductionValue = value; break; }
            case 4: { player.playerData.speed *= value; break; } // 얘는 플레이어 스피드에 즉시 적용
            case 5:
                {
                    player.playerData.magnetRange = value;
                    player.ChangeMagnetRange();
                    break;
                }
        }
    }

    // Reposition에서 ClearWall의 position이 변하면 followcam에게 전해줌
    public void SendClearWall_RightX(float xValue)
    {
        followCam.clearWall_RightEndX = xValue;
    }

    // Reposition에서 Stage1 맵 타일 하나가 변경되면 NavMeshControl에게 전해주는 데이터들
    public void SendDirectionNavMesh(float DirectionX, float DirectionY)
    {
        navMeshControl.DirectionX = DirectionX;
        navMeshControl.DirectionY = DirectionY;


    }


    void PlayerInit()
    {
        playerData.speed = 6;
        playerData.hp = 100;
        playerData.maxHp = 100;
        playerData.Exp = 0;
        playerData.level = 0;

        playerData.nextExp = new int[100];

        int num = 0;
        for (int i = 0; i < playerData.nextExp.Length; i++)
        {
            if (playerData.level >= 30)
            {
                num += 100;
                playerData.nextExp[i] = num;
            }
            else
            {
                num += 5;
                playerData.nextExp[i] = num;
            }
        }

        playerData.damageReductionValue = 1f;
        playerData.magnetRange = 0.25f;

    }
}
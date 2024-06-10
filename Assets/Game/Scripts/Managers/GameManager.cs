using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 개발용, 적 마리수 계수
    private int enemyCoefficient = 2;

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
    private float coolTime = 3f;
    private float coolTimer = 0f;

    // player Area Size
    public int playerAreaSize;
    float playerSpeedSave;

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
    public static GameObject gameOverObject;
    // GameClear 오브젝트
    public static GameObject gameClearObject;
    // Pause 오브젝트
    public static GameObject pauseObject;
    // GameOption 오브젝트
    public static GameObject optionObject;

    // HpStatus
    public static GameObject HpStatusObject;
    // HpStatusLettering
    public static GameObject HpStatusLetteringObject;
    // Active SkillPenel
    public GameObject ActiveSkillPanelObject;
    // Passive SkillPanel
    public GameObject PassiveSkillPanelObject;
    // CharacterProfile
    public static GameObject CharacterProfileObject;
    // Boss HP
    public static GameObject BossHPObject;
    // SettingPage
    public static GameObject SettingPageObject;

    public PlayerData playerData; // 플레이어 데이터 객체

    public NavMeshControl navMeshControl; //Nav mesh AI 객체

    public bool isSettingPageOn = false;
    public bool isPausePageOn = false;
    public bool isClearPageOn = false;
    public bool isDeadPageOn = false;
    public bool isSkillSelectPageOn = false;
    public bool isStageClear = false; // 마지막 Stage 이전 Stage들 클리어
    // Pause 버튼 재활성화 위한 bool 변수
    public bool isPauseReClicked = false;
    public bool isWantPauseButtonActive = false;

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

        Transform canvasTransform = GameObject.Find("Canvas").transform;

        gameOverObject = canvasTransform.Find("Game Over Menu").gameObject;
        gameClearObject = canvasTransform.Find("Clear Menu").gameObject;
        pauseObject = canvasTransform.Find("Pause Menu").gameObject;
        optionObject = canvasTransform.Find("Game Option Button").gameObject;
        BossHPObject = canvasTransform.Find("Panels/Top Panel/Boss HP Status").gameObject;
        SettingPageObject = canvasTransform.Find("Setting Page").gameObject;

        HpStatusObject = canvasTransform.Find("Panels/Under Panel/HpStatus").gameObject;
        HpStatusLetteringObject = canvasTransform.Find("Panels/Under Panel/HpStatusLettering").gameObject;
        ActiveSkillPanelObject = canvasTransform.Find("Panels/Under Panel/SkillPanel").gameObject;
        PassiveSkillPanelObject = canvasTransform.Find("Panels/Left Panel/SkillPanel").gameObject;
        CharacterProfileObject = canvasTransform.Find("Panels/Under Panel/CharacterProfile").gameObject;

        // 시작 시 비활성화
        gameOverObject.SetActive(false);
        gameClearObject.SetActive(false);
        pauseObject.SetActive(false);
        optionObject.SetActive(true);
        BossHPObject.SetActive(false);
        SettingPageObject.SetActive(false);

        sceneName = SceneManager.GetActiveScene().name;

        // 클래스 객체들 초기화
        inputManager = FindAnyObjectByType<InputManager>();
        skillManager = FindAnyObjectByType<SkillManager>();
        skillSelectManager = FindAnyObjectByType<SkillSelectManager>();

        // 플레이어 생성
        player = Instantiate(playerPrefab);
        player.playerData = playerData; // player에 playerData 할당
        PlayerInit();

        // inputManger Delegate 할당
        inputManager.onPauseButtonClicked = OnPauseButtonClicked;
        inputManager.onPlayButtonClicked = OnPlayButtonClicked;

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
        sceneGameTime = gameTime;
        sceneName = scene.name;
        enemies.Clear();

        switch (sceneName)
        {
            case "Stage1":
                Init();
                tilemapManager = FindAnyObjectByType<TilemapManager>();
                StageSetting();
                skillSelectManager.ChooseStartSkill(); // 시작 스킬 선택

                // Stage1 배경음 플레이
                GameAudioManager.instance.bgmPlayer.clip = GameAudioManager.instance.bgmClips[(int)GameAudioManager.Bgm.Stage1];
                GameAudioManager.instance.bgmPlayer.Play();

                SpawnStartEnemies();
                break;

            case "Stage2":
                maxGameTime += gameTime;
                tilemapManager = FindAnyObjectByType<TilemapManager>();
                StageSetting();
                // Stage2 배경음 플레이
                GameAudioManager.instance.bgmPlayer.clip = GameAudioManager.instance.bgmClips[(int)GameAudioManager.Bgm.Stage2];
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
                GameAudioManager.instance.bgmPlayer.clip = GameAudioManager.instance.bgmClips[(int)GameAudioManager.Bgm.Stage3];
                GameAudioManager.instance.bgmPlayer.Play();

                SpawnStartEnemies();
                break;
        }

        SetPlayerInfo(); // player 위치 설정


        Invoke("EEE", 2);
        
    }

    void Init()
    {
        gameTime = 0;
        maxGameTime = 5 * 60;
        isBossSpawned = false;
        isGameOver = false;
        isEnemiesTooMany = false;

        // UI 관련 설정
        isSettingPageOn = false;
        isPausePageOn = false;
        isClearPageOn = false;
        isDeadPageOn = false;
        isSkillSelectPageOn = false;
        isStageClear = false;
        isPauseReClicked = false;
        isWantPauseButtonActive = false;

        // UI 활성화
        //HpBarObject.SetActive(true);
        HpStatusLetteringObject.SetActive(true);
        HpStatusObject.SetActive(true);
        ActiveSkillPanelObject.SetActive(true);
        PassiveSkillPanelObject.SetActive(true);
        CharacterProfileObject.SetActive(true);

        // 시작 시 비활성화
        gameOverObject.SetActive(false);
        gameClearObject.SetActive(false);
        pauseObject.SetActive(false);
        optionObject.SetActive(true);
        BossHPObject.SetActive(false);
        SettingPageObject.SetActive(false);

        skillSelectManager.Init();
        //GameAudioManager.instance.Init();
        inputManager.Init();
        player.Init();
        PlayerInit();
        skillManager.Init();
        skillManager.player = player;

        Time.timeScale = 1;
    }

    void EEE()
    {
        /*Debug.Log("GameManager Canvas 객체들 검사");
        Debug.Log(gameOverObject);
        Debug.Log(gameClearObject);
        Debug.Log(pauseObject);
        Debug.Log(optionObject);
        Debug.Log(HpStatusObject);
        Debug.Log(HpStatusLetteringObject);
        Debug.Log(ActiveSkillPanelObject);
        Debug.Log(PassiveSkillPanelObject);
        Debug.Log(CharacterProfileObject);
        Debug.Log(BossHPObject);
        Debug.Log(SettingPageObject);
        inputManager.AAA();
        Debug.Log("SkillManager Player 객체 검사");
        Debug.Log(skillManager.player);*/
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
        
        if (sceneName == "Stage1")
            isSkillSelectPageOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver)
        {
            gameTime += Time.deltaTime; // 게임 시간 증가
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
    }

    private void SpawnStartEnemies()
    {
        switch (sceneName)
        {
            case "Stage1":
                SpawnEnemies(0, 10 * enemyCoefficient); // 시작 적 소환
                break;
            case "Stage2":
                SpawnEnemies(3, 10 * enemyCoefficient); // 시작 적 소환
                break;
            case "Stage3":
                SpawnEnemies(6, 10 * enemyCoefficient); // 시작 적 소환
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
                playerData.speed = playerSpeedSave; // 플레이어 속도 원래대로
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
        if (gameTime <= 30)
        {
            SpawnEnemies(0, 1 * enemyCoefficient); // EvilTree 몬스터 소환
        }
        else if (gameTime <= 60)
        {
            SpawnEnemies(0, 3 * enemyCoefficient); // EvilTree 몬스터 소환
        }
        else if (gameTime <= 90)
        {
            SpawnEnemies(0, 3 * enemyCoefficient); // EvilTree 몬스터 소환
            SpawnEnemies(1, 1 * enemyCoefficient); // Pumpkin 몬스터 소환
        }
        else if (gameTime < 120)
        {
            SpawnEnemies(0, 3 * enemyCoefficient); // EvilTree 몬스터 소환
            SpawnEnemies(1, 2 * enemyCoefficient); // Pumpkin 몬스터 소환
        }
        else if (gameTime < 150)
        {
            SpawnEnemies(0, 3 * enemyCoefficient); // EvilTree 몬스터 소환
            SpawnEnemies(1, 3 * enemyCoefficient); // Pumpkin 몬스터 소환
        }
        else if (gameTime <= 180)
        {
            SpawnEnemies(0, 2 * enemyCoefficient); // EvilTree 몬스터 소환
            SpawnEnemies(1, 4 * enemyCoefficient); // Pumpkin 몬스터 소환
        }
        else if (gameTime <= 210)
        {
            SpawnEnemies(0, 1 * enemyCoefficient); // EvilTree 몬스터 소환
            SpawnEnemies(1, 3 * enemyCoefficient); // Pumpkin 몬스터 소환
            SpawnEnemies(2, 2 * enemyCoefficient); // Warlock 몬스터 소환
        }
        else if (gameTime < 240)
        {
            SpawnEnemies(1, 2 * enemyCoefficient); // Pumpkin 몬스터 소환
            SpawnEnemies(2, 4 * enemyCoefficient); // Warlock 몬스터 소환
        }
        else if (gameTime < 270)
        {
            SpawnEnemies(1, 2 * enemyCoefficient); // Pumpkin 몬스터 소환
            SpawnEnemies(2, 5 * enemyCoefficient); // Warlock 몬스터 소환
        }
        else if (gameTime < 300)
        {
            SpawnEnemies(1, 3 * enemyCoefficient); // Pumpkin 몬스터 소환
            SpawnEnemies(2, 6 * enemyCoefficient); // Warlock 몬스터 소환
        }
    }
    void Stage2Spawn()
    {
        if (gameTime <= sceneGameTime + 30)
        {
            SpawnEnemies(3, 2 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
        }
        else if (gameTime <= sceneGameTime + 60)
        {
            SpawnEnemies(3, 4 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
        }
        else if (gameTime <= sceneGameTime + 90)
        {
            SpawnEnemies(3, 4 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
            SpawnEnemies(4, 1 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
        }
        else if (gameTime < sceneGameTime + 120)
        {
            SpawnEnemies(3, 3 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
            SpawnEnemies(4, 2 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
        }
        else if (gameTime < sceneGameTime + 150)
        {
            SpawnEnemies(3, 3 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
            SpawnEnemies(4, 3 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
        }
        else if (gameTime <= sceneGameTime + 180)
        {
            SpawnEnemies(3, 2 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
            SpawnEnemies(4, 4 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
        }
        else if (gameTime <= sceneGameTime + 210)
        {
            SpawnEnemies(3, 1 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
            SpawnEnemies(4, 3 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
            SpawnEnemies(5, 3 * enemyCoefficient); // Skeleton_Horse 몬스터 소환
        }
        else if (gameTime < sceneGameTime + 240)
        {
            SpawnEnemies(4, 2 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
            SpawnEnemies(5, 6 * enemyCoefficient); // Skeleton_Horse 몬스터 소환
        }
        else if (gameTime < sceneGameTime + 270)
        {
            SpawnEnemies(4, 3 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
            SpawnEnemies(5, 7 * enemyCoefficient); // Skeleton_Horse 몬스터 소환
        }
        else if (gameTime < sceneGameTime + 300)
        {
            SpawnEnemies(4, 4 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
            SpawnEnemies(5, 8 * enemyCoefficient); // Skeleton_Horse 몬스터 소환
        }
    }

    void Stage3Spawn()
    {
        if (gameTime <= sceneGameTime + 30)
        {
            SpawnEnemies(6, 2 * enemyCoefficient); // Ghoul 몬스터 소환
        }
        else if (gameTime <= sceneGameTime + 60)
        {
            SpawnEnemies(6, 4 * enemyCoefficient); // Ghoul 몬스터 소환
        }
        else if (gameTime <= sceneGameTime + 90)
        {
            SpawnEnemies(6, 4 * enemyCoefficient); // Ghoul 몬스터 소환
            SpawnEnemies(7, 1 * enemyCoefficient); // Spitter 몬스터 소환
        }
        else if (gameTime < sceneGameTime + 120)
        {
            SpawnEnemies(6, 2 * enemyCoefficient); // Ghoul 몬스터 소환
            SpawnEnemies(7, 3 * enemyCoefficient); // Spitter 몬스터 소환
        }
        else if (gameTime < sceneGameTime + 150)
        {
            SpawnEnemies(6, 2 * enemyCoefficient); // Ghoul 몬스터 소환
            SpawnEnemies(7, 4 * enemyCoefficient); // Spitter 몬스터 소환
        }
        else if (gameTime <= sceneGameTime + 180)
        {
            SpawnEnemies(6, 2 * enemyCoefficient); // Ghoul 몬스터 소환
            SpawnEnemies(7, 3 * enemyCoefficient); // Spitter 몬스터 소환
            SpawnEnemies(8, 2 * enemyCoefficient); // Summoner 몬스터 소환
        }
        else if (gameTime <= sceneGameTime + 210)
        {
            SpawnEnemies(6, 1 * enemyCoefficient); // Ghoul 몬스터 소환
            SpawnEnemies(7, 3 * enemyCoefficient); // Spitter 몬스터 소환
            SpawnEnemies(8, 4 * enemyCoefficient); // Summoner 몬스터 소환
        }
        else if (gameTime < sceneGameTime + 240)
        {
            SpawnEnemies(7, 2 * enemyCoefficient); // Spitter 몬스터 소환
            SpawnEnemies(8, 5 * enemyCoefficient); // Summoner 몬스터 소환
            SpawnEnemies(9, 3 * enemyCoefficient); // BloodKing 몬스터 소환
        }
        else if (gameTime < sceneGameTime + 270)
        {
            SpawnEnemies(8, 5 * enemyCoefficient); // Summoner 몬스터 소환
            SpawnEnemies(9, 8 * enemyCoefficient); // BloodKing 몬스터 소환
        }
        else if (gameTime < sceneGameTime + 300)
        {
            SpawnEnemies(8, 6 * enemyCoefficient); // Summoner 몬스터 소환
            SpawnEnemies(9, 9 * enemyCoefficient); // BloodKing 몬스터 소환
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
            SwitchBGM((int)GameAudioManager.Bgm.Boss);

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
        InputManager.PauseButtonObject.interactable = false; // Pause버튼 비활성화
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
        isStageClear = true;
        isClearPageOn = true;

        gameClearObject.SetActive(true);
        // UI 비활성화
        BossHPObject.SetActive(false);
        HpStatusLetteringObject.SetActive(false);
        HpStatusObject.SetActive(false);
        ActiveSkillPanelObject.SetActive(false);
        PassiveSkillPanelObject.SetActive(false);
        CharacterProfileObject.SetActive(false);

        yield return new WaitForSeconds(0.5f); // 0.5초 이후 시간 차 두기
        GameAudioManager.instance.PlaySfx(GameAudioManager.Sfx.Win); // 승리시 효과음
        InputManager.PauseButtonObject.interactable = false; // Pause버튼 비활성화
        SwitchBGM((int)GameAudioManager.Bgm.Clear);
        Time.timeScale = 0; // 화면 멈추기
    }

    // Pause 버튼이 클릭 됐을 시 실행됨
    private void OnPauseButtonClicked()
    {
        if (!isPauseReClicked)
        {
            // 인게임 중에만 작동
            if (!isDeadPageOn && !isPauseReClicked && !isWantPauseButtonActive)
            {
                //Debug.Log("첫번째이자 Pause 화면 나오게");
                Time.timeScale = 0;
                pauseObject.SetActive(true);
                isPausePageOn = true;

                // UI 비활성화
                HpStatusLetteringObject.SetActive(false);
                HpStatusObject.SetActive(false);
                ActiveSkillPanelObject.SetActive(false);
                PassiveSkillPanelObject.SetActive(false);
                CharacterProfileObject.SetActive(false);
            }

            if (!isDeadPageOn && !isPauseReClicked && isWantPauseButtonActive)
            {
                //Debug.Log("N번째이자 Pause 화면 나오게");
                Time.timeScale = 0;
                pauseObject.SetActive(true);
                isPausePageOn = true;

                // UI 비활성화
                HpStatusLetteringObject.SetActive(false);
                HpStatusObject.SetActive(false);
                ActiveSkillPanelObject.SetActive(false);
                PassiveSkillPanelObject.SetActive(false);
                CharacterProfileObject.SetActive(false);
            }

            if (pauseObject.activeSelf)
            {
                isPausePageOn = false;
                isPauseReClicked = true;
            }
        }
        else // 두번째 클릭이면
        {
            // 다시 게임 진행
            if (!isDeadPageOn && isPauseReClicked)
            {
                Time.timeScale = 1;
                pauseObject.SetActive(false);
                isPausePageOn = false;

                // UI 활성화
                HpStatusLetteringObject.SetActive(true);
                HpStatusObject.SetActive(true);
                ActiveSkillPanelObject.SetActive(true);
                PassiveSkillPanelObject.SetActive(true);
                CharacterProfileObject.SetActive(true);

                if (!pauseObject.activeSelf)
                {
                    isWantPauseButtonActive = true;
                    //Debug.Log("Pause 화면 안나오게");
                }
                isPauseReClicked = false;
            }

        }
    }

    // Play 버튼이 클릭 됐을 시 실행됨
    private void OnPlayButtonClicked()
    {
        pauseObject.SetActive(false);
        isPausePageOn = false;
        Time.timeScale = 1;

        // UI 활성화
        HpStatusLetteringObject.SetActive(true);
        HpStatusObject.SetActive(true);
        ActiveSkillPanelObject.SetActive(true);
        PassiveSkillPanelObject.SetActive(true);
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

        int ranNum = UnityEngine.Random.Range(1, 11);

        if (ranNum <= 4)
        {
            switch (killedEnemy.tag)
            {
                case "EvilTree":
                    ExpSpawn(0, 2, killedEnemy);
                    break;
                case "Pumpkin":
                    ExpSpawn(0, 2, killedEnemy);
                    break;
                case "WarLock":
                    ExpSpawn(0, 4, killedEnemy);
                    break;
                case "Skeleton_Sword":
                    ExpSpawn(0, 4, killedEnemy);
                    break;
                case "Skeleton_Archer":
                    ExpSpawn(1, 8, killedEnemy);
                    break;
                case "Skeleton_Horse":
                    ExpSpawn(1, 8, killedEnemy);
                    break;
                case "Ghoul":
                    ExpSpawn(1, 8, killedEnemy);
                    break;
                case "Spitter":
                    ExpSpawn(2, 15, killedEnemy);
                    break;
                case "Summoner":
                    ExpSpawn(2, 20, killedEnemy);
                    break;
                case "BloodKing":
                    ExpSpawn(2, 20, killedEnemy);
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
        InputManager.PauseButtonObject.interactable = false;
    }

    private void OnSkillSelectObjectHided()
    {
        isSkillSelectPageOn = false;
        InputManager.PauseButtonObject.interactable = true;

        player.isSkillSelectComplete = true;
        Debug.Log("스킬 선택 완료, isSkillSelectComplete: "+ player.isSkillSelectComplete);
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

    public void PlayerSpeedUp()
    {
        playerSpeedSave = playerData.speed;
        playerData.speed = 20;
    }

    void PlayerInit()
    {
        playerData.speed = 6;
        playerData.hp = 100;
        playerData.maxHp = 100;
        playerData.Exp = 0;
        playerData.level = 1;
        playerData.kill = 0;

        playerData.nextExp = new int[100];

        int expNum = 10;
        playerData.nextExp[1] = expNum;
        for (int i = 2; i < playerData.nextExp.Length; i++)
        {
            expNum += 5;
            playerData.nextExp[i] = expNum;
        }

        playerData.damageReductionValue = 1f;
        playerData.magnetRange = 0.25f;
    }
}
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static Player;

// Pause 걸면 이전에는 인게임 속 UI들(피통, 스킬 패널, 프로필)이 안사라져서
// 사라지게 하려고 gameObject로 선언한거랑
// OnPauseButtonClicked() ,onPlayButtonClicked() 메소드 수정했음

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager를 instance화

    // 게임 시간
    public float gameTime;
    //private float maxGameTime = 15 * 60f;

    // 적 최대 생성 거리 (최소는 20, EnemyManager에 있음)
    private float maxEnemySpawnRange = 30;

    // Enemy0 스폰 관련 설정
    private float spawnCoolTime0 = 10;
    private int compensationNum0= 1; // 스폰 쿨타임 보정값

    // Enemy1 스폰 관련 설정
    private float spawnCoolTime1 = 10;
    private int compensationNum1 = 1; // 스폰 쿨타임 보정값

    // level 시간 설정
    private float level2Time = 30;

    // GameOver가 됐는지 판별하는 변수
    private bool isGameOver;

    // Enemy들을 담을 리스트
    private List<Enemy> enemies = new List<Enemy>();

    // 사용할 클래스 객체들
    public Player player;
    private EnemyManager enemyManager;
    private FollowCam followCam;
    private InputManager inputManager;
    private SkillManager skillManager;
    private SkillSelectManager skillSelectManager;
    private EXP exp;

    // GameObject에서 프리팹을 넣어주기 위해 public으로 설정
    public Player playerPrefab;

    // EXP 프리팹
    public EXP expPrefab1;
    public EXP expPrefab2;
    public EXP expPrefab3;

    // GameOver 오브젝트
    public GameObject gameOverObject;

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

    private void Awake()
    {
        instance = this; // GameManager를 인스턴스화

        // 시작 시 비활성화
        gameOverObject.SetActive(false);
        pauseObject.SetActive(false);
        HpBarObject.SetActive(true);

        // 클래스 객체들 초기화
        CreatePlayer();
        enemyManager = FindAnyObjectByType<EnemyManager>();
        inputManager = FindAnyObjectByType<InputManager>();
        followCam = FindAnyObjectByType<FollowCam>();
        skillManager = FindAnyObjectByType<SkillManager>();
        skillSelectManager = FindAnyObjectByType<SkillSelectManager>();

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
    }

    void Start()
    {
        enemyManager.CreateEnemies(50, player, 2, maxEnemySpawnRange); // 몬스터 소환

        skillSelectManager.ChooseStartSkill(); // 시작 스킬 선택
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameOver) {
            gameTime += Time.deltaTime; // 게임 시간 증가

            CalculateEnemySpawnTime(); // 소환할 적을 지정하고 스폰
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
    private void CalculateEnemySpawnTime()
    {
        bool isLevel2TimeOver = gameTime >= level2Time; // level2가 되야 할 시간이 지났나 판별

        // Enemy0, 1의 스폰 쿨타임이 지났나 판별
        bool is_spawnCoolTimeOver0 = gameTime > spawnCoolTime0 * compensationNum0;
        bool is_spawnCoolTimeOver1 = gameTime > spawnCoolTime1 * compensationNum1;

        // 쿨타임이 찰 때마다 쿨타임을 보정
        if (is_spawnCoolTimeOver0)
            compensationNum0++;

        if (is_spawnCoolTimeOver1)
            compensationNum1++;

        bool is_spawn1ok = is_spawnCoolTimeOver0 && !isLevel2TimeOver;
        bool is_spawn2ok = is_spawnCoolTimeOver1 && isLevel2TimeOver;

        // 소환되어야 할 Enemy를 스폰
        if (is_spawn1ok)
        {
            enemyManager.CreateEnemies(20, player, 2, maxEnemySpawnRange);
            enemyManager.CreateEnemies(40, player, 3, maxEnemySpawnRange);
        }
        
        if (is_spawn2ok)
        {
            enemyManager.CreateEnemies(10, player, 2, maxEnemySpawnRange);
            enemyManager.CreateEnemies(20, player, 3, maxEnemySpawnRange);
            enemyManager.CreateEnemies(30, player, 4, maxEnemySpawnRange);
        }
            
    }

    // 플레이어가 죽었을 시 실행됨
    private void OnPlayerHasKilled(Player player)
    {
        isGameOver = true;
        gameOverObject.SetActive(true);
        inputManager.PauseButtonObject.interactable = false;
    }

    // Pause버튼이 클릭됐을 시 실행됨
    private void OnPauseButtonClicked()
    {
        pauseObject.SetActive(true);
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

        exp = Instantiate(expPrefab1);

        exp.expAmount = 1;

        exp.X = killedEnemy.X;
        exp.Y = killedEnemy.Y + 1f;
    }

    // 플레이어가 레벨 업 했을 시 실행
    private void OnPlayerLevelUP()
    {
        skillSelectManager.DisplayLevelupPanel();
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
}

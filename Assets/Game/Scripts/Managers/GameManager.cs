using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager를 instance화

    // 게임 시간
    public float gameTime;
    
    //private float maxGameTime = 15 * 60f;

    // 적 최대 생성 거리 (최소는 20, EnemyManager에 있음)
    private float maxEnemySpawnRange = 30;
    private float maxBossSpawnRange = 30;

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

    // 사용할 클래스 객체들
    public Player player;
    private EnemyManager enemyManager;
    private FollowCam followCam;
    private InputManager inputManager;
    private BossManager bossManager;
   
    // GameObject에서 프리팹을 넣어주기 위해 public으로 설정
    public Player playerPrefab;

    // GameOver 오브젝트
    public GameObject gameOverObject;

    // Pause 오브젝트
    public GameObject pauseObject;

    public GameObject ExpBar;
    public GameObject HpBar;
    public GameObject Timer;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 시작 시 비활성화
        gameOverObject.SetActive(false);
        pauseObject.SetActive(false);
        OnHUDOn();

        // 클래스 객체들 초기화
        CreatePlayer();
        enemyManager = FindAnyObjectByType<EnemyManager>();
        inputManager = FindAnyObjectByType<InputManager>();
        followCam = FindAnyObjectByType<FollowCam>();
        bossManager=FindAnyObjectByType<BossManager>();

        // 몬스터 소환
        enemyManager.CreateEnemies(100, player, 3, maxEnemySpawnRange);
        bossManager.CreateBoss(player, maxBossSpawnRange);

        // inputManger Delegate 할당
        inputManager.onPauseButtonClicked = OnPauseButtonClicked;
        inputManager.onPlayButtonClicked = OnPlayButtonClicked;

        // followCam 플레이어 객체 할당
        followCam.player = player;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameOver) {
            gameTime += Time.deltaTime; // 게임 시간 증가

            CalculateEnemySpawnTime(); // 소환할 적을 지정하고 스폰
        }
    }

    // Player 생성 함수
    private void CreatePlayer(){
        player = Instantiate(playerPrefab);
        player.onPlayerWasKilled = OnPlayerHasKilled;
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
            enemyManager.CreateEnemies(100, player, 2, maxEnemySpawnRange);

        if (is_spawn2ok)
            enemyManager.CreateEnemies(100, player, 3, maxEnemySpawnRange);
    }

    // 플레이어가 죽었을 시 실행됨
    private void OnPlayerHasKilled(Player player)
    {
        isGameOver = true;
        gameOverObject.SetActive(true);
        OnHUDOff();
    }

    // Pause버튼이 클릭됐을 시 실행됨
    private void OnPauseButtonClicked()
    {
        pauseObject.SetActive(true);
        OnHUDOff();
    }

    private void OnPlayButtonClicked()
    {
        pauseObject.SetActive(false);
        OnHUDOn();
    }

    private void OnHUDOn()
    {
        ExpBar.SetActive(true);
        HpBar.SetActive(true);
        Timer.SetActive(true);
    }

    private void OnHUDOff()
    {
        ExpBar.SetActive(false);
        HpBar.SetActive(false);
        Timer.SetActive(false);
    }
}

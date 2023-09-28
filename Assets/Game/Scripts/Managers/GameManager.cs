using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager를 instance화

    //게임 시간
    private float gameTime;
    //private float maxGameTime = 15 * 60f;

    // Enemy0 스폰 관련 설정
    private float spawnCoolTime0 = 10;
    private int compensationNum0= 1; // 스폰 쿨타임 보정값

    // Enemy1 스폰 관련 설정
    private float spawnCoolTime1 = 10;
    private int compensationNum1 = 1; // 스폰 쿨타임 보정값

    // level 시간 설정
    private float level2Time = 30;

    // 사용할 클래스 객체들
    public Player player;
    private EnemyManager enemyManager;
    private FollowCam followCam;
   
    // GameObject에서 프리팹을 넣어주기 위해 public으로 설정
    public Player playerPrefab;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 클래스 객체들 초기화
        CreatePlayer();
        enemyManager = FindAnyObjectByType<EnemyManager>();

        // 몬스터 소환
        enemyManager.CreateEnemies(100, player, 0, 20.0f);

        followCam = FindAnyObjectByType<FollowCam>();
        followCam.player = player;
    }

    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime; // 게임 시간 증가

        CalculateEnemySpawnTime(); // 소환할 적을 지정하고 스폰

        
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
            enemyManager.CreateEnemies(100, player, 0, 20.0f);

        if (is_spawn2ok)
            enemyManager.CreateEnemies(100, player, 1, 20.0f);
    }

    // 플레이어가 죽었을 시 실행됨
    private void OnPlayerHasKilled(Player player)
    {
        //
    }
}

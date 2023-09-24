using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager를 instance화

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
        enemyManager.CreateEnemies(10, player);

        followCam = FindAnyObjectByType<FollowCam>();
        followCam.player = player;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Player 생성 함수
    private void CreatePlayer(){
        player = Instantiate(playerPrefab);
        player.onPlayerWasKilled = OnPlayerHasKilled;
    }

    // 플레이어가 죽었을 시 실행됨
    private void OnPlayerHasKilled(Player player)
    {
        //
    }
}

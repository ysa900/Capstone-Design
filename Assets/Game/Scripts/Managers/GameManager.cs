using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager�� instanceȭ

    //���� �ð�
    private float gameTime;
    private float maxGameTime = 15 * 60f;

    private bool is_spawnTrigger0= false;
    private float spawnCoolTime0 = 5;
    private int num0= 1;

    private bool is_spawnTrigger1 = false;
    private float spawnCoolTime1 = 10;
    private int num1 = 1;

    private float level2Time = 6f;
    private bool isLevel2TimeOver = false;

    // ����� Ŭ���� ��ü��
    public Player player;
    private EnemyManager enemyManager;
    private FollowCam followCam;
   
    // GameObject���� �������� �־��ֱ� ���� public���� ����
    public Player playerPrefab;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Ŭ���� ��ü�� �ʱ�ȭ
        CreatePlayer();
        enemyManager = FindAnyObjectByType<EnemyManager>();

        // ���� ��ȯ
        enemyManager.CreateEnemies(100, player, 0, 15.0f);

        followCam = FindAnyObjectByType<FollowCam>();
        followCam.player = player;
    }

    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime;
 
     
        isLevel2TimeOver = gameTime > level2Time;

        is_spawnTrigger0 = gameTime > spawnCoolTime0 * num0;
        is_spawnTrigger1 = gameTime > spawnCoolTime1 * num1;

        is_spawnTrigger0 = is_spawnTrigger0 && !isLevel2TimeOver;
        is_spawnTrigger1 = is_spawnTrigger1 && isLevel2TimeOver;


        if (is_spawnTrigger0)
        {
            enemyManager.CreateEnemies(100, player, 0, 15.0f);
            is_spawnTrigger0 = false;
            num0++;
            
        }
        
        if(is_spawnTrigger1)
        {
            enemyManager.CreateEnemies(100, player, 1, 20.0f);
            is_spawnTrigger1 = false;
            num1++;
        }

    }

    // Player ���� �Լ�
    private void CreatePlayer(){
        player = Instantiate(playerPrefab);
        player.onPlayerWasKilled = OnPlayerHasKilled;
    }

    // �÷��̾ �׾��� �� �����
    private void OnPlayerHasKilled(Player player)
    {
        //
    }
}

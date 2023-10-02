using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager�� instanceȭ

    // ���� �ð�
    private float gameTime;
    //private float maxGameTime = 15 * 60f;

    // �� �ִ� ���� �Ÿ� (�ּҴ� 20, EnemyManager�� ����)
    private float maxEnemySpawnRange = 30;

    // Enemy0 ���� ���� ����
    private float spawnCoolTime0 = 10;
    private int compensationNum0= 1; // ���� ��Ÿ�� ������

    // Enemy1 ���� ���� ����
    private float spawnCoolTime1 = 10;
    private int compensationNum1 = 1; // ���� ��Ÿ�� ������

    // level �ð� ����
    private float level2Time = 30;

    // GameOver�� �ƴ��� �Ǻ��ϴ� ����
    private bool isGameOver;

    // ����� Ŭ���� ��ü��
    public Player player;
    private EnemyManager enemyManager;
    private FollowCam followCam;
    private InputManager inputManager;
   
    // GameObject���� �������� �־��ֱ� ���� public���� ����
    public Player playerPrefab;

    // GameOver ������Ʈ
    public GameObject gameOverObject;

    // Pause ������Ʈ
    public GameObject pauseObject;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // ���� �� ��Ȱ��ȭ
        gameOverObject.SetActive(false);
        pauseObject.SetActive(false);

        // Ŭ���� ��ü�� �ʱ�ȭ
        CreatePlayer();
        enemyManager = FindAnyObjectByType<EnemyManager>();
        inputManager = FindAnyObjectByType<InputManager>();
        followCam = FindAnyObjectByType<FollowCam>();

        // ���� ��ȯ
        enemyManager.CreateEnemies(50, player, 2, maxEnemySpawnRange);

        // inputManger Delegate �Ҵ�
        inputManager.onPauseButtonClicked = OnPauseButtonClicked;
        inputManager.onPlayButtonClicked = onPlayButtonClicked;

        // followCam �÷��̾� ��ü �Ҵ�
        followCam.player = player;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameOver) {
            gameTime += Time.deltaTime; // ���� �ð� ����

            CalculateEnemySpawnTime(); // ��ȯ�� ���� �����ϰ� ����
        }
    }

    // Player ���� �Լ�
    private void CreatePlayer(){
        player = Instantiate(playerPrefab);
        player.onPlayerWasKilled = OnPlayerHasKilled;
    }

    // Enemy ���� �ð��� ����� ��ȯ�� ���� �����ϴ� �Լ�
    private void CalculateEnemySpawnTime()
    {
        bool isLevel2TimeOver = gameTime >= level2Time; // level2�� �Ǿ� �� �ð��� ������ �Ǻ�

        // Enemy0, 1�� ���� ��Ÿ���� ������ �Ǻ�
        bool is_spawnCoolTimeOver0 = gameTime > spawnCoolTime0 * compensationNum0;
        bool is_spawnCoolTimeOver1 = gameTime > spawnCoolTime1 * compensationNum1;

        // ��Ÿ���� �� ������ ��Ÿ���� ����
        if (is_spawnCoolTimeOver0)
            compensationNum0++;

        if (is_spawnCoolTimeOver1)
            compensationNum1++;

        bool is_spawn1ok = is_spawnCoolTimeOver0 && !isLevel2TimeOver;
        bool is_spawn2ok = is_spawnCoolTimeOver1 && isLevel2TimeOver;

        // ��ȯ�Ǿ�� �� Enemy�� ����
        if (is_spawn1ok)
            enemyManager.CreateEnemies(50, player, 3, maxEnemySpawnRange);

        if (is_spawn2ok)
            enemyManager.CreateEnemies(50, player, 4, maxEnemySpawnRange);
    }

    // �÷��̾ �׾��� �� �����
    private void OnPlayerHasKilled(Player player)
    {
        isGameOver = true;
        gameOverObject.SetActive(true);
    }

    // Pause��ư�� Ŭ������ �� �����
    private void OnPauseButtonClicked()
    {
        pauseObject.SetActive(true);
    }

    private void onPlayButtonClicked()
    {
        pauseObject.SetActive(false);
    }
}

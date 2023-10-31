using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static Player;

// Pause �ɸ� �������� �ΰ��� �� UI��(����, ��ų �г�, ������)�� �Ȼ������
// ������� �Ϸ��� gameObject�� �����ѰŶ�
// OnPauseButtonClicked() ,onPlayButtonClicked() �޼ҵ� ��������

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager�� instanceȭ

    // ���� �ð�
    public float gameTime;
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

    // Enemy���� ���� ����Ʈ
    private List<Enemy> enemies = new List<Enemy>();

    // ����� Ŭ���� ��ü��
    public Player player;
    private EnemyManager enemyManager;
    private FollowCam followCam;
    private InputManager inputManager;
    private SkillManager skillManager;
    private SkillSelectManager skillSelectManager;
    private EXP exp;

    // GameObject���� �������� �־��ֱ� ���� public���� ����
    public Player playerPrefab;

    // EXP ������
    public EXP expPrefab1;
    public EXP expPrefab2;
    public EXP expPrefab3;

    // GameOver ������Ʈ
    public GameObject gameOverObject;

    // Pause ������Ʈ
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
        instance = this; // GameManager�� �ν��Ͻ�ȭ

        // ���� �� ��Ȱ��ȭ
        gameOverObject.SetActive(false);
        pauseObject.SetActive(false);
        HpBarObject.SetActive(true);

        // Ŭ���� ��ü�� �ʱ�ȭ
        CreatePlayer();
        enemyManager = FindAnyObjectByType<EnemyManager>();
        inputManager = FindAnyObjectByType<InputManager>();
        followCam = FindAnyObjectByType<FollowCam>();
        skillManager = FindAnyObjectByType<SkillManager>();
        skillSelectManager = FindAnyObjectByType<SkillSelectManager>();

        // inputManger Delegate �Ҵ�
        inputManager.onPauseButtonClicked = OnPauseButtonClicked;
        inputManager.onPlayButtonClicked = onPlayButtonClicked;

        // followCam �÷��̾� ��ü �Ҵ�
        followCam.player = player;

        // skillManager�� ��ü �Ҵ�
        skillManager.player = player;

        // skillManager Delegate �Ҵ�
        skillManager.onShiledSkillActivated = OnShieldSkillActivated;
        skillManager.onShiledSkillUnActivated = OnShieldSkillUnActivated;


        // delegate �Ҵ�
        enemyManager.onEnemiesChanged = OnEnemiesChanged;
        enemyManager.onEnemyKilled = OnEnemyKilled;

        // delegate �Ҵ�
        skillSelectManager.onSkillSelectObjectDisplayed = OnSkillSelectObjectDisplayed;
        skillSelectManager.onSkillSelectObjectHided = OnSkillSelectObjectHided;
    }

    void Start()
    {
        enemyManager.CreateEnemies(50, player, 2, maxEnemySpawnRange); // ���� ��ȯ

        skillSelectManager.ChooseStartSkill(); // ���� ��ų ����
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameOver) {
            gameTime += Time.deltaTime; // ���� �ð� ����

            CalculateEnemySpawnTime(); // ��ȯ�� ���� �����ϰ� ����
        }

        skillManager.enemies = enemies;
    }

    // Player ���� �Լ�
    private void CreatePlayer(){
        player = Instantiate(playerPrefab);
        player.onPlayerWasKilled = OnPlayerHasKilled;
        player.onPlayerLevelUP = OnPlayerLevelUP;
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

    // �÷��̾ �׾��� �� �����
    private void OnPlayerHasKilled(Player player)
    {
        isGameOver = true;
        gameOverObject.SetActive(true);
        inputManager.PauseButtonObject.interactable = false;
    }

    // Pause��ư�� Ŭ������ �� �����
    private void OnPauseButtonClicked()
    {
        pauseObject.SetActive(true);
        // UI ��Ȱ��ȭ
        HpBarObject.SetActive(false);
        HpStatusLetteringObject.SetActive(false);
        HpStatusObject.SetActive(false);
        SkillPanelObject.SetActive(false);
        CharacterProfileObject.SetActive(false);
    }

    private void onPlayButtonClicked()
    {
        pauseObject.SetActive(false);
        // UI Ȱ��ȭ
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

    // ���� ���� �� delegate�� �Ҵ����� �Լ�
    private void OnShieldSkillActivated()
    {
        player.isPlayerShielded = true;
    }

    // ���� ���� �� delegate�� �Ҵ����� �Լ�
    private void OnShieldSkillUnActivated()
    {
        player.isPlayerShielded = false;
    }

    // ���� �׾��� �� �����ϴ� �Լ�
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

    // �÷��̾ ���� �� ���� �� ����
    private void OnPlayerLevelUP()
    {
        skillSelectManager.DisplayLevelupPanel();
    }

    private void OnSkillSelectObjectDisplayed()
    {
        // UI ��Ȱ��ȭ
        HpBarObject.SetActive(false);
        HpStatusLetteringObject.SetActive(false);
        HpStatusObject.SetActive(false);
        SkillPanelObject.SetActive(false);
        CharacterProfileObject.SetActive(false);
        inputManager.PauseButtonObject.interactable = false;
    }

    private void OnSkillSelectObjectHided()
    {
        // UI Ȱ��ȭ
        HpBarObject.SetActive(true);
        HpStatusLetteringObject.SetActive(true);
        HpStatusObject.SetActive(true);
        SkillPanelObject.SetActive(true);
        CharacterProfileObject.SetActive(true);
        inputManager.PauseButtonObject.interactable = true;
    }
}

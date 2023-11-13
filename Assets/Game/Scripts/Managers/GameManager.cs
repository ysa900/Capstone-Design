using System.Collections.Generic;
using UnityEngine;

// Pause �ɸ� �������� �ΰ��� �� UI��(����, ��ų �г�, ������)�� �Ȼ������
// ������� �Ϸ��� gameObject�� �����ѰŶ�
// OnPauseButtonClicked() ,onPlayButtonClicked() �޼ҵ� ��������

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager�� instanceȭ

    // ���� �ð�
    public float gameTime;
    //private float maxGameTime = 15 * 60f;

    // �� ���� ��Ÿ��
    public float CoolTime = 0f;

    // �� �ִ� ���� �Ÿ� (�ּҴ� 20, EnemyManager�� ����)
    private float maxEnemySpawnRange = 30;

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
        skillSelectManager.onPlayerHealed = OnPlayerHealed;
    }

    void Start()
    {
        enemyManager.CreateEnemies(50, player, 0, maxEnemySpawnRange); // ���� ��ȯ

        skillSelectManager.ChooseStartSkill(); // ���� ��ų ����
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameOver) {
            gameTime += Time.deltaTime; // ���� �ð� ����
            CoolTime += Time.deltaTime;

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

       
        if (gameTime <= 10f && CoolTime >= 10f)
        {
            enemyManager.CreateEnemies(50, player, 0, maxEnemySpawnRange); // Ghoul ���� ��ȯ
            CoolTime = 0f;

        }
        else if (gameTime <= 20f && CoolTime >= 10f)
        {

            enemyManager.CreateEnemies(50, player, 1, maxEnemySpawnRange); // Spitter ���� ��ȯ
            CoolTime = 0f;
        }
        else if (gameTime <= 30f && CoolTime >= 10f)
        {
            enemyManager.CreateEnemies(50, player, 2, maxEnemySpawnRange); //Summoner ���� ��ȯ
            CoolTime = 0f;
        }
        else if (gameTime <= 45f && CoolTime >= 10f)
        {
            enemyManager.CreateEnemies(50, player, 3, maxEnemySpawnRange); //BloodKing ���� ��ȯ
            CoolTime = 0f;
        }
        else
        {
            // ���� ����
        }

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

    private void OnPlayerHealed()
    {
        player.hp += 10;
        if(player.hp > 100) { player.hp = 100; }
    }
}

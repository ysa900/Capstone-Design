using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

// Pause �ɸ� �������� �ΰ��� �� UI��(����, ��ų �г�, ������)�� �Ȼ������
// ������� �Ϸ��� gameObject�� �����ѰŶ�
// OnPauseButtonClicked() ,onPlayButtonClicked() �޼ҵ� ��������

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager�� instanceȭ

    // ���� �ð�
    public float gameTime;
    public float maxGameTime = 5 * 60f; // ���� ���� �ð�

    // �� ���� ��Ÿ��
    private float CoolTime = 2f;
    private float CoolTimer = 0f;

    // GameOver�� �ƴ��� �Ǻ��ϴ� ����
    public bool isGameOver;

    // ������ �̹� ���� �ƴ��� �Ǻ��ϴ� ����
    private bool isBossSpawned;

    // ���� �ʹ� ������(300���� �̻�) �Ǻ��ϴ� ����
    private bool isEnemiesTooMany;

    // Enemy���� ���� ����Ʈ
    [SerializeField]
    [ReadOnly]
    private List<Enemy> enemies = new List<Enemy>();

    // ����� Ŭ���� ��ü��
    public Player player;
    public Boss boss;
    private GameAudioManager gameAudioManager;
    private FollowCam followCam;
    private InputManager inputManager;
    private SkillManager skillManager;
    private SkillSelectManager skillSelectManager;
    private EXP exp;
    private BossManager bossManager;
    public PoolManager poolManager;

    // GameObject���� �������� �־��ֱ� ���� public���� ����
    public Player playerPrefab;

    // EXP ������
    public EXP expPrefab1;
    public EXP expPrefab2;
    public EXP expPrefab3;

    // GameOver ������Ʈ
    public GameObject gameOverObject;
    // GameClear ������Ʈ
    public GameObject gameClearObject;

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
    // Boss HP
    public GameObject BossHPObject;

    private void Awake()
    {
        instance = this; // GameManager�� �ν��Ͻ�ȭ

        // ���� �� ��Ȱ��ȭ
        gameOverObject.SetActive(false);
        gameClearObject.SetActive(false);
        pauseObject.SetActive(false);
        HpBarObject.SetActive(true);
        BossHPObject.SetActive(false);

        // Ŭ���� ��ü�� �ʱ�ȭ
        CreatePlayer();
        inputManager = FindAnyObjectByType<InputManager>();
        gameAudioManager = FindAnyObjectByType<GameAudioManager>();
        followCam = FindAnyObjectByType<FollowCam>();
        skillManager = FindAnyObjectByType<SkillManager>();
        skillSelectManager = FindAnyObjectByType<SkillSelectManager>();
        bossManager = FindAnyObjectByType<BossManager>();
        poolManager = FindAnyObjectByType<PoolManager>();

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

        // PoolManager Player �Ҵ�
        poolManager.player = player;

        // EnemyManager delegate �Ҵ�
        poolManager.enemyManager.onEnemiesChanged = OnEnemiesChanged;
        poolManager.enemyManager.onEnemyKilled = OnEnemyKilled;

        // SkillSelectManager delegate �Ҵ�
        skillSelectManager.onSkillSelectObjectDisplayed = OnSkillSelectObjectDisplayed;
        skillSelectManager.onSkillSelectObjectHided = OnSkillSelectObjectHided;
        skillSelectManager.onPlayerHealed = OnPlayerHealed;

        // BossManager delegate �Ҵ�
        bossManager.onBossHasKilled = OnBossHasKilled;

        //gameTime = maxGameTime;
        //player.isPlayerShielded = true;
        //player.level = 20;
    }

    void Start()
    {
        gameAudioManager.PlaySfx(GameAudioManager.Sfx.Select); // GameStart ���� ȿ����
        gameAudioManager.PlayBGM(0, true); // ����� ����

        SpawnEnemies(0, 50); // ���� �� ��ȯ

        skillSelectManager.ChooseStartSkill(); // ���� ��ų ����
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameOver) {
            gameTime += Time.deltaTime; // ���� �ð� ����
            CoolTimer += Time.deltaTime;

            if (!isBossSpawned) {
                isEnemiesTooMany = enemies.Count > 300;
                SpawnBoss();
                if (!isEnemiesTooMany)
                    CalculateEnemySpawnTimeNSpawn(); // ��ȯ�� ���� �����ϰ� ����
            }
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
    private void CalculateEnemySpawnTimeNSpawn()
    {
        if (gameTime <= 60 * 1 && CoolTimer >= CoolTime)
        {
            SpawnEnemies(0, 10); // Ghoul ���� ��ȯ
            CoolTimer = 0f;
        }
        else if (gameTime <= 60 * 2 && CoolTimer >= CoolTime)
        {
            SpawnEnemies(0, 5); // Ghoul ���� ��ȯ
            SpawnEnemies(1, 10); // Spitter ���� ��ȯ
            CoolTimer = 0f;
        }
        else if (gameTime <= 60 * 3 && CoolTimer >= CoolTime)
        {
            SpawnEnemies(0, 2); // Ghoul ���� ��ȯ
            SpawnEnemies(1, 5); // Spitter ���� ��ȯ
            SpawnEnemies(2, 10); // Summoner ���� ��ȯ
            CoolTime = 1.5f;
            CoolTimer = 0f;
        }
        else if (gameTime < 60 * 4 && CoolTimer >= CoolTime)
        {
            SpawnEnemies(0, 2); // Ghoul ���� ��ȯ
            SpawnEnemies(1, 5); // Spitter ���� ��ȯ
            SpawnEnemies(2, 8); // Summoner ���� ��ȯ
            SpawnEnemies(3, 15); // BloodKing ���� ��ȯ
            CoolTime = 1f;
            CoolTimer = 0f;
        }
        else if (gameTime < 60 * 5 && CoolTimer >= CoolTime)
        {
            SpawnEnemies(0, 2); // Ghoul ���� ��ȯ
            SpawnEnemies(1, 4); // Spitter ���� ��ȯ
            SpawnEnemies(2, 6); // Summoner ���� ��ȯ
            SpawnEnemies(3, 20); // BloodKing ���� ��ȯ
            CoolTime = 1f;
            CoolTimer = 0f;
        }
    }

    // Enemy ��ȯ �Լ�
    void SpawnEnemies(int index, int num)
    {
        for (int i = 0; i < num; i++)
        {
            poolManager.GetEnemy(index); // ���� ��ȯ
        }
    }

    void SpawnBoss()
    {
        if (gameTime >= maxGameTime)
        {
            // ���� ����
            bossManager.player = player;
            bossManager.CreateBoss();

            skillManager.isBossAppear = true;
            skillManager.boss = bossManager.boss;

            // ���� HP�� active
            BossHPObject.SetActive(true);

            // ���� BGM ON
            gameAudioManager.PlayBGM(0, false); // �⺻ ����� ����
            gameAudioManager.PlayBGM(1, true); // ���� ����� ����

            isBossSpawned = true;
        }
    }

    // �÷��̾ �׾��� �� �����
    private void OnPlayerHasKilled(Player player)
    {
        StartCoroutine(PlayerHasKilled()); // ȿ���� �ֱ� ���� �ڷ�ƾ ���� �� ���
    }
    IEnumerator PlayerHasKilled()
    {
        isGameOver = true;
        gameOverObject.SetActive(true);

        yield return new WaitForSeconds(0.5f); // 0.5�� ���� �ð� �� �α�
        gameAudioManager.PlaySfx(GameAudioManager.Sfx.Lose); // ĳ���� ��� �� ȿ����
        inputManager.PauseButtonObject.interactable = false; // Pause��ư ��Ȱ��ȭ
        gameAudioManager.PlayBGM(0, false); // ����� ����
        Time.timeScale = 0; // ȭ�� ���߱�
    }

    // ������ �׾��� �� �����
    private void OnBossHasKilled()
    {
        StartCoroutine(BossHasKilled()); // ȿ���� �ֱ� ���� �ڷ�ƾ ���� �� ���
    }
    IEnumerator BossHasKilled()
    {
        player.isPlayerShielded = true;
        isGameOver = true;
        gameClearObject.SetActive(true);

        yield return new WaitForSeconds(0.5f); // 0.5�� ���� �ð� �� �α�
        gameAudioManager.PlaySfx(GameAudioManager.Sfx.Win); // �¸��� ȿ����
        inputManager.PauseButtonObject.interactable = false; // Pause��ư ��Ȱ��ȭ
        gameAudioManager.PlayBGM(0, false); // ����� ����
        Time.timeScale = 0; // ȭ�� ���߱�
    }

    // Pause��ư�� Ŭ������ �� �����
    private void OnPauseButtonClicked()
    {
        pauseObject.SetActive(true);
        gameAudioManager.PlaySfx(GameAudioManager.Sfx.Select); // ��ư Ŭ�� ȿ����
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
        gameAudioManager.PlaySfx(GameAudioManager.Sfx.Select); // ��ư Ŭ�� ȿ����
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

        int ranNum = UnityEngine.Random.Range(0, 11);

        if(ranNum >= 6)
        {
            if (killedEnemy.tag == "Ghoul")
            {
                exp = poolManager.GetExp(0);

                exp.expAmount = 1;
                exp.index = 0;

                exp.X = killedEnemy.X;
                exp.Y = killedEnemy.Y + 1f;
            }
            else if (killedEnemy.tag == "Spitter")
            {
                exp = poolManager.GetExp(1);

                exp.expAmount = 2;
                exp.index = 1;

                exp.X = killedEnemy.X;
                exp.Y = killedEnemy.Y + 1f;
            }
            else if (killedEnemy.tag == "Summoner")
            {
                exp = poolManager.GetExp(1);

                exp.expAmount = 3;
                exp.index = 1;

                exp.X = killedEnemy.X;
                exp.Y = killedEnemy.Y + 1f;
            }
            else if (killedEnemy.tag == "BloodKing")
            {
                exp = poolManager.GetExp(2);

                exp.expAmount = 4;
                exp.index = 2;

                exp.X = killedEnemy.X;
                exp.Y = killedEnemy.Y + 1f;
            }
        }
    }

    // �÷��̾ ���� �� ���� �� ����
    private void OnPlayerLevelUP()
    {
        skillSelectManager.DisplayLevelupPanel();
        gameAudioManager.PlaySfx(GameAudioManager.Sfx.LevelUp); // ������ �� ȿ����
        gameAudioManager.EffectBGM(true); // AudioFilter �ѱ�
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

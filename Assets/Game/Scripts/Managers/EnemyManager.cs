using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // Enemy들을 담을 리스트
    [SerializeField]
    [ReadOnly]
    List<Enemy> enemies = new List<Enemy>();

    // Enemy 클래스 객체
    Enemy enemy;

    // Enemy를 구분 짓기 위한 index
    int enemyNameIndex = 0;

    // Enemy들 이름
    string[] EnemyNames = { "evilTree", "pumpkin","warlock", 
                            "Skeleton_Sword", "Skeleton_Archer", "Skeleton_Horse",
                            "Ghoul", "Spitter", "Summoner", "BloodKing" };

    private GameAudioManager gameAudioManager;

    // Enemy들이 생성되었을 때에 GameManager에게 Enemy 리스트를 전달해주기 위한 delegate
    public delegate void OnEnemiesChanged(List<Enemy> enemies);
    public OnEnemiesChanged onEnemiesChanged;

    // Enemy가 죽었을 때 GameManager에게 알려주기 위한 delegate
    public delegate void OnEnemyKilled(Enemy killedEnemy);
    public OnEnemyKilled onEnemyKilled;

    float hitDelayTime = 0.1f;
    float hitDelatTimer = 0f;

    private void Awake()
    {
        gameAudioManager = FindAnyObjectByType<GameAudioManager>();
    }

    private void FixedUpdate()
    {
        hitDelatTimer += Time.fixedDeltaTime;
    }

    // 적 정보를 입력하고 적을 생성하는 함수
    public void SetEnemyInfo(Enemy select, Player player, int index)
    {
        enemy = select;

        enemy.index = index;

        enemy.name = EnemyNames[index] + enemyNameIndex; // 적들 이름 구분짓기 위한 방법
        enemyNameIndex++;

        enemy.player = player;

        // delegate 할당
        enemy.onEnemyWasKilled = OnEnemyWasKilled;
        enemy.onEnemyHit = OnEnemyHit;

        enemies.Add(enemy);

        onEnemiesChanged(enemies); // GameManager에게 emeies를 전달
    }

    // Enemy가 죽었을 때 실행할 것들
    private void OnEnemyWasKilled(Enemy killedEnemy, bool isKilledByPlayer)
    {
        if (isKilledByPlayer)
        {
            onEnemyKilled(killedEnemy); // 킬 수 늘리고, 경험치 떨구도록 GameManager에게 알려주기

            if (!GameManager.instance.isGameOver) //  캐릭터 사망하기 전까지만 실행
            {
                gameAudioManager.PlaySfx(GameAudioManager.Sfx.Dead); // Enemy 사망 시 효과음
            }
        }

        enemies.Remove(killedEnemy);

        onEnemiesChanged(enemies); // enmy 배열 업데이트하도록 GameManager에게 알려주기
    }

    void OnEnemyHit()
    {

        bool isDelayOver = hitDelatTimer >= hitDelayTime;
        if (isDelayOver)
        {
            GameAudioManager.instance.PlaySfx(GameAudioManager.Sfx.Range); // Enemy 피격 효과음
            hitDelatTimer = 0;
        }
    }
}
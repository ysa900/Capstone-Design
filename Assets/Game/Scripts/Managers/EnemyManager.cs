using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private float minEnemySpawnRange = 20; // 적 최소 생성 거리

    private float degree = 0; // 적을 원형으로 소환할 때 각도

    // Enemy들을 담을 리스트
    List<Enemy> enemies = new List<Enemy>();

    // Enemy 클래스 객체
    Enemy enemy;

    private GameAudioManager gameAudioManager;

    // Enemy 프리팹
    public Enemy ghoulPrefab;
    public Enemy spitterPrefab;
    public Enemy summonerPrefab;
    public Enemy bloodKingPrefab;

    // Enemy들이 생성되었을 때에 GameManager에게 Enemy 리스트를 전달해주기 위한 delegate
    public delegate void OnEnemiesChanged(List<Enemy> enemies);
    public OnEnemiesChanged onEnemiesChanged;

    // Enemy가 죽었을 때 GameManager에게 알려주기 위한 delegate
    public delegate void OnEnemyKilled(Enemy killedEnemy);
    public OnEnemyKilled onEnemyKilled;

    private void Awake()
    {
        gameAudioManager = FindAnyObjectByType<GameAudioManager>();
    }

    // Enemy들을 생성하는 함수
    // enemyType: 0 ~ ? (현재 0 ~ 1), 이게 몬스터 종류 결정
    public void CreateEnemies(int enemyNum, Player player, int enemyType, float maxRadius)
    {
        switch (enemyType)
        {

            case 0:
                {
                    SetEnemyInfoNSummon(enemyNum, player, ghoulPrefab, maxRadius);
                    break;
                }
            case 1:
                {
                    SetEnemyInfoNSummon(enemyNum, player, spitterPrefab, maxRadius);
                    break;
                }
            case 2:
                {
                    SetEnemyInfoNSummon(enemyNum, player, summonerPrefab, maxRadius);
                    break;
                }
            case 3:
                {
                    SetEnemyInfoNSummon(enemyNum, player, bloodKingPrefab, maxRadius);
                    break;
                }
        }

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

    // 적 정보를 입력하고 적을 생성하는 함수
    private void SetEnemyInfoNSummon(int enemyNum, Player player, Enemy enemyPrefab, float maxRadius)
    {
        for (int i = 0; i < enemyNum; i++)
        {
            enemy = Instantiate(enemyPrefab);

            float playerX = player.transform.position.x;
            float playerY = player.transform.position.y;

            // 몬스터가 원형으로 소환되게 함
            float radius = UnityEngine.Random.Range(minEnemySpawnRange, maxRadius);
            degree = UnityEngine.Random.Range(0f, 360f);

            float tmpX = (float)Math.Cos(degree) * radius;
            float tmpY = (float)Math.Sin(degree) * radius;

            enemy.X = tmpX + playerX;
            enemy.Y = tmpY + playerY;

            if (degree <= -360)
            {
                degree %= -360;
            }

            enemy.player = player;
            enemy.onEnemyWasKilled = OnEnemyWasKilled;

            enemies.Add(enemy);
        }
    }
}
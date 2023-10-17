using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // 적 최소 생성 거리
    private float minEnemySpawnRange = 20;

    // Enemy들을 담을 리스트
    List<Enemy> enemies = new List<Enemy>();

    // Enemy 클래스 객체
    Enemy enemy;

    // Enemy 프리팹
    public Enemy zombiePrefab1;
    public Enemy zombiePrefab2;
    public Enemy ghoulPrefab;
    public Enemy spitterPrefab;
    public Enemy summonerPrefab;

    // Enemy들이 생성되었을 때에 GameManager에게 Enemy 리스트를 전달해주기 위한 delegate
    public delegate void OnEnemiesChanged(List<Enemy> enemies);
    public OnEnemiesChanged onEnemiesChanged;

    // Enemy들을 생성하는 함수
    // enemyType: 0 ~ ? (현재 0 ~ 1), 이게 몬스터 종류 결정
    public void CreateEnemies(int enemyNum, Player player, int enemyType, float maxRadius)
    {
        switch (enemyType)
        {
            case 0:
                {
                    SetEnemyInfoNSummon(enemyNum, player, zombiePrefab1, maxRadius);
                    break;
                }
            case 1:
                {
                    SetEnemyInfoNSummon(enemyNum, player, zombiePrefab2, maxRadius);
                    break;
                }
            case 2:
                {
                    SetEnemyInfoNSummon(enemyNum, player, ghoulPrefab, maxRadius);
                    break;
                }
            case 3:
                {
                    SetEnemyInfoNSummon(enemyNum, player, spitterPrefab, maxRadius);
                    break;
                }
            case 4:
                {
                    SetEnemyInfoNSummon(enemyNum, player, summonerPrefab, maxRadius);
                    break;
                }

        }

        onEnemiesChanged(enemies); // GameManager에게 emeies를 전달
    }

    // Enemy가 죽었을 때 실행할 것들
    private void OnEnemyWasKilled(Enemy killedEnemy)
    {
        enemies.Remove(killedEnemy);
        onEnemiesChanged(enemies);
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
            // x는 랜덤 값을 받고, y = 루트(r^2 - x^2)
            float randomRadius = UnityEngine.Random.Range(minEnemySpawnRange, maxRadius);
            enemy.X = UnityEngine.Random.Range(-randomRadius, randomRadius);
            double tmp = Math.Pow(randomRadius, 2) - Math.Pow(enemy.X, 2);
            enemy.Y = (float)Math.Sqrt(tmp);

            if (UnityEngine.Random.Range(0, 2) == 0) // Y값에 루트를 하면 항상 양수만 나오니까 랜덤으로 음수값 부여
                enemy.Y = -enemy.Y;

            enemy.Y += playerY;
            enemy.X += playerX; 
            enemy.player = player;
            enemy.onEnemyWasKilled = OnEnemyWasKilled;

            enemies.Add(enemy);
        }
    }
}
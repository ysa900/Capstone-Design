using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyManager : MonoBehaviour
{
    // Enemy들을 담을 리스트
    List<Enemy> enemies;

    // Enemy 클래스 객체
    Enemy enemy;
    Enemy1 enemy1;

    // Enemy 프리팹
    public Enemy enemyPrefab;
    public Enemy1 enemyPrefab1;
    // Enemy들을 생성하는 함수
    public void CreateEnemies(int num, Player player)
    {
        for(int i = 0; i < num; i++)
        {
            enemy = Instantiate(enemyPrefab);
      

            enemy.X = UnityEngine.Random.Range(-20, 20);
            enemy.Y = UnityEngine.Random.Range(-10, 10);
     

    
            enemy.player = player;

            enemy.onEnemyWasKilled = OnEnemyWasKilled;

            //enemies.Add(enemy);
        }

        for (int i = 0; i < num; i++)
        {
            enemy1 = Instantiate(enemyPrefab1);


            enemy1.X = UnityEngine.Random.Range(-10, 10);
            enemy1.Y = UnityEngine.Random.Range(-20, 20);



            enemy1.player = player;

   

            //enemies.Add(enemy);
        }

    }

    // Enemy가 죽었을 때 실행할 것들
    private void OnEnemyWasKilled(Enemy killedEnemy)
    {
        //
    }
}
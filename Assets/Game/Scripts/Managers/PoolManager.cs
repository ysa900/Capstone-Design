using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    const int ENEMY_NUM = 4;

    // 사용할 클래스 객체들
    public EnemyManager enemyManager;
    public SkillManager skillManager;
    public BossManager bossManager;

    public Player player;

    // 프리팹 보관할 변수
    public Enemy[] Enemy_prefabs = new Enemy[ENEMY_NUM]; // ENEMY_NUM = 4

    // 풀 담당을 하는 리스트들
    List<Enemy>[] Enemy_pools;

    private void Awake()
    {
        // 클래스 객체들 초기화
        enemyManager = FindAnyObjectByType<EnemyManager>();
        skillManager = FindAnyObjectByType<SkillManager>();
        bossManager = FindAnyObjectByType<BossManager>();

        Enemy_pools = new List<Enemy>[Enemy_prefabs.Length];
        for(int index = 0; index < Enemy_pools.Length; index++)
        {
            Enemy_pools[index] = new List<Enemy>();
        }

        for(int i = 0; i < Enemy_pools.Length; i++)
        {
            CreateEnemies(i, 50);
        }
    }

    public Enemy GetEnemy(int index)
    {
        Enemy select = null;

        // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
        foreach(Enemy item in Enemy_pools[index]) { 

            if(!item.gameObject.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;
                if(select.GetComponent<IPullingObject>() != null)
                {
                    enemyManager.SetEnemyInfo(select, player, index);
                    select.GetComponent<IPullingObject>().Init();

                }
                select.gameObject.SetActive(true);
                break;
            }
        }

        // 못 찾았으면?      
        if(!select)
        {
            // 새롭게 생성하고 select 변수에 할당
            // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
            select = Instantiate(Enemy_prefabs[index]);

            enemyManager.SetEnemyInfo(select, player, index);

            select.Init();

            select.transform.SetParent(this.gameObject.transform);
            Enemy_pools[index].Add(select);
        }

        return select;
    }

    public void Get(int index) { }
    
    public void ReturnEnemy(Enemy obj, int index)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(this.gameObject.transform);
        Enemy_pools[index].Add(obj);
    }

    // Enemy를 새롭게 생성
    void CreateEnemies(int index, int num)
    {
        for(int i = 0; i < num; i++)
        {
            Enemy enemy = Instantiate(Enemy_prefabs[index]);

            enemy.transform.SetParent(this.gameObject.transform); // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
            Enemy_pools[index].Add(enemy);

            enemy.gameObject.SetActive(false);
        }
    }
}

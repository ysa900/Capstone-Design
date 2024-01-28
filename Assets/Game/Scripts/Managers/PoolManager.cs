using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    const int ENEMY_NUM = 4;

    // ����� Ŭ���� ��ü��
    public EnemyManager enemyManager;
    public SkillManager skillManager;
    public BossManager bossManager;

    public Player player;

    // ������ ������ ����
    public Enemy[] Enemy_prefabs = new Enemy[ENEMY_NUM]; // ENEMY_NUM = 4

    // Ǯ ����� �ϴ� ����Ʈ��
    List<Enemy>[] Enemy_pools;

    private void Awake()
    {
        // Ŭ���� ��ü�� �ʱ�ȭ
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

        // ������ Ǯ�� ����ִ�(��Ȱ��ȭ��) ���� ������Ʈ ����    
        foreach(Enemy item in Enemy_pools[index]) { 

            if(!item.gameObject.activeSelf)
            {
                // �߰��ϸ� select ������ �Ҵ�
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

        // �� ã������?      
        if(!select)
        {
            // ���Ӱ� �����ϰ� select ������ �Ҵ�
            // �ڱ� �ڽ�(transform) �߰� ����: hierarchyâ ������������ �� ����
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

    // Enemy�� ���Ӱ� ����
    void CreateEnemies(int index, int num)
    {
        for(int i = 0; i < num; i++)
        {
            Enemy enemy = Instantiate(Enemy_prefabs[index]);

            enemy.transform.SetParent(this.gameObject.transform); // �ڱ� �ڽ�(transform) �߰� ����: hierarchyâ ������������ �� ����
            Enemy_pools[index].Add(enemy);

            enemy.gameObject.SetActive(false);
        }
    }
}

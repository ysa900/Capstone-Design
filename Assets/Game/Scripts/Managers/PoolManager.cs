using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    const int ENEMY_NUM = 4;
    const int EXP_NUM = 3;
    const int SKILL_NUM = 15; // ��: 0 ~ 6, ����: 7 ~ 10, ��: 11 ~ 14

    // ����� Ŭ���� ��ü��
    public EnemyManager enemyManager;
    public Player player;

    // ������ ������ ����
    public Enemy[] Enemy_prefabs = new Enemy[ENEMY_NUM]; // ENEMY_NUM = 4
    public EXP[] Exp_prefabs = new EXP[EXP_NUM]; // EXP_NUM = 3
    public Skill[] Skill_prefabs = new Skill[SKILL_NUM]; // SKILL_NUM = 15

    // Ǯ ����� �ϴ� ����Ʈ��
    List<Enemy>[] Enemy_pools;
    List<EXP>[] Exp_pools;
    List<Skill>[] Skill_pools;

    private void Awake()
    {
        // Ŭ���� ��ü�� �ʱ�ȭ
        enemyManager = FindAnyObjectByType<EnemyManager>();

        Enemy_pools = new List<Enemy>[Enemy_prefabs.Length];
        for(int index = 0; index < Enemy_pools.Length; index++)
        {
            Enemy_pools[index] = new List<Enemy>();
        }

        Exp_pools = new List<EXP>[Exp_prefabs.Length];
        for (int index = 0; index < Exp_pools.Length; index++)
        {
            Exp_pools[index] = new List<EXP>();
        }

        Skill_pools = new List<Skill>[Skill_prefabs.Length];
        for (int index = 0; index < Skill_pools.Length; index++)
        {
            Skill_pools[index] = new List<Skill>();
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

            select.transform.SetParent(this.gameObject.transform.GetChild(0));
            Enemy_pools[index].Add(select);
        }

        return select;
    }
    public void ReturnEnemy(Enemy obj, int index)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(this.gameObject.transform.GetChild(0));
        Enemy_pools[index].Add(obj);
    }

    // Enemy�� ���Ӱ� ����
    void CreateEnemies(int index, int num)
    {
        for(int i = 0; i < num; i++)
        {
            Enemy enemy = Instantiate(Enemy_prefabs[index]);

            enemy.transform.SetParent(this.gameObject.transform.GetChild(0)); // �ڱ� �ڽ�(transform) �߰� ����: hierarchyâ ������������ �� ����
            Enemy_pools[index].Add(enemy);

            enemy.gameObject.SetActive(false);
        }
    }

    public EXP GetExp(int index)
    {
        EXP select = null;

        // ������ Ǯ�� ����ִ�(��Ȱ��ȭ��) ���� ������Ʈ ����    
        foreach (EXP item in Exp_pools[index])
        {
            if (!item.gameObject.activeSelf)
            {
                // �߰��ϸ� select ������ �Ҵ�
                select = item;

                if (select.GetComponent<IPullingObject>() != null)
                    select.GetComponent<IPullingObject>().Init();
                
                select.gameObject.SetActive(true);
                break;
            }
        }

        // �� ã������?      
        if (!select)
        {
            // ���Ӱ� �����ϰ� select ������ �Ҵ�
            // �ڱ� �ڽ�(transform) �߰� ����: hierarchyâ ������������ �� ����
            select = Instantiate(Exp_prefabs[index]);

            select.Init();

            select.transform.SetParent(this.gameObject.transform.GetChild(1));
            Exp_pools[index].Add(select);
        }

        return select;
    }
    public void ReturnExp(EXP obj, int index)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(this.gameObject.transform.GetChild(1));
        Exp_pools[index].Add(obj);
    }

    // Exp�� ���Ӱ� ����
    void CreateExps(int index, int num)
    {
        for (int i = 0; i < num; i++)
        {
            EXP exp = Instantiate(Exp_prefabs[index]);

            exp.transform.SetParent(this.gameObject.transform.GetChild(1)); // �ڱ� �ڽ�(transform) �߰� ����: hierarchyâ ������������ �� ����
            Exp_pools[index].Add(exp);

            exp.gameObject.SetActive(false);
        }
    }

    public Skill GetSkill(int index, Object target)
    {
        Skill select = null;

        // ������ Ǯ�� ����ִ�(��Ȱ��ȭ��) ���� ������Ʈ ����    
        foreach (Skill item in Skill_pools[index])
        {
            if (!item.gameObject.activeSelf)
            {
                // �߰��ϸ� select ������ �Ҵ�
                select = item;

                if (select.GetComponent<IPullingObject>() != null)
                {
                    if (target is Enemy)// ��� ����ȯ Ȱ��
                    {
                        select.enemy = target as Enemy;
                    }
                    else if (target is Boss)
                        select.boss = target as Boss;

                    select.player = player;
                    select.index = index; // return�� ���� index �ο�

                    select.GetComponent<IPullingObject>().Init();
                }
                select.gameObject.SetActive(true);
                break;
            }
        }

        // �� ã������?      
        if (!select)
        {
            // ���Ӱ� �����ϰ� select ������ �Ҵ�
            select = Instantiate(Skill_prefabs[index]);

            if (select.GetComponent<IPullingObject>() != null)
            {
                if (target is Enemy) // ��� ����ȯ Ȱ��
                {
                    select.enemy = target as Enemy;
                }
                else if (target is Boss)
                    select.boss = target as Boss;

                select.player = player;
                select.index = index; // return�� ���� index �ο�

                select.GetComponent<IPullingObject>().Init();
            }

            select.transform.SetParent(this.gameObject.transform.GetChild(2));
            Skill_pools[index].Add(select);
        }

        return select;
    }

    public Skill GetSkill(int index)
    {
        Skill select = null;

        // ������ Ǯ�� ����ִ�(��Ȱ��ȭ��) ���� ������Ʈ ����    
        foreach (Skill item in Skill_pools[index])
        {
            if (!item.gameObject.activeSelf)
            {
                // �߰��ϸ� select ������ �Ҵ�
                select = item;

                if (select.GetComponent<IPullingObject>() != null)
                {
                    select.player = player;
                    select.index = index; // return�� ���� index �ο�

                    select.GetComponent<IPullingObject>().Init();
                }
                select.gameObject.SetActive(true);
                break;
            }
        }

        // �� ã������?      
        if (!select)
        {
            // ���Ӱ� �����ϰ� select ������ �Ҵ�
            select = Instantiate(Skill_prefabs[index]);

            if (select.GetComponent<IPullingObject>() != null)
            {
                select.player = player;
                select.index = index; // return�� ���� index �ο�

                select.GetComponent<IPullingObject>().Init();
            }

            select.transform.SetParent(this.gameObject.transform.GetChild(2));
            Skill_pools[index].Add(select);
        }

        return select;
    }

    public void ReturnSkill(Skill obj, int index)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(this.gameObject.transform.GetChild(2));
        Skill_pools[index].Add(obj);
    }

    // Skill�� ���Ӱ� ����
    void CreateSkills(int index, int num)
    {
        for (int i = 0; i < num; i++)
        {
            Skill skill = Instantiate(Skill_prefabs[index]);

            skill.transform.SetParent(this.gameObject.transform.GetChild(2)); // �ڱ� �ڽ�(transform) �߰� ����: hierarchyâ ������������ �� ����
            Skill_pools[index].Add(skill);

            skill.gameObject.SetActive(false);
        }
    }
}

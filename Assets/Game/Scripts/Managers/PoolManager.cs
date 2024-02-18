using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    const int ENEMY_NUM = 4;
    const int EXP_NUM = 3;
    const int SKILL_NUM = 15; // 불: 0 ~ 6, 전기: 7 ~ 10, 물: 11 ~ 14

    // 사용할 클래스 객체들
    public EnemyManager enemyManager;
    public Player player;

    // 프리팹 보관할 변수
    public Enemy[] Enemy_prefabs = new Enemy[ENEMY_NUM]; // ENEMY_NUM = 4
    public EXP[] Exp_prefabs = new EXP[EXP_NUM]; // EXP_NUM = 3
    public Skill[] Skill_prefabs = new Skill[SKILL_NUM]; // SKILL_NUM = 15

    // 풀 담당을 하는 리스트들
    List<Enemy>[] Enemy_pools;
    List<EXP>[] Exp_pools;
    List<Skill>[] Skill_pools;

    private void Awake()
    {
        // 클래스 객체들 초기화
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

    // Enemy를 새롭게 생성
    void CreateEnemies(int index, int num)
    {
        for(int i = 0; i < num; i++)
        {
            Enemy enemy = Instantiate(Enemy_prefabs[index]);

            enemy.transform.SetParent(this.gameObject.transform.GetChild(0)); // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
            Enemy_pools[index].Add(enemy);

            enemy.gameObject.SetActive(false);
        }
    }

    public EXP GetExp(int index)
    {
        EXP select = null;

        // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
        foreach (EXP item in Exp_pools[index])
        {
            if (!item.gameObject.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;

                if (select.GetComponent<IPullingObject>() != null)
                    select.GetComponent<IPullingObject>().Init();
                
                select.gameObject.SetActive(true);
                break;
            }
        }

        // 못 찾았으면?      
        if (!select)
        {
            // 새롭게 생성하고 select 변수에 할당
            // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
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

    // Exp를 새롭게 생성
    void CreateExps(int index, int num)
    {
        for (int i = 0; i < num; i++)
        {
            EXP exp = Instantiate(Exp_prefabs[index]);

            exp.transform.SetParent(this.gameObject.transform.GetChild(1)); // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
            Exp_pools[index].Add(exp);

            exp.gameObject.SetActive(false);
        }
    }

    public Skill GetSkill(int index, Object target)
    {
        Skill select = null;

        // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
        foreach (Skill item in Skill_pools[index])
        {
            if (!item.gameObject.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;

                if (select.GetComponent<IPullingObject>() != null)
                {
                    if (target is Enemy)// 상속 형변환 활용
                    {
                        select.enemy = target as Enemy;
                    }
                    else if (target is Boss)
                        select.boss = target as Boss;

                    select.player = player;
                    select.index = index; // return을 위해 index 부여

                    select.GetComponent<IPullingObject>().Init();
                }
                select.gameObject.SetActive(true);
                break;
            }
        }

        // 못 찾았으면?      
        if (!select)
        {
            // 새롭게 생성하고 select 변수에 할당
            select = Instantiate(Skill_prefabs[index]);

            if (select.GetComponent<IPullingObject>() != null)
            {
                if (target is Enemy) // 상속 형변환 활용
                {
                    select.enemy = target as Enemy;
                }
                else if (target is Boss)
                    select.boss = target as Boss;

                select.player = player;
                select.index = index; // return을 위해 index 부여

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

        // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
        foreach (Skill item in Skill_pools[index])
        {
            if (!item.gameObject.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;

                if (select.GetComponent<IPullingObject>() != null)
                {
                    select.player = player;
                    select.index = index; // return을 위해 index 부여

                    select.GetComponent<IPullingObject>().Init();
                }
                select.gameObject.SetActive(true);
                break;
            }
        }

        // 못 찾았으면?      
        if (!select)
        {
            // 새롭게 생성하고 select 변수에 할당
            select = Instantiate(Skill_prefabs[index]);

            if (select.GetComponent<IPullingObject>() != null)
            {
                select.player = player;
                select.index = index; // return을 위해 index 부여

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

    // Skill을 새롭게 생성
    void CreateSkills(int index, int num)
    {
        for (int i = 0; i < num; i++)
        {
            Skill skill = Instantiate(Skill_prefabs[index]);

            skill.transform.SetParent(this.gameObject.transform.GetChild(2)); // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
            Skill_pools[index].Add(skill);

            skill.gameObject.SetActive(false);
        }
    }
}

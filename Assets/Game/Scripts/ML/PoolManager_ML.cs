using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager_ML : MonoBehaviour
{
    const int ENEMY_NUM = 10;
    const int EXP_NUM = 3;
    const int SKILL_NUM = 15; // 불: 0 ~ 6, 전기: 7 ~ 10, 물: 11 ~ 14
    const int BOSS_SKILL_NUM = 4;

    // 사용할 클래스 객체들
    public EnemyManager_ML enemyManager_ML;
    public Player_ML player_ML;

    // 프리팹 보관할 변수
    public Enemy_ML[] Enemy_ML_prefabs = new Enemy_ML[ENEMY_NUM]; // ENEMY_NUM = 7
    public EXP[] Exp_prefabs = new EXP[EXP_NUM]; // EXP_NUM = 3
    public Skill[] Skill_prefabs = new Skill[SKILL_NUM]; // SKILL_NUM = 15
    public BossSkill[] Boss_Skill_prefabs = new BossSkill[BOSS_SKILL_NUM]; // BOSS_SKILL_NUM = 4
    public GameObject damageText;
    public Arrow arrow;
    public Arrow_ML arrow_ML;

    // 풀 담당을 하는 리스트들
    List<Enemy_ML>[] Enemy_ML_pools;
    List<EXP>[] Exp_pools;
    List<Skill>[] Skill_pools;
    List<BossSkill>[] Boss_Skill_pools;
    List<GameObject> Damage_Text_pools = new List<GameObject>();
    List<Arrow> arrow_pools = new List<Arrow>();
    List<Arrow_ML> arrow_pools_ML = new List<Arrow_ML>(); // ML

    private void Awake()
    {
        // ML
        Enemy_ML_pools = new List<Enemy_ML>[Enemy_ML_prefabs.Length];
        for (int index = 0; index < Enemy_ML_pools.Length; index++)
        {
            Enemy_ML_pools[index] = new List<Enemy_ML>();
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

        Boss_Skill_pools = new List<BossSkill>[Boss_Skill_prefabs.Length];
        for (int index = 0; index < Boss_Skill_prefabs.Length; index++)
        {
            Boss_Skill_pools[index] = new List<BossSkill>();
        }
    }

    public Enemy_ML GetEnemy(int index)
    {
        Enemy_ML select = null;

        // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
        foreach (Enemy_ML item in Enemy_ML_pools[index])
        {

            if (!item.gameObject.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;
                if (select.GetComponent<IPoolingObject>() != null)
                {
                    enemyManager_ML.SetEnemyInfo(select, player_ML, index);
                    select.GetComponent<IPoolingObject>().Init();
                }
                select.gameObject.SetActive(true);
                break;
            }
        }

        // 못 찾았으면?      
        if (!select)
        {
            // 새롭게 생성하고 select 변수에 할당
            select = Instantiate(Enemy_ML_prefabs[index]);

            enemyManager_ML.SetEnemyInfo(select, player_ML, index);

            select.Init(); // 얘는 Init 해줘야됨

            select.transform.SetParent(this.gameObject.transform.GetChild(0));
            Enemy_ML_pools[index].Add(select);
        }

        return select;
    }
    public void ReturnEnemy(Enemy_ML obj, int index)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(this.gameObject.transform.GetChild(0));
        Enemy_ML_pools[index].Add(obj);
    }

    // Enemy를 새롭게 생성
    void CreateEnemies(int index, int num)
    {
        for (int i = 0; i < num; i++)
        {
            Enemy_ML enemy_ML = Instantiate(Enemy_ML_prefabs[index]);

            enemy_ML.transform.SetParent(this.gameObject.transform.GetChild(0));
            Enemy_ML_pools[index].Add(enemy_ML);

            enemy_ML.gameObject.SetActive(false);
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

                if (select.GetComponent<IPoolingObject>() != null)
                    select.GetComponent<IPoolingObject>().Init();

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

            //select.Init();

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

                if (select.GetComponent<IPoolingObject>() != null)
                {
                    if (target is Enemy_ML)// 상속 형변환 활용
                    {
                        select.enemy_ML = target as Enemy_ML;
                    }
                    else if (target is Boss)
                        select.boss = target as Boss;

                    select.player_ML = player_ML;
                    select.returnIndex = index; // return을 위해 index 부여

                    select.GetComponent<IPoolingObject>().Init();
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

            if (select.GetComponent<IPoolingObject>() != null)
            {
                if (target is Enemy_ML) // 상속 형변환 활용
                {
                    select.enemy_ML = target as Enemy_ML;
                }
                else if (target is Boss)
                {
                    select.boss = target as Boss;
                }

                select.player_ML = player_ML;
                select.returnIndex = index; // return을 위해 index 부여

                //select.GetComponent<IPoolingObject>().Init();
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

                if (select.GetComponent<IPoolingObject>() != null)
                {
                    select.player_ML = player_ML;
                    select.returnIndex = index; // return을 위해 index 부여

                    select.GetComponent<IPoolingObject>().Init();
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

            if (select.GetComponent<IPoolingObject>() != null)
            {
                select.player_ML = player_ML;
                select.returnIndex = index; // return을 위해 index 부여

                //select.GetComponent<IPoolingObject>().Init();
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

    public BossSkill GetBossSkill(int index)
    {
        BossSkill select = null;

        // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근
        foreach (BossSkill item in Boss_Skill_pools[index])
        {
            if (!item.gameObject.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;

                if (select.GetComponent<IPoolingObject>() != null)
                {
                    select.index = index; // return을 위해 index 부여

                    select.GetComponent<IPoolingObject>().Init();
                }
                select.gameObject.SetActive(true);
                break;
            }
        }

        // 못 찾았으면?      
        if (!select)
        {
            // 새롭게 생성하고 select 변수에 할당
            select = Instantiate(Boss_Skill_prefabs[index]);

            if (select.GetComponent<IPoolingObject>() != null)
            {
                // ML에 필요없어서 주석 처리
                // select.player = player;
                select.index = index; // return을 위해 index 부여

                //select.GetComponent<IPoolingObject>().Init();
            }

            select.transform.SetParent(this.gameObject.transform.GetChild(3));
            Boss_Skill_pools[index].Add(select);
        }

        return select;
    }
    public BossSkill GetBossSkill(int index, float num) // Boss Laser 때문에 만든 것
    {
        BossSkill select = null;

        // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근
        foreach (BossSkill item in Boss_Skill_pools[index])
        {
            if (!item.gameObject.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;

                if (select.GetComponent<IPoolingObject>() != null)
                {
                    select.index = index; // return을 위해 index 부여
                    select.laserTurnNum = num;

                    select.GetComponent<IPoolingObject>().Init();
                }
                select.gameObject.SetActive(true);
                break;
            }
        }

        // 못 찾았으면?      
        if (!select)
        {
            // 새롭게 생성하고 select 변수에 할당
            select = Instantiate(Boss_Skill_prefabs[index]);

            if (select.GetComponent<IPoolingObject>() != null)
            {
                // ML에 필요없어서 주석 처리
                // select.player = player;
                select.index = index; // return을 위해 index 부여
                select.laserTurnNum = num;

                //select.GetComponent<IPoolingObject>().Init();
            }

            select.transform.SetParent(this.gameObject.transform.GetChild(3));
            Boss_Skill_pools[index].Add(select);
        }

        return select;
    }
    public BossSkill GetBossSkill(int index, float x, float y, bool b) // Grid laser 때문에 만든 것
    {
        BossSkill select = null;

        // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근
        foreach (BossSkill item in Boss_Skill_pools[index])
        {
            if (!item.gameObject.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;

                if (select.GetComponent<IPoolingObject>() != null)
                {
                    select.index = index; // return을 위해 index 부여
                    select.X = x;
                    select.Y = y;
                    select.isRightTop = b;

                    select.GetComponent<IPoolingObject>().Init();
                }
                select.gameObject.SetActive(true);
                break;
            }
        }

        // 못 찾았으면?      
        if (!select)
        {
            // 새롭게 생성하고 select 변수에 할당
            select = Instantiate(Boss_Skill_prefabs[index]);

            if (select.GetComponent<IPoolingObject>() != null)
            {
                // ML에 필요없어서 주석 처리
                // select.player = player;
                select.index = index; // return을 위해 index 부여
                select.X = x;
                select.Y = y;
                select.isRightTop = b;

                //select.GetComponent<IPoolingObject>().Init();
            }

            select.transform.SetParent(this.gameObject.transform.GetChild(3));
            Boss_Skill_pools[index].Add(select);
        }

        return select;
    }
    public void ReturnBossSkill(BossSkill obj, int index)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(this.gameObject.transform.GetChild(3));
        Boss_Skill_pools[index].Add(obj);
    }

    public GameObject GetText(int damage, string skillTag)
    {
        GameObject select = null;

        // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
        foreach (GameObject item in Damage_Text_pools)
        {
            if (!item.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;
                if (select.GetComponent<IPoolingObject>() != null)
                {
                    select.GetComponent<DamageText>().damage = damage;
                    select.GetComponent<DamageText>().skillTag = skillTag;
                    select.GetComponent<IPoolingObject>().Init();
                }
                select.gameObject.SetActive(true);
                break;
            }
        }

        // 못 찾았으면?      
        if (!select)
        {
            // 새롭게 생성하고 select 변수에 할당
            // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
            select = Instantiate(damageText);
            select.GetComponent<DamageText>().damage = damage;
            select.GetComponent<DamageText>().skillTag = skillTag;

            select.transform.SetParent(this.gameObject.transform.GetChild(4));
            Damage_Text_pools.Add(select);
        }

        return select;
    }
    public void ReturnText(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(this.gameObject.transform.GetChild(4));
        Damage_Text_pools.Add(obj);
    }

    void CreateTexts(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject tmpObject = Instantiate(damageText);

            tmpObject.transform.SetParent(this.gameObject.transform.GetChild(4)); // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
            Damage_Text_pools.Add(tmpObject);

            tmpObject.gameObject.SetActive(false);
        }
    }

    public Arrow_ML GetArrow(float x, float y, Enemy_ML enemy)
    {
        Arrow_ML select = null;

        // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
        foreach (Arrow_ML item in arrow_pools_ML)
        {
            if (!item.gameObject.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;

                select.X = enemy.X + x;
                select.Y = enemy.Y + y;

                if (select.GetComponent<IPoolingObject>() != null)
                    select.GetComponent<IPoolingObject>().Init();

                select.gameObject.SetActive(true);
                break;
            }
        }

        // 못 찾았으면?      
        if (!select)
        {
            // 새롭게 생성하고 select 변수에 할당
            // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
            select = Instantiate(arrow_ML);

            select.X = enemy.X + x;
            select.Y = enemy.Y + y;

            select.transform.SetParent(this.gameObject.transform.GetChild(5));
            arrow_pools_ML.Add(select);
        }

        return select;
    }
    public void ReturnArrow(Arrow obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(this.gameObject.transform.GetChild(5));
        arrow_pools.Add(obj);
    }
    // ML
    public void ReturnArrow(Arrow_ML obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(this.gameObject.transform.GetChild(5));
        arrow_pools_ML.Add(obj);
    }

    void CreateArrows(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Arrow tmpObject = Instantiate(arrow);

            tmpObject.transform.SetParent(this.gameObject.transform.GetChild(5)); // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
            arrow_pools.Add(tmpObject);

            tmpObject.gameObject.SetActive(false);
        }
    }
}
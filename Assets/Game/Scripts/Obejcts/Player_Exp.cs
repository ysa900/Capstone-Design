using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Exp : Player
{
    float minDistanceToExp; // 제일 가까운 경험치와 플레이어 사이의 거리

    protected override void Start()
    {
        base.Start();

        minDistanceToExp = 0f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        distanceToExp = new List<float>();

        coolTimer += Time.fixedDeltaTime;

        for (int i = 0; i < GameManager.instance.poolManager.Exp_Active_pools.Length; i++)
        {
            for (int j = 0; j < GameManager.instance.poolManager.Exp_Active_pools[i].Count; j++)
            {
                distanceToExp.Add(Vector3.Distance(transform.position, GameManager.instance.poolManager.Exp_Active_pools[i][j].transform.position));
            }
        }

        for (int i = 0; i < distanceToExp.Count; i++)
        {
            minDistanceToExp = distanceToExp[0];
            if (minDistanceToExp > distanceToExp[i])
            {
                 minDistanceToExp = distanceToEnemy[i];
            }
        }
   }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void OnEpisodeBegin()
    {
        if(count != 0)
        {
            Debug.Log("에피소드 시작");
            GameManager.instance.playerData.kill = 0;
            increaseWeight = 2f;
            SceneManager.LoadScene("Stage1");
        }
        count++;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.instance.gameTime >= 90f)
        {
            GameManager.instance.skillManager.skillData.skillSelected[3] = true;
            GameManager.instance.skillManager.skillData.Damage[0] = 40f;
        }
        else if (GameManager.instance.gameTime >= 40f)
        {
            GameManager.instance.skillManager.skillData.skillSelected[2] = true;
        }
        else if (GameManager.instance.gameTime >= 20f)
        {
            GameManager.instance.skillManager.skillData.skillSelected[1] = true;
        }

        if (GameManager.instance.gameTime >= 240f)
        {
            SetReward(+3f);
        }
        else if (GameManager.instance.gameTime == 300f)
        {
            SetReward(+4);
            isEpisodeEnd = true;
            EndEpisode();
        }

        if (GameManager.instance.playerData.hp < 90f)
        {
            SetReward(-3);
            isEpisodeEnd = true;
            EndEpisode();
        }

        if (isExpGet)
        {
            SetReward(+1);
        }

        if (expCount != 0 && expCount % 10 == 0)
        {
            SetReward(increaseWeight);
            increaseWeight += 0.5f;
        }

        if (minDistanceToExp < 2f)
        {
            SetReward(+0.01f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (coolTimer >= coolTime)
        {
            switch (collision.gameObject.tag)
            {
                case "EvilTree":
                case "Pumpkin":
                case "WarLock":
                case "Skeleton_Sword":
                case "Skeleton_Horse":
                case "Skeleton_Archer":
                case "Splitter":
                case "Ghoul":
                case "Summoner":
                case "BloodKing":
                        SetReward(-0.5f / increaseWeight);
                    break;
            }
            coolTimer = 0f;
        }
    }
}
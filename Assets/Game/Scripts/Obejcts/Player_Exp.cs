using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Exp : Player
{
    float minDistanceToExp; // 제일 가까운 경험치와 플레이어 사이의 거리

    protected override void Start()
    {
        base.Start();

        minDistanceToExp = 10f;
    }

    private void Update()
    {

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        distanceToExp = new List<float>();

        coolTimer += Time.fixedDeltaTime;

        if (GameManager.instance.gameTime >= 240f)
        {
            SetReward(+3f);
        }
        else if (GameManager.instance.gameTime == 300f)
        {
            SetReward(+4);
            endCheckPoint--;
            EndEpisode();
        }

        if (GameManager.instance.playerData.hp < 90f)
        {
            SetReward(-3);
            Debug.Log("Hp 깎여서 마이너스");
            endCheckPoint--;
            EndEpisode();
        }

        if (expCount != 0 && expCount % 10 == 0)
        {
            SetReward(increaseWeight);
            increaseWeight += 0.5f;
        }


        if (preExp < expCount)
        {
            SetReward(+1);
            Debug.Log("Exp와 먹어서 리워드 +1 획득");
            preExp = expCount;
        }

        if (minDistanceToExp < 5f)
        {
            SetReward(+0.01f);
            Debug.Log("Exp와 가까워져서 리워드 +0.01 획득");
            minDistanceToExp = 10f;
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
            expCount = 0;
            preExp = 0;
            endCheckPoint = 1;
            minDistanceToExp = 10f;
            SceneManager.LoadScene("Stage1");
        }
        count++;
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        nextMove.x = actions.ContinuousActions[0];
        nextMove.y = actions.ContinuousActions[1];

        transform.Translate(nextMove * Time.deltaTime * speed);

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
                minDistanceToExp = distanceToExp[i];
            }
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
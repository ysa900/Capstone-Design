using System;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Exp : Player
{
    float minDistanceToExp; // 제일 가까운 경험치와 플레이어 사이의 거리

    protected override void Start()
    {
        base.Start();
        csvTest = FindAnyObjectByType<CsvTest>(); // 로그 기록용
        GameManager.instance.playerData.kill = 0;
        minDistanceToExp = float.MaxValue;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        UpdateMinDistanceToExp();

        // 학습 위한 플레이어의 스킬 세팅
        if (GameManager.instance.gameTime >= 140f)
        {
            GameManager.instance.skillManager.skillData.skillSelected[4] = true;
            GameManager.instance.skillManager.skillData.Damage[2] = 7.5f;
        }
        else if (GameManager.instance.gameTime > 80f)
        {
            GameManager.instance.skillManager.skillData.skillSelected[3] = true;
            GameManager.instance.skillManager.skillData.Damage[0] = 30f;
        }
        else if (GameManager.instance.gameTime >= 40f)
        {
            GameManager.instance.skillManager.skillData.skillSelected[2] = true;
        }
        else if (GameManager.instance.gameTime > 20f)
        {
            GameManager.instance.skillManager.skillData.skillSelected[1] = true;
        }

        if (GameManager.instance.gameTime >= 300f)
        {
            SetReward(+4);
            EndEpisode();
        }
        else if (GameManager.instance.gameTime >= 240f)
        {
            SetReward(+3);
        }

        if (GameManager.instance.playerData.hp < 50f)
        {
            SetReward(-3);
            Debug.Log("Hp 깎여서 마이너스");
            EndEpisode();
        }

        if (expCount != 0 && expCount % 2 == 0)
        {
            SetReward(increaseWeight);
        }

        if (expCount != 0 && expCount % 5 == 0)
        {
            SetReward(+2f);
        }

        if (preExp < expCount)
        {
            SetReward(+2);
            Debug.Log("Exp와 먹어서 리워드 +2 획득");
            preExp = expCount;
        }

        if (minDistanceToExp < 2f)
        {
            SetReward(+0.4f);
            Debug.Log("Exp와 가까워져서 리워드 +0.4 획득");
            minDistanceToExp = float.MaxValue;
        }

        if (delayTimer >= delayTime)
        {
            SetReward(+0.2f);
            increaseWeight += 0.5f;
            delayTimer = 0f;
        }

        if (GameManager.instance.gameTime >= 20f)
        {
            for (int i = 0; i < GameManager.instance.enemies.Count; i++)
            {
                GameManager.instance.enemies[i].transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void OnEpisodeBegin()
    {
        if (count != 0)
        {
            Debug.Log("에피소드 시작");
            GameManager.instance.playerData.kill = 0;
            increaseWeight = 2f;
            expCount = 0;
            preExp = 0;
            minDistanceToExp = float.MaxValue;
            SceneManager.LoadScene("Stage1");
        }
        count++;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        nextMove.x = actions.ContinuousActions[0];
        nextMove.y = actions.ContinuousActions[1];

        transform.Translate(nextMove * Time.deltaTime * speed);
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
                    SetReward(-1f / increaseWeight);
                    break;
            }
            coolTimer = 0f;
        }
    }

    private void UpdateMinDistanceToExp()
    {
        float currentMinDistance = float.MaxValue;

        foreach (var exp in GameManager.instance.poolManager.Exp_Active_pools)
        {
            float distance = Vector3.Distance(transform.position, exp.transform.position);
            if (distance < currentMinDistance)
            {
                currentMinDistance = distance;
            }
        }

        minDistanceToExp = currentMinDistance;
    }

    void EndEpisode()
    {
        isEndEpisode = true;

        GameManager.instance.player.expCount = CalculateExp(); // Placeholder
        GameManager.instance.playerData.kill = CalculateKills(); // Placeholder

        // Log the episode data
        Debug.Log("에피소드 종료됐는 지 확인용 : " + GameManager.instance.player.isEndEpisode);
        Debug.Log("Kill 수: " + GameManager.instance.playerData.kill);
        Debug.Log("Exp 획득량: " + GameManager.instance.player.expCount);

        csvTest.WriteData();
        base.EndEpisode();

        isEndEpisode = false;
    }

    int CalculateExp()
    {
        // Placeholder for actual EXP calculation logic
        return GameManager.instance.player.expCount;
    }

    int CalculateKills()
    {
        // Placeholder for actual Kill calculation logic
        return GameManager.instance.playerData.kill;
    }
}
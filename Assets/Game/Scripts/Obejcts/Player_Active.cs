using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Active : Player
{
    int preKill;
    bool testBool = false;

    // GameObject[] modifiedEnemy;
    protected override void Start()
    {
        base.Start();
        csvTest = FindAnyObjectByType<CsvTest>(); // 로그 기록용
        GameManager.instance.playerData.kill = 0;
        preKill = 0;

        isEndEpisode = false;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        distanceToExp = new List<float>();
        coolTimer += Time.fixedDeltaTime;

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

        if (GameManager.instance.playerData.kill >= 1000)
        {
            if (preKill < GameManager.instance.playerData.kill)
            {
                SetReward(+0.05f);
                Debug.Log("몹 죽여서 보상6 획득");
                preKill = GameManager.instance.playerData.kill;
            }
        }
        else if (GameManager.instance.playerData.kill < 1000 && GameManager.instance.playerData.kill >= 700)
        {
            if (preKill < GameManager.instance.playerData.kill)
            {
                SetReward(+0.04f);
                Debug.Log("몹 죽여서 보상5 획득");
                preKill = GameManager.instance.playerData.kill;
            }
        }
        else if (GameManager.instance.playerData.kill < 700 && GameManager.instance.playerData.kill >= 500)
        {
            if (preKill < GameManager.instance.playerData.kill)
            {
                SetReward(+0.03f);
                Debug.Log("몹 죽여서 보상4 획득");
                preKill = GameManager.instance.playerData.kill;
            }
        }
        else if (GameManager.instance.playerData.kill < 500 && GameManager.instance.playerData.kill >= 250)
        {
            if (preKill < GameManager.instance.playerData.kill)
            {
                SetReward(+0.03f);
                Debug.Log("몹 죽여서 보상3 획득");
                preKill = GameManager.instance.playerData.kill;
            }
        }
        else if (GameManager.instance.playerData.kill < 300 && GameManager.instance.playerData.kill >= 100)
        {
            if (preKill < GameManager.instance.playerData.kill)
            {
                SetReward(+0.02f);
                Debug.Log("몹 죽여서 보상2 획득");
                preKill = GameManager.instance.playerData.kill;
            }
        }
        else if (GameManager.instance.playerData.kill < 100 && GameManager.instance.playerData.kill > 0)
        {
            if (preKill < GameManager.instance.playerData.kill)
            {
                SetReward(+0.01f);
                Debug.Log("몹 죽여서 보상1 획득");
                preKill = GameManager.instance.playerData.kill;
            }
        }

        if (GameManager.instance.playerData.hp < 50f) // HP 50%
        {
            SetReward(-3);
            isEndEpisode = true;
            Debug.Log("Hp 깎여서 마이너스");
            EndEpisode();
        }

        if (delayTimer >= delayTime)
        {
            increaseWeight += 0.5f;
            delayTimer = 0f;
        }

        // if (GameManager.instance.gameTime >= 10f && !testBool)
        // {
        //     for(int i = 0; i < GameManager.instance.enemies.Count; i++)
        //     {
        //         StartCoroutine(GameManager.instance.enemies[i].makeEnemyHardPattern());
        //     }
        //     testBool = true;
        // }
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
            preKill = 0;
            increaseWeight = 2f;
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
                        SetReward(-0.5f / increaseWeight);
                    break;
            }
            coolTimer = 0f;
        }
    }

    void EndEpisode()
    {
        isEndEpisode = true;

        GameManager.instance.player.expCount = CalculateExp(); // Placeholder
        GameManager.instance.playerData.kill = CalculateKills(); // Placeholder

        // Log the episode data
        Debug.Log("에피소드 종료됐는 지 확인용 : " + GameManager.instance.player.isEndEpisode);
        Debug.Log("Kill 수: " + GameManager.instance.playerData.kill);
        Debug.Log("Exp 획득량: " +  GameManager.instance.player.expCount);

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
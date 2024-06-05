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
 
    protected override void Start()
    {
        base.Start();
        
        preKill = 0;
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
            GameManager.instance.skillManager.skillData.Damage[0] = 40f;
        }
        else if (GameManager.instance.gameTime >= 40f)
        {
            GameManager.instance.skillManager.skillData.skillSelected[2] = true;
        }
        else if (GameManager.instance.gameTime > 20f)
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
            endCheckPoint--;
            EndEpisode();
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

        if (GameManager.instance.playerData.hp < 85f) // HP 85%
        {
            SetReward(-3);
            Debug.Log("Hp 깎여서 마이너스");
            endCheckPoint--;
            EndEpisode();
        }

        if (delayTimer >= delayTime)
        {
            increaseWeight += 0.5f;
            delayTimer = 0f;
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
            preKill = 0;
            endCheckPoint = 1;
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
}
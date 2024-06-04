using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Active : Player
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        distanceToExp = new List<float>();
        coolTimer += Time.fixedDeltaTime;

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

        if (GameManager.instance.playerData.kill >= 700)
        {
            if (GameManager.instance.isEnemyKilled)
            {
                SetReward(+0.04f);
                GameManager.instance.isEnemyKilled = false;
            }
        }
        else if (GameManager.instance.playerData.kill < 700 && GameManager.instance.playerData.kill >= 250)
        {
            if (GameManager.instance.isEnemyKilled)
            {
                SetReward(+0.03f);
                GameManager.instance.isEnemyKilled = false;
            }
        }
        else if (GameManager.instance.playerData.kill < 250 && GameManager.instance.playerData.kill >= 100)
        {
            if (GameManager.instance.isEnemyKilled)
            {
                SetReward(+0.02f);
                GameManager.instance.isEnemyKilled = false;
            }
        }
        else if (GameManager.instance.playerData.kill < 100 && GameManager.instance.playerData.kill > 0)
        {
            if (GameManager.instance.isEnemyKilled)
            {
                SetReward(+0.01f);
                GameManager.instance.isEnemyKilled = false;
            }
        }

        if (GameManager.instance.playerData.hp < 90f)
        {
            SetReward(-3);
            isEpisodeEnd = true;
            EndEpisode();
        }

        if (delayTimer >= delayTime)
        {
            increaseWeight += 0.5f;
            delayTimer = 0f;
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
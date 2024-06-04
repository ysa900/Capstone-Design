using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Passive : Player
{
    float minDistanceToEnemy; // 제일 가까운 적과 플레이어 사이의 거리

    protected override void Start()
    {
        base.Start();

        minDistanceToEnemy = 0f;

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        distanceToEnemy = new List<float>();

        for (int i = 0; i < GameManager.instance.enemies.Count; i++)
        {
            // Debug.Log("Transform Position: " + transform.position);
            // Debug.Log("Enemies Positions: " + GameManager.instance.enemies[i].transform.position);
            distanceToEnemy.Add(Vector3.Distance(transform.position, GameManager.instance.enemies[i].transform.position));
        }

        for (int i = 0; i < distanceToEnemy.Count; i++)
        {
            minDistanceToEnemy = distanceToEnemy[0];
            if (minDistanceToEnemy > distanceToEnemy[i])
            {
                minDistanceToEnemy = distanceToEnemy[i];
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
            increaseWeight = 0.5f;
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

        if (GameManager.instance.playerData.hp < 90f) // Hp 90%
        {
            SetReward(-3);
            isEpisodeEnd = true;
            EndEpisode();
        }

        if (delayTimer >= delayTime)
        {
            SetReward(increaseWeight); // 살아남을수록 리워드 부여
            increaseWeight += 0.5f; // 리워드 가중치 증가
            delayTimer = 0f;
        }

        if (minDistanceToEnemy > 5f)
        {
            SetReward(+0.0001f);
            Debug.Log("멀어져서 Reward +0.001f");
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
                        SetReward(-1f / increaseWeight);
                    break;
            }
            coolTimer = 0f;
        }
    }
}
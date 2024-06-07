using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Passive : Player
{
    float minDistanceToEnemy;
    List<float> distanceToEnemy;

    protected override void Start()
    {
        base.Start();
        csvTest = FindAnyObjectByType<CsvTest>(); // 로그 기록용
        GameManager.instance.playerData.kill = 0;
        minDistanceToEnemy = float.MaxValue;
        distanceToEnemy = new List<float>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        UpdateMinDistanceToEnemy();

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

        if (GameManager.instance.gameTime >= 300f)
        {
            SetReward(+4);
            EndEpisode();
        }
        else if (GameManager.instance.gameTime >= 240f)
        {
            SetReward(+3);
        }
        else if (GameManager.instance.gameTime >= 180f)
        {
            SetReward(+2);
        }

        if (GameManager.instance.playerData.hp < 93f) // Hp 90%
        {
            SetReward(-3);
            Debug.Log("Hp 깎여서 마이너스");
            EndEpisode();
        }

        if (delayTimer >= delayTime)
        {
            SetReward(increaseWeight); // 살아남을수록 리워드 부여
            Debug.Log("오래 살아남아서 20초마다 보상 획득");
            increaseWeight += 0.5f; // 리워드 가중치 증가
            delayTimer = 0f;
        }

        // 매 프레임마다 적과의 거리를 계산하고 보상을 부여합니다.
        if (minDistanceToEnemy > 7f)
        {
            SetReward(+0.01f); // 보상 크기 조정
            Debug.Log("멀어져서 Reward +0.01 보상 획득");
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
            increaseWeight = 0.5f;
            minDistanceToEnemy = float.MaxValue;
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

    private void UpdateMinDistanceToEnemy()
    {
        float currentMinDistance = float.MaxValue;

        if (GameManager.instance.enemies == null || GameManager.instance.enemies.Count == 0)
        {
            return;
        }

        foreach (var enemy in GameManager.instance.enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < currentMinDistance)
            {
                currentMinDistance = distance;
            }
        }

        minDistanceToEnemy = currentMinDistance;
    }

    void EndEpisode()
    {
        isEndEpisode = true;

        GameManager.instance.player.expCount = CalculateExp();
        GameManager.instance.playerData.kill = CalculateKills();

        Debug.Log("에피소드 종료됐는 지 확인용 : " + GameManager.instance.player.isEndEpisode);
        Debug.Log("Kill 수: " + GameManager.instance.playerData.kill);
        Debug.Log("Exp 획득량: " + GameManager.instance.player.expCount);

        csvTest.WriteData();
        base.EndEpisode();

        isEndEpisode = false;
    }

    int CalculateExp()
    {
        return GameManager.instance.player.expCount;
    }

    int CalculateKills()
    {
        return GameManager.instance.playerData.kill;
    }
}

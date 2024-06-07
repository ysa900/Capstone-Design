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

    // 딴딴이 기믹용
    private bool gimmickExecuted = false;
    private bool hardModeActivated = false;

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
            isEndEpisode = true;
            Debug.Log("Hp 깎여서 마이너스");
            EndEpisode();
        }

        if (delayTimer >= delayTime)
        {
            increaseWeight += 0.5f;
            delayTimer = 0f;
        }

        // 게임 시간이 10초에 도달했는지 확인
        if (GameManager.instance.gameTime >= 10f && !hardModeActivated)
        {
            //makeEnemyHard();
            hardModeActivated = true;
        }

        // 지정된 시간에 기믹이 실행되었는지 확인
        if (gimmickExecuted)
        {
            patternTimer += Time.deltaTime;
            if (patternTimer >= patternTime)
            {
                //makeEnemyNormal();
                patternTimer = 0f;
                gimmickExecuted = false; // 원상복구 후 다시 실행되지 않도록 설정
            }
        }

    }
    /*
    private void makeEnemyHard()
    {
        string[] tags = { "EvilTree", "Pumpkin", "Warlock" };

        foreach (string tag in tags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject enemyObj in enemies)
            {
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    int index = hpIndices[tag];

                    // 자식 오브젝트 활성화
                    Transform child = enemy.transform.GetChild(0);
                    if (child != null)
                    {
                        child.gameObject.SetActive(true);
                    }

                    // HP 증가
                    originalHP[enemy] = new float[enemy.enemy_HP.Length];
                    enemy.enemy_HP.CopyTo(originalHP[enemy], 0); // 기존 HP 저장
                    enemy.enemy_HP[index] *= 1.5f; // HP 1.5배 증가
                    Debug.Log($"{enemy.name} HP 증가: {enemy.enemy_HP[index]}"); // 디버그 로그 추가
                    modifiedEnemies.Add(enemy); // 수정된 적 목록에 추가
                }
            }
        }
        // 기믹이 실행되었음을 표시하고 타이머 초기화
        gimmickExecuted = true;
        patternTimer = 0f;
    }

    private void makeEnemyNormal()
    {
        foreach (Enemy enemy in modifiedEnemies)
        {
            if (enemy != null)
            {
                int index = hpIndices[enemy.tag];

                // 자식 오브젝트 비활성화
                Transform child = enemy.transform.GetChild(0);
                if (child != null)
                {
                    child.gameObject.SetActive(false);
                }

                // HP 원상복구
                if (originalHP.ContainsKey(enemy))
                {
                    originalHP[enemy].CopyTo(enemy.enemy_HP, 0); // 기존 HP로 복원
                    Debug.Log($"{enemy.name} HP 원상복구: {enemy.enemy_HP[index]}"); // 디버그 로그 추가
                }
            }
        }
        // 수정된 적 목록과 원래 HP 목록 초기화
        modifiedEnemies.Clear();
        originalHP.Clear();
    }
    */

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
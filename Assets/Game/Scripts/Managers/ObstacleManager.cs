using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{

    float weight;


    Vector3 prePlayerPosition;
    Vector3 curPlayerPosition;
    Vector3 playerVector;
    Vector3 cumulativeVector;


    float angleDegree;

    int Direction;
    int playerTendency;
    int countIndex;


    float CoolTime;
    float CoolTimer;
    float ObstacleCoolTime1;
    float ObstacleCoolTimer1;
    float ObstacleCoolTime2;
    float ObstacleCoolTimer2;

    public Obstacle statuePrefab;
    public Obstacle obstacle;

    bool isPatternTimerOn = false;

    // Start is called before the first frame update
    void Start()
    {
        weight = 1f;
        prePlayerPosition = Vector3.zero;
        curPlayerPosition  = Vector3.zero;
        playerVector = Vector3.zero;
        cumulativeVector = Vector3.zero;
        angleDegree = 0f;

        Direction = 0;
        playerTendency = 0;
        countIndex = 0;

        CoolTime = 2f;
        CoolTimer = 0f;
        ObstacleCoolTime1 = 10f;
        ObstacleCoolTimer1 = 0f;
        ObstacleCoolTime2 = 120f;
        ObstacleCoolTimer2 = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        curPlayerPosition = GameManager.instance.player.transform.position;

        CoolTimer += Time.deltaTime;
        ObstacleCoolTimer1 += Time.deltaTime;
        ObstacleCoolTimer2 += Time.deltaTime;

        playerVector = curPlayerPosition  - prePlayerPosition;

        CalculateVector(playerVector);
        weight += 0.1f;
      

        if (CoolTimer >= CoolTime)
        {
            prePlayerPosition = curPlayerPosition;
            CoolTimer = 0f;
        }

        if(ObstacleCoolTimer1 >= ObstacleCoolTime1)
        {
 
           for (int i = 0; i < 20; i++)
           {
             CreateObstacle(Direction, 0);

           }
              ObstacleCoolTimer1 = 0f;
        }

        if (ObstacleCoolTimer2 >= ObstacleCoolTime2)
        {
            CreateObstacle(Direction, 2);

            ObstacleCoolTimer2 = 0f;
        }

    }


    public void CalculateVector(Vector3 playerVector)
    {
        
        cumulativeVector += playerVector * weight;
        Direction = DecisionDirection(cumulativeVector);
   
        
    }

    public int DecisionDirection(Vector3 cumulativeVector)
    {

        angleDegree = Quaternion.FromToRotation(Vector3.right, cumulativeVector).eulerAngles.z;

        //X축 오른쪽
        if (angleDegree >= 337.5f || angleDegree < 22.5f)
            Direction = 0;
        //제1사분면 대각선
        else if(angleDegree >= 22.5f && angleDegree < 67.5f)
            Direction = 1;
        //Y축 위쪽
        else if(angleDegree >= 67.5f && angleDegree < 112.5)
            Direction = 2;
        //제2사분면 대각선
        else if (angleDegree >= 112.5 && angleDegree < 157.5)
            Direction = 3;
        //X축 왼쪽
        else if (angleDegree >= 157.5f && angleDegree <202.5)
            Direction = 4;
        //제3사분면 대각선
        else if (angleDegree >= 202.5f && angleDegree < 247.5f)
            Direction = 5;
        //Y축 아래쪽
        else if (angleDegree >= 247.5f && angleDegree < 292.5f)
            Direction = 6;
        //제4사분면 대각선
        else if (angleDegree >= 292.5f && angleDegree < 337.5f)
            Direction = 7;


        return Direction;

    }


    public void CreateObstacle(int Direction, int playerTend)
    {

        
        switch (playerTend)
        {
            case 0:
                VoidPlayerObstacile(Direction);
                break;
            case 1:
                ExpPlayerObstacle(Direction);
                break;
            case 2:
                KillPlayerObstacle();
                break;
            case 3:
                Exp_killPlayerObstacle(Direction);
                break;


        }


    }

    public int PlayerTendFun()
    {
        //K-means 알고리즘을 통한 군집화 분류 나중에 구현
        /* 
       (Exp, Enemy)  
        0,0 -> 소심(0)
        1,0 -> EXP 선호(1)
        0,1 -> 적극(2)
        1,1 -> 매우 적극(3)
        */

        if(countIndex == 4)
        {
            countIndex = 0;
        }

        int playerTend = countIndex;
        countIndex++;

        //파이썬 ML 소켓 통신


        return playerTend;
    }


    public void VoidPlayerObstacile(int Direction)
    {
        //이동경로 차단 패턴

        float radius = UnityEngine.Random.Range(15f,30f);
        float ranDegree = UnityEngine.Random.Range(angleDegree - 22.5f, angleDegree + 22.5f);
        float X_Range = 0f;
        float Y_Range = 0f;
        float height = 30f;
        Debug.Log("설치");
        X_Range = curPlayerPosition.x + (float)Mathf.Cos(ranDegree * Mathf.Deg2Rad) * radius;
        Y_Range = curPlayerPosition.y + (float)Mathf.Sin(ranDegree * Mathf.Deg2Rad) * radius;
        obstacle = Instantiate(statuePrefab);
        obstacle.transform.position = new Vector3(X_Range, Y_Range + height, 0f);
        obstacle.height = height;

    }




    public void ExpPlayerObstacle(int Direction)
    {
        //이동경로 차단 패턴
        //EXP 못 먹게 유도 패턴 구현

       
    }

    public void KillPlayerObstacle()
    {
        //몬스터 hp 강화 및 이펙트생성
        for (int i = 0; i < GameManager.instance.enemies.Count; i++)
        {
            StartCoroutine(GameManager.instance.enemies[i].makeEnemyHardPattern());
        }
        isPatternTimerOn = true;

    }

    public void Exp_killPlayerObstacle(int Direction)
    {
        // 이동경로 차단, 몬스터 강화

    }



}

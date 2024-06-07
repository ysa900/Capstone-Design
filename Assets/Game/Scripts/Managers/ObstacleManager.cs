using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{

    public float weight;

    public Vector3 prePlayerPosition;
    public Vector3 curPlayerPosition;
    public Vector3 playerVector;
    public Vector3 cumulativeVector;

    public float angleRedian;
    public float angleDegree;

    int Direction;
    int playerTendency;
    float max;

    float CoolTime;
    float CoolTimer;
    float ObstacleCoolTime;
    float ObstacleCoolTimer;

    public GameObject statue;

    // Start is called before the first frame update
    void Start()
    {
        weight = 1f;
        prePlayerPosition = Vector3.zero;
        curPlayerPosition  = Vector3.zero;
        playerVector = Vector3.zero;
        cumulativeVector = Vector3.zero;
        angleRedian = 0f;
        angleDegree = 0f;
        max = 0f;

        Direction = 0;
        playerTendency = 0;

        CoolTime = 2f;
        CoolTimer = 0f;
        ObstacleCoolTime = 1f;
        ObstacleCoolTimer = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        curPlayerPosition = GameManager.instance.player.transform.position;

        CoolTimer += Time.deltaTime;
        ObstacleCoolTimer += Time.deltaTime;
      

        playerVector = curPlayerPosition  - prePlayerPosition;



        
        CalculateVector(playerVector);
        weight += 0.1f;
      

        if (CoolTimer >= CoolTime)
        {
            prePlayerPosition = curPlayerPosition;
            CoolTimer = 0f;
        }
        if(ObstacleCoolTimer >= ObstacleCoolTime)
        {
            CreateObstacle(Direction, playerTendency);
            ObstacleCoolTimer = 0f;
        }


    }


    public void CalculateVector(Vector3 playerVector)
    {
        cumulativeVector += playerVector * weight;
        Direction = DecisionDirection(cumulativeVector);
        Debug.Log(Direction);
        playerTendency = PlayerTendFun();
    }

    public int DecisionDirection(Vector3 cumulativeVector)
    {
        angleRedian = Mathf.Atan2(cumulativeVector.y,cumulativeVector.x);
        angleDegree = angleRedian * Mathf.Rad2Deg;

        //제1사분면 아래쪽
        if (angleDegree >= 0 && angleDegree < 45f)
            Direction = 0;
        //제1사분면 위쪽
        else if(angleDegree >= 45f && angleDegree < 90f)
            Direction = 1;
        //제2사분면 위쪽
        else if(angleDegree >= 90f && angleDegree < 135f)
            Direction = 2;
        //제2사분면 아래쪽
        else if (angleDegree >= 90f && angleDegree < 180f)
            Direction = 3;
        //제3사분면 위쪽
        else if (angleDegree >= -180f && angleDegree < -135f)
            Direction = 4;
        //제3사분면 아래쪽
        else if (angleDegree >= -135f && angleDegree < -90f)
            Direction = 5;
        //제4사분면 아래쪽
        else if (angleDegree >= -90f && angleDegree < -45f)
            Direction = 6;
        //제4사분면 위쪽
        else if (angleDegree >= -45f && angleDegree < 0)
            Direction = 7;

      

        return Direction;

    }


    public void CreateObstacle(int Direction, int playerTend)
    {

        
        switch (playerTend)
        {
            case 0:
                VoidPlayerObstacile(Direction, statue);
                break;
            case 1:
                ExpPlayerObstacle(Direction, statue);
                break;
            case 2:
                KillPlayerObstacle(Direction);
                break;
            case 3:
                Exp_killPlayerObstacle(Direction, statue);
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
        int playerTend = 0;

        //파이썬 ML 소켓 통신


        return playerTend;
    }


    public void VoidPlayerObstacile(int Direction, GameObject ObstacleType)
    {
        //이동경로 차단 패턴

        float X_Range = 0f;
        float Y_Range = 0f;

        switch (Direction)
        {
            case 0: //제1사분면 아래쪽
                X_Range = curPlayerPosition.x + UnityEngine.Random.Range(10f, 20f);
                Y_Range = curPlayerPosition.y + UnityEngine.Random.Range(10f, X_Range);
                Instantiate(ObstacleType);
                ObstacleType.transform.position = new Vector2(X_Range, Y_Range);
                Debug.Log("제1사분면 아래쪽");
                break;

            case 1://제1사분면 위쪽
                Y_Range = curPlayerPosition.x + UnityEngine.Random.Range(10f, 20f);
                X_Range = curPlayerPosition.y + UnityEngine.Random.Range(10f, X_Range);
                Instantiate(ObstacleType);
                ObstacleType.transform.position = new Vector2(X_Range, Y_Range);
                Debug.Log("제1사분면 위쪽");
                break;
            case 2: //제2사분면 위쪽
                Y_Range = curPlayerPosition.y + UnityEngine.Random.Range(10f, 20f);
                X_Range = curPlayerPosition.x - UnityEngine.Random.Range(10f, Y_Range);
                Instantiate(ObstacleType);
                ObstacleType.transform.position = new Vector2(X_Range, Y_Range);
                Debug.Log("제2사분면 위쪽");
                break;
            case 3: //제2사분면 아래쪽
                X_Range = curPlayerPosition.x - UnityEngine.Random.Range(10f, 20f); ;
                Y_Range = curPlayerPosition.y + UnityEngine.Random.Range(10f, X_Range);
                Instantiate(ObstacleType);
                ObstacleType.transform.position = new Vector2(X_Range, Y_Range);
                Debug.Log("제2사분면 아래쪽");
                break;
            case 4: //제3사분면 위쪽
                X_Range = curPlayerPosition.x - UnityEngine.Random.Range(10f, 20f);
                Y_Range = curPlayerPosition.y - UnityEngine.Random.Range(10f, X_Range);
                Instantiate(ObstacleType);
                ObstacleType.transform.position = new Vector2(X_Range, Y_Range);
                Debug.Log("제3사분면 위쪽");
                break;
            case 5: //제3사분면 아래쪽
                Y_Range = curPlayerPosition.y - UnityEngine.Random.Range(10f, 20f); ;
                X_Range = curPlayerPosition.x - UnityEngine.Random.Range(10f, Y_Range);
                Instantiate(ObstacleType);
                ObstacleType.transform.position = new Vector2(X_Range, Y_Range);
                Debug.Log("제3사분면 아래쪽");
                break;
            case 6: //제4사분면 아래쪽
                Y_Range = curPlayerPosition.y - UnityEngine.Random.Range(10f, 20f);
                X_Range = curPlayerPosition.x + UnityEngine.Random.Range(10f, Y_Range);
                Instantiate(ObstacleType);
                ObstacleType.transform.position = new Vector2(X_Range, Y_Range);
                Debug.Log("제4사분면 아래쪽");
                break;
            case 7: //제4사분면 위쪽
                X_Range = curPlayerPosition.x + UnityEngine.Random.Range(10f, 20f);
                Y_Range = curPlayerPosition.y - UnityEngine.Random.Range(10f, X_Range);
                Instantiate(ObstacleType);
                ObstacleType.transform.position = new Vector2(X_Range, Y_Range);
                Debug.Log("제4사분면 위쪽");
                break;

        }



    }




    public void ExpPlayerObstacle(int Direction, GameObject ObstacleType)
    {
      // 이동경로 차단
    }

    public void KillPlayerObstacle(int Direction)
    {
       // 몬스터 강화

    }

    public void Exp_killPlayerObstacle(int Direction, GameObject ObstacleType)
    {
        // 이동경로 차단, 몬스터 강화

    }
}

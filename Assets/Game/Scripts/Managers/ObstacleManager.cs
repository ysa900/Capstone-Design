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
    [SerializeField]float ObstacleCoolTimer1;
    float ObstacleCoolTime2;
    [SerializeField] float ObstacleCoolTimer2;

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
        ObstacleCoolTime1 = 120f;
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

        if (!GameManager.instance.isStageClear)
        {
            if(ObstacleCoolTimer1 >= ObstacleCoolTime1)
            {
            
               for (int i = 0; i < 20; i++)
               {
                 CreateObstacle(Direction, 0);

               }
               ObstacleCoolTimer1 = 0f;
               ObstacleCoolTime1 = 10f;
            }

            if (ObstacleCoolTimer2 >= ObstacleCoolTime2)
            {
                CreateObstacle(Direction, 2);

                ObstacleCoolTimer2 = 0f;
            }
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

        //X�� ������
        if (angleDegree >= 337.5f || angleDegree < 22.5f)
            Direction = 0;
        //��1��и� �밢��
        else if(angleDegree >= 22.5f && angleDegree < 67.5f)
            Direction = 1;
        //Y�� ����
        else if(angleDegree >= 67.5f && angleDegree < 112.5)
            Direction = 2;
        //��2��и� �밢��
        else if (angleDegree >= 112.5 && angleDegree < 157.5)
            Direction = 3;
        //X�� ����
        else if (angleDegree >= 157.5f && angleDegree <202.5)
            Direction = 4;
        //��3��и� �밢��
        else if (angleDegree >= 202.5f && angleDegree < 247.5f)
            Direction = 5;
        //Y�� �Ʒ���
        else if (angleDegree >= 247.5f && angleDegree < 292.5f)
            Direction = 6;
        //��4��и� �밢��
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
        //K-means �˰������� ���� ����ȭ �з� ���߿� ����
        /* 
       (Exp, Enemy)  
        0,0 -> �ҽ�(0)
        1,0 -> EXP ��ȣ(1)
        0,1 -> ����(2)
        1,1 -> �ſ� ����(3)
        */

        if(countIndex == 4)
        {
            countIndex = 0;
        }

        int playerTend = countIndex;
        countIndex++;

        //���̽� ML ���� ���


        return playerTend;
    }


    public void VoidPlayerObstacile(int Direction)
    {
        //�̵���� ���� ����

        float radius = UnityEngine.Random.Range(5f,20f);
        float ranDegree = UnityEngine.Random.Range(angleDegree - 45f, angleDegree + 45f);
        float X_Range = 0f;
        float Y_Range = 0f;
        float height = 30f;

        X_Range = curPlayerPosition.x + (float)Mathf.Cos(ranDegree * Mathf.Deg2Rad) * radius;
        Y_Range = curPlayerPosition.y + (float)Mathf.Sin(ranDegree * Mathf.Deg2Rad) * radius;
        obstacle = Instantiate(statuePrefab);
        obstacle.transform.position = new Vector3(X_Range, Y_Range + height, 0f);
        obstacle.height = height;

    }




    public void ExpPlayerObstacle(int Direction)
    {
        //�̵���� ���� ����
        //EXP �� �԰� ���� ���� ����

       
    }

    public void KillPlayerObstacle()
    {
        //���� hp ��ȭ �� ����Ʈ����
        for (int i = 0; i < GameManager.instance.enemies.Count; i++)
        {
            StartCoroutine(GameManager.instance.enemies[i].makeEnemyHardPattern());
        }
        isPatternTimerOn = true;

    }

    public void Exp_killPlayerObstacle(int Direction)
    {
        // �̵���� ����, ���� ��ȭ

    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_ML : MonoBehaviour
{
    float speed = 6f;

    int XMove;
    int YMove;

    bool isDecided;

    bool isXPlus;
    bool isXStop;
    bool isYPlus;
    bool isYStop;

    float GoingTime;
    float GoingTimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (!isDecided)
        {
            XMove = UnityEngine.Random.Range(0, 3);
            YMove = UnityEngine.Random.Range(0, 3);

            GoingTime = UnityEngine.Random.Range(0f, 2f);

            isDecided = true;
        }
        else
        {
            float tmpx = transform.position.x;
            float tmpy = transform.position.y;

            switch (XMove)
            {
                case 0:
                    isXPlus = true;
                    isXStop = false;
                    break;
                case 1:
                    isXPlus = false;
                    isXStop = false;
                    break;
                case 2:
                    isXPlus = false;
                    isXStop = true;
                    break;
            }

            switch (YMove)
            {
                case 0:
                    isYPlus = true;
                    isYStop = false;
                    break;
                case 1:
                    isYPlus = false;
                    isYStop = false;
                    break;
                case 2:
                    isYPlus = false;
                    isYStop = true;
                    break;
            }

            if (!isXStop)
            {
                if (isXPlus)
                {
                    tmpx += Time.fixedDeltaTime * speed;
                }
                else
                {
                    tmpx -= Time.fixedDeltaTime * speed;
                }
            }

            if (!isYStop)
            {
                if (isYPlus)
                {
                    tmpy += Time.fixedDeltaTime * speed;
                }
                else
                {
                    tmpy -= Time.fixedDeltaTime * speed;
                }
            }

            transform.position = new Vector2(tmpx, tmpy);

            if(GoingTimer > GoingTime)
            {
                isDecided = false;
                GoingTimer = 0;
            }

            GoingTimer += Time.fixedDeltaTime;
        }
    }
}

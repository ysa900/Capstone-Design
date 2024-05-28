﻿using System;
using UnityEngine;
using System.Collections;


public class FollowCam : MonoBehaviour
{
    public Player player;

    string sceneName;
    float cameraHalfWidth, cameraHalfHeight;
    bool isClearWallDetected;
    public float clearWall_RightEndX = 0;

    private void Start()
    {
        StartCoroutine(WaitForNSec(0.5f));
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        cameraHalfHeight = Camera.main.orthographicSize;
    }

    private void Update()
    {
        // 카메라가 투명벽의 오른쪽 끝보다 오른쪽에 있는 상태에서,
        // 카메라가 투명벽의 오른쪽 끝에 닿으면 카메라 x축 안따라감

        if (sceneName == "Stage2")
        {
            isClearWallDetected = (transform.position.x - cameraHalfWidth >= clearWall_RightEndX) &&
                (clearWall_RightEndX > player.transform.position.x - cameraHalfWidth);
        }

        if (isClearWallDetected)
        {   
            Vector2 newCamPosition = new Vector2(transform.position.x, player.transform.position.y);
            transform.position = new Vector3(newCamPosition.x, newCamPosition.y, transform.position.z);
        }
        else
        {
            Vector2 newCamPosition = new Vector2(player.transform.position.x, player.transform.position.y);
            transform.position = new Vector3(newCamPosition.x, newCamPosition.y, transform.position.z);
        }
    }

    IEnumerator WaitForNSec(float time)
    {
        yield return new WaitForSeconds(time);

        sceneName = GameManager.instance.sceneName;
        
    }
}


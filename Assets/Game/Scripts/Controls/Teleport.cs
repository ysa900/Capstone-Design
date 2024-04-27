﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport:MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isPlayer = collision.gameObject.tag == "Player";
        if(isPlayer) SceneManager.LoadScene("Stage3"); // Stage3 씬 불러오기
    }
}


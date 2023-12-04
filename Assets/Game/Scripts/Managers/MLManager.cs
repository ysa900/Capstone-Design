using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class MLManager : MonoBehaviour
{
    private Player_ML[] players;
    private Boss_Bullet_ML[] boss_Bullets;

    Boss_Bullet_ML boss_Bullet;

    public Player_ML playerPrefab;
    public Boss_Bullet_ML boss_BulletPrefab;
    public GameObject[] gameObjects;


    // Start is called before the first frame update
    void Start()
    {
        players = new Player_ML[9];
        boss_Bullets = new Boss_Bullet_ML[9];


        for (int i = 0; i < gameObjects.Length; i++)
        {
            Vector2 pos = gameObjects[i].transform.position;

            players[i] = Instantiate(playerPrefab);
            pos.x += 2f; pos.y += 1f;
            players[i].transform.position = pos;


            Vector2 pos2 = gameObjects[i].transform.position;
            
            
            boss_Bullet = Instantiate(boss_BulletPrefab);
            boss_Bullets[i] = boss_Bullet;
            pos2.x -= 5f; pos2.y -= 5f;
            boss_Bullets[i].transform.position = pos2;

            boss_Bullets[i].player = players[i];
            boss_Bullet.gameObject = gameObjects[i];
        }


    

    }


    }



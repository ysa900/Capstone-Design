using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CsvTest : MonoBehaviour {

	List<string> columns;
	float CoolTime;
	float CoolTimer;
    int index;
    CsvFileWriter writer;
    float PrePlayerX;
    float PrePlayerY;

    void Start ()
	{
        CoolTime = 1f;
        CoolTimer = 0f;
        index = 1;
        PrePlayerX = GameManager.instance.player.transform.position.x;
        PrePlayerY = GameManager.instance.player.transform.position.y;
        writer = new CsvFileWriter("Assets/Resources/playerInfo.csv");

         columns = new List<string>() { "Index","Vector X", "Vector Y", "Exp Count", "Kill" };// making Index Row
         writer.WriteRow(columns);
         columns.Clear();
      
    }

	void FixedUpdate () {

		    CoolTimer += GameManager.instance.gameTime;
         
             if (CoolTimer >= CoolTime)
			{
                PrePlayerX = GameManager.instance.player.transform.position.x - PrePlayerX;
                PrePlayerY = GameManager.instance.player.transform.position.y - PrePlayerY;


                columns.Add(index.ToString()); 
                columns.Add(PrePlayerX.ToString());
                columns.Add(PrePlayerY.ToString());
                if (GameManager.instance.player.isEpisodeEnd)
                {
                    columns.Add(GameManager.instance.player.expCount.ToString()); // 먹은 Exp
                    columns.Add(GameManager.instance.playerData.kill.ToString()); // 처치 몹 kill 수
                    GameManager.instance.player.isEpisodeEnd = false;
                }
                else
                {
                    columns.Add(" "); // 먹은 Exp
                    columns.Add(" "); // 처치 몹 kill 수
                }

                writer.WriteRow(columns);
                columns.Clear();
                Count();
                CoolTimer = 0f;
                PrePlayerX = GameManager.instance.player.transform.position.x;
                PrePlayerY = GameManager.instance.player.transform.position.y;
        }

	}

	public void Count()
	{
		index++;
	}
}

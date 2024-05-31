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

         columns = new List<string>() { "Index","Vector X", "Vector Y" };// making Index Row
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

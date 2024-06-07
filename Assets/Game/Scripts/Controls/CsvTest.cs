using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CsvTest : MonoBehaviour
{
    List<string> columns;
    int index;
    string filePath;

    void Start()
    {
        index = 1;
        filePath = Application.dataPath + "/Resources/playerInfo.csv"; // 절대 경로 설정
        Debug.Log("File Path: " + filePath); // 파일 경로 디버그 출력
        using (var writer = new CsvFileWriter(filePath))
        {
            columns = new List<string>() { "Episode Number", "PlayTime", "Exp Count", "Kill" }; // making Index Row
            writer.WriteRow(columns);
            Debug.Log("Header written to CSV file.");
        }
        columns.Clear();
    }

    public void WriteData()
    {
        using (var writer = new CsvFileWriter(filePath))
        {
            columns.Add(index.ToString());
            columns.Add(GameManager.instance.gameTime.ToString()); // 전처리용 시간체크
            columns.Add(GameManager.instance.player.expCount.ToString()); // Exp 획득량
            columns.Add(GameManager.instance.playerData.kill.ToString()); // 처치 몹 kill 수
            writer.WriteRow(columns);
            columns.Clear();
            Count();
        }
    }

    void OnApplicationQuit()
    {
        // 애플리케이션 종료 시 호출됨
        if (GameManager.instance.player.isEndEpisode)
        {
            WriteData();
            Count();
        }
    }

    public void Count()
    {
        index++;
    }
}
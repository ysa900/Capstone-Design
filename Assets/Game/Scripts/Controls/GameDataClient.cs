using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

public class GameDataClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private BinaryFormatter formatter;
    private float timer = 0f;

    void Start()
    {
        try
        {
            client = new TcpClient("localhost", 12345);
            stream = client.GetStream();
            formatter = new BinaryFormatter();
            Debug.Log("Connected to Python server.");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to connect to Python server: " + e.Message);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 60f) // 60초마다 데이터 전송
        {
            SendGameData();
            timer = 0f;
        }
    }

    void SendGameData()
    {
        if (client == null || !client.Connected)
        {
            Debug.LogError("Not connected to Python server.");
            return;
        }

        try
        {
            int expCount = GameManager.instance.player.expCount;
            int killCount = GameManager.instance.playerData.kill;

            // 데이터 전송
            List<int> dataList = new List<int> { expCount, killCount };
            formatter.Serialize(stream, dataList);
            Debug.Log("Data sent: " + string.Join(", ", dataList));

            // 결과 수신
            object dataReceived = formatter.Deserialize(stream);
            int clusterIndex = (int)dataReceived;
            Debug.Log("Cluster index received from server: " + clusterIndex);

            // 게임 기믹 추가 로직
            // ObstacleManager.cs의 게임 기믹 추가 함수 불러오기
            // ApplyGameGimmick(clusterIndex);
        }
        catch (Exception e)
        {
            Debug.LogError("Error during data transmission: " + e.Message);
        }
    }
    void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
    }
}
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager�� instanceȭ

    // ����� Ŭ���� ��ü��
    public Player player;
    private EnemyManager enemyManager;
    private FollowCam followCam;
   
    // GameObject���� �������� �־��ֱ� ���� public���� ����
    public Player playerPrefab;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Ŭ���� ��ü�� �ʱ�ȭ
        CreatePlayer();
        enemyManager = FindAnyObjectByType<EnemyManager>();

        // ���� ��ȯ
        enemyManager.CreateEnemies(100, player, 0, 15.0f);
        enemyManager.CreateEnemies(100, player, 1, 20.0f);

        followCam = FindAnyObjectByType<FollowCam>();
        followCam.player = player;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Player ���� �Լ�
    private void CreatePlayer(){
        player = Instantiate(playerPrefab);
        player.onPlayerWasKilled = OnPlayerHasKilled;
    }

    // �÷��̾ �׾��� �� �����
    private void OnPlayerHasKilled(Player player)
    {
        //
    }
}

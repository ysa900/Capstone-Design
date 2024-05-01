using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TilemapManager : MonoBehaviour
{  
    // Stage1 오브젝트들
    bool isStage1End;
    public GameObject teleport_hole;
    bool isTeleportHoleAlreadySpawned;

    // Stage2 오브젝트들
    public GameObject Corridor1;
    public GameObject Corridor2;
    public GameObject bossRoom;
    
    
    bool isStage2End;
    bool isBossRoomAlreadyMoved;
   
    private void Update()
    {
        isStage1End = SceneManager.GetActiveScene().name == "Stage1" && GameManager.instance.gameTime >= 5 * 60f;

        if (isStage1End && !isTeleportHoleAlreadySpawned)
        {
            Vector3 playerPosition = GameManager.instance.player.transform.position;
            Vector3 newPos = new Vector3(playerPosition.x, playerPosition.y+10, 0);
            teleport_hole.transform.position = newPos;
            teleport_hole.SetActive(true);
            isTeleportHoleAlreadySpawned = true;
        }

        isStage2End = SceneManager.GetActiveScene().name == "Stage2" && GameManager.instance.gameTime >= 5 * 60f;

        if (isStage2End && !isBossRoomAlreadyMoved)
        {
            GameObject RightCorridor = Corridor1.transform.position.x > Corridor2.transform.position.x ? Corridor1 : Corridor2;

            Vector2 newPos = new Vector2(RightCorridor.transform.position.x, RightCorridor.transform.position.y);
            newPos.x += 165.2f; // Corridor나 BossRoom 크기 조절하면 바꿔줘야 함
            newPos.y += -7.9f;

            bossRoom.transform.position = newPos;
            bossRoom.SetActive(true);

            isBossRoomAlreadyMoved = true;
        }
    }
}


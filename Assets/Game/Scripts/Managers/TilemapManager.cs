using System;
using UnityEngine;

public class TilemapManager : MonoBehaviour
{
    // Stage2 오브젝트들
    public GameObject Corridor1;
    public GameObject Corridor2;
    public GameObject bossRoom;
    public int buildIndex;
    bool isStage2End;
    bool isBossRoomAlreadyMoved;

    private void Update()
    {
        isStage2End = buildIndex == 3 && GameManager.instance.gameTime >= 5 * 60f;
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


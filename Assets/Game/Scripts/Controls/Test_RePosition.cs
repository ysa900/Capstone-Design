using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_RePosition : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 플레이어 프리팹 하위에 Area라는게 있음 
        if (!collision.CompareTag("Area"))
        {
            return;
        }

        Vector3 playerPosition = TestManager.instance.testPlayer.transform.position;
        Vector3 myPosition = transform.position;
        float diffX = Mathf.Abs(playerPosition.x - myPosition.x);
        float diffY = Mathf.Abs(playerPosition.y - myPosition.y);

        Vector3 playerDirection = TestManager.instance.testPlayer.inputVec;
        float dirtionX = playerDirection.x < 0 ? -1 : 1; // playerDirection이 마이너스면 -1, 플러스면 1
        float dirtionY = playerDirection.y < 0 ? -1 : 1;

        switch (transform.tag)
        {
            case "Ground":
                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirtionX * 40); // 오른쪽 방향 * (-1 or 1) * 거리
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirtionY * 40);// 윗 방향 * (-1 or 1) * 거리
                }
                break;
            case "Enemy":

                break;
        }
    }
}

using UnityEngine;

public class RePositon : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 플레이어의 Area와 충돌을 감지해 벗어났다면 실행
        // 플레이어 프리팹 하위에 Area라는게 있음 
        if (!collision.CompareTag("Area"))
        {
            return;
        }

        Vector3 playerPosition = GameManager.instance.player.transform.position;
        Vector3 myPosition = transform.position;
        float diffX = Mathf.Abs(playerPosition.x - myPosition.x);
        float diffY = Mathf.Abs(playerPosition.y - myPosition.y);

        float dirtionX = myPosition.x < playerPosition.x ? 1 : -1; // playerDirection이 마이너스면 -1, 플러스면 1
        float dirtionY = myPosition.y < playerPosition.y ? 1 : -1;

        switch (transform.tag)
        {
            case "Ground":
                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirtionX * 80); // 오른쪽 방향 * (-1 or 1) * 거리
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirtionY * 80);// 윗 방향 * (-1 or 1) * 거리
                }
                break;
        }
    }
}
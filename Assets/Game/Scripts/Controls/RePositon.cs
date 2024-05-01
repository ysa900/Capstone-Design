using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RePositon : MonoBehaviour
{
    public GameObject clearWall;

    bool isStage2TimeOver;
    int playerAreaSize;

    private void Start()
    {
        
        // clearWall의 오른쪽 끝 좌표를 GameManger를 통해 FollowCam에게 전달 
        if (clearWall != null) SendClearWall_RightX();

        // GameManager가 초기화 할 동안 대기 후 가져오기
        StartCoroutine(WaitForNSec(0.5f));
    }

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
                if(diffX > playerAreaSize && diffY > playerAreaSize)
                {
                    transform.Translate(Vector3.right * dirtionX * playerAreaSize);
                    transform.Translate(Vector3.up * dirtionY * playerAreaSize);
                }
                else if(diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirtionX * playerAreaSize); // 오른쪽 방향 * (-1 or 1) * 거리
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirtionY * playerAreaSize);// 윗 방향 * (-1 or 1) * 거리
                }
                break;

            case "Corridor":
                // Stage2에서 5분이 지나면 Reposition이 멈추고, 보스 방으로 가는 길이 열려야 함
                isStage2TimeOver = SceneManager.GetActiveScene().name == "Stage2" && GameManager.instance.gameTime >= 5 * 60;
                
                if (isStage2TimeOver) break;

                if (playerPosition.x >= myPosition.x)
                {
                    transform.Translate(Vector3.right * 85 * 2); // 오른쪽 방향 * 거리
                    clearWall.transform.Translate(Vector3.right * 85);

                    // clearWall의 오른쪽 끝 좌표를 GameManger를 통해 FollowCam에게 전달 
                    SendClearWall_RightX();
                }
                break;
        }
    }

    // clearWall의 오른쪽 끝 좌표를 GameManger를 통해 FollowCam에게 전달 
    void SendClearWall_RightX()
    {
        float clearWall_RightX = clearWall.transform.position.x + clearWall.transform.localScale.x / 2;
        GameManager.instance.SendClearWall_RightX(clearWall_RightX);
    }

    IEnumerator WaitForNSec(float time)
    {
        yield return new WaitForSeconds(time);

        playerAreaSize = (int)GameManager.instance.player.gameObject.GetComponentInChildren<BoxCollider2D>().size.x;
    }
}
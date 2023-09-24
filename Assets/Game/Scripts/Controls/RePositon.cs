using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RePositon : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        // �÷��̾��� Area�� �浹�� ������ ����ٸ� ����
        // �÷��̾� ������ ������ Area��°� ���� 
        if (!collision.CompareTag("Area"))
        {
            return;
        }

        Vector3 playerPosition = GameManager.instance.player.transform.position;
        Vector3 myPosition = transform.position;
        float diffX = Mathf.Abs(playerPosition.x - myPosition.x);
        float diffY = Mathf.Abs(playerPosition.y - myPosition.y);

        Vector3 playerDirection = GameManager.instance.player.inputVec;
        float dirtionX = playerDirection.x < 0 ? -1 : 1; // playerDirection�� ���̳ʽ��� -1, �÷����� 1
        float dirtionY = playerDirection.y < 0 ? -1 : 1;

        switch (transform.tag)
        {
            case "Ground":
                if(diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirtionX * 80); // ������ ���� * (-1 or 1) * �Ÿ�
                }
                else if(diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirtionY * 80);// �� ���� * (-1 or 1) * �Ÿ�
                }
                break;
            case "Enemy":

                break;
        }
    }
}

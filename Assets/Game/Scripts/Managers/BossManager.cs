using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Enemy;
using static UnityEngine.EventSystems.EventTrigger;

public class BossManager : MonoBehaviour
{
    // �� �ּ� ���� �Ÿ�
    private float minBossSpawnRange = 20;

    Boss boss;

    public Boss bossPrefab;

    public void CreateBoss(Player player, float maxRadius)
    {
        SetBossInfoNSummon(player,bossPrefab, maxRadius);

    }

    private void OnBossWasKilled(Boss killedBoss)
    {
        //
    }

    private void SetBossInfoNSummon( Player player, Boss bossPrefab, float maxRadius)
    {
        boss = Instantiate(bossPrefab);

        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;


        // ���Ͱ� �������� ��ȯ�ǰ� ��
        // x�� ���� ���� �ް�, y = ��Ʈ(r^2 - x^2)
        float randomRadius = UnityEngine.Random.Range(minBossSpawnRange, maxRadius);
        boss.X = UnityEngine.Random.Range(-randomRadius, randomRadius);
        double tmp = Math.Pow(randomRadius, 2) - Math.Pow(boss.X, 2);
        boss.Y = (float)Math.Sqrt(tmp);

        if (UnityEngine.Random.Range(0, 2) == 0) // Y���� ��Ʈ�� �ϸ� �׻� ����� �����ϱ� �������� ������ �ο�
            boss.Y = -boss.Y;

        boss.Y += playerY;
        boss.X += playerX;
        boss.player = player;
        boss.onBossWasKilled = OnBossWasKilled;


    }


}

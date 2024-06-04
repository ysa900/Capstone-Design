using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // 다루게 될 데이터 enum으로 선언
    public enum InfoType { Exp, Level, Kill, Time, Hp, HpStatus, BossHP, ExpCheckPoint }
    public InfoType type;

    Text InfomationText;
    Slider myHpSlider;
    Slider myExpSlider;
    Slider bossHPSlider;

    void Awake()
    {
        InfomationText = GetComponent<Text>();
        myHpSlider = GetComponent<Slider>();
        myExpSlider = GetComponent<Slider>();
        bossHPSlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:
                float curExp = GameManager.instance.playerData.Exp;
                float maxExp = GameManager.instance.playerData.nextExp[GameManager.instance.playerData.level];

                myExpSlider.value = curExp / maxExp;
                break;

            case InfoType.ExpCheckPoint:
                InfomationText.text = string.Format("{0:F0}", GameManager.instance.player.expCount);
                break;

            case InfoType.Level:
                InfomationText.text = string.Format("Lv.{0:F0}", GameManager.instance.playerData.level);
                break;

            case InfoType.Kill:
                InfomationText.text = string.Format("{0:F0}", GameManager.instance.playerData.kill);
                break;

            case InfoType.Hp:
                float currentHp = GameManager.instance.playerData.hp;
                float maxHp = GameManager.instance.playerData.maxHp;
                myHpSlider.value = currentHp / maxHp;
                break;

            case InfoType.HpStatus:
                float currentHp2 = GameManager.instance.playerData.hp;
                float maxHp2 = GameManager.instance.playerData.maxHp;

                InfomationText.text = string.Format("{0} / {1}", (int)currentHp2, (int)maxHp2);
                break;

            case InfoType.BossHP:
                float currentBossHp = GameManager.instance.boss.hp;
                float bossMaxHp = GameManager.instance.boss.maxHp;

                bossHPSlider.value = currentBossHp / bossMaxHp * 100;
                break;

            case InfoType.Time:
                float playTime = GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(playTime / 60);
                int sec = Mathf.FloorToInt(playTime % 60);
                InfomationText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;

        }
    }
}
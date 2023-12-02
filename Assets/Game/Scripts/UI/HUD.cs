using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // �ٷ�� �� ������ enum���� ����
    public enum InfoType { Exp, Level, Kill, Time, Hp, HpStatus, BossHP }
    public InfoType type;

    Text hpText, timeText;
    Slider myHpSlider;
    Slider bossHPSlider;


    void Awake()
    {
        hpText=GetComponent<Text>();
        timeText=GetComponent<Text>();
        myHpSlider=GetComponent<Slider>();
        bossHPSlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch(type)
        {
            case InfoType.Exp:
                float curExp = GameManager.instance.player.Exp;
                float maxExp = GameManager.instance.player.nextExp[GameManager.instance.player.level];
                myHpSlider.value = curExp/maxExp;
                
                break;
            case InfoType.Level:
                timeText.text = string.Format("Lv.{0:F0}",GameManager.instance.player.level);
                break;

            case InfoType.Kill:
                timeText.text = string.Format("{0:F0}", GameManager.instance.player.kill);
                break;

            case InfoType.Hp:
                float currentHp = GameManager.instance.player.hp;
                float maxHp = GameManager.instance.player.maxHp;

                myHpSlider.value = currentHp / maxHp;
                
                //Debug.Log(currentHp + " " + maxHp);
                break;

            case InfoType.HpStatus:
                float currentHp2 = GameManager.instance.player.hp;
                float maxHp2 = GameManager.instance.player.maxHp;

                hpText.text = string.Format("{0} / {1}", (int)currentHp2, (int)maxHp2);

                //Debug.Log(currentHp + " " + maxHp);
                break;
            case InfoType.BossHP:
                float currentBossHp = GameManager.instance.boss.hp;
                float bossMaxHp = GameManager.instance.boss.maxHp;

                bossHPSlider.value = currentBossHp / bossMaxHp * 100;

                //Debug.Log(currentBossHp + " " + bossMaxHp);
                break;
            case InfoType.Time:
                float playTime = GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(playTime / 60);
                int sec = Mathf.FloorToInt(playTime % 60);

                timeText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;

        }
    }
}
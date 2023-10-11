using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // 다루게 될 데이터 enum으로 선언
    public enum InfoType { Exp, Level, Kill, Time, Hp, HpStatus }
    public InfoType type;

    Text hpText, timeText;
    Slider myHpSlider;


    void Awake()
    {
        hpText=GetComponent<Text>();
        timeText=GetComponent<Text>();
        myHpSlider=GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch(type)
        {
            case InfoType.Exp:

                break;
            case InfoType.Level:

                break;

            case InfoType.Kill:

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
            case InfoType.Time:
                float playTime = GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(playTime / 60);
                int sec = Mathf.FloorToInt(playTime % 60);

                timeText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;

        }
    }
}
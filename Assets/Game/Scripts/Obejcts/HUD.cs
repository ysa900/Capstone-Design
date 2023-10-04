using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // 다루게 될 데이테 enum으로 선언
    public enum InfoType { Exp, Level, Kill, Time, Hp }
    public InfoType type;

    Text myText;
    Slider mySlider;

    void Awake()
    {
        myText=GetComponent<Text>();
        mySlider=GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch(type)
        {
            case InfoType.Exp:

                break;
            case InfoType.Level:

                break;
            case InfoType.Time:
                float playTime = GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(playTime / 60);
                int sec = Mathf.FloorToInt(playTime % 60);

                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Kill:

                break;
            case InfoType.Hp:
                float currentHp = GameManager.instance.player.hp;
                float maxHp = GameManager.instance.player.maxHp;

                mySlider.value = currentHp / maxHp;

                Debug.Log(currentHp + " " + maxHp);
                break;
        }
    }
}
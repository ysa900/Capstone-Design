using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashManager : MonoBehaviour
{
    // 참조 사이트 링크
    // https://cherish-my-codes.tistory.com/entry/Unity-인트로-비동기로드-씬-만들기

    [SerializeField] Image image = null;
    [SerializeField] TextMeshProUGUI text = null;

    public bool isTypingFinish = false;
    //float delay = 1f;

    public delegate void OnImageShowFinish(bool isImageShowFinish);
    public OnImageShowFinish onImageShowFinish;

    void Start()
    {
        StartCoroutine(FadeTextToFullAlpha(1f, image, text));
    }


    public IEnumerator FadeTextToFullAlpha(float t, Image i, TextMeshProUGUI j)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        j.color = new Color(j.color.r, j.color.g, j.color.b, 0);

        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }

        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        onImageShowFinish(true);

        /*
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
        */
        /*
        while (j.color.a < 1.0f)
        {
            j.color = new Color(j.color.r, j.color.g, j.color.b, j.color.a + (Time.deltaTime / t));
            yield return null;
        }
        */
        j.color = new Color(j.color.r, j.color.g, j.color.b, 1);

        yield return new WaitUntil(() => isTypingFinish);

        while (j.color.a > 0.0f)
        {
            j.color = new Color(j.color.r, j.color.g, j.color.b, j.color.a - (Time.deltaTime / t));
            yield return null;
        }

        SceneManager.LoadScene("Lobby");
    }

}
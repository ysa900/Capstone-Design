using System.Collections;
using UnityEngine;
using KoreanTyper;
using TMPro;

public class KoreanTyperCode : MonoBehaviour
{
    private SplashManager splashManager;

    //public float delay = 1f;
    public string message;
    private bool isImageShowFinish = false;

    TextMeshProUGUI myText;
    WaitForSeconds typingWait;

    void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
        splashManager = FindAnyObjectByType<SplashManager>();
        splashManager.onImageShowFinish = OnImageShowFinish;

        message = myText.text;
        Debug.Log(message);
        typingWait = new WaitForSeconds(0.05f);
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => isImageShowFinish);
        StartCoroutine(TypingMsg());
    }

    private void OnImageShowFinish(bool isImageShowFinish)
    {
        this.isImageShowFinish = isImageShowFinish;
    }

    public void StartTyping(string value)
    {
        message = value;
        StartCoroutine(TypingMsg());
    }

    IEnumerator TypingMsg()
    {
        int typingLength = message.GetTypingLength();
        myText.text = "";

        for (int index = 0; index <= typingLength; index++)
        {
            yield return typingWait;
            myText.text = message.Typing(index);
        }

        yield return new WaitForSeconds(2f); // 글자 다 써지고 나서 사라지기 전까지의 대기시간
        splashManager.isTypingFinish = true;
    }
}

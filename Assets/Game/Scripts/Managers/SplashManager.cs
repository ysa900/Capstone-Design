using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashManager : MonoBehaviour
{
    public KoreanTyperSimple[] koreanTyper;
    
    [SerializeField] Image backgroundImage = null; // 뒷배경
    [SerializeField] GameObject GuideTextObject;
    [SerializeField] GameObject[] imageObjects = null;
    [SerializeField] GameObject GuidePanel;
    [SerializeField] UnityEngine.UI.Button goNextStageButtonObject;
    int currentPoint = 0;

    private void Awake()
    {
        // Text 단락 초기화
        koreanTyper[currentPoint].gameObject.SetActive(true);
        for (int index = 1; index < koreanTyper.Length; index++)
        {

            koreanTyper[index].gameObject.SetActive(false);
        }

        // 이미지 초기화
        for (int index = 0; index < imageObjects.Length; index++)
        {
            imageObjects[index].SetActive(false);
        }

        GuideTextObject.SetActive(false);
        GuidePanel.SetActive(false);
    }

    void Start()
    {
        UnityEngine.UI.Button goNextStageButton = goNextStageButtonObject.GetComponent<UnityEngine.UI.Button>();
        goNextStageButton.onClick.AddListener(OnSelectNextStage);

        for (int index=0; index<koreanTyper.Length; index++)
        {
            koreanTyper[index].onTextTypeFinish = OnTextTypeFinish;
        }

        StartCoroutine(TypingInterruptedInput()); // 처음 시작 단락
    }

    private void OnShowStory()
    {
        StopCoroutine(TypingInterruptedInput());

        if (currentPoint == koreanTyper.Length - 1) // 마지막 단락까지 왔으면
        {
            
            StartCoroutine(TypingInterruptedInput());

            GuideTextObject.SetActive(true);

            switch(SceneManager.GetActiveScene().name)
            {
                case "Splash0": case "Splash1": // 시작화면 및 Stage1 시작 전
                    StartCoroutine(WaitForNextScene());
                    break;
                default: // 나머지 (Stage2, 3 ...)
                    StartCoroutine(WaitForGuidePanel());
                    break;
            }

            return;
        }

        if (koreanTyper.Length > 1)
        {
            imageObjects[currentPoint].SetActive(true);

            StartCoroutine(WaitForNextInput());
            
            StartCoroutine(TypingInterruptedInput());
        }
    }

    IEnumerator WaitForNextInput()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);

        koreanTyper[++currentPoint].gameObject.SetActive(true);

    }

    IEnumerator WaitForNextScene()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);

        switch (SceneManager.GetActiveScene().name)
        {
            case "Splash0":
                SceneManager.LoadScene("Lobby");
                break;
            case "Splash1":
                //SceneManager.LoadScene("Stage1");
                SceneManager.LoadScene("Stage1");
                break;
            case "Splash2":
                //SceneManager.LoadScene("Stage2");
                SceneManager.LoadScene("Stage2");
                break;

        }
    }

    IEnumerator WaitForGuidePanel()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);

        switch(SceneManager.GetActiveScene().name)
        {
            case "Splash2":
                GuidePanel.SetActive(true);
                break;
        }
    }

    IEnumerator TypingInterruptedInput()
    {
        yield return new WaitForSeconds(1.2f);

        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return));

        koreanTyper[currentPoint].isInputOccured = true;
    }

    private void OnTextTypeFinish()
    {
        OnShowStory();
    }

    private void OnSelectNextStage()
    {
        StartCoroutine(WaitForNextScene());
    }
}
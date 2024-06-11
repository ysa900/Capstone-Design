using UnityEngine;
using UnityEngine.SceneManagement;

public class Canvas : MonoBehaviour
{
    // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
    private static Canvas _instance;

    // 인스턴스에 접근하기 위한 프로퍼티
    public static Canvas instance
    {
        get
        {
            // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
            if (!_instance)
            {
                _instance = FindAnyObjectByType(typeof(Canvas)) as Canvas;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제한다.
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        // 아래의 함수를 사용하여 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않는다.
        DontDestroyOnLoad(gameObject);
    }

    // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
    public void JudgeActiveCanvas()
    {
        bool isSplashScene =
            SceneManager.GetActiveScene().name == "Lobby" ||
            SceneManager.GetActiveScene().name == "Splash1" ||
            SceneManager.GetActiveScene().name == "Splash2" ||
            SceneManager.GetActiveScene().name == "Splash3";

        if (isSplashScene) { gameObject.SetActive(false); }
        else { gameObject.SetActive(true); }
    }
}

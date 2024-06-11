using UnityEngine;
using UnityEngine.SceneManagement;

public class Canvas : MonoBehaviour
{
    // �̱��� ������ ����ϱ� ���� �ν��Ͻ� ����
    private static Canvas _instance;

    // �ν��Ͻ��� �����ϱ� ���� ������Ƽ
    public static Canvas instance
    {
        get
        {
            // �ν��Ͻ��� ���� ��쿡 �����Ϸ� �ϸ� �ν��Ͻ��� �Ҵ����ش�.
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
        // �ν��Ͻ��� �����ϴ� ��� ���λ���� �ν��Ͻ��� �����Ѵ�.
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        // �Ʒ��� �Լ��� ����Ͽ� ���� ��ȯ�Ǵ��� ����Ǿ��� �ν��Ͻ��� �ı����� �ʴ´�.
        DontDestroyOnLoad(gameObject);
    }

    // ü���� �ɾ �� �Լ��� �� ������ ȣ��ȴ�.
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

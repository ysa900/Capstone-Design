using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    // Restart 버튼
    public UnityEngine.UI.Button restartButtonObject;

    // Start is called before the first frame update
    void Start()
    {
        // Restart 버튼 눌렀을 때
        UnityEngine.UI.Button restartButton = restartButtonObject.GetComponent<UnityEngine.UI.Button>();
        restartButton.onClick.AddListener(RestartButtonClicked);
    }

    private void RestartButtonClicked()
    {
        SceneManager.LoadScene("Game");
    }
}

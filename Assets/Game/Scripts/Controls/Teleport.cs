using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleport:MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isPlayer = collision.gameObject.tag == "Player";


        if (isPlayer)
        {
            switch(SceneManager.GetActiveScene().name)
            {
                case "Stage1":
                    GameAudioManager.instance.bgmPlayer.Stop();
                    GameManager.instance.isStageClear = false;
                    SceneManager.LoadScene("Splash2"); // n회: Stage1 -> Stage2
                    break;

                case "Stage2":
                    GameAudioManager.instance.bgmPlayer.Stop();
                    GameManager.instance.isStageClear = false;
                    SceneManager.LoadScene("Splash3"); // n회: Stage2 -> Stage3
                    break;
            } 
        }
    }
}


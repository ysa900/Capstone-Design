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
                    SceneManager.LoadScene("Splash2"); // Stage1 -> Splash2
                    break;
                case "Stage2":
                    GameAudioManager.instance.bgmPlayer.Stop();
                    SceneManager.LoadScene("Splash3"); // Stage2 -> Splash3
                    break;
            } 
        }
    }
}

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
                    SceneManager.LoadScene("Stage3"); // Stage1 -> Splash2
                    break;
                case "Stage2":
                    SceneManager.LoadScene("Splash3"); // Stage2 -> Splash3
                    break;
            } 
        }
    }
}


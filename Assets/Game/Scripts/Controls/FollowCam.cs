using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Player player;

    private void Start()
    {
        
    }

    private void Update()
    {
        Vector2 newCamPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        transform.position = new Vector3(newCamPosition.x, newCamPosition.y, transform.position.z);
    }
}


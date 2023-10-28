using UnityEngine;

public class EXP : Object
{
    public int expAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPlayer iPlayer = collision.GetComponent<IPlayer>();

        if (iPlayer == null)
        {
            return;
        }

        iPlayer.GetExp(expAmount);

        Destroy(gameObject);
    }
}


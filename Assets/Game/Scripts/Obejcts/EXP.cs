using UnityEngine;

public class EXP : Object
{
    public int expAmount;
<<<<<<< Updated upstream
=======
    public int index;

    public void Init(){}

    void FixedUpdate()
    {

    }

    private void FindPlayerAndMove()
    {

    }
>>>>>>> Stashed changes

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


using UnityEngine;

public class EXP : Object, IPullingObject
{
    public int expAmount;
    public int index;

    public void Init(){}

    void FixedUpdate()
    {

    }

    private void FindPlayerAndMove()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IPlayer iPlayer = collision.GetComponent<IPlayer>();

        if (iPlayer == null)
        {
            return;
        }

        iPlayer.GetExp(expAmount);

        GameManager.instance.poolManager.ReturnExp(this, index);
    }
}


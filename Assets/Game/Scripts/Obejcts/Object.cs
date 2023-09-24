using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    protected int x;

    public int X
    {
        get => x;
        set
        {
            x = value;

            Vector2 pos = transform.position;
            transform.position = new Vector2(x, pos.y);
        }
    }

    protected int y;

    public int Y
    {
        get => y;
        set
        {
            y = value;

            Vector2 pos = transform.position;
            transform.position = new Vector2(pos.x, y);
        }
    }
    
}
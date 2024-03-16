using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public static TestManager instance;
    public TestPlayer testPlayer;

    private void Awake()
    {
        instance = this;
    }
}

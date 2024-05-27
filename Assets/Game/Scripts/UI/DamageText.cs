using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour, IPoolingObject
{
    public float moveSpeed;
    public float alphaSpeed;
    public float destroyTime;
    public int damage;
    public string skillTag;

    float delayTime = 0.5f;
    float delayTimer = 0;
    string color;

    TextMeshPro text;
    Color alpha;

    public void Init()
    {
        delayTimer = 0;

        alpha.a = 1.0f;
        text.color = alpha;
        
        switch (skillTag)
        {
            case "Fire":
                color = "#FF5050";
                break;
            case "Electric":
                color = "#D2F7FF";
                break;
            case "Water":
                color = "#64BCFF";
                break;
        }
        text.text = "<color=" + color + ">" + damage.ToString() + "</color>";


        Invoke("DestroyObject", destroyTime);
    }

    void Awake()
    {
        text = GetComponent<TextMeshPro>();
        alpha = text.color;
    }

    void Start()
    {
        Init();
    }


    void Update()
    {
        transform.Translate(new Vector3(moveSpeed * Time.deltaTime, -moveSpeed * Time.deltaTime, 0));
        
        if( delayTimer > delayTime )
        {
            alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
            text.color = alpha;
        }
        
        delayTimer += Time.deltaTime;
    }

    void DestroyObject()
    {
        GameManager.instance.poolManager.ReturnText(gameObject);
    }
}

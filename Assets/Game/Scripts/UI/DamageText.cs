using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour, IPullingObject
{
    public float moveSpeed;
    public float alphaSpeed;
    public float destroyTime;
    public int damage;

    float delayTime = 0.5f;
    float delayTimer = 0;

    TextMeshPro text;
    Color alpha;

    public void Init()
    {
        text.text = damage.ToString();

        alpha.a = 1.0f;
        text.color = alpha;
        delayTimer = 0;

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

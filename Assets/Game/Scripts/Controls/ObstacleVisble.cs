using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObstacleVisble : MonoBehaviour
{
    [SerializeField] private float alphaSpeed;

    SpriteRenderer sr;
    TilemapRenderer tr;
    Tilemap tile;
    Color srAlphaColor, tileAlphaColor;

    bool isEnterFirst = true;
    bool isExitFirst = true;
    bool isTriggerMatch = false;

    private void Start()
    {
        switch (name)
        {
            case "opaque_wall":
                tr = GetComponent<TilemapRenderer>();
                tile = GetComponent<Tilemap>();
                tileAlphaColor = tile.color;
                break;
            default:
                sr = GetComponent<SpriteRenderer>();
                srAlphaColor = sr.color;
                break;
        }


    }
    private void Update()
    {
        if(isTriggerMatch) // 장애물 들어갈 때
        {
            if(sr == null) // 자기 자신(오브젝트)이 Tilemap인 경우
            {
                tileAlphaColor.a = Mathf.Lerp(tileAlphaColor.a, 0.7f, Time.deltaTime * alphaSpeed);
                tile.color = tileAlphaColor;
            }
            else // 자기 자신(오브젝트)이 SpriteRenderer인 경우
            {
                srAlphaColor.a = Mathf.Lerp(srAlphaColor.a, 0.7f, Time.deltaTime * alphaSpeed);
                sr.color = srAlphaColor;
            }
        }
        else // 장애물에서 나올 때
        {
            if (sr == null) // 자기 자신(오브젝트)이 Tilemap인 경우
            {
                tileAlphaColor.a = Mathf.Lerp(tileAlphaColor.a, 1f, Time.deltaTime * alphaSpeed);
                tile.color = tileAlphaColor;

            }
            else // 자기 자신(오브젝트)이 SpriteRenderer인 경우
            {
                srAlphaColor.a = Mathf.Lerp(srAlphaColor.a, 1f, Time.deltaTime * alphaSpeed);
                sr.color = srAlphaColor;
            }


        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        if(!collision.CompareTag("Player"))
        {
            return; // Player 태그 아니면 함수 탈출
        }

        if (isEnterFirst == true) isEnterFirst = false;
        else { isEnterFirst = true; return; }

        isTriggerMatch = true; // Player와 Collider 접촉 시

        switch(name)
        {
            
        }
        
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        if (isExitFirst == true) { isExitFirst = false; return; }
        else { isExitFirst = true; }


        isTriggerMatch = false;

        switch (name)
        {
            

        }
    }
}

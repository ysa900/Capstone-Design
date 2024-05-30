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
    Player player;
    float playerYpostion;

    bool isTriggerMatch = false;
    bool isTwowayObstacle = false;

    private void Start()
    {
        player = GameManager.instance.player;

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

        switch (name)
        {
            case "Left Pillar":
            case "Right Pillar":
            case "Tree":
            case "Pillar":
                isTwowayObstacle=true;
                break;
        }
    }
    private void Update()
    {
        if (isTwowayObstacle)
        {
            playerYpostion = player.transform.position.y + 1.9f;

            if (playerYpostion - this.transform.position.y > 0)
            {
                transform.Find("Under Clear wall").gameObject.SetActive(true);
                transform.Find("Upper Clear wall").gameObject.SetActive(false);
                sr.sortingOrder = 6;
            }
            else
            {
                transform.Find("Under Clear wall").gameObject.SetActive(false);
                transform.Find("Upper Clear wall").gameObject.SetActive(true);
                sr.sortingOrder = 3;
            }
        }

        if (isTriggerMatch) // 장애물 들어갈 때
        {
            if (sr == null) // 자기 자신(오브젝트)이 Tilemap인 경우
            {
                tileAlphaColor.a = Mathf.Lerp(tileAlphaColor.a, 0.5f, Time.deltaTime * alphaSpeed);
                tile.color = tileAlphaColor;
            }
            else // 자기 자신(오브젝트)이 SpriteRenderer인 경우
            {
                srAlphaColor.a = Mathf.Lerp(srAlphaColor.a, 0.5f, Time.deltaTime * alphaSpeed);
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

        if (!collision.CompareTag("Player"))
        {
            return; // Player 태그 아니면 함수 탈출
        }


        isTriggerMatch = true; // Player와 Collider 접촉 시
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        isTriggerMatch = false;
    }
}

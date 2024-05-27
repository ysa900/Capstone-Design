using UnityEngine;

public class PlayerAttachSkill : Skill, IPoolingObject
{
    // 플레이어 좌표를 기준으로 위치를 어디로 가야하나를 받는 변수
    public float xPositionNum;
    public float yPositionNum;

    public bool isAttachSkill; // 플레이어 근처에 붙어다니는 스킬이냐
    public bool isFlipped; // 스킬 프리팹이 뒤집힌 상태냐
    public bool isYFlipped; // 스킬을 y축으로 뒤집어야되냐

    protected SpriteRenderer spriteRenderer; // 적 방향을 바꾸기 위해 flipX를 가져오기 위한 변수

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    // 플레이어에 붙어다니는 스킬
    protected void AttachPlayer()
    {
        if (player.isPlayerLookLeft)
        {
            X = player.transform.position.x - xPositionNum;
        }
        else
        {
            X = player.transform.position.x + xPositionNum;
        }
        Y = player.transform.position.y + yPositionNum;

        if (isFlipped)
        {
            spriteRenderer.flipX = !player.isPlayerLookLeft;
        }
        else if (isYFlipped)
        {
            spriteRenderer.flipY = !player.isPlayerLookLeft;
        }
        else
        {
            spriteRenderer.flipX = player.isPlayerLookLeft;
        }
    }
}
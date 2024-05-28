using System;
using UnityEngine;

public class Electric_Ball : PlayerAttachSkill
{
    public float degree = 0f;
    private float tmpX; // Cirle을 계산할 때 0,0을 기준으로 생각한 X
    private float tmpY; // Cirle을 계산할 때 0,0을 기준으로 생각한 X

    protected override void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime;

        if (destroySkill)
        {
            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

            GameManager.instance.poolManager.ReturnSkill(this, returnIndex);
            return;
        }
        else
            CircleMove();

        base.FixedUpdate();
    }

    // 플레이어 주위를 빙빙 도는 스킬
    private void CircleMove()
    {
        degree -= speed;

        tmpX = (float)Math.Cos(degree * Mathf.Deg2Rad) * xPositionNum;
        tmpY = (float)Math.Sin(degree * Mathf.Deg2Rad) * xPositionNum; //이거 잘못쓴거 아님 (xPositionNum이 여기서 반지름 역할)

        X = tmpX + player.transform.position.x;
        Y = tmpY + player.transform.position.y;

        if (degree <= -360)
        {
            degree %= -360;
        }
    }
}


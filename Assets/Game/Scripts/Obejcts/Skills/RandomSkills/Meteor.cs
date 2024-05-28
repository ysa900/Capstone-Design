using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : RandomSkill
{
    float impactPonitX;
    float impactPonitY;

    Rigidbody2D rigid; // 물리 입력을 받기위한 변수
    Vector2 direction; // 날아갈 방향

    bool isShadowAlive; // 그림자가 살아있으면 알파값을 조정하기 위함
    float alpha = 0;

    RandomSkill skillObject;

    public override void Init()
    {
        float tmpX = player.transform.position.x;
        float tmpY = player.transform.position.y;

        float ranNum = UnityEngine.Random.Range(-14f, 10f);
        float ranNum2 = UnityEngine.Random.Range(-8f, 3f);

        tmpX += ranNum;
        tmpY += ranNum2;

        impactPonitX = tmpX;
        impactPonitY = tmpY;

        // 메테오 방향 보정 (충돌 지점 바라보게)
        Vector2 direction = new Vector2(-9f, -14f);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion angleAxis = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
        transform.rotation = rotation;

        // 메테오 위치 보정
        X = tmpX + 9f;
        Y = tmpY + 14f;

        setDirection();

        // 그림자 나타내고 지우기, 위치 보정 해준 것
        StartCoroutine(DisplayShadowNDestroy(tmpX + 2.0f, tmpY + 2.2f));

        alpha = 0;

        base.Init();
    }

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();

        Init();
    }

    private new void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime;

        if (destroySkill)
        {
            RandomSkill explode;
            explode = GameManager.instance.poolManager.GetSkill(2) as RandomSkill;

            explode.X = X;
            explode.Y = Y;

            explode.aliveTime = 0.5f;
            explode.damage = damage;

            Transform parent = explode.transform.parent;

            explode.transform.parent = null;
            explode.transform.localScale = new Vector3(scale, scale, 0);
            explode.transform.parent = parent;
            
            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

            GameManager.instance.poolManager.ReturnSkill(this, returnIndex);
            
            return;
        }
        else
        {
            MoveToimpactPonit();
        }

        // meteor 그림자 설정
        if (isShadowAlive)
        {
            if (alpha < 1f)
                alpha += 0.02f;
        }
        else
            alpha = 0f;

        skillObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);

        base.FixedUpdate();
    }

    // 날아갈 방향을 정하는 함수
    private void setDirection()
    {
        Vector2 impactVector = new Vector2(impactPonitX, impactPonitY);
        Vector2 nowVector = transform.position;

        direction = impactVector - nowVector;

        direction = direction.normalized;
    }

    // 폭발 지점으로 이동하는 함수
    private void MoveToimpactPonit()
    {
        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);

        X = transform.position.x;
        Y = transform.position.y;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // 아무것도 하지 않기 (지우면 안됨, 지우면 virtual로 선언 된 RandomSkill의 TriggerEnter가 작동 됨)
    }

    // 메테오 떨어질 때 그림자 오브젝트 생성 후 제거
    IEnumerator DisplayShadowNDestroy(float x, float y)
    {
        skillObject = GameManager.instance.poolManager.GetSkill(4) as RandomSkill;

        skillObject.transform.position = new Vector2(x, y);

        // 그림자 sacle 조정
        Transform parent = skillObject.gameObject.transform.parent;

        skillObject.gameObject.transform.parent = null;

        float myScale = transform.localScale.x;

        // * 6 / 1.5는 메테오와 메테오 그림자 사이의 스케일 조정
        skillObject.gameObject.transform.localScale = new Vector3(myScale * 6 / (float)1.5, myScale * 6 / (float)1.5, 0);
        skillObject.gameObject.transform.parent = parent;

        skillObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);

        isShadowAlive = true;

        skillObject.aliveTime = 0.6f;

        yield return new WaitForSeconds(0.6f); // 지정한 초 만큼 쉬기

        isShadowAlive = false;

        GameManager.instance.poolManager.ReturnSkill(skillObject, 4);
    }
}

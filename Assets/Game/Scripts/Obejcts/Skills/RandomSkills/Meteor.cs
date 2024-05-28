using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : RandomSkill
{
    float impactPonitX;
    float impactPonitY;

    Rigidbody2D rigid; // ���� �Է��� �ޱ����� ����
    Vector2 direction; // ���ư� ����

    bool isShadowAlive; // �׸��ڰ� ��������� ���İ��� �����ϱ� ����
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

        // ���׿� ���� ���� (�浹 ���� �ٶ󺸰�)
        Vector2 direction = new Vector2(-9f, -14f);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion angleAxis = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
        Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
        transform.rotation = rotation;

        // ���׿� ��ġ ����
        X = tmpX + 9f;
        Y = tmpY + 14f;

        setDirection();

        // �׸��� ��Ÿ���� �����, ��ġ ���� ���� ��
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
                onSkillFinished(skillIndex); // skillManager���� delegate�� �˷���

            GameManager.instance.poolManager.ReturnSkill(this, returnIndex);
            
            return;
        }
        else
        {
            MoveToimpactPonit();
        }

        // meteor �׸��� ����
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

    // ���ư� ������ ���ϴ� �Լ�
    private void setDirection()
    {
        Vector2 impactVector = new Vector2(impactPonitX, impactPonitY);
        Vector2 nowVector = transform.position;

        direction = impactVector - nowVector;

        direction = direction.normalized;
    }

    // ���� �������� �̵��ϴ� �Լ�
    private void MoveToimpactPonit()
    {
        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);

        X = transform.position.x;
        Y = transform.position.y;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // �ƹ��͵� ���� �ʱ� (����� �ȵ�, ����� virtual�� ���� �� RandomSkill�� TriggerEnter�� �۵� ��)
    }

    // ���׿� ������ �� �׸��� ������Ʈ ���� �� ����
    IEnumerator DisplayShadowNDestroy(float x, float y)
    {
        skillObject = GameManager.instance.poolManager.GetSkill(4) as RandomSkill;

        skillObject.transform.position = new Vector2(x, y);

        // �׸��� sacle ����
        Transform parent = skillObject.gameObject.transform.parent;

        skillObject.gameObject.transform.parent = null;

        float myScale = transform.localScale.x;

        // * 6 / 1.5�� ���׿��� ���׿� �׸��� ������ ������ ����
        skillObject.gameObject.transform.localScale = new Vector3(myScale * 6 / (float)1.5, myScale * 6 / (float)1.5, 0);
        skillObject.gameObject.transform.parent = parent;

        skillObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);

        isShadowAlive = true;

        skillObject.aliveTime = 0.6f;

        yield return new WaitForSeconds(0.6f); // ������ �� ��ŭ ����

        isShadowAlive = false;

        GameManager.instance.poolManager.ReturnSkill(skillObject, 4);
    }
}

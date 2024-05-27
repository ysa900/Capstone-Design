using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice_Spike : RandomSkill
{
    bool isCoroutineNow; // ���� �ڷ�ƾ�� �����ϰ� �ִ����� üũ�� ����

    Animator animator_ground;

    public override void Init()
    {
        base.Init();
        animator_ground = transform.Find("ground").GetComponent<Animator>();
        
    }


    // Update is called once per frame
    protected override void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime;

        if (destroySkill && !isCoroutineNow)
        {
            StartCoroutine(Disappear());
        }

        base.FixedUpdate();
    }

    IEnumerator Disappear()
    {
        animator.SetTrigger("Finish");
        animator_ground.SetTrigger("Finish");

        isCoroutineNow = true;

        yield return new WaitForSeconds(0.2f); // ������ �� ��ŭ ����

        if (onSkillFinished != null)
            onSkillFinished(skillIndex); // skillManager���� delegate�� �˷���

        GameManager.instance.poolManager.ReturnSkill(this, returnIndex);

        isCoroutineNow = false;
    }
}

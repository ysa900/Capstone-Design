using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice_Spike : RandomSkill
{
    bool isCoroutineNow; // 현재 코루틴을 실행하고 있는지를 체크할 변수

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

        yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기

        if (onSkillFinished != null)
            onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

        GameManager.instance.poolManager.ReturnSkill(this, returnIndex);

        isCoroutineNow = false;
    }
}

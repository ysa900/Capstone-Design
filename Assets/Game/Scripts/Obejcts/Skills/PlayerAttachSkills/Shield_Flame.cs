using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Shield_Flame : PlayerAttachSkill
{
    public delegate void OnShieldSkillDestroyed();
    public OnShieldSkillDestroyed onShieldSkillDestroyed;

    Transform absorberTransform;

    bool oldisPlayerLookLeft = false;
    bool isAbsorberOn;

    float absorbTime = 1.47f;
    float absorbTimer = 0f;

    public override void Init()
    {
        absorberTransform.gameObject.SetActive(false);
        absorberTransform.GetComponent<CircleCollider2D>().enabled = true;
        absorberTransform.GetComponent<PolygonCollider2D>().enabled = false;
        absorberTransform.GetComponent<PointEffector2D>().forceMagnitude = -150f;
        absorbTimer = 0;
        isAbsorberOn = false;

        base.Init();
    }

    private void Awake()
    {
        absorberTransform = transform.Find("Absorber");
    }

    protected override void FixedUpdate()
    {
        bool destroySkill = aliveTimer > aliveTime - 0.11f; // 끝나기 0.1초 전에 실행 (0.11로 한 이유는 버그 방지)

        if (destroySkill)
        {
            StartCoroutine(Destroy()); // 끝나기 0.1초 전에 magnitude +150으로 변경해서 적들 밀치게 함

            return;
        }
        else
        {
            AttachPlayer();
        }

        bool isAbsorbTime = absorbTimer > absorbTime;
        if (isAbsorbTime && !isAbsorberOn) 
        {
            absorberTransform.gameObject.SetActive(true);
            StartCoroutine(Active_Absorber());
            isAbsorberOn = true;
        }

        absorbTimer += Time.fixedDeltaTime;
        base.FixedUpdate();
    }

    new void AttachPlayer()
    {
        X = player.transform.position.x;
        Y = player.transform.position.y;

        spriteRenderer.flipX = player.isPlayerLookLeft;

        if (player.isPlayerLookLeft == oldisPlayerLookLeft) return; // 플레이어가 flip 안했으면 return

        float xPosNum = 0;
        if (player.isPlayerLookLeft) xPosNum = -xPositionNum;
        else xPosNum = xPositionNum;

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i).GetComponent<Transform>();

            Vector2 childPosition = childTransform.position;
            childPosition.x += xPosNum;
            childTransform.position = childPosition;
        }

        oldisPlayerLookLeft = player.isPlayerLookLeft;
    }

    IEnumerator Destroy()
    {
        absorberTransform.GetComponent<CircleCollider2D>().enabled = true;
        absorberTransform.GetComponent<PolygonCollider2D>().enabled = false;
        absorberTransform.GetComponent<PointEffector2D>().forceMagnitude = 1500f;

        yield return new WaitForSeconds(0.1f);

        if (onSkillFinished != null)
            onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

        onShieldSkillDestroyed();

        GameManager.instance.poolManager.ReturnSkill(this, returnIndex);
    }

    IEnumerator Active_Absorber()
    {
        yield return new WaitForSeconds(0.05f);

        absorberTransform.GetComponent<CircleCollider2D>().enabled = false;
        absorberTransform.GetComponent<PolygonCollider2D>().enabled = true;
    }
}
using TMPro;
using UnityEngine;

public class HeavensEclipse : Skill
{
    FollowCam followCam;

    float BurstTime = 2.0f;
    float BurstTimer = 0;

    public float burstDamage;

    SpriteRenderer eclipseSky_Renderer;
    Color eclipseSky_color;

    [SerializeField] float alphaSpeed;

    public override void Init()
    {
        BurstTimer = 0;
        eclipseSky_color.a = 0f;
        eclipseSky_Renderer.color = eclipseSky_color;

        base.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        followCam = GameManager.instance.followCam;
        eclipseSky_Renderer = transform.Find("EclipseSky").GetComponent<SpriteRenderer>();
        eclipseSky_color = eclipseSky_Renderer.color;
    }

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
        {
            AttachCamera();

            eclipseSky_color.a = Mathf.Lerp(eclipseSky_color.a, 0.75f, Time.fixedDeltaTime * alphaSpeed);
            eclipseSky_Renderer.color = eclipseSky_color;
        }

        bool isBurstTimeNow = BurstTimer > BurstTime;
        if (isBurstTimeNow) damage = burstDamage;

        BurstTimer += Time.fixedDeltaTime;
        base.FixedUpdate();
    }

    void AttachCamera()
    {
        X = followCam.transform.position.x;
        Y = followCam.transform.position.y;
    }
}

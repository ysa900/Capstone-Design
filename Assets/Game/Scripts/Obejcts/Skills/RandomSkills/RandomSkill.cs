using UnityEngine;

public class RandomSkill : Skill, IPoolingObject
{
    public float scale;

    protected Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        Init();
    }

}
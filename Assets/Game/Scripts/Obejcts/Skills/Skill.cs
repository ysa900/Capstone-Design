using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Skill : Object, IPoolingObject
{
    // 도트 데미지 관련 변수
    public bool isDotDamageSkill;
    protected float dotDelayTime = 0.2f;
    protected List<float> DotDelayTimers = new List<float>();
    protected List<string> DotDamagedEnemies_Name = new List<string>();

    // 스킬이 꺼질 때 SkillManager의 isSkillsActivated[]를 끄기 위한 delegate
    public delegate void OnSkillFinished(int index);
    public OnSkillFinished onSkillFinished;

    public void Init() { } // PoolManager때문에 이거 지우면 안됨

    public Enemy enemy;
    public Boss boss;
    public Player player;

    public float speed;
    public float damage;

    public int skillIndex;
    public int returnIndex;
}
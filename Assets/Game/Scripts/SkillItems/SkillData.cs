using UnityEngine;

[CreateAssetMenu(fileName = "Skills", menuName = "Scriptble Object/SkillData")]

public class SkillData : ScriptableObject
{
    public enum SkillType { Fire1, Fire2, Fire3, Fire4,
                            Lightning1, Lightning2, Lightning3, Lightning4,
                            Water1, Water2, Water3, Water4,
                            Wind1, Wind2, Wind3, Wind4 }


    // Start is called before the first frame update
    [Header("# Main Info")]
    public SkillType skillType;
    public int skillId;
    public string skillName;
    public string skillDescription;
    public Sprite skillicon;


    [Header("# Level Data")]
    public float baseDamage;
    public int baseCount;
    public float baseDelay;
    public float[] damages;
    public int[] counts;
    public float[] delay;

    [Header("# Skill")]
    public GameObject projectile;

}

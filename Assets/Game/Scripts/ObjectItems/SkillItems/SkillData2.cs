using UnityEngine;

[CreateAssetMenu(fileName = "Skills2", menuName = "Scriptable Object/SkillData2")]

public class SkillData2 : ScriptableObject
{

    // Start is called before the first frame update
    [Header("# Main Info")]
    public string[] skillName;
    public string[] skillDescription;

    [Header("# Data")]
    public int[] level;
    public float[] Damage;
    public float[] Delay;
    public float[] scale;

    [Header("# Skill")]
    public bool[] skillSelected;
    public Sprite[] skillicon;
    public GameObject[] projectile;

}

public class Skill : Object, IPullingObject
{
    public void Init(){}

    public Enemy enemy;
    public Boss boss;
    public Player player;

    public float speed;
    public float damage;

    public int index;
}
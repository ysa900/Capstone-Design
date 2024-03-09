public class Skill : Object, IPullingObject
{
    public void Init() { } // PoolManager때문에 이거 지우면 안됨

    public Enemy enemy;
    public Boss boss;
    public Player player;

    public float speed;
    public float damage;

    public int index;
}
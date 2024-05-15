public class BossSkill : Object, IPoolingObject
{
    public void Init() { } // PoolManager때문에 이거 지우면 안됨

    public Enemy enemy;
    public Boss boss;
    public Player player;

    public float damage;

    public int index;


    // 풀링땜에 어쩔수 없지 만든 변수들
    public float laserTurnNum; // boss_laser꺼
    public bool isRightTop; // boss_grid꺼

}

using UnityEngine;

public class EXP : Object, IPoolingObject
{
    public int expAmount;
    public int index;
    public Player player;

    bool isInMagnetRange; // Exp가 자석 범위 안에 있나
    bool isAbsorberActivated; // trigger에 Player의 Absorber가 감지됐다면
    Rigidbody2D rigid;

    public void Init(){
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Start()
    {
        Init();
    }

    void FixedUpdate()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 myPosition = transform.position;

        float distance = Vector2.Distance(myPosition, playerPosition);
        isInMagnetRange = distance <= player.playerData.magnetRange * player.transform.localScale.x; // 플레이어 사이즈 만큼 보정

        // Absorber가 감지됐으면 잠금 풀기
        if (isInMagnetRange) { rigid.constraints = RigidbodyConstraints2D.FreezeRotation; } 
        else { rigid.constraints = RigidbodyConstraints2D.FreezeAll; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {/*
        if(!isAbsorberActivated || !isInMagnetRange) // Absorber가 감지 안됐거나 자석 범위를 벗어났으면 실행
            isAbsorberActivated = collision.name == "Absorber";
        */
        IPlayer iPlayer = collision.GetComponent<IPlayer>();

        if (iPlayer == null)
        {
            return;
        }
        
        iPlayer.GetExp(expAmount);

        GameManager.instance.poolManager.ReturnExp(this, index);
    }
}


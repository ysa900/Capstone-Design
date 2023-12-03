using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour, IPlayer
{
    // Ű���� ����Ű �Է��� ���� ����
    public Vector2 inputVec;

    [SerializeField]
    // �÷��̾� ����
    public float speed;
    public float hp;
    public float maxHp = 100;
    public int Exp;
    public int level;
    public int[] nextExp;
    
    //ų ��
    public int kill;

    public bool isPlayerDead; // �÷��̾ �׾����� �Ǻ��ϴ� ����

    public bool isPlayerLookLeft; // �÷��̾ ���� �ִ� ������ �˷��ִ� ����

    public bool isPlayerShielded; // �÷��̾ ��ȣ���� ��ȣ�� �ް��ֳ�

    Rigidbody2D rigid; // ���� �Է��� �ޱ����� ����
    SpriteRenderer spriteRenderer; // �÷��̾� ������ �ٲٱ� ���� flipX�� �������� ���� ����
    Animator animator; // �ִϸ��̼� ������ ���� ����

    // �÷��̾ �׾��� �� GameManager���� �˷��ֱ� ���� delegate
    public delegate void OnPlayerWasKilled(Player player);
    public OnPlayerWasKilled onPlayerWasKilled;

    // �÷��̾ ������ ���� �� GameManager���� �˷��ֱ� ���� delegate
    public delegate void OnPlayerLevelUP();
    public OnPlayerLevelUP onPlayerLevelUP;

    private GameAudioManager gameAudioManager;

    private void Awake()
    {
        gameAudioManager = FindAnyObjectByType<GameAudioManager>();
    }

    void Start()
    {
        // ���� �ʱ�ȭ
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        nextExp = new int[100];
        int num = 0;
        for (int i = 0; i < nextExp.Length; i++)
        {
            if(level >= 30)
            {
                num += 100;
                nextExp[i] = num;


            }
            else
            {
                num += 10;
                nextExp[i] = num;

            }

 
        }
    }

    // Update is called once per frame
    void Update()
    {
        ReceiveDirectionInput(); // Ű���� ����Ű �Է��� �������� �Լ�
    }

    // ���� ���� �����Ӹ��� ȣ��Ǵ� �����ֱ� �Լ�
    private void FixedUpdate()
    {
        MovePlayer();
    }

    // �������� ������ ������ ����Ǵ� �Լ�
    private void LateUpdate()
    {
        animator.SetFloat("Speed", inputVec.magnitude); // animator�� floatŸ���� ���� Speed�� inpuVec�� ũ�⸸ŭ���� �����Ѵ�

        isPlayerLookLeft = inputVec.x < 0; // �÷��̾ ������ ���� ������

        if (inputVec.x != 0) // Ű�� �ȴ����� ���� ���� �ȵǵ��� �ϱ� ���� inputVec.x�� 0�� �ƴ� ��츸 �����ϰ� �Ѵ�
        {
            spriteRenderer.flipX = isPlayerLookLeft; // �÷��̾ x������ �����´�
        }
        else
        {
            isPlayerLookLeft = spriteRenderer.flipX;
        }
    }

    // Ű���� ����Ű �Է��� �������� �Լ�
    private void ReceiveDirectionInput()
    {
        // ����, ���� ���� �Է��� �޴´�
        // inputmanager�� �⺻ �������ִ�
        // GetAxisRaw�� �ؾ� ���� ��Ȯ�� ��Ʈ�� ����
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");
    }

    // �÷��̾ �����̴� �Լ�
    private void MovePlayer()
    {
        // �÷��̾��� ���⺤�͸� �����ͼ� �ӵ��� ����
        // fixedDeltaTime�� ���� ������ �ð�
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;

        // �Է¹��� �������� �÷��̾� ��ġ ����
        rigid.MovePosition(rigid.position + nextVec);
    }

    //player ����ġ ȹ�� �Լ�
    public void GetExp(int expAmount)
    {
        Exp += expAmount;

        if (Exp >= nextExp[level])
        {
            onPlayerLevelUP(); // delegate ȣ��
            
            level++;
            Exp = 0;
        }

    }

    // �÷��̾ ���𰡿� �浹�ϸ� �������� �Դ´�
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isPlayerDead)
        {
            if (!isPlayerShielded)
            {
                hp -= Time.deltaTime * 5;
                gameAudioManager.PlaySfx(GameAudioManager.Sfx.Melee); // �ǰ�  ȿ����
            }
            
            if (hp <= 0)
            {
                isPlayerDead = true;

                animator.SetBool("Dead", true);

                onPlayerWasKilled(this);

                rigid.constraints = RigidbodyConstraints2D.FreezeAll;

                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // �׾��� �� ������ ���� ũ�� ������ ũ�� ���� ���� ��
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (!isPlayerDead)
        {
            if (!isPlayerShielded)
            {
                hp -= damage;
                gameAudioManager.PlaySfx(GameAudioManager.Sfx.Melee); // �ǰ�  ȿ����
            }
            
            if (hp <= 0)
            {
                isPlayerDead = true;

                animator.SetBool("Dead", true);

                onPlayerWasKilled(this);

                rigid.constraints = RigidbodyConstraints2D.FreezeAll;

                transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); // �׾��� �� ������ ���� ũ�� ������ ũ�� ���� ���� ��
            }
        }
    }

}

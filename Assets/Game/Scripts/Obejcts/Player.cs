using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Ű���� ����Ű �Է��� ���� ����
    public Vector2 inputVec;

    [SerializeField]
    // �÷��̾� ����
    public float speed;
    public float hp;

    private bool isPlayerDead;

    Rigidbody2D rigid; // ���� �Է��� �ޱ����� ����
    SpriteRenderer spriteRenderer; // �÷��̾� ������ �ٲٱ� ���� flipX�� �������� ���� ����
    Animator animator; // �ִϸ��̼� ������ ���� ����

    // �÷��̾ �׾��� �� GameManager���� �˷��ֱ� ���� delegate
    public delegate void OnPlayerWasKilled(Player player);
    public OnPlayerWasKilled onPlayerWasKilled;

    void Start()
    {
        // ���� �ʱ�ȭ
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ReceiveDirectionInput();
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

        bool isPlayerLookLeft = inputVec.x < 0; // �÷��̾ ������ ���� ������

        if (inputVec.x != 0) // Ű�� �ȴ����� ���� ���� �ȵǵ��� �ϱ� ���� inputVec.x�� 0�� �ƴ� ��츸 �����ϰ� �Ѵ�
        {
            spriteRenderer.flipX = isPlayerLookLeft; // �÷��̾ x������ �����´�
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

    // �÷��̾ ���𰡿� �浹�ϸ� �������� �Դ´�
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isPlayerDead)
        {
            hp -= Time.deltaTime * 10;

            if (hp < 0)
            {
                isPlayerDead = true;

                animator.SetBool("Dead", true);

                onPlayerWasKilled(this);

                rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }
}

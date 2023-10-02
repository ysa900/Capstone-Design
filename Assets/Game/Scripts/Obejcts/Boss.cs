using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Object
{
    // �÷��̾� ��ü
    public Player player;

    // ���� ����
    public int hp;
    public float speed;
    private float colliderOffsetX; // collider�� offset x��ǥ
    private float colliderOffsetY; // collider�� offset y��ǥ


    //Boss�� �׾��� �� BossManager���� �˷��ֱ� ���� delegate
    public delegate void OnBossWasKilled(Boss killedBoss);
    public OnBossWasKilled onBossWasKilled;

    // ���� �Է��� �ޱ����� ����
    Rigidbody2D rigid;

    SpriteRenderer spriteRenderer; // �� ������ �ٲٱ� ���� flipX�� �������� ���� ����

    Animator animator; // �ִϸ��̼� ������ ���� ����

    CapsuleCollider2D capsuleCollider; // Collider�� offset�� �����ϱ� ���� ����

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        colliderOffsetX = capsuleCollider.offset.x; // offset �ʱⰪ�� ����
        colliderOffsetY = capsuleCollider.offset.y;
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        MoveToPlayer();
    }

    // �÷��̾� �������� �̵��ϴ� �Լ�
    private void MoveToPlayer()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 myPosition = transform.position;

        Vector2 direction = playerPosition - myPosition;
        direction = direction.normalized;

        bool isBossLookLeft = direction.x < 0;
        spriteRenderer.flipX = isBossLookLeft;

        Vector2 colliderOffset; // CapsuleCollider�� offset�� ���� Vector2

        if (isBossLookLeft) // Enemy�� ������ ���� collider�� x�� ��Ī�� ���ش�
        {
            colliderOffset = new Vector2(-colliderOffsetX, colliderOffsetY);
        }
        else
        {
            colliderOffset = new Vector2(colliderOffsetX, colliderOffsetY);

        }
        capsuleCollider.offset = colliderOffset; // capsuleCollider�� ����

        rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);
    }

    // IDamageable�� �Լ� TakeDamage
    public void TakeDamage(GameObject causer, float damage)
    {
        hp = hp - (int)damage;

        if (hp < 0)
        {
            Debug.Log("���� ���");

            animator.SetBool("Dead", true);

            //  �븮�� ȣ��
            onBossWasKilled(this);

            Destroy(gameObject);
        }

    }

}

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class Boss_Bullet_ML : Agent
{
    public Player_ML player;
    [SerializeField] Transform target;
    public GameObject gameObject;

    float damage = 10f;

    bool isDead;
    bool isOnLeftSide;

    //Animator animator;

    public override void OnEpisodeBegin()
    {
        //Init();
        
        target = player.GetComponent<Transform>(); 
        Vector2 pos = gameObject.transform.position;

        target.position = new Vector2(pos.x + UnityEngine.Random.Range(-5f, 5f), pos.y + UnityEngine.Random.Range(-5f, 5f));
        transform.position = new Vector2(pos.x + UnityEngine.Random.Range(-7f, 7f), pos.y + UnityEngine.Random.Range(-7f, 7f));


    }


    /*private void Init()
    {
        if (!(GameManager.instance.boss == null))
        {
            bool isBossLookLeft = GameManager.instance.boss.isBossLookLeft;

            float bulletX = GameManager.instance.boss.transform.position.x;
            float bulletY = GameManager.instance.boss.transform.position.y;

            if (isBossLookLeft)
            {
                bulletX -= 2.5f;
            }
            else
            {
                bulletX += 2.5f;
            }

            bulletY -= 3f;

            transform.position = new Vector2(bulletX, bulletY);

            player = GameManager.instance.player;
            target = player.GetComponent<Transform>();

            //animator = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }*//*

    }*/

    private void FixedUpdate()
    {

        if (!isDead)
        {

            target = player.transform;

            Vector2 direction = new Vector2(transform.position.x - target.position.x, transform.position.y - target.position.y);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
            transform.rotation = rotation;

            isOnLeftSide = Mathf.Cos(angle * Mathf.Deg2Rad) < 0; // cos값이 -면 플레이어를 기준으로 왼쪽에 있는 것

            //spriteRenderer.flipY = isOnLeftSide;

   /*         if (aliveTimer > aliveTime)
            {
               // StartCoroutine(Dead());
            }
            aliveTimer += Time.fixedDeltaTime;*/
        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.localPosition);
        sensor.AddObservation(transform.localPosition);


    }

    [SerializeField] float speed;
    Vector2 nextMove;

    public override void OnActionReceived(ActionBuffers actions)
    {
        nextMove.x = actions.ContinuousActions[0];
        nextMove.y = actions.ContinuousActions[1];

        transform.Translate(nextMove * Time.deltaTime * speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == target)
        {
            SetReward(+1);
            Debug.Log("Success");
            EndEpisode();
        }
        else
        {
            SetReward(-1);
            Debug.Log("fail");
            EndEpisode();
        }
    }

   /* IEnumerator Dead()
    {
        //animator.SetTrigger("Hit");

        isDead = true;

        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<CapsuleCollider2D>().enabled = false;

        yield return new WaitForSeconds(0.35f); // 지정한 초 만큼 쉬기

        Destroy(gameObject);
    }
*/



}



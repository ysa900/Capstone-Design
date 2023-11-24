using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using static UnityEngine.GraphicsBuffer;

public class Boss_Bullet : Agent
{
    Transform target;
    Vector2 myVector, targetVector;
    float randomX, randomY;
    System.Random rand = new System.Random();

    private void Start()
    {
        target = GameManager.instance.player.GetComponent<Transform>();
        myVector=new Vector2(0.0f, 0.0f);
        targetVector = new Vector2(6.0f, 3.5f);
    }

    public override void OnEpisodeBegin()
    {
        
        randomX = rand.Next(-13, 13);
        randomY = rand.Next(-8, 8);
        // 투사체 랜덤하게 소환
        myVector = new Vector2(randomX, randomY);
        transform.position = myVector;

        randomX = rand.Next(-13, 13);
        randomY = rand.Next(-8, 8);
        // 타겟(플레이어) 랜덤하게 소환
        targetVector = new Vector2(randomX, randomY);
        target.position = targetVector;

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(target.position);
    }

    [SerializeField] float speed = 50;
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
            Debug.Log("collide player");
            EndEpisode();
        }
        else
        {
            SetReward(-1);
            Debug.Log("collide wall");
            EndEpisode();
        }
    }
}


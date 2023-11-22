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

    public override void OnEpisodeBegin()
    {
        transform.position = Vector2.zero;
        target=GameManager.instance.player.GetComponent<Transform>();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(target.position);
    }

    [SerializeField] float speed = 1;
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
            Debug.Log("S");
            EndEpisode();
        }
        else
        {
            SetReward(-1);
            EndEpisode();
        }
    }
}


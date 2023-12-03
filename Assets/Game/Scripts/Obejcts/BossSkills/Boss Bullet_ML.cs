using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using static UnityEngine.GraphicsBuffer;

public class Boss_Bullet_ML : Agent
{

    public Transform target;



    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector2.zero;
        target.localPosition = new Vector2(Random.Range(-8f, 8f), Random.Range(-8f, 8f));
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(target.localPosition);
        
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


}


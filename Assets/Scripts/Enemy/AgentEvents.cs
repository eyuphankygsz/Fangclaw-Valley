using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AgentEvents : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private UnityEvent _events;

    EnemyStateMachine _stateMachine;

    private void Awake()
    {
        _stateMachine = GetComponent<EnemyStateMachine>();
    }
    private void SetState(string stateName)
    {
        _stateMachine.SetCurrentState(stateName);
    }


    public void SetEvents(CustomEvents cEvents)
    {
        _events = cEvents.GetEvents();
    }
    public void SetStopDistance(float distance)
    {
        _agent.stoppingDistance = distance;
    }
    public void SetPosition(Transform tf)
    {
        transform.position = tf.position;
        transform.rotation = tf.rotation;
    }
    public void SetDestination()
    {
        _agent.SetDestination(_target.position);
        StartCoroutine(OnStopped());
    }

    private IEnumerator OnStopped()
    {
        while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance || _agent.velocity.sqrMagnitude > 0.1f)
        {
            Debug.Log("Test");
            yield return null;
        }

        _events?.Invoke();
    }

}

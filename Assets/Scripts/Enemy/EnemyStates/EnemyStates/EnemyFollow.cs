using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour, IEnemyState
{
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _enemy;

    [SerializeField]
    private EnemyStateTransitionList _transitions;
    [SerializeField]
    private TimeForExitFollow _timeForExitFollow;
    [SerializeField]
    private EnemyOpenDoor _openDoor;
    [SerializeField]
    private Animator _animator;
    public void EnterState()
    {
        _timeForExitFollow.ResetTime();
        _agent.speed = _speed;
        _animator.SetBool("Follow", true);
    }

    public void ExitState()
    {

    }

    public EnemyStateTransitionList GetTransitions()
    {
        return _transitions;
    }

    public void UpdateState()
    {
        if (_agent.velocity.magnitude < 0.1f)
            _openDoor.CheckDoors();

        if (_agent.remainingDistance <= _agent.stoppingDistance)
            TurnEnemy();

        _agent.SetDestination(_target.position);
    }

    private void TurnEnemy()
    {
        Vector3 lookAt = _target.position;
        lookAt.y = _enemy.position.y;

        _enemy.LookAt(lookAt);
    }
}

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
    StateTransitionList _transitions;

    public void EnterState()
	{
		_agent.speed = _speed;
	}

	public void ExitState()
	{
		throw new System.NotImplementedException();
	}

	public StateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{
		_agent.SetDestination(_target.position);
	}
}

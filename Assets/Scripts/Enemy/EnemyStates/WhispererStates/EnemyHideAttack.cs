using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Playables;

public class EnemyHideAttack : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private NavMeshAgent _agent;
	[SerializeField]
	private NavMeshAgent _animator;
	[SerializeField]
	private float _speed, _radius;

	[SerializeField]
	EnemyStateTransitionList _transitions;
	[SerializeField]
	private LayerMask _layer;

	[SerializeField]
	private UnityEvent _events;
	[SerializeField]
	private PlayerForce _force;
	[SerializeField]
	private NoStateLock _playerStateLock;
	[SerializeField]
	private Transform _enemy, _player;

	[SerializeField]
	private IsAnimationOver _isAnimOver;

	private bool _reached, _started;
	private Transform _destination;
	private Transform _forceTransform;
	private PlayableDirector _director;
	public void EnterState()
	{
		_isAnimOver.SetOver(false);

		_agent.stoppingDistance = 0;
		RaycastHit hit;
		Ray ray = new Ray(transform.position, _player.transform.position - transform.position);

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layer))
		{
			TakeHideTable table = hit.transform.GetComponent<TakeHideTable>();
			_destination = table.GetEnemyTransform();
			_forceTransform = table.GetPlayerTransform();
			_director = table.GetPlayable();

			_agent.SetDestination(table.GetEnemyTransform().position);
			_force.SetEvents(new OnLookEvents() { ForceEvents = _events });

		}

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
		if (_agent.remainingDistance <= _agent.stoppingDistance)
			_reached = true;

		if (_reached && !_started)
		{
			_started = true;
			_enemy.transform.rotation = _destination.rotation;
			_force.StartForce(_forceTransform);
		}
	}
	public void PlayDirector()
	{
		_director.Play();
	}
	public void UnFreezePlayer()
	{
		_playerStateLock.Lock = false;
	}

}

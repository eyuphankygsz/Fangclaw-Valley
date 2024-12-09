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
	[SerializeField]
	private HideOut _exitForHide;

	private bool _reached, _started;
	private Transform _destination;
	private Transform _forceTransform;
	private PlayableDirector _director;


	private Vector3 _pos;

	public void EnterState()
	{
		_exitForHide.SetHideAttack();
		_isAnimOver.SetOver(false);
		_agent.stoppingDistance = 0;
		_pos = _player.transform.position;


		Collider[] colls = Physics.OverlapSphere(_pos, 1, _layer);
		if (colls.Length > 0)
		{
			TakeHideTable table = colls[0].transform.GetComponent<TakeHideTable>();
			_destination = table.GetEnemyTransform();
			_forceTransform = table.GetPlayerTransform();
			_director = table.GetPlayable();

			_agent.SetDestination(table.GetEnemyTransform().position);


			OnLookEvents onLook = new GameObject().AddComponent<OnLookEvents>();
			onLook.gameObject.SetActive(false);
			onLook.ForceEvents = _events;

			_force.SetEvents(onLook);
		}

	}

	public void ExitState()
	{
		_isAnimOver.SetOver(true);
		_reached = false;
		_started = false;
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
		Debug.Log("UFP");
	}

	private void OnDrawGizmos()
	{
		if (_pos == null)
			return;

		Gizmos.DrawLine(transform.position, _pos);
		Gizmos.DrawWireSphere(_pos, 1);
	}
}

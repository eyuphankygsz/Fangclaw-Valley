using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SDaddySearch : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private NavMeshAgent _agent;
	[SerializeField]
	private float _searchRange;
	[SerializeField]
	private float _speed;
	[SerializeField]
	private const float _minDistance = 2f;
	private Vector3 _searchCenter;

	private bool _isWandering, _isSearchingNewTarget, _isSearching;

	[SerializeField]
	private TimeForSearch _timeForSearch;
	[SerializeField]
	private TimeForExitSearch _timeForExitSearch;
	[SerializeField]
	EnemyStateTransitionList _transitions;

	[SerializeField]
	private Animator _animator;


	[SerializeField]
	private AudioClip _sawYou;
	[SerializeField]
	CanPlayAudio _canPlayAudio;

	private bool _started;

	[SerializeField]
	private IEnemyController _controller;

	private void Awake()
	{
		_controller = GetComponentInParent<IEnemyController>();
	}
	public void EnterState()
	{
		if (_controller.IsOnChase)
		{
			_controller.SetChase(-1);
			_controller.IsOnChase = !_controller.IsOnChase;
		}
		if(_sawYou != null)
		_canPlayAudio.EnablePlay(_sawYou);

		_animator.SetBool("Walk", false);
		_searchCenter = _agent.destination;
		_timeForSearch.ResetTime();
		_timeForExitSearch.ResetTime();
		_agent.speed = _speed;
		FindNewWanderPoint(true);
	}

	public void ExitState()
	{
		_animator.SetBool("Search", false);
		_animator.SetBool("Walk", false);
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{
		if (_isSearching)
			if (_timeForSearch.CheckCondition())
				FindNewWanderPoint(false);

		if (_isSearchingNewTarget) return;
		CheckTarget();
	}

	private void CheckTarget()
	{
		if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
			if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
			{
				_isWandering = false;
				_animator.SetBool("Walk", false);
				_animator.SetBool("Search", true);
				_isSearching = true;
				_isSearchingNewTarget = true;
				_timeForSearch.ResetTime();
			}
	}
	private void FindNewWanderPoint(bool first)
	{
		_isSearching = false;
		_isSearchingNewTarget = true;
		_isWandering = false;

		if (!first)
			_searchCenter = transform.position;
		
		int i = 0;
		while (!_isWandering)
		{
			Vector3 randomPoint;
			do
			{
				i++;
				Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(_minDistance, _searchRange);
				randomPoint = _searchCenter + new Vector3(randomCircle.x, 0, randomCircle.y);
				if (i == 10)
					break;
			}
			while (Vector3.Distance(_searchCenter, randomPoint) < _minDistance);

			NavMeshHit hit;
			if (NavMesh.SamplePosition(randomPoint, out hit, _searchRange, NavMesh.AllAreas))
			{
				_isSearchingNewTarget = false;
				_animator.SetBool("Walk", true);
				_animator.SetBool("Search", false);
				_isWandering = true;
				_searchCenter = hit.position;
				_agent.SetDestination(hit.position);
			}

		}
	}
}

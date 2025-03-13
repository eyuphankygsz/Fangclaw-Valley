using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SDaddyEscape : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private Animator _animator;
	[SerializeField]
	private EnemyStateTransitionList _transitions;


	[SerializeField]
	private NavMeshAgent _agent;
	[SerializeField]
	private Transform[] _escapes;
	[SerializeField]
	private Transform _enemy;
	[SerializeField]
	private Transform _player;


	[SerializeField]
	private EnemyOpenDoor _openDoor;

	private Transform _selectedEscape;
	private float _stoppingDistance;

	[SerializeField]
	private Collider _collider;
	private bool _arrived, _escapeAnimationStarted;

	[SerializeField]
	private AudioSource _audioSource;

	[SerializeField]
	private float _speed;
	[SerializeField]
	private EnemyStateMachine _stateMachine;
	[SerializeField]
	private IsSpawned _isSpawned;

	private bool _entered, _animationPlayed, _spawnCheck;
	
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

		_arrived = _entered = _animationPlayed = _spawnCheck = false;
		_isSpawned.SetSpawned(false);
		_stoppingDistance = _agent.stoppingDistance;
		_agent.stoppingDistance = 0;
		_agent.speed = _speed;

		_animator.SetBool("RunAway", true);

		List<KeyValuePair<Transform, float>> escapePointsWithDistances = new List<KeyValuePair<Transform, float>>();

		for (int i = 0; i < _escapes.Length; i++)
		{
			float distance = Vector3.Distance(_escapes[i].position, transform.position);
			escapePointsWithDistances.Add(new KeyValuePair<Transform, float>(_escapes[i], distance));
		}
		escapePointsWithDistances.Sort((x, y) => x.Value.CompareTo(y.Value));


		foreach (var escapePoint in escapePointsWithDistances)
		{
			NavMeshPath path = new NavMeshPath();
			bool pathFound = _agent.CalculatePath(escapePoint.Key.position, path);

			// Check if the path is valid and complete
			if (pathFound && path.status == NavMeshPathStatus.PathComplete)
			{
				_agent.SetDestination(escapePoint.Key.position);
				_selectedEscape = escapePoint.Key;
				return;
			}
		}
		Debug.LogWarning("No valid path found to any escape point.");
	}

	public void ExitState()
	{
		_collider.enabled = true;
		_agent.stoppingDistance = _stoppingDistance;
		_controller.Stunned = false;
		_animator.SetBool("RunAway", false);
		_animator.SetBool("Escape", false);
		_animator.SetBool("Spawn", false);
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{
		_openDoor.CheckDoors();

		if (!_entered)
		{
			if (_arrived)
			{
				if (CheckExitAnimation("Escape"))
				{
					_entered = true;
					_animationPlayed = false;
				}
			}
			if (_agent.remainingDistance <= _agent.stoppingDistance && !_arrived)
				OnArrived();
		}
		else if (_entered)
		{
			_arrived = false;

			if (!_spawnCheck)
			{
				_spawnCheck = true;
				_agent.enabled = false;
				StartCoroutine(WaitForRespawn());
			}

			if (_spawnCheck)
			{
				if (CheckExitAnimation("Spawn") && !_isSpawned.CheckCondition())
				{
					_isSpawned.SetSpawned(true);
				}
			}



		}
	}
	private IEnumerator WaitForRespawn()
	{
		yield return new WaitForSeconds(Random.Range(5, 15));

		Transform closestPoint = null;
		Transform pathable = null;

		float minDistance = 0;

		foreach (var escapePoint in _escapes)
		{
			_agent.enabled = false;
			_enemy.transform.position = escapePoint.position;
			_agent.enabled = true;

			NavMeshPath path = new NavMeshPath();
			bool pathFound = _agent.CalculatePath(_player.position, path);

			if (closestPoint == null)
			{
				if (pathFound && path.status == NavMeshPathStatus.PathComplete)
					pathable = escapePoint;


				closestPoint = escapePoint;
				minDistance = Vector3.Distance(escapePoint.position, _player.position);
				continue;
			}
			else
			{
				float newDistance = Vector3.Distance(escapePoint.position, _player.position);
				if (newDistance < minDistance)
				{
					if (pathFound && path.status == NavMeshPathStatus.PathComplete)
						pathable = escapePoint;

					closestPoint = escapePoint;
					minDistance = newDistance;

				}
			}


		}
		_agent.enabled = false;
		_enemy.transform.position = pathable == null ? closestPoint.position : pathable.position;
		_agent.enabled = true;

		_animator.SetBool("Spawn", true);
		_animator.SetBool("Escape", false);
	}

	private void OnArrived()
	{
		_arrived = true;
		_collider.enabled = false;
		_agent.transform.rotation = _selectedEscape.rotation;
		_animator.SetBool("Escape", true);
		_animator.SetBool("RunAway", false);
		_audioSource.Stop();
	}

	private bool CheckExitAnimation(string animationName)
	{
		AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

		_animationPlayed = stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1f;

		return _animationPlayed;
	}
}







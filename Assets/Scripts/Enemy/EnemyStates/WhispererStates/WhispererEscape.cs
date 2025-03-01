using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class WhispererEscape : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private Animator _animator;
	[SerializeField]
	private EnemyStateTransitionList _transitions;
	[SerializeField]
	private WhispererController _controller;
	[SerializeField]
	private NavMeshAgent _agent;
	[SerializeField]
	private Transform[] _escapes;

	[SerializeField]
	private EnemyOpenDoor _openDoor;

	private Transform _selectedEscape;
	private float _stoppingDistance;

	[SerializeField]
	private Collider _collider;
	private bool _arrived, _escapeAnimationStarted;

	[SerializeField]
	private AudioSource _audioSource;
	
	public void EnterState()
	{
		_arrived = false;
		_stoppingDistance = _agent.stoppingDistance;
		_agent.stoppingDistance = 0;
		_agent.speed = 4;

		_animator.SetBool("Follow", true);

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
		_animator.SetBool("Follow", false);
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{
		_openDoor.CheckDoors();
		if (_agent.remainingDistance <= _agent.stoppingDistance && !_arrived)
		{
			Debug.Log("REMAINING DISTANCE: " + _agent.remainingDistance);
			Debug.Log("STOPPING DISTANCE: " + _agent.stoppingDistance);
			OnArrived();
		}
	}

	private void OnArrived()
	{
		_arrived = true;
		_collider.enabled = false;
		_agent.transform.rotation = _selectedEscape.rotation;
		_animator.SetBool("Escape", true);
		_animator.SetBool("Follow", false);
		_audioSource.Stop();
	}
}







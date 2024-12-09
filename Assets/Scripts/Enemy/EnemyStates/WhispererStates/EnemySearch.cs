using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySearch : MonoBehaviour, IEnemyState
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

	public void EnterState()
    {
        _canPlayAudio.EnablePlay(_sawYou);

		_animator.SetBool("Search", true);
		_searchCenter = _agent.destination;
        _timeForSearch.ResetTime();
        _timeForExitSearch.ResetTime();
        _agent.speed = _speed;
        FindNewWanderPoint();
    }

    public void ExitState()
	{
        Debug.Log("WHY");
		_animator.SetBool("Search", false);
	}

	public EnemyStateTransitionList GetTransitions()
    {
        return _transitions;
    }

    public void UpdateState()
    {
        if (_isSearching)
            if (_timeForSearch.CheckCondition())
                FindNewWanderPoint();

        if (_isSearchingNewTarget) return;
        CheckTarget();
    }

    private void CheckTarget()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
            {
                _isWandering = false; 
                _animator.SetBool("Follow", false);
				_animator.SetBool("Search", true);
				_isSearching = true;
                _isSearchingNewTarget = true;
                _timeForSearch.ResetTime();
            }
    }
    private void FindNewWanderPoint()
    {
        _isSearching = false;
        _isSearchingNewTarget = true;
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
                _animator.SetBool("Follow", true);
				_animator.SetBool("Search", false);
				_isWandering = true;
                _searchCenter = hit.position;
                _agent.SetDestination(hit.position);
            }

        }
    }
}

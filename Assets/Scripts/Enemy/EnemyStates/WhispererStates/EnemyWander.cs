using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWander : MonoBehaviour, IEnemyState
{
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private float _wanderRange;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private const float minDistance = 4f;
    private Vector3 _wanderCenter;

    private bool _isWandering, _isSearchingNewTarget, _isSearching;

    [SerializeField]
    private TimeForExitWander _timeBeforeWander;
    [SerializeField]
    private TimeForSearch _timeForSearch;
    [SerializeField]
    EnemyStateTransitionList _transitions;
    [SerializeField]
    private Animator _animator;


	[SerializeField]
	private AudioClip _sawYou;
	[SerializeField]
	CanPlayAudio _canPlayAudio;

	public void EnterState()
    {
        _canPlayAudio.EnablePlay(_sawYou);

        _timeBeforeWander.ResetTime();
        _agent.speed = _speed;
        _animator.SetBool("Follow", true);
        FindNewWanderPoint();
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
        if (_isSearching)
            if (_timeForSearch.CheckCondition())
            {
                Debug.Log("Checking Condition...");
                FindNewWanderPoint();
                _timeForSearch.ResetFrameFreeze();
            }


        if (_isSearchingNewTarget) return;
        CheckTarget();
    }

    private void CheckTarget()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
            {
                _animator.SetBool("Follow", false);
                _animator.SetBool("Search", true);
				_isWandering = false;
                _isSearching = true;
                _isSearchingNewTarget = true;
                _timeForSearch.ResetTime();
            }
    }
    private void FindNewWanderPoint()
    {
        _isSearching = false;
        _isSearchingNewTarget = true;
        _wanderCenter = transform.position;
        int i = 0;
        while (!_isWandering)
        {
            Vector3 randomPoint;
            do
            {
                i++;
                Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minDistance, _wanderRange);
                randomPoint = _wanderCenter + new Vector3(randomCircle.x, 0, randomCircle.y);
                if (i == 10)
                    break;
            }
            while (Vector3.Distance(_wanderCenter, randomPoint) < minDistance);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, _wanderRange, NavMesh.AllAreas))
            {
                _isSearchingNewTarget = false;
                _isWandering = true;
                _wanderCenter = hit.position;
                _agent.SetDestination(hit.position);
                _animator.SetBool("Follow", true);
                _animator.SetBool("Search", false);
				Debug.Log("Found");
            }

        }
    }
}
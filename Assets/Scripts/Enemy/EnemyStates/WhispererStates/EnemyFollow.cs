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
	private TimeForExitStuck _timeForExitStuck;
	[SerializeField]
	private IsStuck _isStuck;

	[SerializeField]
	private EnemyOpenDoor _openDoor;
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private AudioSource _audioSource;
	[SerializeField]
	private AudioClip _sawYou;
	[SerializeField]
	CanPlayAudio _canPlayAudio;


	private Vector3 _targetLastPos;

	private void Awake()
	{
		_canPlayAudio.AddAudio(_sawYou);
	}
	public void EnterState()
	{
		_timeForExitFollow.ResetTime();
		_agent.speed = _speed;
		_animator.SetBool("Follow", true);
		_timeForExitStuck.ResetTime();
		_isStuck.SetLastPoint(transform.position);
		if (_canPlayAudio.CanPlay(_sawYou))
		{
			_audioSource.clip = _sawYou;
			_audioSource.Play();
		}
	}

	public void ExitState()
	{
		_animator.SetBool("Follow", false);
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{
		if (_isStuck.GetStuck())
			_openDoor.CheckDoors();

		if (_agent.remainingDistance <= _agent.stoppingDistance)
			TurnEnemy();

		if (Vector3.Distance(_targetLastPos, _target.position) > 1f || !_agent.hasPath)
		{
			_targetLastPos = _target.position;
			_agent.SetDestination(_target.position);
		}
	}

	private void TurnEnemy()
	{
		Vector3 relativePos = _target.position - transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
		_enemy.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);

		//Vector3 lookAt = _target.position;
		//lookAt.y = _enemy.position.y;

		//_enemy.LookAt(lookAt);
	}
}
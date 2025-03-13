using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GPiggyFollow : MonoBehaviour, IEnemyState
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
	private TimeForExitStunFollow _timeForExitStunFollow;
	[SerializeField]
	private TimeForExitStuck _timeForExitStuck;
	[SerializeField]
	private IsStuck _isStuck;

	[SerializeField]
	private EnemyOpenDoor _openDoor;
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private AudioSource _generalAudioSource;
	[SerializeField]
	private AudioClip _sawYou;
	[SerializeField]
	CanPlayAudio _canPlayAudio;

	[SerializeField]
	private AudioSource _squekAudioSource;
	[SerializeField]
	private AudioClip[] _randomSqueks;
	private int _squekIndex;
	private Coroutine _squekRoutine;
	private WaitForSeconds _squekWait;
	private float _squekTimeLeft = 0;

	[SerializeField]
	private IEnemyController _controller;

	private void Awake()
	{
		_controller = GetComponentInParent<IEnemyController>();
		_canPlayAudio.AddAudio(_sawYou);
	}
	public void EnterState()
	{
		if (!_controller.IsOnChase)
		{
			_controller.SetChase(1);
			_controller.IsOnChase = !_controller.IsOnChase;
		}

		_timeForExitFollow.ResetTime();
		_timeForExitStunFollow.ResetTime();
		_timeForExitStuck.ResetTime();

		_agent.speed = _speed;
		_animator.SetBool("Follow", true);
		_isStuck.SetLastPoint(transform.position);

		_squekRoutine = StartCoroutine(TrySquek());

		if (_canPlayAudio.CanPlay(_sawYou))
		{
			_generalAudioSource.clip = _sawYou;
			_generalAudioSource.Play();
		}
	}

	public void ExitState()
	{
		if (_squekRoutine != null)
			StopCoroutine(_squekRoutine);

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

		Vector3 diff = _target.position - transform.position;
		if (diff.sqrMagnitude > _agent.stoppingDistance * _agent.stoppingDistance)
		{
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
	private IEnumerator TrySquek()
	{
		while (true)
		{

			if (_squekTimeLeft > 0)
			{
				_squekWait = new WaitForSeconds(_squekTimeLeft);
				_squekTimeLeft = 0;
				yield return _squekWait;
			}
			_squekAudioSource.clip = _randomSqueks[_squekIndex];
			_squekAudioSource.Play();
			_squekIndex = (_squekIndex + 1) % _randomSqueks.Length;
			yield return new WaitWhile(() => _generalAudioSource.isPlaying);

			_squekTimeLeft = Random.Range(0.6f, 1.5f);
			while(_squekTimeLeft > 0)
			{
				_squekTimeLeft -= Time.deltaTime;
				yield return null;
			}

		}
	}

}

using UnityEngine;
using UnityEngine.AI;

public class WhispererFollow : MonoBehaviour, IEnemyState
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
	private AudioSource _audioSource;
	[SerializeField]
	private AudioClip _sawYou;
	[SerializeField]
	CanPlayAudio _canPlayAudio;

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


		if (!CanReachTarget(_target.position))
		{
			Vector3 safePosition = FindClosestValidNavMeshPoint();
			_agent.SetDestination(safePosition);
			Debug.Log("Engel tespit edildi! Alternatif rota belirleniyor...");
		}
		else
		{
			_agent.SetDestination(_target.position);
		}
	}
	private bool CanReachTarget(Vector3 targetPosition)
	{
		NavMeshPath path = new NavMeshPath();
		_agent.CalculatePath(targetPosition, path);

		return path.status == NavMeshPathStatus.PathComplete;
	}
	private Vector3 FindClosestValidNavMeshPoint()
	{
		NavMeshHit hit;
		if (NavMesh.SamplePosition(_target.position, out hit, Mathf.Infinity, NavMesh.AllAreas))
		{
			return hit.position; // En yakýn geçerli NavMesh noktasýný döndür
		}

		return transform.position; // Eðer bir nokta bulunamazsa mevcut konumda kal
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

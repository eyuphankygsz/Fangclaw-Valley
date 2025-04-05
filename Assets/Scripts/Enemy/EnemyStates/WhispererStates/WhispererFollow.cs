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

	private NavMeshPath _path;
	private Vector3 _pos;

	private void Awake()
	{
		_controller = GetComponentInParent<IEnemyController>();
		_canPlayAudio.AddAudio(_sawYou);
	}
	private void Start()
	{
		_path = new NavMeshPath();
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
		_openDoor.CheckDoors();
		if (_agent.remainingDistance <= _agent.stoppingDistance)
			TurnEnemy();


		if (!CanReachTarget(_target.position))
		{
			Vector3 safePosition = FindClosestValidNavMeshPoint();
			_agent.SetDestination(safePosition);
			_pos = safePosition;
			Debug.Log("Engel tespit edildi! Alternatif rota belirleniyor...");
			Debug.Log("Pos: " + safePosition);
		}
		else
		{
			_agent.SetDestination(_target.position);
		}
	}
	private bool CanReachTarget(Vector3 targetPosition)
	{
		_agent.CalculatePath(targetPosition, _path);

		return _path.status == NavMeshPathStatus.PathComplete;
	}
	private Vector3 FindClosestValidNavMeshPoint()
	{
		NavMeshHit hit;
		if (NavMesh.SamplePosition(_target.position, out hit, 2, NavMesh.AllAreas))
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
	private void OnDrawGizmos()
	{
		if (_path != null && _path.corners.Length > 1)
		{
			Gizmos.color = Color.green; // Yolu çizmek için renk
			for (int i = 0; i < _path.corners.Length - 1; i++)
			{
				// Yoldaki her iki köþe arasýna çizgi çizin
				Gizmos.DrawLine(_path.corners[i], _path.corners[i + 1]);
			}
		}
		Gizmos.DrawWireSphere(_pos, 0.5f);

	}
}

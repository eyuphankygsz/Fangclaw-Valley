using UnityEngine;
using UnityEngine.AI;

public class GPiggyEscape : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private Animator _animator;
	[SerializeField]
	private EnemyStateTransitionList _transitions;

	[SerializeField]
	private NavMeshAgent _agent;
	[SerializeField]
	private Transform _enemy;
	[SerializeField]
	private Transform _player;


	[SerializeField]
	private EnemyOpenDoor _openDoor;

	private float _stoppingDistance;

	[SerializeField]
	private Collider _collider;
	private bool _disappeared;

	[SerializeField]
	private AudioSource _audioSource;

	[SerializeField]
	private float _speed;
	[SerializeField]
	private EnemyStateMachine _stateMachine;

	[SerializeField]
	private Renderer _renderer;
	private MaterialPropertyBlock _propBlock;

	[SerializeField]
	private IsAnimationOver _isAnimOver;

	[SerializeField]
	private AudioClip _disappearSfx;

	[SerializeField]
	private IEnemyController _controller;

	private void Awake()
	{
		_controller = GetComponentInParent<IEnemyController>();
		_propBlock = new MaterialPropertyBlock();

		_renderer.GetPropertyBlock(_propBlock);
		_propBlock.SetFloat("_Dissolve", 0);
		_renderer.SetPropertyBlock(_propBlock);
	}
	public void EnterState()
	{
		if (_controller.IsOnChase)
		{
			_controller.SetChase(-1);
			_controller.IsOnChase = !_controller.IsOnChase;
		}

		_isAnimOver.SetOver(false);
		_disappeared = false;
		_stoppingDistance = _agent.stoppingDistance;
		_agent.stoppingDistance = 0;
		_agent.speed = _speed;

		_audioSource.clip = _disappearSfx;
		_audioSource.Play();
		_animator.SetBool("Escape", true);
	}

	public void ExitState()
	{
		_propBlock.SetFloat("_Dissolve", 0);
		_renderer.SetPropertyBlock(_propBlock);

		_collider.enabled = true;
		_agent.stoppingDistance = _stoppingDistance;
		_controller.Stunned = false;
		_animator.SetBool("Escape", false);
		_audioSource.Stop();
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{
		if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Escape"))
		{
			float dissolveCount = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

			if (dissolveCount >= 1)
			{
				dissolveCount = 1;
				if (!_disappeared)
					Disappear();
			}

			_propBlock.SetFloat("_Dissolve", dissolveCount);
			_renderer.SetPropertyBlock(_propBlock);
		}

	}

	private void Disappear()
	{
		_disappeared = true;
		_isAnimOver.SetOver(true);
		_collider.enabled = false;
		_audioSource.Stop();
	}

}







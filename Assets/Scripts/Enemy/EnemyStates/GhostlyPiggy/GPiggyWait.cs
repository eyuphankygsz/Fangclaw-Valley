using UnityEngine;
using UnityEngine.AI;

public class GPiggyWait : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private Animator _animator;
	[SerializeField]
	private IEnemyController _controller;
	[SerializeField]
	private NavMeshAgent _agent;

	[SerializeField]
	private Collider _collider;
	[SerializeField]
	private EnemyStateTransitionList _transitions;

	[SerializeField]
	private Renderer _renderer;
	private MaterialPropertyBlock _propBlock;

	[SerializeField]
	private AudioSource _audioSource;
	[SerializeField]
	private GhostlyPigSpawner _spawner;
	[SerializeField]
	private GameObject _piggy;

	[SerializeField]
	private AudioSource _sfx;

	private void Awake()
	{
		_propBlock = new MaterialPropertyBlock();
		_controller = GetComponentInParent<IEnemyController>();
	}
	public void EnterState()
	{
		(_controller as GhostlyPiggyController).CurrentShineTimer = 0;
		_sfx.Stop();
		_spawner.SetOccupied(_piggy);
		_collider.enabled = false;
		_controller.Stunned = true;

		_renderer.GetPropertyBlock(_propBlock);
		_propBlock.SetFloat("_Dissolve", 1);
		_renderer.SetPropertyBlock(_propBlock);
		_audioSource.Stop();
	}

	public void ExitState()
	{
		_collider.enabled = true;
		_controller.Stunned = false;

		_propBlock.SetFloat("_Dissolve", 0);
		_renderer.SetPropertyBlock(_propBlock);
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{
	}
}







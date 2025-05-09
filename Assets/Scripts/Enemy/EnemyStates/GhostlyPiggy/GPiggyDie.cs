using UnityEngine;
using Zenject;

public class GPiggyDie : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private Animator _animator;
	[SerializeField]
	private AudioSource _src;
	[SerializeField]
	private AudioClip _clip;


	[SerializeField]
	private Collider _collider;
	[Inject]
	private GameManager _manager;


	public void EnterState()
	{
		_collider.enabled = false;
		_manager.IsOnChase--;
		_animator.SetBool("Die", true);

	}

	public void ExitState()
	{
		_animator.SetBool("Die", false);
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return null;
	}

	public void UpdateState()
	{

	}
}

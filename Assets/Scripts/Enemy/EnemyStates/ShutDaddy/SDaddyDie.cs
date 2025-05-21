using UnityEngine;
using UnityEngine.Events;

public class SDaddyDie : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private Animator _animator;
	[SerializeField]
	private Collider _collider;
	[SerializeField]
	private UnityEvent _onDeathEvents;


	public void EnterState()
	{

		_collider.enabled = false;
		_animator.SetBool("Die", true);
		_onDeathEvents?.Invoke();
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







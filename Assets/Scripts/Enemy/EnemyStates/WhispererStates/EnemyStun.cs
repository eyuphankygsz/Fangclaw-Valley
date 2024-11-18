using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStun : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private Animator _animator;
	[SerializeField]
	private EnemyStateTransitionList _transitions;
	[SerializeField]
	private WhispererController _controller;

	public void EnterState()
	{
		_animator.SetBool("Stun", true);
	}

	public void ExitState()
	{
		_controller.Stunned = false;
		_animator.SetBool("Stun", false);
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDaddyAttack : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private const float _minDistance = 2f;

	private bool _startChecking;

	[SerializeField]
	private IsAnimationOver _isAnimOver;
	[SerializeField]
	EnemyStateTransitionList _transitions;
	[SerializeField]
	private TimeForAttack _timeForAttack;
	[SerializeField]
	private IsTurnedToTarget _turned;

	[SerializeField]
	private IEnemyController _controller;

	private void Awake()
	{
		_controller = GetComponentInParent<IEnemyController>();
	}

	public void EnterState()
	{
		_timeForAttack.ResetTime();
		_isAnimOver.SetOver(false);
		_animator.SetTrigger("AttackNormal");
	}

	public void ExitState()
	{
		_startChecking = false;
		_turned.CanTurn = false;
		_animator.ResetTrigger("AttackNormal");
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{
		if (_startChecking && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
			_turned.CanTurn = true;

		if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("AttackNormal"))
			_startChecking = true;
	}
}

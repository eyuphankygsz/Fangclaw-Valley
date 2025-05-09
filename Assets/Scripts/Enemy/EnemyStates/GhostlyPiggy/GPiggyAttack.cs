using UnityEngine;

public class GPiggyAttack : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private Animator _animator;

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
	private AudioSource _audioSource; 
	[SerializeField]
	private AudioClip _attackSFX;

	[SerializeField]
	private IEnemyController _controller;

	private const string AttackAnimationState = "AttackNormal"; // Constant for animation state

	private void Awake()
	{
		_controller = GetComponentInParent<IEnemyController>();
	}

	public void EnterState()
	{
		_startChecking = false;
		_audioSource.clip = _attackSFX;
		_audioSource?.Play();
		_timeForAttack.ResetTime();
		_isAnimOver.SetOver(false);
		_animator?.SetTrigger(AttackAnimationState); // Use constant here
	}

	public void ExitState()
	{
		_startChecking = false;
		_turned.CanTurn = false;
		_animator?.ResetTrigger(AttackAnimationState); // Use constant here
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{
		if (_startChecking && _animator?.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
			_turned.CanTurn = true;

		if (_animator != null && !_animator.GetCurrentAnimatorStateInfo(0).IsName(AttackAnimationState)) // Use constant here
			_startChecking = true;
		else if (_animator == null)
		{
			Debug.LogError("Animator is not assigned in " + gameObject.name); // Error handling
		}
	}
}

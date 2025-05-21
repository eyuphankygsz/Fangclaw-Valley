using UnityEngine;

public class GPiggyAttack : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private Animator _animator;

	private bool _startChecking, _startCheckCheck;

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
		_startCheckCheck = true;
		_startChecking = false;
		_audioSource.clip = _attackSFX;
		_audioSource?.Play();
		_timeForAttack.ResetTime();
		_isAnimOver.SetOver(false);
		
		if(!_animator.GetCurrentAnimatorStateInfo(0).IsName(AttackAnimationState))
		    _animator.SetTrigger(AttackAnimationState);
		else{
			_animator.Play(AttackAnimationState,0,0);
		}
		_controller.StartAnimationCheck(AttackAnimationState);
	}

	public void ExitState()
	{
		_startCheckCheck = false;
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
		if (_startChecking && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= _animator.GetCurrentAnimatorStateInfo(0).length)
		{	
			_turned.CanTurn = true;
		}
		else if(_startCheckCheck && _isAnimOver.CheckCondition()){
			_turned.CanTurn = true;
		}
		else if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(AttackAnimationState) && !_startChecking){
			_startCheckCheck = false;
			_startChecking = true;
		}

	}
}

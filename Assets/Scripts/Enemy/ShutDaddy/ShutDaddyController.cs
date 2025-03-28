using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class ShutDaddyController : MonoBehaviour, IEnemyController
{
	[SerializeField]
	private Transform _player;
	[SerializeField]
	private CanSeePlayer _canSee;
	[SerializeField]
	private NavMeshAgent _agent;
	[SerializeField]
	private IsAnimationOver _animOver;
	[SerializeField]
	private EnemyStateMachine _machine;
	[SerializeField]
	private MonoBehaviour _startState, _escapeState, _followState, _attackState;


	private EnemyAttackController _enemyAttackController;

	public bool Stop;
	public bool Stunned { get; set; }
	public bool DiscardTime { get; set; }
	public bool IsOnChase { get; set; }
	public EnemyStateMachine StateMachine { get; set; }

	[Inject]
	private GameManager _gameManager;

	private void Awake()
	{
		_enemyAttackController = GetComponent<EnemyAttackController>();
		StateMachine = GetComponent<EnemyStateMachine>();
	}
	void Start()
	{
		_machine.SetCurrentState(_startState as IEnemyState);
	}

	// Update is called once per frame
	void Update()
	{
		_canSee.SendRays();
		_enemyAttackController.TryAttack();
	}

	public void SetPosition(Transform tf)
	{
		transform.position = tf.position;
		transform.rotation = tf.rotation;
	}
	public void FollowPlayer()
	{
		_agent.SetDestination(_player.position);

	}
	public void EndAnimation()
	{
		_animOver.SetOver(true);
	}
	public void SearchPlayer()
	{

	}
	public void Shined()
	{
		if (Stunned) return;
		_agent.ResetPath();
		_agent.destination = transform.position;
		_machine.SetCurrentState(_escapeState as IEnemyState);
		Stunned = true;
	}
	public void StopShined()
	{
	}
	public void Wander()
	{

	}
	private bool IsPlayerFar()
	{
		if (Vector3.Distance(transform.position, _player.position) > 20)
			return true;
		return false;
	}
	public bool IsShined()
	{
		return false;
	}

	public void SetChase(int chase)
	{
		_gameManager.IsOnChase += chase;
	}


	private Coroutine _animationCheckRoutine;
	private AnimatorStateInfo _animatorStateInfo;
	private Animator _animator;
	private WaitForSeconds _wfs = new WaitForSeconds(0.1f);

	public void StartAnimationCheck(string name)
	{
		_animOver.SetOver(false);
		if (_animationCheckRoutine != null)
			StopCoroutine(_animationCheckRoutine);
		_animationCheckRoutine = StartCoroutine(CheckAnimation(name));
	}
	private IEnumerator CheckAnimation(string name)
	{
		_animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		if (!_animatorStateInfo.IsName(name))
			yield return _wfs;


		Debug.Log(_animator.GetCurrentAnimatorStateInfo(0).IsName(name));

		while (true)
		{
			_animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
			if (!_animatorStateInfo.IsName(name))
			{
				_animOver.SetOver(true);
				break;
			}
			else if (_animatorStateInfo.normalizedTime >= _animatorStateInfo.length)
			{
				_animOver.SetOver(true);
				break;
			}
			yield return null;
		}
	}

}
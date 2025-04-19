using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class GhostlyPiggyController : MonoBehaviour, IEnemyController
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
	private MonoBehaviour _startState, _escapeState, _followState;


	public bool Stop;
	public bool Stunned { get; set; }
	public bool DiscardTime { get; set; }
	public bool IsOnChase { get; set; }


	private EnemyAttackController _enemyAttackController;
	public float CurrentShineTimer;
	private float _decreaseSpeed = -1f, _increaseSpeed = 0.6f;
	private bool _shining;

	public EnemyStateMachine StateMachine { get; set; }

	[Inject]
	private GameManager _gameManager;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
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

		ShineTimer(_shining);

	}

	public void SetPosition(Transform tf)
	{
		_agent.enabled = false;
	
		transform.position = tf.position;
		transform.rotation = tf.rotation;
		
		_agent.enabled = true;
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
		_shining = true;
	}
	public void StopShined()
	{
		_shining = false;
	}
	public void ShineTimer(bool increase)
	{
		CurrentShineTimer += (increase ? _increaseSpeed : _decreaseSpeed) * Time.deltaTime;
		if (CurrentShineTimer >= 1)
		{
			CurrentShineTimer = 1;
			TryEscape();
		}
		else if (CurrentShineTimer <= 0)
		{
			CurrentShineTimer = 0;
		}
	}
	private void TryEscape()
	{
		if (Stunned) return;
		_agent.ResetPath();
		_agent.destination = transform.position;
		_machine.SetCurrentState(_escapeState as IEnemyState);
		Stunned = true;
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
		return _shining;
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
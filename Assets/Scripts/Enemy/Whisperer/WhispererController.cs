using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WhispererController : MonoBehaviour
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
	private MonoBehaviour _startState, _stunState, _escapeState, _followState, _attackState, _attackHState;
	[SerializeField]
	private TimeForExitStun _stunTime;
	[SerializeField]
	private IsTimeArrived _isTimeArrived;

	public bool Stop;
	public bool Stunned;
	public bool DiscardTime;

	void Start()
	{
		_machine.SetCurrentState(_startState as IEnemyState);
	}

	// Update is called once per frame
	void Update()
	{
		_canSee.SendRays();
		CheckTime();
	}

	public void SetPosition(Transform tf)
	{
		transform.position = tf.position;
		transform.rotation = tf.rotation;
	}
	public void CheckTime()
	{
		if (!_isTimeArrived.CheckCondition() && !DiscardTime)
			if(CanEscapeStates())
			_machine.SetCurrentState("Escape");
	}
	private bool CanEscapeStates()
	{
		IEnemyState state = _machine.GetCurrentState();
		if(state != _followState as IEnemyState 
			&& state != _stunState as IEnemyState
			&& state != _attackState as IEnemyState
			&& state != _escapeState as IEnemyState
			&& state != _attackHState as IEnemyState)
			return true;

		return false;
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
		_stunTime.ResetTime();
		if (Stunned) return;
		_machine.SetCurrentState(_stunState as IEnemyState);
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
}
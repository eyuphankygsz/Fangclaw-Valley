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
	private MonoBehaviour _startState, _stunState;
	[SerializeField]
	private TimeForExitStun _stunTime;



	public bool Stop;
	public bool Stunned;


	void Start()
	{
		_machine.SetCurrentState(_startState as IEnemyState);
	}

	// Update is called once per frame
	void Update()
	{
		_canSee.SendRays();
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
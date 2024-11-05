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

	private WhispererStates _state;
	public bool Stop;


	void Start()
	{

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
	public void SearchPlayer()
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
}

public enum WhispererStates
{
	Wander,
	Follow,
	Search
}
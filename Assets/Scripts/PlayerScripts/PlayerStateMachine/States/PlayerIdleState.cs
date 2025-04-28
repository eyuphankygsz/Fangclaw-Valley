using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerIdleState : MonoBehaviour, IPlayerState
{
	[SerializeField]
	private CharacterController _controller;
	private ControlSchema _control;

	[SerializeField]
	private PlayerGravity _gravity;

	[SerializeField]
	StateTransitionList _transitions;

	public void EnterState()
	{

	}

	public void UpdateState()
	{
		Vector3 move = Vector3.zero;
		move.y = _gravity.CalculateGravity() * Time.deltaTime;
		_controller.Move(move);
	}

	public void ExitState()
	{
	}


	public StateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void OnInputEnable(ControlSchema schema)
	{
		_control = schema;
	}
}

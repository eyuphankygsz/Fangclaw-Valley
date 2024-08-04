using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : MonoBehaviour, IPlayerState
{
	private bool _gravityOff { get; set; }

	private CharacterController _controller;
	private float _gravity = -9.8f;


	[SerializeField] private float _gravityMultiplier = 1;

	[SerializeField] private float _baseSpeed, _speedBoost;
	private float _speed;

	[SerializeField] private float _maxStamina;
	private float _stamina;
	[SerializeField] private bool _running;
	private Coroutine _runningCoroutine;

	private ControlSchema _controls;
	private Vector2 _movementInput;
	public void EnterState()
	{
		
	}

	public void ExitState()
	{
		throw new System.NotImplementedException();
	}

	public void UpdateState()
	{
		throw new System.NotImplementedException();
	}
}

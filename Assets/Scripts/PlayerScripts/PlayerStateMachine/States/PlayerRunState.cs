using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunState : MonoBehaviour, IPlayerState, IInputHandler
{
	[SerializeField]
	private CharacterController _controller;

	[SerializeField]
	private float _speed;
	[SerializeField]
	PlayerStamina _playerStamina;
	[SerializeField]
	PlayerGravity _gravity;

	[SerializeField]
	private StateTransitionList _transitionList;



	private Vector2 _movementInput;

	private ControlSchema _controls;

	private Coroutine _runningCoroutine;
	private bool _running;


	public void EnterState() { }
	public void UpdateState()
	{
		ApplyMovement();
	}
	public void ExitState()
	{
		if (_runningCoroutine != null)
			StopCoroutine(_runningCoroutine);
		_runningCoroutine = StartCoroutine(_playerStamina.IncreaseStamina());
		_running = false;

		OnInputDisable();
	}

	private void ApplyMovement()
	{
		var movement = (transform.forward * _movementInput.y + transform.right * _movementInput.x) * _speed;
		movement.y = _gravity.CalculateGravity();
		_controller.Move(movement * Time.deltaTime);
	}


	public StateTransitionList GetTransitions()
	{
		return _transitionList;
	}
	public void OnInputEnable(ControlSchema schema)
	{
		_controls = schema;
		_movementInput = schema.Player.Movement.ReadValue<Vector2>();
		OnRunPerformed(new InputAction.CallbackContext());
		_controls.Player.Movement.performed += OnMovePerformed;
		_controls.Player.Movement.canceled += OnMoveCanceled;

		_controls.Player.Run.performed += OnRunPerformed;
		_controls.Player.Run.canceled += OnRunCanceled;
	}

	public void OnInputDisable()
	{
		_controls.Player.Movement.performed -= OnMovePerformed;
		_controls.Player.Movement.canceled -= OnMoveCanceled;

		_controls.Player.Run.performed -= OnRunPerformed;
		_controls.Player.Run.canceled -= OnRunCanceled;
	}


	private void OnMovePerformed(InputAction.CallbackContext context)
		=> _movementInput = context.ReadValue<Vector2>();

	private void OnMoveCanceled(InputAction.CallbackContext context)
		=> _movementInput = Vector2.zero;
	private void OnRunPerformed(InputAction.CallbackContext context)
	{
		if (_running) return;
		StartRunRoutine(true);
	}

	private void OnRunCanceled(InputAction.CallbackContext context)
	{
		if (!_running) return;
		StartRunRoutine(false);
	}
	private void StartRunRoutine(bool run)
	{
		_running = run;
		if (_runningCoroutine != null)
			StopCoroutine(_runningCoroutine);

		_runningCoroutine = StartCoroutine(run ? _playerStamina.DecreaseStamina()
									 : _playerStamina.IncreaseStamina());
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJumpState : MonoBehaviour, IPlayerState, IInputHandler
{
	[SerializeField]
	private CharacterController _controller;

	[SerializeField]
	private float _speed;
	[SerializeField]
    private PlayerGravity _gravity;
	

	[SerializeField]
	private StateTransitionList _transitionList;

	private Vector2 _movementInput;

	private ControlSchema _controls;



	#region StateHandle
	public void EnterState()
	{ 

	}
	public void UpdateState()
	{
		ApplyMovement();
	}
	public void ExitState()
	{
		OnInputDisable();
	}
	#endregion
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
	#region InputHandle
	public void OnInputEnable(ControlSchema schema)
	{
		_controls = schema;
		_movementInput = schema.Player.Movement.ReadValue<Vector2>();
        OnJumpPerformed(new InputAction.CallbackContext());
		_controls.Player.Movement.performed += OnMovePerformed;
		_controls.Player.Movement.canceled += OnMoveCanceled;

		_controls.Player.Jump.performed += OnJumpPerformed;
		_controls.Player.Jump.canceled += OnJumpCanceled;
	}

	public void OnInputDisable()
	{
		_controls.Player.Movement.performed -= OnMovePerformed;
		_controls.Player.Movement.canceled -= OnMoveCanceled;

		_controls.Player.Jump.performed -= OnJumpPerformed;
		_controls.Player.Jump.canceled -= OnJumpCanceled;
	}


	private void OnMovePerformed(InputAction.CallbackContext context)
		=> _movementInput = context.ReadValue<Vector2>();

	private void OnMoveCanceled(InputAction.CallbackContext context)
		=> _movementInput = Vector2.zero;
	private void OnJumpPerformed(InputAction.CallbackContext context)
	{
		return;
	}

	private void OnJumpCanceled(InputAction.CallbackContext context)
	{
		return;
	}
	#endregion
}

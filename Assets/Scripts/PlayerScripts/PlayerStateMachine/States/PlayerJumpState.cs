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
	private PlayerGroundCheck _groundCheck;

	[SerializeField]
	private StateTransitionList _transitionList;

	private Vector2 _movementInput;

	private ControlSchema _controls;

	private float _jumpSpeed = 1.2f;
	private float _jumpVelocity;

	#region StateHandle
	public void EnterState()
	{
		_groundCheck.CantCheckGround = true;
		_jumpVelocity = Mathf.Sqrt(_jumpSpeed * -1 * _gravity.CalculateGravity());
	}
	public void UpdateState()
	{
		ApplyMovement();
	}
	public void ExitState()
	{
		_groundCheck.CantCheckGround = false;
		OnInputDisable();
	}
	#endregion
	private void ApplyMovement()
	{
		_jumpVelocity += _gravity.CalculateGravity() * Time.deltaTime;
		var movement = (transform.forward * _movementInput.y + transform.right * _movementInput.x) * _speed;
		movement.y = _jumpVelocity;

		_controller.Move(movement * Time.deltaTime);
		_groundCheck.CantCheckGround = false;
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

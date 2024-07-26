using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IInputHandler
{
	public bool GravityOff { get; set; }

	private CharacterController _controller;
	private float _gravity = -9.8f;


	[SerializeField] private float _gravityMultiplier = 1;
	[SerializeField] private float _speed;

	private ControlSchema _controls;
	private Vector2 _movementInput;




	void Awake()
	{
		_controller = GetComponent<CharacterController>();
	}
	public void OnInputEnable(ControlSchema schema)
	{
		Debug.Log("Controls Setting...");
		_controls = schema;
		_controls.Player.Movement.performed += OnMovePerformed;
		_controls.Player.Movement.canceled += OnMoveCanceled;
	}

	public void OnInputDisable()
	{
		_controls.Player.Movement.performed -= OnMovePerformed;
		_controls.Player.Movement.canceled -= OnMoveCanceled;
	}

	private void OnMovePerformed(InputAction.CallbackContext context) => _movementInput = context.ReadValue<Vector2>();
	private void OnMoveCanceled(InputAction.CallbackContext context) => _movementInput = Vector2.zero;
	public void ManageMove()
	{
		ApplyMovement();
	}
	private float CalculateGravity()
	{
		return GravityOff ? 0 : _gravity * _gravityMultiplier;
	}
	private void ApplyMovement()
	{
		Vector3 movement = (transform.forward * _movementInput.y) + (transform.right * _movementInput.x);
		movement *= _speed;
		movement.y = CalculateGravity();
		_controller.Move(movement * Time.deltaTime);
	}

}

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

	[SerializeField] private float _baseSpeed, _speedBoost;
	[SerializeField] private float _speed;

	[SerializeField] private float _maxStamina;
	[SerializeField] private float _stamina;
	[SerializeField] private bool _running;
	private Coroutine _runningCoroutine;

	private ControlSchema _controls;
	private Vector2 _movementInput;


	void Awake()
	{
		_controller = GetComponent<CharacterController>();
		_stamina = _maxStamina;
		_speed = _baseSpeed;
	}
	public void OnInputEnable(ControlSchema schema)
	{
		_controls = schema;
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

	private void OnMovePerformed(InputAction.CallbackContext context) => _movementInput = context.ReadValue<Vector2>();
	private void OnMoveCanceled(InputAction.CallbackContext context) => _movementInput = Vector2.zero;

	private void OnRunPerformed(InputAction.CallbackContext context)
	{
		if (_running || _stamina <= 0.6f) return;
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

		if (run)
		{
			_speed = _baseSpeed + _speedBoost;
			_runningCoroutine = StartCoroutine(DecreaseStamina());
		}
		else
		{
			_speed = _baseSpeed;
			_runningCoroutine = StartCoroutine(IncreaseStamina());
		}
	}

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

	private IEnumerator DecreaseStamina()
	{
		while (_stamina > 0)
		{
			_stamina -= Time.deltaTime;
			if (_stamina < 0)
			{
				_stamina = 0;
				StartRunRoutine(run: false);
			}
			yield return null;
		}
	}

	private IEnumerator IncreaseStamina()
	{
		while (_stamina < _maxStamina)
		{
			_stamina += Time.deltaTime;
			if (_stamina > _maxStamina)
			{
				_stamina = _maxStamina;
				StopCoroutine(_runningCoroutine);
			}
			yield return null; // Wait until the next frame
		}
	}
}

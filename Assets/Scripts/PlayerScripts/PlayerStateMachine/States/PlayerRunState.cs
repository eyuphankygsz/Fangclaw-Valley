using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunState : MonoBehaviour, IPlayerState, IInputHandler
{
	[SerializeField]
	private CharacterController _controller;

	[SerializeField]
	private float _speed;
	[SerializeField]
	private PlayerStamina _playerStamina;
	[SerializeField]
	private PlayerGravity _gravity;

	[SerializeField]
	private StateTransitionList _transitionList;

	[SerializeField]
	private PlayerStep _step;
	[SerializeField]
	private float _volume;
	[SerializeField]
	private float _stepTime, _stepOffset;


	private Vector2 _movementInput;

	private ControlSchema _controls;

	private bool _running;


	public void EnterState() { _step.Setup(_stepTime, _stepOffset, _volume); }
	public void UpdateState()
	{
		ApplyMovement();
		_step.Step();

	}
	public void ExitState()
	{
		_playerStamina.ChangeStamina(increase: true);
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
		_playerStamina.ChangeStamina(!run);
	}
}

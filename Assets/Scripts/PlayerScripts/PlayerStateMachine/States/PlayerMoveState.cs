using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerMoveState : MonoBehaviour, IPlayerState, IInputHandler
{
	[SerializeField]
	private CharacterController _controller;

	[SerializeField] private float _speed;

	private ControlSchema _controls;
	private Vector2 _movementInput;

	[SerializeField]
	private PlayerGravity _gravity;

	[Inject]
	private PlayerUI _playerUI;

	[SerializeField]
	StateTransitionList _transitions;

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

	public void OnInputEnable(ControlSchema schema)
	{
		_controls = schema; 
		_movementInput = schema.Player.Movement.ReadValue<Vector2>();
		
		_controls.Player.Movement.performed += OnMovePerformed;
		_controls.Player.Movement.canceled += OnMoveCanceled;
	}

	public void OnInputDisable()
	{
		_controls.Player.Movement.performed -= OnMovePerformed;
		_controls.Player.Movement.canceled -= OnMoveCanceled;
	}

	private void OnMovePerformed(InputAction.CallbackContext context)
	{
		_movementInput = context.ReadValue<Vector2>();
	}

	private void OnMoveCanceled(InputAction.CallbackContext context)
	{
		_movementInput = Vector2.zero;
	}

	private void ApplyMovement()
	{
		var movement = (transform.forward * _movementInput.y + transform.right * _movementInput.x) * _speed;
		movement.y = _gravity.CalculateGravity();
		_controller.Move(movement * Time.deltaTime);
	}

	public StateTransitionList GetTransitions()
	{
		return _transitions;
	}



	//private void HandleUI()
	//{
	//	_playerUI.ChangeStaminaBar(_stamina);
	//}

	//public List<ICondition> GetTransitions()
	//{
	//	return _transitionList;
	//}
}

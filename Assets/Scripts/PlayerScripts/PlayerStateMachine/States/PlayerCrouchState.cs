using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouchState : MonoBehaviour, IPlayerState, IInputHandler
{
	[SerializeField]
	private CharacterController _controller;
	[SerializeField]
	private float _crouchHeight, _standSpeed;
	[SerializeField]
	private float _offsetYCenter;

	private float _normalHeight;

	[SerializeField]
	private float _speed, _crouchSpeed = 1;

	[SerializeField]
	private PlayerGravity _gravity;
	[SerializeField]
	private PlayerCrouch _crouchHelper;
	[SerializeField]
	private PlayerHide _hideHelper;
	[SerializeField]
	private NoStateLock _stateLock;

	[SerializeField]
	private StateTransitionList _transitionList;

	private Vector2 _movementInput;

	private ControlSchema _controls;

	private Coroutine _crouchRoutine;
	private bool _crouching, _pressed;

	public bool Crouching { get { return _crouching; } }
	public bool Crouched { get; private set; }

	private void Awake()
	{
		_normalHeight = _controller.height;
	}

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

		_hideHelper.CheckHide();

		if (!_pressed)
		{
			StartRunRoutine(_crouching);
			_pressed = true;
		}
	}



	public StateTransitionList GetTransitions()
	{
		return _transitionList;
	}

	#region CrouchHandle
	private void StartRunRoutine(bool run)
	{
		_crouching = run;
		if (_crouchRoutine != null)
			StopCoroutine(_crouchRoutine);

		_crouchRoutine = StartCoroutine(run ? Crouch() : UnCrouch());
	}

	private IEnumerator Crouch()
	{
		while (_controller.height > _crouchHeight)
		{
			if (_stateLock.Lock)
				yield return null;

			_controller.height -= Time.deltaTime * _crouchSpeed;
			_controller.center += Vector3.up * Time.deltaTime * 1;

			if (_controller.center.y > _offsetYCenter)
				_controller.center = new Vector3(0, _offsetYCenter, 0);

			if (_controller.height < _crouchHeight)
			{
				_controller.height = _crouchHeight;
				Crouched = true;
			}

			yield return null;
		}
	}
	private IEnumerator UnCrouch()
	{
		while (_controller.height < _normalHeight)
		{
			if (_stateLock.Lock)
				yield return null;

			if (_crouchHelper.CanGetUp())
			{
				_controller.height += Time.deltaTime * _crouchSpeed;
				_controller.center -= Vector3.up * Time.deltaTime * _crouchSpeed;

				if (_controller.center.y < 0)
					_controller.center = new Vector3(0, 0, 0);

				Vector3 moveUp = Vector3.zero;
				moveUp.y = _standSpeed * Time.deltaTime;
				_controller.Move(moveUp);
				if (_controller.height > _normalHeight)
				{
					_controller.height = _normalHeight;
					Crouched = false;
				}

			}

			yield return null;
		}
	}
	#endregion
	#region InputHandle
	public void OnInputEnable(ControlSchema schema)
	{
		_controls = schema;
		_movementInput = schema.Player.Movement.ReadValue<Vector2>();
		_crouching = true; 
		_pressed = false;

		_controls.Player.Movement.performed += OnMovePerformed;
		_controls.Player.Movement.canceled += OnMoveCanceled;

		_controls.Player.Crouch.performed += OnCrouchPerformed;
		_controls.Player.Crouch.canceled += OnCrouchCanceled;
	}

	public void OnInputDisable()
	{
		
		_controls.Player.Movement.performed -= OnMovePerformed;
		_controls.Player.Movement.canceled -= OnMoveCanceled;

		_controls.Player.Crouch.performed -= OnCrouchPerformed;
		_controls.Player.Crouch.canceled -= OnCrouchCanceled;
	}


	private void OnMovePerformed(InputAction.CallbackContext context)
		=> _movementInput = context.ReadValue<Vector2>();

	private void OnMoveCanceled(InputAction.CallbackContext context)
		=> _movementInput = Vector2.zero;
	private void OnCrouchPerformed(InputAction.CallbackContext context)
	{
		Debug.Log("PERFORMED");
		if (_crouching) return;
		_pressed = false;
		_crouching = true;
	}

	private void OnCrouchCanceled(InputAction.CallbackContext context)
	{
		Debug.Log("CANCELED");
		if (!_crouching) return;
		_pressed = false;
		_crouching = false;
	}
	#endregion
}

using UnityEngine;
using UnityEngine.InputSystem;

public class MouseDirection : MonoBehaviour, IInputHandler
{
    public static MouseDirection Instance { get; private set; }

    private ControlSchema _controls;
    private Vector2 _rotation;
    private void Awake()
    {
            Instance = this;
    }
    public Vector2 GetCameraDirection()
    {
        return _rotation;
    }

	private void SetMouseDirection(InputAction.CallbackContext ctx)
	{
		_rotation = ctx.ReadValue<Vector2>();
	}
	private void StopMouseDirection(InputAction.CallbackContext ctx)
	{
		_rotation = Vector2.zero;
	}
	public void OnInputEnable(ControlSchema schema)
	{
        _controls = schema;
        _controls.Player.Camera.performed += SetMouseDirection;
        _controls.Player.Camera.canceled += StopMouseDirection;
	}

	public void OnInputDisable()
	{
		_controls.Player.Camera.performed -= SetMouseDirection;
		_controls.Player.Camera.canceled -= StopMouseDirection;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceManager : MonoBehaviour
{
	public static InputDeviceManager Instance;

	public enum InputDeviceType
	{
		KeyboardMouse,
		Gamepad
	}

	public InputDeviceType CurrentDevice { get; private set; } = InputDeviceType.KeyboardMouse;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		InputSystem.onActionChange += OnInputActionChange;
	}

	private void OnDestroy()
	{
		InputSystem.onActionChange -= OnInputActionChange;
	}

	private void OnInputActionChange(object obj, InputActionChange change)
	{
		if (change == InputActionChange.ActionPerformed)
		{
			if (obj is InputAction action && action.activeControl != null)
			{
				// Aktif kontrol cihazýný belirle
				var device = action.activeControl.device;

				if (CurrentDevice != InputDeviceType.KeyboardMouse && (device is Keyboard || device is Mouse))
				{
					CurrentDevice = InputDeviceType.KeyboardMouse;
					Debug.Log($"Switched To: {CurrentDevice}");
				}
				else if (CurrentDevice != InputDeviceType.Gamepad && device is Gamepad)
				{
					CurrentDevice = InputDeviceType.Gamepad;
					Debug.Log($"Switched To: {CurrentDevice}");
				}
			}
		}
	}
}

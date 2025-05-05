using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		Application.targetFrameRate = 999;
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

	[SerializeField]
	private List<KeyDisplay> _keyNames;
	public string GetDisplayName(string inputName)
	{

		return _keyNames.FirstOrDefault(x => x.InputName == inputName)?.DisplayName ?? "UNKNOWN";

		//string displayName = "UNKNOWN";

		//var found = _keyNames.FirstOrDefault(x => x.InputName == inputName);
		//if(found != null)
		//	displayName = found.DisplayName;

		//return displayName;
	}
}

[System.Serializable]
public class KeyDisplay
{
	public string InputName;
	public string DisplayName;
}

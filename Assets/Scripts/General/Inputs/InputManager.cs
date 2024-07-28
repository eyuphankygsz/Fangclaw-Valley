using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
	public static InputManager Instance { get; private set; }

	public ControlSchema Controls { get; private set; }

	[SerializeField]
	private GameObject _player;

	private void Awake()
	{
		Instance = this;
		Controls = new ControlSchema();
		LoadBindings();
	}

	private void OnEnable()
	{
		foreach (var item in _player.GetComponents<IInputHandler>())
			item.OnInputEnable(Controls);

		Controls.Enable();
	}

	private void OnDisable()
	{
		if (_player != null)
			foreach (var item in _player.GetComponents<IInputHandler>())
				item.OnInputDisable();

		Controls.Disable();
	}

	private void LoadBindings()
	{
		var inputact = Controls.asset.actionMaps;
		foreach (var map in Controls.asset.actionMaps)
			foreach (var action in map.actions)
				for (int i = 0; i < action.bindings.Count; i++)
				{
					string key = $"{map.name}/{action.name}/{i}";

					if (PlayerPrefs.HasKey(key))
					{
						string binding = PlayerPrefs.GetString(key);
						action.ApplyBindingOverride(i, binding);
					}
				}
	}

	private void SaveBinding(InputAction action, int bindingIndex, string bindingPath)
	{
		string key = $"{action.actionMap.name}/{action.name}/{bindingIndex}";
		PlayerPrefs.SetString(key, bindingPath);
		PlayerPrefs.Save();
	}

	public void ChangeBinding(TheInput input, TextMeshProUGUI text)
	{
		text.text = "...";
		if (GetInputAction(input.ActionName, out InputAction action) == null)
			return;

		action.Disable();
		action.PerformInteractiveRebinding(input.InputIndex)
			.OnComplete(operation =>
			{
				operation.Dispose();
				string newBinding = GetBindingPath(action, input.InputIndex);
				text.text = GetReadableBinding(newBinding);
				SaveBinding(action, input.InputIndex, newBinding);
				action.Enable();
			})
			.Start();
	}

	public void SetButtonText(TheInput input, TextMeshProUGUI text)
	{
		if (GetInputAction(input.ActionName, out InputAction action) == null)
			return;
		text.text = GetReadableBinding(GetBindingPath(action, input.InputIndex));
	}
	public void BackToDeafult(List<InputButton> inputs)
	{
		foreach (var input in inputs)
		{
			if (GetInputAction(input.Input.ActionName, out InputAction action) == null)
				return;

			action.ApplyBindingOverride(input.Input.InputIndex, input.Input.DefaultValue);
			SetButtonText(input.Input, input.TMP);
			SaveBinding(action, input.Input.InputIndex, input.Input.DefaultValue);
		}
	}
	private InputAction GetInputAction(string actionName, out InputAction action)
	{
		action = Controls.FindAction(actionName);
		return action;
	}
	private string GetBindingPath(InputAction action, int index)
		=> action.bindings[index].effectivePath;
	private string GetReadableBinding(string binding)
		=> InputControlPath.ToHumanReadableString(binding, InputControlPath.HumanReadableStringOptions.OmitDevice);

}

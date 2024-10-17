using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputManager : IInitializable
{

	public ControlSchema Controls { get; private set; }

	private PlayerController _player;

	public void Initialize()
	{
		Controls = new ControlSchema();
		LoadBindings();
	}

	public void Setup(PlayerController playerObject)
	{
		_player = playerObject;
		foreach (var item in _player.GetComponents<IInputHandler>())
			item.OnInputEnable(Controls);

		Controls.Enable();
	}

	private void Desetup()
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
			if (GetInputAction(input.GetInput().ActionName, out InputAction action) == null)
				return;

			action.ApplyBindingOverride(input.GetInput().InputIndex, input.GetInput().DefaultValue);
			SetButtonText(input.GetInput(), input.GetTMP());
			SaveBinding(action, input.GetInput().InputIndex, input.GetInput().DefaultValue);
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

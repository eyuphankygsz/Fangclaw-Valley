using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class SettingButtons : MonoBehaviour
{

	[SerializeField]
	GameObject _content;

	List<InputButton> _inputButtons;

	[Inject]
	private InputManager _inputManager;

	[SerializeField]
	private List<Setting> _settingButtons;

	public void SetBindingsToDefault()
	{
		if (_inputButtons == null)
			_inputButtons = _content.GetComponentsInChildren<InputButton>().ToList();
		_inputManager.BackToDeafult(_inputButtons);
	}
	public void SetSettingsToDefault()
	{
		foreach (var button in _settingButtons)
			button.Restore();
	}
	public void SaveSettings()
	{
		foreach (var button in _settingButtons)
			button.Save();
	}
	public void ResetPlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ResetButton : MonoBehaviour
{

	[SerializeField]
	GameObject _content;

	List<InputButton> _inputButtons;

	[Inject]
	private InputManager _inputManager;

	public void SetToDefault()
	{
		if (_inputButtons == null) 
			_inputButtons = _content.GetComponentsInChildren<InputButton>().ToList();
		_inputManager.BackToDeafult(_inputButtons);
	}
	public void ResetPlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
	}
}

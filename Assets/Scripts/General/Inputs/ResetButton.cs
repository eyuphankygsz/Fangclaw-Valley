using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResetButton : MonoBehaviour
{

	[SerializeField]
	GameObject _content;

	List<InputButton> _inputButtons;


	public void SetToDefault()
	{
		if (_inputButtons == null) 
			_inputButtons = _content.GetComponentsInChildren<InputButton>().ToList();
		InputManager.Instance.BackToDeafult(_inputButtons);
	}
	public void ResetPlayerPrefs()
	{
		PlayerPrefs.DeleteAll();
	}
}

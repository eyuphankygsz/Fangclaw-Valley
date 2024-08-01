using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class InputButton : MonoBehaviour
{
	[field: SerializeField]
	public TheInput Input { get; private set; }

	[field: SerializeField]
	public TextMeshProUGUI TMP { get; private set; }
	
	[Inject]
	private InputManager _inputManager;
	public void Start()
	{
		_inputManager.SetButtonText(Input, TMP);
	}
	public void ChangeKey()
	{
		_inputManager.ChangeBinding(Input, TMP);
	}
}

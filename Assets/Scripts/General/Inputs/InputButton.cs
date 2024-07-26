using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputButton : MonoBehaviour
{
	[field: SerializeField]
	public TheInput Input { get; private set; }

	[field: SerializeField]
	public TextMeshProUGUI TMP { get; private set; }
	public void Start()
	{
		InputManager.Instance.SetButtonText(Input, TMP);
	}
	public void ChangeKey()
	{
		InputManager.Instance.ChangeBinding(Input, TMP);
	}
}

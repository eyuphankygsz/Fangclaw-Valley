using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Keys : Interactable
{
	[SerializeField]
	private string _keyName;
	public override void OnInteract(Enum_Weapons weapon)
	{
		PlayerPrefs.SetString(_keyName, "Taken");
		gameObject.SetActive(false);
	}
}

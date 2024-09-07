using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_MusicItem : Interactable
{
	private MusicMechanism _mechanism;
	[SerializeField]
	private char _character;

	private void Awake()
	{
		_mechanism = GetComponentInParent<MusicMechanism>();
	}
	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		_mechanism.AddNote(_character);
	}
	public override GameData GetGameData()
	{
		return null;
	}

	public override void LoadData()
	{
		return;
	}

}

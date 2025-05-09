using UnityEngine;

public class Interactable_MusicItem : Interactable
{
	private MusicMechanism _mechanism;
	[SerializeField]
	private char _character;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	private void Awake()
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
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

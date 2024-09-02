using UnityEngine;
using Zenject;

public class Interactable_WeaponPick : Interactable
{
	[SerializeField]
	private Enum_Weapons _weapon;

	private WeaponPickData _data;


	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		PlayerController.AddWeapon(_weapon);
		gameObject.SetActive(false);

	}

	public override GameData GetGameData()
	{
		_data = new WeaponPickData()
		{
			Name = InteractableName,
			IsTaken = !gameObject.activeSelf
		};
		return _data;
	}

	public override void LoadData()
	{
		WeaponPickData data = _saveManager.GetData<WeaponPickData>(InteractableName);
		if (data == null) return;

		gameObject.SetActive(!data.IsTaken);
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
	}
}
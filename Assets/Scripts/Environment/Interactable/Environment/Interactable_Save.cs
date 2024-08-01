using System.Collections;
using UnityEngine;

public class Interactable_Save : Interactable
{
	private Coroutine _saveRoutine;
	public override void LoadData()
	{
		return;
	}

	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		if (_saveRoutine != null) return;

		_saveRoutine = StartCoroutine(SaveGameRoutine());
	}

	private IEnumerator SaveGameRoutine()
	{
		yield return _saveManager.SaveGame(OnSaveComplete);
	}
	private void OnSaveComplete()
	{
		_saveRoutine = null;
	}
	public override GameData GetGameData()
	{
		return null;
	}
}

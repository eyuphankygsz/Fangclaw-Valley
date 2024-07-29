using System.Collections;
using System.Collections.Generic;
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
		if (_saveRoutine != null) return;

		_saveRoutine = StartCoroutine(SaveGameRoutine());
	}

	private IEnumerator SaveGameRoutine()
	{
		yield return SaveManager.Instance.SaveGame(OnSaveComplete);
	}
	private void OnSaveComplete()
	{
		_saveRoutine = null;
	}
	public override InteractableData SaveData()
	{
		return null;
	}
}

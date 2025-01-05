using System.Linq;
using UnityEngine;

public class StandartUse : UseFunctions
{
	[SerializeField]
	InventoryManager _inventoryManager;
	public override void Use(InventoryItemHolder lastSelectedHolder)
	{
		var usefunction = _inventoryManager.UseFunctions.FirstOrDefault(x => x.Key == lastSelectedHolder.Item.name).Value;
		if (!usefunction.Use())
		{
			_inventoryManager.DisableCursorMenu(false);
			return;
		}

		_inventoryManager.TheAudioSource.PlayOneShot(lastSelectedHolder.Item.UseSFX);
		_inventoryManager.RemoveItemQuantityFromInventory(lastSelectedHolder, 1);
		_inventoryManager.DisableCursorMenu(false);
	}
}
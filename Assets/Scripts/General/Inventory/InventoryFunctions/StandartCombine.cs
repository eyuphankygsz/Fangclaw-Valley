using UnityEngine;

public class StandartCombine : CombineFunctions
{
	[SerializeField]
	InventoryManager _inventoryManager;
	public override void Combine(InventoryItemHolder lastSelectedHolder)
	{
		_inventoryManager.Combine = true;
		_inventoryManager.OnMenu = false;
		_inventoryManager.DisableCursorMenu(true);
		_inventoryManager.TakeItem(lastSelectedHolder);
	}
}
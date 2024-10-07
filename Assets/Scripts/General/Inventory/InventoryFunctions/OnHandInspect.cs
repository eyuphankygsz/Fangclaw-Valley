using UnityEngine;

public class OnHandInspect : InspectFunctions
{
	[SerializeField]
	InventoryManager _inventoryManager;
	[SerializeField]
	PlayerWeaponController _controller;
	public override void Inspect(InventoryItemHolder lastSelectedHolder)
	{
		_controller.EquipExternalWeapon(lastSelectedHolder.Item.name);
		_inventoryManager.DisableCursorMenu(false);
		PauseMenu.Instance.Close();
	}
}

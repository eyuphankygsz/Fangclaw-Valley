using System.Linq;
using UnityEngine;

public class StandartDrop : DropFunctions
{
	[SerializeField]
	private InventoryManager _inventoryManager;
	[SerializeField]
	private PlayerWeaponController _weaponController;
	public override void Drop(InventoryItemHolder lastSelectedHolder)
	{
		_inventoryManager.TheAudioSource.PlayOneShot(lastSelectedHolder.Item.InspectSFX);
		_inventoryManager.ObjectPool.GetObject(_inventoryManager.PlayerDropTransform.position, lastSelectedHolder.Item.name);
		_weaponController.TryDropExternalWeapon(lastSelectedHolder.Item.name);
		lastSelectedHolder.AddQuantity(-1);
		_inventoryManager.DisableCursorMenu(false);

	}
}
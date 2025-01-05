using System.Linq;
using UnityEngine;

public class StandartDrop : DropFunctions
{
	[SerializeField]
	InventoryManager _inventoryManager;
	public override void Drop(InventoryItemHolder lastSelectedHolder)
	{
		_inventoryManager.TheAudioSource.PlayOneShot(lastSelectedHolder.Item.InspectSFX);
		_inventoryManager.ObjectPool.GetObject(_inventoryManager.PlayerDropTransform.position, lastSelectedHolder.Item.name);
		lastSelectedHolder.AddQuantity(-1);
		_inventoryManager.DisableCursorMenu(false);
	}
}
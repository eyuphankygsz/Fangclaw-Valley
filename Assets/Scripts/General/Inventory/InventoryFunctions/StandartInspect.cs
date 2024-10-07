using System.Collections;
using UnityEngine;

public class StandartInspect : InspectFunctions
{
	[SerializeField]
	InventoryManager _inventoryManager;
	public override void Inspect(InventoryItemHolder lastSelectedHolder)
	{
		_inventoryManager.TheAudioSource.PlayOneShot(lastSelectedHolder.Item.Sound);
		_inventoryManager.ItemPicture.sprite = lastSelectedHolder.Item.ItemSprite;
		_inventoryManager.ItemPicture.enabled = true;
		_inventoryManager.ItemTitle.text = lastSelectedHolder.Item.ItemName.GetLocalizedString();
		_inventoryManager.ItemDescription.text = lastSelectedHolder.Item.ItemDescription.GetLocalizedString();
		_inventoryManager.DisableCursorMenu(false);
	}
}

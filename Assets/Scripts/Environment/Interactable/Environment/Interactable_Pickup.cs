using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Pickup : Interactable
{
	[SerializeField]
	private InventoryItem _item;
	[SerializeField]
	private int _quantity;
	public override void OnInteract(Enum_Weapons weapon)
	{
		InventoryManager.Instance.AddItemToInventory(_item, _quantity);
		gameObject.SetActive(false);
	}
}

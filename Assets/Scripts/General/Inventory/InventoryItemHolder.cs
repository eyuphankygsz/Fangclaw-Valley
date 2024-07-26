using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemHolder : MonoBehaviour
{

	[field: SerializeField]
	public InventoryItem Item { get; private set; }
	
	[field: SerializeField]
	public int Quantity { get; private set; }
	
	[field: SerializeField]
	public int MaxQuantity { get; private set; }

	public void Setup(InventoryItem item, int quantity)
	{
		Item = item;
		MaxQuantity = item.StackQuantity;
		Quantity = quantity;
	}
	public void OnSelect()
	{

	}
	public void AddQuantity(int quantity)
	{
		Quantity += quantity;
	}

}

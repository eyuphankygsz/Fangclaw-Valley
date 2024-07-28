using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

		GetComponent<Image>().sprite = item.ItemSprite;
		GetComponent<Button>().onClick.AddListener(OnSelect);
	}
	public void OnSelect()
	{
		InventoryManager.Instance.SelectItem(this);
	}
	public void AddQuantity(int quantity)
	{
		Quantity += quantity;
	}

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CombineManager : MonoBehaviour
{
	[SerializeField]
	List<CombinePart> _combines;

	[Inject]
	private InventoryManager _inventoryManager;

	private void Start()
	{
		_inventoryManager.SetCombineManager(this);
	}
	public void CombineItems(InventoryItemHolder first, InventoryItemHolder second)
	{
		var combineable = _combines.FirstOrDefault(combine =>
			(combine.FirstItem == first.Item && combine.FirstCount <= first.Quantity && combine.SecondItem == second.Item && combine.SecondCount <= second.Quantity) ||
			(combine.FirstItem == second.Item && combine.FirstCount <= second.Quantity && combine.SecondItem == first.Item && combine.SecondCount <= first.Quantity));

		if (combineable == null) return;

		first.AddQuantity(combineable.FirstItem == first ? -combineable.FirstCount : -combineable.SecondCount);
		second.AddQuantity(combineable.FirstItem == first ? -combineable.SecondCount : -combineable.FirstCount);

		_inventoryManager.AddItemToInventory(combineable.ResultItem, combineable.ResultCount, null);

	}

}


[Serializable]
public class CombinePart
{
	public InventoryItem FirstItem;
	public int FirstCount;

	public InventoryItem SecondItem;
	public int SecondCount;

	public InventoryItem ResultItem;
	public int ResultCount;
}
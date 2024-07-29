using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
	public static InventoryManager Instance { get; private set; }
	[SerializeField]
	private GameObject _itemHolderPrefab;
	[SerializeField]
	private Transform _itemHolderTransform;

	[SerializeField]
	private List<InventoryItem> _allItems;

	private List<InventoryItemHolder> _itemHoldersList = new List<InventoryItemHolder>();


	[SerializeField]
	private Image _itemPicture;
	[SerializeField]
	private TextMeshProUGUI _itemTitle;
	[SerializeField]
	private TextMeshProUGUI _itemDescription;

	private void Awake()
	{
		Instance = this;
	}

	public void Start()
	{
		Load();
	}


	public void AddItemToInventory(InventoryItem item, int quantity)
	{
		var itemHolders = GetInventoryHolders(item);
		int leftOvers = quantity;
		int index = 0;

		while (leftOvers > 0)
		{
			InventoryItemHolder holder = itemHolders[index++];

			int leftSpace = holder.MaxQuantity - holder.Quantity;

			int addedQuantity = Mathf.Clamp(leftOvers, 0, leftSpace);
			holder.AddQuantity(addedQuantity);

			leftOvers -= addedQuantity;

			if (leftOvers > 0 && index == itemHolders.Count)
				itemHolders.Add(CreateItemHolder(item));


		}
	}
	public void RemoveItemFromInventory(InventoryItem item)
	{
		InventoryItemHolder holder = _itemHoldersList
										 .Where(h => h.Item == item)
										 .OrderBy(h => h.Quantity)
										 .FirstOrDefault();
		_itemHoldersList.Remove(holder);
		Destroy(holder.gameObject);
	}
	public InventoryItem GetItem(string itemName, int quantity)
	{
		return _itemHoldersList
				   .Where(holder =>
					   holder.Item.ItemName == itemName)
				   .Select(holder => holder.Item)
				   .FirstOrDefault();
	}
	public void SelectItem(InventoryItemHolder holder)
	{
		_itemTitle.text = holder.Item.ItemName;
		_itemPicture.sprite = holder.Item.ItemSprite;
		_itemPicture.enabled = true;
		_itemDescription.text = holder.Item.ItemDescription;
	}
	public void Load()
	{
		InventoryData data = (InventoryData)SaveManager.Instance.GetData(null, SaveType.Inventory);
		if (data == null) 
			return;

		foreach (var itemData in data.Items)
		{
			InventoryItem item = FindItemByName(itemData.ItemName);
			if (item != null)
				AddItemToInventory(item, itemData.Quantity);
		}
	}

	public List<InventoryItemHolder> GetHolders()
	{
		return _itemHoldersList;
	}
	private List<InventoryItemHolder> GetInventoryHolders(InventoryItem item)
	{
		var itemHolder = _itemHoldersList
						 .Where(i => i.Item == item && i.Quantity < i.MaxQuantity)
						 .ToList();

		if (itemHolder.Count == 0)
			itemHolder.Add(CreateItemHolder(item));
		return itemHolder;
	}

	private InventoryItemHolder CreateItemHolder(InventoryItem item)
	{
		var newItemHolder = Instantiate(_itemHolderPrefab, _itemHolderTransform).GetComponent<InventoryItemHolder>();
		newItemHolder.Setup(item, 0);
		_itemHoldersList.Add(newItemHolder);
		return newItemHolder;
	}

	private InventoryItem FindItemByName(string itemName) =>
		_allItems.Where(x => x.ItemName == itemName).FirstOrDefault();


}
[Serializable]
public class InventoryData
{
	public List<InventoryDataItem> Items;
}
[Serializable]
public class InventoryDataItem
{
	public string ItemName;
	public int Quantity;
}

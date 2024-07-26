using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

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

	private void Awake()
	{
		Instance = this;
	}

	public void Start()
	{
		LoadItems();
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
	public void SaveItems()
	{
		InventoryData data = new InventoryData
		{
			Items = _itemHoldersList.Select(holder =>
									 new InventoryDataItem
									 {
										 ItemName = holder.Item.ItemName,
										 Quantity = holder.Quantity
									 })
								   .ToList()
		};

		string json = JsonUtility.ToJson(data, true);
		File.WriteAllText(Application.persistentDataPath + "/inventory.json", json);
	}
	public void LoadItems()
	{
		string path = Application.persistentDataPath + "/inventory.json";
		if (File.Exists(path))
		{
			string json = File.ReadAllText(path);
			InventoryData data = JsonUtility.FromJson<InventoryData>(json);

			foreach (var itemData in data.Items)
			{
				InventoryItem item = FindItemByName(itemData.ItemName);
				if (item != null)
					AddItemToInventory(item, itemData.Quantity);
			}
		}
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

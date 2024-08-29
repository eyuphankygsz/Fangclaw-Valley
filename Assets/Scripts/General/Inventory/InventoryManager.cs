using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InventoryManager : MonoBehaviour
{
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

	[Inject]
	private SaveManager _saveManager;

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
				itemHolders.Add(CreateItemHolder(item, 0));

			if (!_saveManager.HasItem(holder.gameObject, holder.GetSaveData()))
				_saveManager.AddSaveableObject(holder.gameObject, holder.GetSaveData());
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
	public void RemoveItemQuantityFromInventory(InventoryItem item, int quantity)
	{
		InventoryItemHolder holder = _itemHoldersList
										 .Where(h => h.Item == item)
										 .OrderBy(h => h.Quantity)
										 .FirstOrDefault();
		holder.AddQuantity(-quantity);

		if (holder.Quantity == 0)
		{
		_itemHoldersList.Remove(holder);
			Destroy(holder.gameObject);

		}
	}
	public InventoryItem GetItem(string itemName, int quantity)
	{
        return _itemHoldersList
				   .Where(holder =>
					   holder.Item.ItemName.GetLocalizedString() == itemName)
				   .Select(holder => holder.Item)
				   .FirstOrDefault();
	}
	public void SelectItem(InventoryItemHolder holder)
	{
		_itemTitle.text = holder.Item.ItemName.GetLocalizedString();
		_itemPicture.sprite = holder.Item.ItemSprite;
		_itemPicture.enabled = true;
		_itemDescription.text = holder.Item.ItemDescription.GetLocalizedString();
	}
	public void Load()
	{
		List<InventoryDataItem> datas = _saveManager.GetDataList<InventoryDataItem>();


		if (datas == null)
			return;

		foreach (var itemData in datas)
		{
			InventoryDataItem data = itemData;

			InventoryItem item = FindItemByName(data.Name);
			if (item != null)
				AddItemToInventory(item, data.Quantity);
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
			itemHolder.Add(CreateItemHolder(item, 0));
		return itemHolder;
	}

	private InventoryItemHolder CreateItemHolder(InventoryItem item, int quantity)
	{
		var newItemHolder = Instantiate(_itemHolderPrefab, _itemHolderTransform).GetComponent<InventoryItemHolder>();
		newItemHolder.Setup(this, item, quantity);
		_itemHoldersList.Add(newItemHolder);
		return newItemHolder;
	}

	private InventoryItem FindItemByName(string itemName) =>
		_allItems.Where(x => x.Name == itemName).FirstOrDefault();


}
[Serializable]
public class InventoryDataItem : GameData
{
	public int Quantity;
}

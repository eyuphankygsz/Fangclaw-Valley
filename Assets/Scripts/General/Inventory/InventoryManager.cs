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
	[SerializeField]
	private Camera _camera;
	[SerializeField]
	private LayerMask _holderLayer;

	private List<InventoryItemHolder> _itemHoldersList = new List<InventoryItemHolder>();


	[SerializeField]
	private Image _itemPicture;
	[SerializeField]
	private TextMeshProUGUI _itemTitle;
	[SerializeField]
	private TextMeshProUGUI _itemDescription;
	[SerializeField]
	private InventoryItemHolder _takeableObject;


	[Inject]
	private SaveManager _saveManager;
	public Color _pointerEnterColor, _pointerExitColor;


	private InventoryItemHolder _lastSelectedHolder;
	[SerializeField]
	private CursorItemHolder _cursorItem;

	public void Start()
	{
		_itemHoldersList = _itemHolderTransform.GetComponentsInChildren<InventoryItemHolder>().ToList();
		_takeableObject.SetInventoryManager(this);
		_takeableObject.gameObject.SetActive(false);
		_itemHoldersList.ForEach(itemHolder => itemHolder.SetInventoryManager(this));
		Load();
	}


	public void AddItemToInventory(InventoryItem item, int quantity)
	{
		var itemHolders = GetNonEmptyInventoryHolders(item);

		if (itemHolders == null)
		{
			AddToChest(item, quantity);
			return;
		}
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
			{
				AddToChest(item, leftOvers);
				return;
			}
		}
	}

	public void AddSavedItemToInventory(InventoryItem item, int quantity, int id)
	{
		var holder = _itemHoldersList[id];
		int leftSpace = holder.MaxQuantity - holder.Quantity;
		holder.AddQuantity(quantity);
	}
	private void AddToChest(InventoryItem item, int quantity)
	{
		PauseMenu.Instance.OpenInventory();
		SetTakenObject(item, quantity);
	}
	private void SetTakenObject(InventoryItem item, int quantity)
	{
		_takeableObject.Setup(this, item, quantity);
		_takeableObject.gameObject.SetActive(true);
	}
	private List<InventoryItemHolder> GetNonEmptyInventoryHolders(InventoryItem item)
	{
		var itemHolder = _itemHoldersList
						 .Where(i => i.Item == item && i.Quantity < i.MaxQuantity)
						 .ToList();

		if (itemHolder.Count == 0)
			return null;
		return itemHolder;
	}
	public void AddHolder(InventoryItemHolder holder)
	{
		_itemHoldersList.Add(holder);
	}
	public void HandleClick(InventoryItemHolder holder)
	{
		if (holder.Item != null)
		{
			if (_lastSelectedHolder == null)
				TakeItem(holder);
			else
				CheckItem(holder);
		}
		else
		{

		}

	}
	private void TakeItem(InventoryItemHolder holder)
	{
		_lastSelectedHolder = holder;
		_cursorItem.Setup(holder);
	}
	private void CheckItem(InventoryItemHolder clicked)
	{
		if (_lastSelectedHolder.Item == clicked.Item)
		{
			int canAdded = clicked.MaxQuantity - clicked.Quantity;
			if (canAdded > 0)
			{
				int added = Mathf.Clamp(_lastSelectedHolder.Quantity, 0, canAdded);
				clicked.AddQuantity(added);
				_lastSelectedHolder.AddQuantity(-added);
			}
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
				AddSavedItemToInventory(item, data.Quantity, data.ID);
		}
	}

	public List<InventoryItemHolder> GetHolders()
	{
		return _itemHoldersList;
	}

	private InventoryItem FindItemByName(string itemName) =>
		_allItems.Where(x => x.Name == itemName).FirstOrDefault();


}
[Serializable]
public class InventoryDataItem : GameData
{
	public int Quantity;
	public int ID;
}

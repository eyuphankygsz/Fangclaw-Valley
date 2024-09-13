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
	[Inject]
	private ObjectPool _objectPool;

	public Color _pointerEnterColor, _pointerExitColor;


	private InventoryItemHolder _lastSelectedHolder;
	[SerializeField]
	private InventoryItemHolder _chestHolder;
	[SerializeField]
	private CursorItemHolder _cursorItem;
	[SerializeField]
	private CursorMenuFollower _cursorMenu;
	[SerializeField]
	private Transform _playerDropTransform;


	private Interactable_Pickup _tempPickup;
	private bool _onMenu;

	[SerializeField]
	private Transform _useFunctionParent;
	private Dictionary<string, UseFunction> _useFunctions = new Dictionary<string, UseFunction>();

	public void Start()
	{
		_itemHoldersList = _itemHolderTransform.GetComponentsInChildren<InventoryItemHolder>().ToList();
		_takeableObject.SetInventoryManager(this);
		_takeableObject.gameObject.SetActive(false);
		_itemHoldersList.ForEach(itemHolder => itemHolder.SetInventoryManager(this));
		_cursorMenu.Setup(this);

		for (int i = 0; i < _useFunctionParent.childCount; i++)
			_useFunctions.Add(_useFunctionParent.GetChild(i).name, _useFunctionParent.GetChild(i).GetComponent<UseFunction>());


		Load();
	}


	public void AddItemToInventory(InventoryItem item, int quantity, Interactable_Pickup pickupObj)
	{
		var itemHolders = GetNonEmptyInventoryHolders(item);
		_tempPickup = pickupObj;

		if (itemHolders == null)
		{
			var emptyHolders = GetEmptyInventoryHolders();
			if (emptyHolders != null)
			{
				AddToInventory(quantity, emptyHolders, item);
				return;
			}

			AddToChest(item, quantity);
			return;
		}
		AddToInventory(quantity, itemHolders, item);
	}

	private void AddToInventory(int quantity, List<InventoryItemHolder> itemHolders, InventoryItem item)
	{
		int leftOvers = quantity;
		int index = 0;

		while (leftOvers > 0)
		{
			InventoryItemHolder holder = itemHolders[index++];

			int leftSpace = holder.Item != null ? (holder.MaxQuantity - holder.Quantity) : item.StackQuantity;
			int addedQuantity = Mathf.Clamp(leftOvers, 0, leftSpace);

			if (holder.Item == null)
				holder.Setup(this, item, addedQuantity);
			else
				holder.AddQuantity(addedQuantity);

			_tempPickup?.AddQuantity(-addedQuantity);
			leftOvers -= addedQuantity;

			if (leftOvers > 0 && index == itemHolders.Count)
			{
				AddToChest(item, leftOvers);
				return;
			}

			_tempPickup?.gameObject.SetActive(false);
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
	public void AddHolder(InventoryItemHolder holder)
	{
		_itemHoldersList.Add(holder);
	}
	private List<InventoryItemHolder> GetEmptyInventoryHolders()
	{
		var itemHolder = _itemHoldersList
						 .Where(i => i.Item == null)
						 .ToList();

		if (itemHolder.Count == 0)
			return null;
		return itemHolder;
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

	public void HandleLeftClick(InventoryItemHolder holder)
	{
		if (_onMenu)
			DisableCursorMenu();

		if (holder.Item != null)
		{
			if (_lastSelectedHolder == null)
				TakeItem(holder);
			else
				CheckItem(holder);
		}
		else
		{
			if (_lastSelectedHolder == null) return;
			else
			{
				ChangeItem(holder);
			}
		}
		if (_chestHolder.Item == null)
		{
			_tempPickup?.gameObject.SetActive(false);
			_chestHolder.gameObject.SetActive(false);
		}
	}
	public void HandleRightClick(InventoryItemHolder holder)
	{
		if (_onMenu)
			_lastSelectedHolder = null;
		if (_lastSelectedHolder == null && holder.Item != null)
		{
			_onMenu = true;
			_lastSelectedHolder = holder;
			_cursorMenu.SetMenu(holder.Item);
		}
		else
		{
			if (_lastSelectedHolder != null)
				_lastSelectedHolder.SetTemporaryStatus(true);
			DisableCursorMenu();
		}
	}
	private void TakeItem(InventoryItemHolder holder)
	{
		_lastSelectedHolder = holder;
		holder.SetTemporaryStatus(isEnable: false);
		_cursorItem.Setup(holder);
	}
	private void ChangeItem(InventoryItemHolder holder)
	{
		if (holder == _chestHolder)
		{
			_lastSelectedHolder.SetTemporaryStatus(isEnable: true);
			DisableCursorFollower();
			return;
		}
		holder.Setup(this, _lastSelectedHolder.Item, _lastSelectedHolder.Quantity);
		_lastSelectedHolder.ResetHolder();
		DisableCursorFollower();
	}
	private void CheckItem(InventoryItemHolder clicked)
	{
		if (clicked == _chestHolder)
		{
			if (_lastSelectedHolder == clicked)
				_lastSelectedHolder.SetTemporaryStatus(true);

			DisableCursorFollower();
			return;
		}
		if (_lastSelectedHolder.Item == clicked.Item)
		{
			if (clicked.TempDisable)
			{
				clicked.SetTemporaryStatus(isEnable: true);
				DisableCursorFollower();
				return;
			}
			int canAdded = clicked.MaxQuantity - clicked.Quantity;
			if (canAdded > 0)
			{
				int added = Mathf.Clamp(_lastSelectedHolder.Quantity, 0, canAdded);
				clicked.AddQuantity(added);
				_lastSelectedHolder.AddQuantity(-added);
				_cursorItem.Setup(_lastSelectedHolder);
			}
		}
		else
		{
			InventoryItem tempItem = clicked.Item;
			int tempQuantity = clicked.Quantity;

			clicked.Setup(this, _lastSelectedHolder.Item, _lastSelectedHolder.Quantity);
			_lastSelectedHolder.Setup(this, tempItem, tempQuantity);
			DisableCursorFollower();
		}
	}
	private void DisableCursorFollower()
	{
		_cursorItem.SetActive(false);
		_lastSelectedHolder = null;
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
					   holder.Item?.ItemName.GetLocalizedString() == itemName)
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

	public void Disable()
	{
		_tempPickup = null;
		_lastSelectedHolder = null;
	}

	public void UseItem()
	{
		_useFunctions[_lastSelectedHolder.Item.name].Use();
		_lastSelectedHolder.AddQuantity(-1);
		DisableCursorMenu();
	}
	public void InspectItem()
	{
		_itemPicture.sprite = _lastSelectedHolder.Item.ItemSprite;
		_itemTitle.text = _lastSelectedHolder.Item.ItemName.GetLocalizedString();
		_itemDescription.text = _lastSelectedHolder.Item.ItemDescription.GetLocalizedString();
		DisableCursorMenu();
	}
	public void CombineItem()
	{

	}
	public void DropItem()
	{
		_objectPool.GetObject(_playerDropTransform.position, _lastSelectedHolder.Item.name);
		_lastSelectedHolder.AddQuantity(-1);
		DisableCursorMenu();
	}
	public void DisableCursorMenu()
	{
		_cursorMenu.gameObject.SetActive(false);
		DisableCursorFollower();
		_onMenu = false;
	}
}
[Serializable]
public class InventoryDataItem : GameData
{
	public int Quantity;
	public int ID;
}

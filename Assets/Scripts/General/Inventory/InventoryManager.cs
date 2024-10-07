using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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

	public Image ItemPicture;
	public TextMeshProUGUI ItemTitle;
	public TextMeshProUGUI ItemDescription;
	public AudioSource TheAudioSource;
	public Color _pointerEnterColor, _pointerExitColor;
	public Dictionary<string, UseFunction> UseFunctions = new Dictionary<string, UseFunction>();
	public bool Combine;
	public bool OnMenu;
	public Transform PlayerDropTransform;


	[SerializeField]
	private List<InspectFunctionHolder> _inspectFunctions;
	[SerializeField]
	private List<UseFunctionHolder> _useFunctions;
	[SerializeField]
	private List<CombineFunctionHolder> _combineFunctions;
	[SerializeField]
	private List<DropFunctionHolder> _dropFunctions;

	[SerializeField]
	private InventoryItemHolder _takeableObject;


	[Inject]
	private SaveManager _saveManager;
	[Inject]
	public ObjectPool ObjectPool;



	private InventoryItemHolder _lastSelectedHolder;
	[SerializeField]
	private InventoryItemHolder _chestHolder;
	[SerializeField]
	private CursorItemHolder _cursorItem;
	[SerializeField]
	private CursorMenuFollower _cursorMenu;



	private Interactable_Pickup TempPickup;

	[SerializeField]
	private Transform _useFunctionParent;

	private CombineManager _combineManager;

	[SerializeField]
	private AudioClip _bagClip;

	private void Awake()
	{
		ObjectPool.InitiateDictionary(_allItems);
	}
	public void Start()
	{
		_itemHoldersList = _itemHolderTransform.GetComponentsInChildren<InventoryItemHolder>().ToList();
		_takeableObject.SetInventoryManager(this);
		_takeableObject.gameObject.SetActive(false);
		_itemHoldersList.ForEach(itemHolder => { itemHolder.SetInventoryManager(this); itemHolder.Start(); });
		_cursorMenu.Setup(this);

		for (int i = 0; i < _useFunctionParent.childCount; i++)
			UseFunctions.Add(_useFunctionParent.GetChild(i).name, _useFunctionParent.GetChild(i).GetComponent<UseFunction>());


		Load();
	}


	public void AddItemToInventory(InventoryItem item, int quantity, Interactable_Pickup pickupObj)
	{
		var itemHolders = GetNonEmptyInventoryHolders(item);
		TempPickup = pickupObj;

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
		TheAudioSource.PlayOneShot(_bagClip);
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

			TempPickup?.AddQuantity(-addedQuantity);
			leftOvers -= addedQuantity;

			if (leftOvers > 0 && index == itemHolders.Count)
			{
				AddToChest(item, leftOvers);
				return;
			}

			TempPickup?.gameObject.SetActive(false);
		}
	}

	public void AddSavedItemToInventory(InventoryItem item, int quantity, int id)
	{
		var holder = _itemHoldersList[id];
		holder.Setup(this, item, quantity);
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
		if (Combine)
		{
			TryCombine(holder);
			return;
		}
		if (OnMenu)
			DisableCursorMenu(false);

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
				ChangeItem(holder);
		}
		if (_chestHolder.Item == null)
		{
			TempPickup?.gameObject.SetActive(false);
			_chestHolder.gameObject.SetActive(false);
		}
	}
	public void HandleRightClick(InventoryItemHolder holder)
	{
		if (OnMenu || Combine)
		{
			DisableCursorMenu(false);
			_lastSelectedHolder = null;
		}
		Combine = false;

		if (_lastSelectedHolder == null && holder.Item != null)
		{
			OnMenu = true;
			_lastSelectedHolder = holder;
			_cursorMenu.SetMenu(holder.Item, holder.IsChestHolder);
		}
		else
		{
			if (_lastSelectedHolder != null)
				_lastSelectedHolder.SetTemporaryStatus(true);
			DisableCursorMenu(false);
		}
	}
	public void TakeItem(InventoryItemHolder holder)
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
			CancelChange(clicked);
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
				TempPickup?.AddQuantity(-added);
				_cursorItem.Setup(_lastSelectedHolder);
			}
		}
		else
		{
			if (clicked.IsChestHolder || _lastSelectedHolder.IsChestHolder)
			{
				CancelChange(clicked);
				return;
			}
			InventoryItem tempItem = clicked.Item;
			int tempQuantity = clicked.Quantity;

			clicked.Setup(this, _lastSelectedHolder.Item, _lastSelectedHolder.Quantity);
			_lastSelectedHolder.Setup(this, tempItem, tempQuantity);
			DisableCursorFollower();
		}
	}
	private void CancelChange(InventoryItemHolder clicked)
	{
		if (_lastSelectedHolder == clicked)
			_lastSelectedHolder.SetTemporaryStatus(true);

		DisableCursorFollower();
	}
	private void DisableCursorFollower()
	{
		_cursorItem?.SetActive(false);
		if (_lastSelectedHolder?.Item != null)
			_lastSelectedHolder.SetTemporaryStatus(isEnable: true);
		_lastSelectedHolder = null;
	}
	public void RemoveItemFromInventory(InventoryItem item)
	{
		InventoryItemHolder holder = _itemHoldersList
										 .Where(h => h.Item == item)
										 .OrderBy(h => h.Quantity)
										 .FirstOrDefault();
		holder.ResetHolder();
	}
	public void RemoveItemQuantityFromInventory(InventoryItem item, int quantity)
	{
		InventoryItemHolder holder = _itemHoldersList
										 .Where(h => h.Item == item)
										 .OrderBy(h => h.Quantity)
										 .FirstOrDefault();
		holder.AddQuantity(-quantity);

		if (holder.Quantity == 0)
			holder.ResetHolder();
	}
	public void RemoveItemQuantityFromInventory(InventoryItemHolder holder, int quantity)
	{
		holder.AddQuantity(-quantity);
		
		if (holder.Quantity == 0)
			holder.ResetHolder();
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
		ItemTitle.text = holder.Item.ItemName.GetLocalizedString();
		ItemPicture.sprite = holder.Item.ItemSprite;
		ItemPicture.enabled = true;
		ItemDescription.text = holder.Item.ItemDescription.GetLocalizedString();
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
		TempPickup = null;
		Combine = false;
		ItemTitle.text = "";
		ItemDescription.text = "";
		ItemPicture.enabled = false;
		DisableCursorMenu(false);
	}

	public void UseItem()
	{
		UseFunctions fHolder = GetUseFunctionHolder(_lastSelectedHolder);
		fHolder.Use(_lastSelectedHolder);
	}
	public void InspectItem()
	{
		InspectFunctions fHolder = GetInspectFunctionHolder(_lastSelectedHolder);
		fHolder.Inspect(_lastSelectedHolder);
    }
	public void CombineItem()
	{
		CombineFunctions fHolder = GetCombineFunctionHolder(_lastSelectedHolder);
		fHolder.Combine(_lastSelectedHolder);
	}
	private void TryCombine(InventoryItemHolder holder)
	{
		if (holder != _lastSelectedHolder)
			_combineManager.CombineItems(holder, _lastSelectedHolder);
		Combine = false;
		DisableCursorMenu(false);
	}
	public void DropItem()
	{
		DropFunctions fHolder = GetDropFunctionHolder(_lastSelectedHolder);
		fHolder.Drop(_lastSelectedHolder);
	}
	private InspectFunctions GetInspectFunctionHolder(InventoryItemHolder holder)
	{
		string function = _lastSelectedHolder.Item.InspectName;
		for (int i = 0; i < _inspectFunctions.Count; i++)
			if (_inspectFunctions[i].Name == function)
				return _inspectFunctions[i].Inspect;

		return null;
	}
	private UseFunctions GetUseFunctionHolder(InventoryItemHolder holder)
	{
		string function = _lastSelectedHolder.Item.UseName;
		for (int i = 0; i < _useFunctions.Count; i++)
			if (_useFunctions[i].Name == function)
				return _useFunctions[i].Use;

		return null;
	}
	private CombineFunctions GetCombineFunctionHolder(InventoryItemHolder holder)
	{
		string function = _lastSelectedHolder.Item.CombineName;
		for (int i = 0; i < _combineFunctions.Count; i++)
			if (_combineFunctions[i].Name == function)
				return _combineFunctions[i].Combine;

		return null;
	}
	private DropFunctions GetDropFunctionHolder(InventoryItemHolder holder)
	{
		string function = _lastSelectedHolder.Item.DropName;
		for (int i = 0; i < _dropFunctions.Count; i++)
			if (_dropFunctions[i].Name == function)
				return _dropFunctions[i].Drop;

		return null;
	}
	public void DisableCursorMenu(bool onlyMenu)
	{
		_cursorMenu.gameObject.SetActive(false);
		if (!onlyMenu)
			DisableCursorFollower();
		OnMenu = false;
	}

	public void SetCombineManager(CombineManager manager)
	{
		_combineManager = manager;
	}
}
[Serializable]
public class InventoryDataItem : GameData
{
	public int Quantity;
	public int ID;
}

[Serializable]
public class InspectFunctionHolder
{
	public string Name;
	public InspectFunctions Inspect;
}
[Serializable]
public class UseFunctionHolder
{
	public string Name;
	public UseFunctions Use;
}
[Serializable]
public class CombineFunctionHolder
{
	public string Name;
	public CombineFunctions Combine;
}
[Serializable]
public class DropFunctionHolder
{
	public string Name;
	public DropFunctions Drop;
}
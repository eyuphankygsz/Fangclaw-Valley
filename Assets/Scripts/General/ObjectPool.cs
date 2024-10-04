using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ObjectPool : MonoBehaviour
{

	private Dictionary<InventoryItem, List<GameObject>> _itemPools = new Dictionary<InventoryItem, List<GameObject>>();

	[Inject]
	readonly PickupFactory _pickupFactory;
	[Inject]
	readonly SaveManager _saveManager;


	private void Start()
	{
		var crateItemList = _saveManager.CrateItems();
		if (crateItemList == null) return;
		foreach (var item in crateItemList)
			SetupCrateItem(item);
	}

	public void InitiateDictionary(List<InventoryItem> itemList)
	{
		foreach (var item in itemList)
			_itemPools.Add(item, CreateItems(item, item.INITIAL_COUNT));
	}

	private void SetupCrateItem(CrateItem crateItem)
	{
		if (crateItem.Taken) return;

		var selectedPoolItem = _itemPools.FirstOrDefault(item => item.Key.Item.GetComponent<Interactable>() != null && item.Key.Item.GetComponent<Interactable>().InteractableName == crateItem.Name);
		var item = GetObject(crateItem.Position, selectedPoolItem.Key);
		if(item.TryGetComponent<Interactable_Pickup>(out Interactable_Pickup pickup))
			pickup.SetQuantity(crateItem.Quantity);
	}

	public GameObject GetObject(Vector3 pos, InventoryItem wantedObject)
	{
		foreach (var item in _itemPools[wantedObject])
			if (!item.activeSelf)
				return ActivateItem(item, pos, wantedObject.INITIAL_QUANTITY);

		GameObject newItem = CreateItem(wantedObject);
		_itemPools[wantedObject].Add(newItem);
		return ActivateItem(newItem, pos, wantedObject.INITIAL_QUANTITY);
	}
	public GameObject GetObject(Vector3 pos, string wantedObject)
	{
		InventoryItem item = null;
		foreach (var itempool in _itemPools)
			if (itempool.Key.name == wantedObject)
			{
				item = itempool.Key;
				break;
			}

		return GetObject(pos, item);
	}

	private List<GameObject> CreateItems(InventoryItem prefab, int itemCount)
	{
		List<GameObject> items = new List<GameObject>();
		for (int i = 0; i < itemCount; i++)
			items.Add(CreateItem(prefab));
		return items;
	}

	private GameObject CreateItem(InventoryItem item)
	{
		GameObject newItem = null;
		if (item.Item.TryGetComponent<Interactable>(out Interactable interactable))
		{
			newItem = _pickupFactory.Create(item).gameObject;
			newItem.GetComponent<Interactable_Pickup>().IsCrateItem = true;
		}
		else
			newItem = Instantiate(item.Item, new Vector3(-1000, -1000, -1000), Quaternion.identity);


		newItem.transform.SetParent(transform);
		newItem.SetActive(false);
		return newItem;
	}

	private GameObject ActivateItem(GameObject item, Vector3 pos, int initialCount)
	{
		if (item.TryGetComponent<Interactable_Pickup>(out Interactable_Pickup interactable))
		{
			interactable.IsCrateItem = true;
			interactable.SetQuantity(initialCount);
		}
		item.transform.position = pos;
		item.SetActive(true);
		return item;
	}
}
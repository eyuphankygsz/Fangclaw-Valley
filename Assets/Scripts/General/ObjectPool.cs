using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObjectPool : MonoBehaviour
{

	[SerializeField]
	private PoolItem[] _poolItems;

	private Dictionary<PoolItem, List<GameObject>> _itemPools = new Dictionary<PoolItem, List<GameObject>>();

	[Inject]
	readonly PickupFactory _pickupFactory;
	[Inject]
	readonly SaveManager _saveManager;


	private void Start()
	{
		Debug.Log("OBJECTPOOL");
		foreach (var item in _poolItems)
			_itemPools.Add(item, CreateItems(item, item.INITIAL_COUNT));

		var crateItemList = _saveManager.CrateItems();
		if (crateItemList == null) return;
		foreach (var item in crateItemList)
			SetupCrateItem(item);
	}

	public GameObject SetupCrateItem(CrateItem crateItem)
	{
		foreach (var item in _itemPools)
		{
			if (item.Key.Item.GetComponent<Interactable>().InteractableName == crateItem.Name)
				return GetObject(crateItem.Position, item.Key);
		}
		return null;
	}

	public GameObject GetObject(Vector3 pos, PoolItem wantedObject)
	{
		foreach (var item in _itemPools[wantedObject])
			if (!item.activeSelf)
				return ActivateItem(item, pos);

		GameObject newItem = CreateItem(wantedObject);
		_itemPools[wantedObject].Add(newItem);
		return ActivateItem(newItem, pos);
	}

	private List<GameObject> CreateItems(PoolItem prefab, int itemCount)
	{
		List<GameObject> items = new List<GameObject>();
		for (int i = 0; i < itemCount; i++)
			items.Add(CreateItem(prefab));
		return items;
	}

	private GameObject CreateItem(PoolItem item)
	{
		GameObject newItem = _pickupFactory.Create(item).gameObject;
		newItem.GetComponent<Interactable_Pickup>().IsCrateItem = true;
		newItem.transform.SetParent(transform);
		newItem.SetActive(false);
		return newItem;
	}

	private GameObject ActivateItem(GameObject item, Vector3 pos)
	{
		item.GetComponent<Interactable_Pickup>().IsCrateItem = true;
		item.transform.position = pos;
		item.SetActive(true);
		return item;
	}
}
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
	public static ObjectPool Instance;

	[SerializeField]
	private PoolItem[] _poolItems;

	private Dictionary<GameObject, List<GameObject>> _itemPools = new Dictionary<GameObject, List<GameObject>>();


	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(this);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(this);

		foreach (var item in _poolItems)
			_itemPools.Add(item.Item, CreateItems(item.Item, item.INITIAL_COUNT));
	}


	private List<GameObject> CreateItems(GameObject item, int itemCount)
	{
		List<GameObject> items = new List<GameObject>();
		for (int i = 0; i < itemCount; i++)
			items.Add(CreateItem(item));
		return items;
	}

	public GameObject GetObject(Vector3 pos, GameObject wantedObject)
	{
		foreach (var item in _itemPools[wantedObject])
			if (!item.activeSelf)
				return ActivateItem(item, pos);

		GameObject newItem = CreateItem(wantedObject);
		_itemPools[wantedObject].Add(newItem);
		return ActivateItem(newItem, pos);
	}

	private GameObject CreateItem(GameObject original)
	{
		GameObject newItem = Instantiate(original, new Vector3(-1000, -1000, -1000), Quaternion.identity);
		newItem.transform.SetParent(transform);
		newItem.SetActive(false);
		return newItem;
	}
	private GameObject ActivateItem(GameObject item, Vector3 pos)
	{
		item.transform.position = pos;
		item.SetActive(true);
		return item;
	}
}

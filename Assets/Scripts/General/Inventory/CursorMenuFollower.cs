using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorMenuFollower : MonoBehaviour
{
	private InventoryManager _inventoryManager;
	[SerializeField]
	private GameObject[] _functionButtons;
	[SerializeField]
	private EventSystem _eventSystem;
	[SerializeField]
	private GameObject _selectedObj;
	private RectTransform _transform;

	public void Setup(InventoryManager manager)
	{
		_inventoryManager = manager;
		_transform = GetComponent<RectTransform>();
		gameObject.SetActive(false);
	}
	public void SetMenu(InventoryItemHolder item, bool isChestHolder)
	{
		Vector3 pos = item.transform.position;
		_transform.position = pos;

		for (int i = 0; i < _functionButtons.Length; i++)
			_functionButtons[i].SetActive(false);

		foreach (var function in item.Item.ItemFunctions)
		{
			if (function == ItemFunctions.Drop && isChestHolder)
				continue;
			_functionButtons[(int)function].SetActive(true);
		}

		gameObject.SetActive(true);
		_eventSystem.SetSelectedGameObject(_selectedObj);
	}
	private void OnDisable()
	{
		if (_inventoryManager != null)
			_inventoryManager.DisableCursorMenu(true);
		else
			gameObject.SetActive(false);
	}
}

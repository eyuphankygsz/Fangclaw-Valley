using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMenuFollower : MonoBehaviour
{
	private InventoryManager _inventoryManager;
	[SerializeField]
	private GameObject[] _functionButtons;

	private RectTransform _transform;

	public void Setup(InventoryManager manager)
	{
		_inventoryManager = manager;
		_transform = GetComponent<RectTransform>();
		gameObject.SetActive(false);
	}
	public void SetMenu(InventoryItem item, bool isChestHolder)
	{
		Vector3 pos = Input.mousePosition;
		_transform.position = pos;

		for (int i = 0; i < _functionButtons.Length; i++)
			_functionButtons[i].SetActive(false);

		foreach (var function in item.ItemFunctions)
		{
			if (function == ItemFunctions.Drop && isChestHolder)
				continue;
			_functionButtons[(int)function].SetActive(true);
		}

		gameObject.SetActive(true);
	}
	private void OnDisable()
	{
		if (_inventoryManager != null)
			_inventoryManager.DisableCursorMenu(true);
		else
			gameObject.SetActive(false);
	}
}

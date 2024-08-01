using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InventoryItemHolder : MonoBehaviour, ISaveable
{
	[Inject]
	InventoryManager _inventoryManager;

	[field: SerializeField]
	public InventoryItem Item { get; private set; }

	[field: SerializeField]
	public int Quantity { get; private set; }

	[field: SerializeField]
	public int MaxQuantity { get; private set; }

	[SerializeField]
	private GameObject _quantityImage;
	[SerializeField]
	private TextMeshProUGUI _quantityText;


	private InventoryDataItem _inventoryDataItem = new InventoryDataItem();


	public void Setup(InventoryItem item, int quantity)
	{
		Item = item;
		MaxQuantity = item.StackQuantity;
		Quantity = quantity;

		GetComponent<Image>().sprite = item.ItemSprite;
		GetComponent<Button>().onClick.AddListener(OnSelect);
		if (quantity == 1)
			_quantityImage.SetActive(false);
		else
			_quantityText.text = Quantity.ToString();
	}
	public void OnSelect()
	{
		_inventoryManager.SelectItem(this);
	}
	public void AddQuantity(int quantity)
	{
		Quantity += quantity;
		if(Quantity > 1)
		{
			_quantityImage.SetActive(true);
			_quantityText.text = Quantity.ToString();
		}
		else
			_quantityImage.SetActive(false);
	}
	public InventoryDataItem GetSaveData()
	{
		return _inventoryDataItem;
	}

	public GameData GetSaveFile()
	{
		_inventoryDataItem = new InventoryDataItem()
		{
			Name = Item.ItemName,
			Quantity = Quantity,
		};
		return _inventoryDataItem;
	}

	public void SetLoadFile()
	{
		throw new System.NotImplementedException();
	}
}

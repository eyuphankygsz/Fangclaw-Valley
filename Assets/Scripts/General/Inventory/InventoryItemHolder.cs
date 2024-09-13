using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class InventoryItemHolder : MonoBehaviour, ISaveable, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	InventoryManager _inventoryManager;

	[SerializeField]
	private bool _chestHolder;
	[SerializeField]
	private int _id;

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
	[SerializeField]
	private Image _itemImage;
	private Image _holderImage;

	private InventoryDataItem _inventoryDataItem = new InventoryDataItem();

	public bool TempDisable { get; set; }

	[Inject]
	private SaveManager _saveManager;

	private void Awake()
	{
		_holderImage = GetComponent<Image>();
	}

	public void SetInventoryManager(InventoryManager inventoryManager)
		=> _inventoryManager = inventoryManager;

	public void Setup(InventoryManager inventoryManager, InventoryItem item, int quantity)
	{
		if (!_chestHolder)
			_saveManager.AddSaveableObject(gameObject, GetSaveData());

		_itemImage.sprite = item.ItemSprite;
		_itemImage.enabled = true;
		_inventoryManager = inventoryManager;
		Item = item;
		MaxQuantity = item.StackQuantity;
		Quantity = quantity;

		_quantityText.text = Quantity.ToString();
		_quantityImage.SetActive(quantity != 1);
	}
	private void OnDisable()
	{
		if (_chestHolder)
		{
			_inventoryManager.Disable();
			gameObject.SetActive(false);
		}
		if(_inventoryManager != null)
		_holderImage.color = _inventoryManager._pointerExitColor;
	}
	public void ResetHolder()
	{
		_itemImage.sprite = null;
		_itemImage.enabled = false;
		Item = null;
		MaxQuantity = 0;
		Quantity = 0;
		_quantityImage.SetActive(false);
		TempDisable = false;
	}
	public void SetTemporaryStatus(bool isEnable)
	{
		TempDisable = !isEnable;
		_itemImage.enabled = isEnable;
		if (Quantity == 1)
			_quantityImage.SetActive(false && isEnable);
		else
		{
			_quantityText.text = Quantity.ToString();
			_quantityImage.SetActive(true && isEnable);
		}
	}
	public void AddQuantity(int quantity)
	{
		Quantity += quantity;

		if (Quantity == 0)
			ResetHolder();

		if (Quantity > 1)
		{
			_quantityImage.SetActive(true);
			_quantityText.text = Quantity.ToString();
		}
		else
			_quantityImage.SetActive(false);
		_quantityText.text = Quantity.ToString();
	}
	public InventoryDataItem GetSaveData()
	{
		return _inventoryDataItem;
	}

	public GameData GetSaveFile()
	{
		if (_chestHolder) return null;
		_inventoryDataItem = new InventoryDataItem()
		{
			Name = Item.Name,
			Quantity = Quantity,
			ID = _id
		};
		return _inventoryDataItem;
	}

	public void SetLoadFile()
	{
		throw new System.NotImplementedException();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		ChangeColor(false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		ChangeColor(true);
	}

	private void ChangeColor(bool on)
	{
		if (_holderImage == null)
			_holderImage = GetComponent<Image>();
		_holderImage.color = on ? _inventoryManager._pointerEnterColor : _inventoryManager._pointerExitColor;
	}

	public void OnPointerUp(PointerEventData eventData)
	{

	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
			_inventoryManager.HandleLeftClick(this);
		else if (eventData.button == PointerEventData.InputButton.Right)
			_inventoryManager.HandleRightClick(this);
	}
}

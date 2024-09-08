using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class InventoryItemHolder : MonoBehaviour, ISaveable, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	InventoryManager _inventoryManager;

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
		_saveManager.AddSaveableObject(gameObject, GetSaveData());
		_itemImage.sprite = item.ItemSprite;
		_itemImage.enabled = true;
		_inventoryManager = inventoryManager;
		Item = item;
		MaxQuantity = item.StackQuantity;
		Quantity = quantity;

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
		Debug.Log(_holderImage);
		Debug.Log(_inventoryManager);

		if (_holderImage == null) _holderImage = GetComponent<Image>();
		_holderImage.color = on ? _inventoryManager._pointerEnterColor : _inventoryManager._pointerExitColor;

	}

	public void OnPointerUp(PointerEventData eventData)
	{

	}

	public void OnPointerDown(PointerEventData eventData)
	{
		_inventoryManager.HandleClick(this);
	}
}

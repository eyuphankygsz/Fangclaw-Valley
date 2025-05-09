using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CursorItemHolder : MonoBehaviour
{
    [SerializeField]
    private Image _itemImage;
	[SerializeField]
	private TextMeshProUGUI _quantityText; 
    [SerializeField]
	private GameObject _quantityImage;

	private RectTransform _rectTransform;

	private void Start()
	{
		_rectTransform = GetComponent<RectTransform>();
        SetActive(false);
	}
	public void Setup(InventoryItemHolder holder)
	{
		if (holder.Item == null)
		{
			SetActive(false);
			return;
		}
		Vector3 mousePos = Input.mousePosition;
		_rectTransform.position = mousePos;
		
		_itemImage.sprite = holder.Item.ItemSprite;
        _quantityText.text = holder.Quantity.ToString();
        _quantityImage.SetActive(holder.Quantity != 1);
        
		SetActive(true);
    }
	private void OnDisable()
	{
		SetActive(false);
	}
	public void SetActive(bool on)
    {
        gameObject.SetActive(on);
    }
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        _rectTransform.position = mousePos;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CursorItemHolder : MonoBehaviour
{
    [SerializeField]
    private Image _itemImage;
    [SerializeField]
    private TextMeshProUGUI _quantityText;
    
    private RectTransform _rectTransform;

	private void Start()
	{
		_rectTransform = GetComponent<RectTransform>();
        SetActive(false);
	}
	public void Setup(InventoryItemHolder holder)
    {
        _itemImage.sprite = holder.Item.ItemSprite;
        _quantityText.text = holder.Quantity.ToString();
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

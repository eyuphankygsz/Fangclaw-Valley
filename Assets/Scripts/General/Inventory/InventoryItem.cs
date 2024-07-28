using UnityEngine;

[CreateAssetMenu(fileName ="New Inventory Item", menuName = "Inventory Items/New Inventory Item")]
public class InventoryItem : ScriptableObject
{
	public string ItemName;
	[TextArea(10,40)]
	public string ItemDescription;
	public Sprite ItemSprite;
	public int StackQuantity;
	public bool OneTime;
}

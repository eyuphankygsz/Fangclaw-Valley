using UnityEngine;

[CreateAssetMenu(fileName ="New Inventory Item", menuName = "Inventory Items/New Inventory Item")]
public class InventoryItem : ScriptableObject
{
	public string ItemName;
	public Sprite ItemSprite;
	public int StackQuantity;
}

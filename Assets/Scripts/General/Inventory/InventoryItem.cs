using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName ="New Inventory Item", menuName = "Inventory Items/New Inventory Item")]
public class InventoryItem : ScriptableObject
{
	public string Name;
	public LocalizedString ItemName;
	public LocalizedString ItemDescription;
	public Sprite ItemSprite;
	public int StackQuantity;
	public bool OneTime;
}

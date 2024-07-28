using UnityEngine;

public class Interactable_Pickup : Interactable
{
	[field: SerializeField]
	public InventoryItem Item { get; private set; }
	[SerializeField]
	private int _quantity;

	public override void OnInteract(Enum_Weapons weapon)
	{
		InventoryManager.Instance.AddItemToInventory(Item, _quantity);
		gameObject.SetActive(false);
	}

	public override InteractableData SaveData()
	{
		return new PickupData
		{
			InteractableName = InteractableName,
			Position = transform.position,
			IsActive = gameObject.activeSelf,
			IsPickedUp = !gameObject.activeSelf
		};
	}

	public override void LoadData()
	{
		PickupData data = (PickupData)SaveManager.Instance.GetData(InteractableName, SaveType.Pickup);
		if (data == null) return;

		transform.position = data.Position;
		gameObject.SetActive(data.IsActive);
	}


}

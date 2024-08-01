using UnityEngine;
using Zenject;

public class Interactable_Pickup : Interactable
{
	[Inject]
	private InventoryManager _inventoryManager;


	public bool IsCrateItem;

	[field: SerializeField]
	public InventoryItem Item { get; set; }
	[SerializeField]
	private int _quantity;

	private PickupData _pickupData = new PickupData();
	private CrateItem _crateItemData = new CrateItem();

	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);

		_inventoryManager.AddItemToInventory(Item, _quantity);
		gameObject.SetActive(false);

	}

	public override GameData GetGameData()
	{
		Vector3 pos = transform.position;
		if (IsCrateItem)
		{
			_crateItemData = new CrateItem
			{
				Name = InteractableName,
				Position = pos,
				Taken = !gameObject.activeSelf,
			};
			return _crateItemData;
		}
		else
		{
			_pickupData = new PickupData
			{
				Name = InteractableName,
				IsPickedUp = !gameObject.activeSelf,
			};
			return _pickupData;
		}
	}

	public override void LoadData()
	{
		if (!IsCrateItem)
		{
			PickupData data = _saveManager.GetData<PickupData>(InteractableName);
			if (data == null) return;

			gameObject.SetActive(!data.IsPickedUp);
		}
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
	}
}
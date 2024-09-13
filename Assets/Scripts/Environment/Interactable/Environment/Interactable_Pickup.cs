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

		_inventoryManager.AddItemToInventory(Item, _quantity, this);
		if (!_used)
			OneTimeEvent();
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
				LeftQuantity = _quantity,
				Position = pos,
			};
			return _pickupData;
		}
	}
	public void AddQuantity(int quantity)
	{
		_quantity += quantity;
	}
	public override void LoadData()
	{
		if (!IsCrateItem)
		{
			PickupData data = _saveManager.GetData<PickupData>(InteractableName);
			if (data == null) return;

			gameObject.transform.position = data.Position;
			gameObject.SetActive(!data.IsPickedUp);
			_quantity = data.LeftQuantity;
			if (data.IsPickedUp)
				OneTimeEvent();
		}
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
	}
	public void SetQuantity(int quantity)
	{
		_quantity = quantity;
	}


}





using Zenject;

public class PickupFactory : PlaceholderFactory<InventoryItem, Interactable_Pickup>
{
	private readonly DiContainer _container;

	public PickupFactory(DiContainer container)
	{
		_container = container;
	}

	public override Interactable_Pickup Create(InventoryItem data)
	{
		var pickupPrefab = data.Item;
		var pickupInstance = _container.InstantiatePrefabForComponent<Interactable_Pickup>(pickupPrefab);
		return pickupInstance;
	}
}
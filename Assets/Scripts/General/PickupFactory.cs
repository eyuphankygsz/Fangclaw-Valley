using Zenject;

public class PickupFactory : PlaceholderFactory<PoolItem, Interactable_Pickup>
{
	private readonly DiContainer _container;

	public PickupFactory(DiContainer container)
	{
		_container = container;
	}

	public override Interactable_Pickup Create(PoolItem data)
	{
		var pickupPrefab = data.Item;
		var pickupInstance = _container.InstantiatePrefabForComponent<Interactable_Pickup>(pickupPrefab);
		return pickupInstance;
	}
}
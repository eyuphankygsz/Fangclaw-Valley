using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
	[SerializeField] private InventoryManager _inventoryManager;
	[SerializeField] private ObjectPool _objectPool;
	public override void InstallBindings()
	{
		Container.Bind<GameManager>().AsSingle();
		Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();
		Container.BindInterfacesAndSelfTo<SaveManager>().AsSingle();
		Container.Bind<ObjectPool>().FromInstance(_objectPool).AsSingle();
		Container.Bind<InventoryManager>().FromInstance(_inventoryManager).AsSingle();
		Container.BindFactory<PoolItem, Interactable_Pickup, PickupFactory>().AsSingle();
	}
}

public class PickupSpawner : MonoBehaviour
{
	[Inject] private PickupFactory _pickupFactory;

	public void SpawnPickup(PoolItem data)
	{
		Interactable_Pickup pickup = _pickupFactory.Create(data);
		// Set the position, parent, or any other properties as needed
	}
}
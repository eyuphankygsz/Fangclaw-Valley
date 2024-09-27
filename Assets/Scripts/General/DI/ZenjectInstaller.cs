using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
	[SerializeField] private InventoryManager _inventoryManager;
	[SerializeField] private ObjectPool _objectPool;
	[SerializeField] private PlayerUI _playerUI;
	[SerializeField] private GameTime _gameTime;
	public override void InstallBindings()
	{
		Container.Bind<GameManager>().AsSingle();
		Container.Bind<WeaponHelpers>().FromNewComponentOnNewGameObject().AsSingle();
		Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();
		Container.BindInterfacesAndSelfTo<SaveManager>().AsSingle();
		Container.Bind<GameTime>().FromInstance(_gameTime).AsSingle();
		Container.Bind<PlayerUI>().FromInstance(_playerUI).AsSingle();
		Container.Bind<ObjectPool>().FromInstance(_objectPool).AsSingle();
		Container.Bind<InventoryManager>().FromInstance(_inventoryManager).AsSingle();
		Container.BindFactory<InventoryItem, Interactable_Pickup, PickupFactory>().AsSingle();
	}
}

public class PickupSpawner : MonoBehaviour
{
	[Inject] private PickupFactory _pickupFactory;

	public void SpawnPickup(InventoryItem data)
	{
		Interactable_Pickup pickup = _pickupFactory.Create(data);
		// Set the position, parent, or any other properties as needed
	}
}
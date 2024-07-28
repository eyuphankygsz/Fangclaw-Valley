using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager
{
	private static SaveManager _instance;
	public static SaveManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new SaveManager();
				_instance.Setup();
			}

			return _instance;
		}
	}

	private List<Interactable> _interactables = new List<Interactable>();
	private List<Interactable> _crateItems = new List<Interactable>();

	private List<InteractableData> _savedInteractables = new List<InteractableData>();
	private PlayerData _playerData;
	private InventoryData _inventoryData;
	private CrateItemsData _crateItemsData;

	private readonly string _interactablesDataPath = Path.Combine(Application.persistentDataPath, "interactables.json");
	private readonly string _playerDataPath = Path.Combine(Application.persistentDataPath, "player.json");
	private readonly string _inventoryDataPath = Path.Combine(Application.persistentDataPath, "inventory.json");
	private readonly string _crateItemsDataPath = Path.Combine(Application.persistentDataPath, "crateitems.json");


	private void Setup()
	{
		Debug.Log(_crateItemsDataPath);
		Load();
	}

	public void AddInteractable(Interactable item)
	{
		_interactables.Add(item);
	}
	public void AddCrateItem(Interactable item)
	{
		_crateItems.Add(item);
	}

	public void SaveGame()
	{
		SaveInteractables();
		SavePlayer();
		SaveInventory();
		SaveCrateItems();
	}
	private void SaveInteractables()
	{
		_savedInteractables.Clear();
		foreach (Interactable item in _interactables)
			_savedInteractables.Add(item.SaveData());

		List<string> jsonDataList = new List<string>();


		foreach (var interactable in _savedInteractables)
		{
			string jsonData = "";
			if (interactable is PickupData)
			{
				jsonData = JsonUtility.ToJson((PickupData)interactable, true);
			}
			else if (interactable is HingedData)
			{
				jsonData = JsonUtility.ToJson((HingedData)interactable, true);
			}
			else if (interactable is CrateData)
			{
				jsonData = JsonUtility.ToJson((CrateData)interactable, true);
			}
			else
			{
				jsonData = JsonUtility.ToJson(interactable, true);
			}

			jsonDataList.Add(jsonData);
		}

		InteractableDataWrapper dataWrapper = new InteractableDataWrapper(jsonDataList);
		string json = JsonUtility.ToJson(dataWrapper, true);
		File.WriteAllText(_interactablesDataPath, json);
	}
	private void SavePlayer()
	{
		_playerData = null;

		var player = PlayerController.Instance;
		PlayerData data = new PlayerData
		{
			Position = player.transform.position,
		};

		var json = JsonUtility.ToJson(data, true);
		File.WriteAllText(_playerDataPath, json);
	}

	private void SaveInventory()
	{
		List<InventoryItemHolder> holders = InventoryManager.Instance.GetHolders();
		InventoryData data = new InventoryData
		{
			Items = holders.Select(holder =>
									 new InventoryDataItem
									 {
										 ItemName = holder.Item.ItemName,
										 Quantity = holder.Quantity
									 })
								   .ToList()
		};

		string json = JsonUtility.ToJson(data, true);
		File.WriteAllText(_inventoryDataPath, json);
	}
	private void SaveCrateItems()
	{
		CrateItemsData data = new CrateItemsData
		{
			Items = _crateItems.Select(interactable =>
										  new CrateItem
										  {
											  ItemName = interactable.InteractableName,
											  Position = interactable.transform.position,
											  Taken = !interactable.gameObject.activeSelf
										  })
							   .ToList()
		};

		string json = JsonUtility.ToJson(data, true);
		File.WriteAllText(_crateItemsDataPath, json);
	}
	public void Load()
	{
		LoadInteractables();
		LoadPlayer();
		LoadInventory();
	}
	public void LoadSecondary()
	{
		LoadCrateItems();
	}
	private void LoadInteractables()
	{
		if (File.Exists(_interactablesDataPath))
		{
			string json = File.ReadAllText(_interactablesDataPath);
			InteractableDataWrapper dataWrapper = JsonUtility.FromJson<InteractableDataWrapper>(json);
			List<InteractableData> interactableDataList = new List<InteractableData>();

			foreach (string jsonData in dataWrapper.JsonDataList)
			{
				InteractableData baseData = JsonUtility.FromJson<InteractableData>(jsonData);
				if (jsonData.Contains("\"IsPickedUp\""))
				{
					PickupData pickupData = JsonUtility.FromJson<PickupData>(jsonData);
					interactableDataList.Add(pickupData);
				}
				else if (jsonData.Contains("\"IsOn\""))
				{
					HingedData hingedData = JsonUtility.FromJson<HingedData>(jsonData);
					interactableDataList.Add(hingedData);
				}
				else if (jsonData.Contains("\"IsShattered\""))
				{
					CrateData crateData = JsonUtility.FromJson<CrateData>(jsonData);
					interactableDataList.Add(crateData);
				}
				else
				{
					interactableDataList.Add(baseData);
				}
			}

			_savedInteractables = interactableDataList;
		}
	}
	private void LoadPlayer()
	{
		if (File.Exists(_playerDataPath))
		{
			string json = File.ReadAllText(_playerDataPath);
			_playerData = JsonUtility.FromJson<PlayerData>(json);
		}
	}
	private InventoryData LoadInventory()
	{
		if (File.Exists(_inventoryDataPath))
		{
			string json = File.ReadAllText(_inventoryDataPath);
			_inventoryData = JsonUtility.FromJson<InventoryData>(json);
		}

		return _inventoryData;
	}
	private void LoadCrateItems()
	{
		if (File.Exists(_crateItemsDataPath) && ObjectPool.Instance != null)
		{
			string json = File.ReadAllText(_crateItemsDataPath);
			_crateItemsData = JsonUtility.FromJson<CrateItemsData>(json);

            foreach (var item in _crateItemsData.Items)
            {
				if (item.Taken) continue;
				GameObject crateItem = ObjectPool.Instance.SetupCrateItem(item);
				_crateItems.Add(crateItem.GetComponent<Interactable>());
            }
        }
	}
	public object GetData(string itemName, SaveType type)
	{
		switch (type)
		{
			case SaveType.Pickup:
				return (PickupData)_savedInteractables.FirstOrDefault(idl => idl.InteractableName == itemName);
			case SaveType.Hinged:
				return (HingedData)_savedInteractables.FirstOrDefault(idl => idl.InteractableName == itemName);
			case SaveType.Crate:
				return (CrateData)_savedInteractables.FirstOrDefault(idl => idl.InteractableName == itemName);
			case SaveType.Player:
				return _playerData;
			case SaveType.Inventory:
				return _inventoryData;
			default:
				return _savedInteractables.FirstOrDefault(idl => idl.InteractableName == itemName);
		}
	}
}


[System.Serializable]
public class InteractableDataWrapper
{
	public List<string> JsonDataList;

	public InteractableDataWrapper(List<string> jsonDataList)
	{
		JsonDataList = jsonDataList;
	}
}


public enum SaveType
{
	Pickup,
	Hinged,
	Crate,
	Player,
	Inventory
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Zenject;
using Newtonsoft.Json.Linq;
public class SaveManager : IInitializable
{
	[Inject]
	private GameManager _gameManager;
	private Dictionary<Type, List<GameData>> _savedData = new Dictionary<Type, List<GameData>>();
	private Dictionary<Type, List<GameObject>> _saveableObjects = new Dictionary<Type, List<GameObject>>();

	private readonly string _savePath = Path.Combine(Application.persistentDataPath, "save.json");


	private Coroutine _saveRoutine;

	private JsonSerializerSettings _jsonSettings;
	public void Initialize()
	{
		Setup();
	}
	private void Setup()
	{
		_jsonSettings = new JsonSerializerSettings
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			TypeNameHandling = TypeNameHandling.All,
			Formatting = Formatting.Indented
		};
		Load();
	}


	public void AddSaveableObject(GameObject obj, GameData dataClass)
	{
		if (dataClass == null) return;
		var dataType = dataClass.GetType();

		if (!_saveableObjects.ContainsKey(dataType))
			_saveableObjects.Add(dataType, new List<GameObject>());

		_saveableObjects[dataType].Add(obj);

	}
	public bool HasItem(GameObject obj, GameData dataClass)
	{
		if (dataClass == null)
			return true;
		var dataType = dataClass.GetType();

		if (!_saveableObjects.ContainsKey(dataType))
			_saveableObjects.Add(dataType, new List<GameObject>());

		return _saveableObjects[dataType].Any(x => x == obj);
	}
	private void PauseGame(bool stop)
	{
		_gameManager.SetSaveGame(stop);
		Time.timeScale = stop ? 0 : 1;
	}
	public IEnumerator SaveGame(Action onComplete)
	{
		PauseGame(true);
		SaveData();
		yield return new WaitForSecondsRealtime(1);
		PauseGame(false);
		onComplete?.Invoke();
	}
	private void SaveData()
	{
		_savedData.Clear();
		foreach (var item in _saveableObjects)
		{
			var dataType = item.Key;
			foreach (var saveableObj in item.Value)
			{
				if (saveableObj == null) continue;
				var saveable = saveableObj.GetComponent<ISaveable>();
				if (saveable != null)
				{
					var data = saveable.GetSaveFile();
					if (!_savedData.ContainsKey(dataType))
						_savedData.Add(dataType, new List<GameData>());

					_savedData[dataType].Add(data);
				}
			}
		}

		DataWrapper dataWrapper = new DataWrapper(_savedData);
		string json = JsonConvert.SerializeObject(dataWrapper, _jsonSettings);
		File.WriteAllText(_savePath, json);
	}

	private void Load()
	{
		LoadData();
	}
	public List<CrateItem> CrateItems()
	{
		var list = GetDataList<CrateItem>();
		return list;
	}
	private void LoadData()
	{
		if (File.Exists(_savePath))
		{
			string json = File.ReadAllText(_savePath);
			DataWrapper dataWrapper = JsonConvert.DeserializeObject<DataWrapper>(json, _jsonSettings);

			if (dataWrapper?.Entries == null)
				return;

			_savedData.Clear();
			foreach (var entry in dataWrapper.Entries)
			{
				var type = Type.GetType(entry.TypeName);
				if (type != null)
				{
					_savedData[type] = entry.Data;
				}
			}
		}
	}


	public T GetData<T>(string itemName) where T : GameData
	{
		var type = typeof(T);
		if (!_savedData.ContainsKey(type)) return null;
		List<GameData> dataSection = _savedData[type];

		return dataSection
				   .Where(data => data.Name == itemName)
				   .FirstOrDefault() as T;
	}
	public List<T> GetDataList<T>() where T : GameData
	{
		var type = typeof(T);
		if (!_savedData.ContainsKey(type))
			return null;

		var data = _savedData[type].OfType<T>().ToList();
		return data;
	}
}


[System.Serializable]
public class DataWrapper
{
	public List<DataEntry> Entries;

	public DataWrapper(Dictionary<Type, List<GameData>> dataList)
	{
		if (dataList == null)
		{
			Entries = new List<DataEntry>();
			return;
		}

		Entries = dataList
			.Select(entry => new DataEntry
			{
				TypeName = entry.Key.AssemblyQualifiedName,
				Data = entry.Value
			}).ToList();
	}

	[System.Serializable]
	public class DataEntry
	{
		public string TypeName;
		public List<GameData> Data;
	}
}

public class Vector3Converter : JsonConverter<Vector3>
{
	public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
	{
		writer.WriteStartObject();
		writer.WritePropertyName("x");
		writer.WriteValue(value.x);
		writer.WritePropertyName("y");
		writer.WriteValue(value.y);
		writer.WritePropertyName("z");
		writer.WriteValue(value.z);
		writer.WriteEndObject();
	}
	public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		var jObject = JObject.Load(reader);
		var x = jObject["x"].Value<float>();
		var y = jObject["y"].Value<float>();
		var z = jObject["z"].Value<float>();
		return new Vector3(x, y, z);
	}
}
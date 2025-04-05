using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

public class QuestManager : MonoBehaviour
{
	[Header("Graphical")]
	[SerializeField]
	private CanvasGroup _group;
	[SerializeField]
	private Transform _questHolder;
	[SerializeField]
	private GameObject _questPrefab;
	[SerializeField]
	private GameObject _additionPrefab;

	[Header("Sound")]
	[SerializeField]
	private AudioSource _audioSource;
	[SerializeField]
	private AudioClip _doneClip, _addClip;


	private bool _opening;
	private Coroutine _alphaRoutine;

	private float _currentAlpha;
	private List<Quest> _allQuestList;
	private List<Quest> _currentQuests;
	private List<Transform> _currentQuestsObjects;

	private Dictionary<Type, List<GameData>> _savedData = new Dictionary<Type, List<GameData>>();
	private string _savePath = Path.Combine(Application.dataPath, "quests.json");
	private readonly string _initJson = "quests/questsinit";
	private JsonSerializerSettings _jsonSettings;

	[Inject]
	private GameManager _manager;
	private void Start()
	{
		_group.alpha = 0;
		_currentAlpha = 0;
		Setup();
	}

	private void Setup()
	{
		string drName = Path.GetDirectoryName(_savePath);

		int length = drName.Length;
		while (length != 0)
		{
			if (drName[length - 1] == '\\')
				break;

			length--;
		}
		_savePath = _savePath.Substring(0, length) + "\\SavesDir\\quests.json";

		//GameObject.FindWithTag("dataPath").GetComponent<TextMeshProUGUI>().text = _savePath;

		_jsonSettings = new JsonSerializerSettings
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			TypeNameHandling = TypeNameHandling.All,
			Formatting = Formatting.Indented
		};
		Load();
	}
	private void Load()
	{
		LoadData();
	}

	private void LoadData()
	{
		Type type = typeof(Quest);
		if (File.Exists(_savePath))
		{
			string json = File.ReadAllText(_savePath);
			DataWrapper dataWrapper = JsonConvert.DeserializeObject<DataWrapper>(json, _jsonSettings);

			if (dataWrapper?.Entries == null)
			{
				Debug.Log("ENTRIES ARE NULL");
				return;
			}

			_savedData.Clear();
			int i = 0;
			foreach (var entry in dataWrapper.Entries)
			{
				type = Type.GetType(entry.TypeName);
				if (type != null)
				{
					_savedData[type] = entry.Data;
				}
			}
			Debug.Log(_savedData);
		}
		else
		{
			string json = Resources.Load<TextAsset>(_initJson).ToString();

			DataWrapper dataWrapper = JsonConvert.DeserializeObject<DataWrapper>(json, _jsonSettings);

			_savedData.Clear();
			foreach (var entry in dataWrapper.Entries)
			{
				type = Type.GetType(entry.TypeName);
				if (type != null)
				{
					_savedData[type] = entry.Data;
				}
			}
			Debug.Log(_savedData);
			File.WriteAllText(_savePath, json);
		}

		_currentQuests = new List<Quest>();
		_currentQuestsObjects = new List<Transform>();
		ConvertAndSort(type);
	}

	public void SaveData()
	{
		_savedData.Clear();
		var dataType = typeof(Quest);
		foreach (var quest in _allQuestList)
		{
			if (!_savedData.ContainsKey(dataType))
				_savedData.Add(dataType, new List<GameData>());

			_savedData[dataType].Add(quest);
		}

		DataWrapper dataWrapper = new DataWrapper(_savedData);
		string json = JsonConvert.SerializeObject(dataWrapper, _jsonSettings);
		File.WriteAllText(_savePath, json);
	}

	public void ConvertAndSort(Type type)
	{
		_allQuestList = new List<Quest>();
		foreach (var item in _savedData[type])
		{
			_allQuestList.Add((Quest)item);
		}

		_allQuestList.Sort((x, y) => x.QuestID.CompareTo(y.QuestID));

		foreach (var item in _allQuestList)
		{
			if (item.QuestStatus == 1)
				AddQuest(item);
		}
	}
	public void AddQuest(int id)
	{
		AddQuest(_allQuestList.First(x => x.QuestID == id));
	}
	public void AddQuest(Quest quest)
	{
		if (quest.QuestStatus == 2)
			return;

		quest.QuestStatus = 1;


		Transform questObj = Instantiate(_questPrefab, _questHolder).transform;
		_currentQuests.Add(quest);
		_currentQuestsObjects.Add(questObj);

		//Headline
		LocalizedString headline = new LocalizedString() { TableReference = "Quests", TableEntryReference = quest.QuestName };
		questObj.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = headline.GetLocalizedString();


		//Descriptions
		int i = 0;
		foreach (var item in quest.Description)
		{
			LocalizedString desc = new LocalizedString() { TableReference = "Quests", TableEntryReference = quest.Description[i] };
			Transform addition = Instantiate(_additionPrefab, questObj).transform;

			addition.GetChild(0).GetComponent<TextMeshProUGUI>().text = desc.GetLocalizedString();

			i++;
		}

		PlayClip(_addClip);
		TryOpenAlpha(true);
	}
	public void RemoveQuest(int id)
	{
		ShowQuests();
		for (int i = 0; i < _currentQuests.Count; i++)
		{
			if (_currentQuests[i].QuestID == id)
			{
				var gonnaDelete = _currentQuestsObjects[i];

				_currentQuestsObjects.RemoveAt(i);
				_currentQuests.RemoveAt(i);

				Destroy(gonnaDelete.gameObject);
				PlayClip(_doneClip);
				return;
			}
		}

	}

	private void PlayClip(AudioClip src)
	{
		_audioSource.clip = _doneClip;
		StartCoroutine(WaitForResumeGame());
	}

	private IEnumerator WaitForResumeGame()
	{
		yield return new WaitUntil(() => !_manager.PauseGame);
		_audioSource.Play();
	}

	private void TryOpenAlpha(bool on)
	{
		if (_opening || (on && _group.alpha == 1) || (!on && _group.alpha == 0))
			return;

		_opening = true;
		if (_alphaRoutine != null)
			StopCoroutine(_alphaRoutine);

		_alphaRoutine = StartCoroutine(OpenAlpha(on));
	}
	public void ShowQuests()
	{
		_group.alpha = 0.9f;
		_opening = false;
		TryOpenAlpha(true);
	}

	private IEnumerator OpenAlpha(bool on)
	{
		while ((on && _currentAlpha < 1) || (!on && _currentAlpha > 0))
		{
			_currentAlpha = _group.alpha;
			_currentAlpha += Time.deltaTime * (on ? 3 : -1) * 0.4f;
			_group.alpha = _currentAlpha;
			yield return null;
		}
		_opening = false;
		if (on)
		{
			yield return new WaitForSeconds(3);
			TryOpenAlpha(false);
		}
	}
}

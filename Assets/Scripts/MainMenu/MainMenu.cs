using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class MainMenu : MonoBehaviour
{
	[SerializeField]
	private string[] _resetPrefNames;


	private string _savePath = Path.Combine(Application.dataPath, "save.json");
	private string _questsPath = Path.Combine(Application.dataPath, "quests.json");
	private JsonSerializerSettings _jsonSettings;


	[SerializeField]
	private GameObject _checkMenu;
	[SerializeField]
	private Button _yesButton, _noButton;
	[SerializeField]
	private TextMeshProUGUI _checkText;
	[SerializeField]
	private LocalizedString _deleteSaveGame;
	[SerializeField]
	private LocalizedString _no, _yes;

	[SerializeField]
	private EventSystem _eventSystem;


	[SerializeField]
	private Button _continueButton;

	[SerializeField]
	private List<AchievementCheck> _achievementChecks;



	private void Awake()
	{
		_savePath  = SetDirectoryName(_savePath);
		_questsPath = SetDirectoryName(_questsPath);

		if (!Directory.Exists(_savePath))
			Directory.CreateDirectory(_savePath);

		_savePath += "\\save.json";
		_questsPath += "\\quests.json";


		_jsonSettings = new JsonSerializerSettings
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			TypeNameHandling = TypeNameHandling.All,
			Formatting = Formatting.Indented
		};
		Debug.Log(_savePath);

		if (File.Exists(_savePath))
		{
			_continueButton.interactable = true;
			_continueButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
			_eventSystem.SetSelectedGameObject(_continueButton.gameObject);
		}

	}
	private string SetDirectoryName(string original)
	{
		string drName = Path.GetDirectoryName(original);

		int length = drName.Length;
		while (length != 0)
		{
			if (drName[length - 1] == '\\')
				break;

			length--;
		}
		original = original.Substring(0, length - 1) + "\\SavesDir";
		return original;
	}
	public void ContinueGame()
	{
		PlayerPrefs.SetString("SceneToLoad", "GameScene");
		SceneManager.LoadScene("LoadingScreen");
	}



	public void StartNewGame()
	{
		_eventSystem.SetSelectedGameObject(null);

		if (File.Exists(_savePath))
		{
			_checkText.text = _deleteSaveGame.GetLocalizedString();

			_yesButton.GetComponentInChildren<TextMeshProUGUI>().text = _yes.GetLocalizedString();
			_yesButton.onClick.RemoveAllListeners();
			_yesButton.onClick.AddListener(() => PrepareNewGame());

			_noButton.GetComponentInChildren<TextMeshProUGUI>().text = _no.GetLocalizedString();
			_noButton.onClick.RemoveAllListeners();
			_noButton.onClick.AddListener(() => _checkMenu.SetActive(false));

			_checkMenu.SetActive(true);

			if (InputDeviceManager.Instance.CurrentDevice == InputDeviceManager.InputDeviceType.Gamepad)
				_eventSystem.SetSelectedGameObject(_noButton.gameObject);

		}
		else
			PrepareNewGame();

	}
	private void PrepareNewGame()
	{
		if (File.Exists(_savePath))
		{
			File.Delete(_savePath);
			foreach (var item in _resetPrefNames)
				PlayerPrefs.DeleteKey(item);
		}
		if (File.Exists(_questsPath))
			File.Delete(_questsPath);

		foreach (var item in _achievementChecks)
		{
			if (item.IsInt)
			{
				if (!string.IsNullOrEmpty(item.CurrentInt))
					PlayerPrefs.DeleteKey(item.CurrentInt);
			}
			else if (item.IsFloat)
			{
				if (!string.IsNullOrEmpty(item.CurrentFloat))
					PlayerPrefs.DeleteKey(item.CurrentFloat);
			}
			else if (item.IsString)
			{
				if (!string.IsNullOrEmpty(item.CurrentString))
					PlayerPrefs.DeleteKey(item.CurrentString);
			}
		}

		PlayerPrefs.SetString("SceneToLoad", "GameScene");
		SceneManager.LoadScene("LoadingScreen");
	}

	[SerializeField]
	private MenuSelector _menuSelector;
	public void Options()
	{
		_menuSelector.OpenMenu(1);
	}



	public void ExitGame() =>
		Application.Quit();

}

using Newtonsoft.Json;
using System.Collections;
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
	private EventSystem _eventSystem;


	[SerializeField]
	private Button _continueButton;





	private void Awake()
	{
		string drName = Path.GetDirectoryName(_savePath);

		int length = drName.Length;
		while (length != 0)
		{
			if (drName[length - 1] == '\\')
				break;

			length--;
		}
		_savePath = _savePath.Substring(0, length) + "\\SavesDir\\save.json";

		//GameObject.FindWithTag("dataPath").GetComponent<TextMeshProUGUI>().text = _savePath;

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

			_yesButton.onClick.RemoveAllListeners();
			_yesButton.onClick.AddListener(() => PrepareNewGame());

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

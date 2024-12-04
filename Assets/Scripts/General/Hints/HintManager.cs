using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Searcher;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.UI;
using Zenject;

public class HintManager : MonoBehaviour
{
	[Inject]
	private GameManager _gameManager;

	public List<Hint> HintList = new List<Hint>();

	private JsonSerializerSettings _jsonSettings;

	[SerializeField]
	private GameObject _hintHolder;
	[SerializeField]
	private Image _hPhoto;
	[SerializeField]
	private TextMeshProUGUI _hText;
	private LocalizedString _localizedText;

	[SerializeField]
	private Animator _animator;
	[SerializeField]
	private TextMeshProUGUI _hSkipText;

	private AudioSource _src;

	private WaitForSecondsRealtime _hSkipTime = new WaitForSecondsRealtime(2f);

	public bool HintShow { get; private set; }

	[SerializeField]
	private List<KeyNames> _keyNames;
	[SerializeField]
	private List<string> _hintsInLine = new List<string>();
	bool _hintFree;

	private void Awake()
	{
		_src = GetComponent<AudioSource>();
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
		string json = Resources.Load<TextAsset>("texts/hints").ToString();
		JsonConvert.DeserializeObject<HintWrapper>(json);

		HintWrapper dataWrapper = JsonConvert.DeserializeObject<HintWrapper>(json, _jsonSettings);
		if (dataWrapper?.Hints == null)
			return;

		HintList.Clear();
		foreach (var entry in dataWrapper.Hints)
		{
			HintList.Add(new Hint() { HintName = entry.HintName, HintPhoto = entry.HintPhoto, HintText = entry.HintText });
			Debug.Log(entry.HintName);
		}
	}
	public void ShowHint(string hintName)
	{
		_hintsInLine.Add(hintName);

		if (!_hintFree)
		{
			_hintFree = true;
			DisplayHint();
		}
	}
	private void DisplayHint()
	{
		string hintName = _hintsInLine[0];
		_hintsInLine.RemoveAt(0);

		_animator.ResetTrigger("Close");
		_animator.Play("OpenAlpha");

		HintShow = true;
		_src.Play();
		Hint hint = HintList.Where(x => x.HintName == hintName).FirstOrDefault();
		if (hint == null) { Debug.Log("Can't Find The Hint"); return; }

		_hPhoto.sprite = Resources.Load<Sprite>(hint.HintPhoto);
		if (_hPhoto.sprite == null) { Debug.Log("Nub"); }

		_localizedText = new LocalizedString() { TableReference = "Hints", TableEntryReference = hint.HintText };
		_hText.text = _localizedText.GetLocalizedString();

		List<string> controls = new List<string>();
		foreach (var key in _keyNames)
		{

			if (!_hText.text.Contains(key.KeyName))
				continue;

			controls.Clear();
			InputAction act = key.Action.action;

			for (int i = 0; i < act.bindings.Count; i++)
			{
				int leng = 0;
				string displayStr = act.bindings[i].effectivePath;
				for (int j = 0; j < displayStr.Length; j++)
				{
					if (displayStr[j] == '/' || displayStr[j] == '>')
					{
						leng--;
						break;
					}
					leng++;
				}

				displayStr = "(" + displayStr.Substring(1, leng) + ")";
				controls.Add(displayStr);
			}
			_hText.text = _hText.text.Replace(key.KeyName, key.Action.action.GetBindingDisplayString() + " |");
		}

		for (int i = 0; i < controls.Count; i++)
			_hText.text = ReplaceFirst(_hText.text, "|", controls[i] + ((i == controls.Count - 1) ? "" : "/"));


		_hintHolder.SetActive(true);
		StartCoroutine(AllowSkip());
		_gameManager.PauseGame = true;
	}
	private string ReplaceFirst(string text, string from, string to)
	{
		int pos = text.IndexOf(from);
		for (int i = 0; i < text.Length; i++)
			if (pos < 0)
				return text;

		return text.Substring(0, pos) + to + text.Substring(pos + from.Length);
	}

	private LocalizedString _skipLocal = new LocalizedString() { TableReference = "Hints", TableEntryReference = "skip" };
	private LocalizedString _orLocal = new LocalizedString() { TableReference = "Hints", TableEntryReference = "or" };
	private IEnumerator AllowSkip()
	{
		yield return _hSkipTime;
		//_hSkipText.text = _skipLocal.GetLocalizedString();
		_hSkipText.gameObject.SetActive(true);

		while (true)
		{
			if (Input.anyKeyDown)
			{
				_animator.SetTrigger("Close");

				yield return new WaitForSecondsRealtime(0.1f);

				if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("CloseAlpha"))
				{
					_hintHolder.SetActive(false);
					HintShow = false;
					_hSkipText.gameObject.SetActive(false);
					if (_hintsInLine.Count > 0)
					{
						_animator.ResetTrigger("Close");
						DisplayHint();
						break;
					}
					_hintFree = false;
					_gameManager.PauseGame = false;
					break;
				}

			}
			yield return null;
		}
	}

}

public class Hint
{
	public string HintName;
	public string HintText;
	public string HintPhoto;
}


[System.Serializable]
public class HintWrapper
{
	public List<Hint> Hints;

	public HintWrapper(List<Hint> hintList)
	{
		if (hintList == null)
		{
			Hints = new List<Hint>();
			return;
		}

		Hints = hintList
			.Select(entry => new Hint
			{
				HintName = entry.HintName,
				HintPhoto = entry.HintPhoto,
				HintText = entry.HintText
			}).ToList();
	}
}

[System.Serializable]
public class KeyNames
{
	public string KeyName;
	public InputActionReference Action;
}
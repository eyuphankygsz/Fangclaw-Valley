using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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

	private WaitForSecondsRealtime _hSkipTime = new WaitForSecondsRealtime(1f);

	public bool HintShow { get; private set; }

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

	public void ShowHint(string name)
	{
		_animator.ResetTrigger("Close");
		_animator.Play("OpenAlpha");

		HintShow = true;
		_src.Play();
		Hint hint = HintList.Where(x => x.HintName == name).FirstOrDefault();
		if (hint == null) { Debug.Log("Can't Find The Hint"); return; }

		_hPhoto.sprite = Resources.Load<Sprite>(hint.HintPhoto);
		if (_hPhoto.sprite == null) { Debug.Log("Nub"); }

		_localizedText = new LocalizedString() { TableReference = "Hints", TableEntryReference = hint.HintText };
		_hText.text = _localizedText.GetLocalizedString();

		_hintHolder.SetActive(true);
		StartCoroutine(AllowSkip());
		_gameManager.PauseGame = true;
	}

	private LocalizedString _skipLocal = new LocalizedString() { TableReference = "Hints", TableEntryReference = "skip" };
	private IEnumerator AllowSkip()
	{
		yield return _hSkipTime;
		_hSkipText.text = _skipLocal.GetLocalizedString();
		_hSkipText.enabled = true;
		while (true)
		{
			if (Input.anyKeyDown)
			{
				_animator.SetTrigger("Close");
				yield return new WaitForSecondsRealtime(0.1f);

				if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("CloseAlpha"))
				{
					_gameManager.PauseGame = false;
					_hintHolder.SetActive(false);
					HintShow = false;
					_hSkipText.enabled = false;
				}
				break;
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
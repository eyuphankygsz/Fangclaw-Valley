using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

public class DialogueManager : MonoBehaviour
{
	public static DialogueManager Instance;

	public TextMeshProUGUI Subtitles;
	[SerializeField]
	private AudioSource _source;

	private int _index = 0;
	private List<AudioObject> _audioObjects = new List<AudioObject>();
	private AudioObject _audioObject;

	Coroutine _routine, _tempRoutine;

	private AudioSource _tempSource;

	[Inject]
	private GameManager _gameManager;

	public bool GamePause;
	public bool OnForce;

	[SerializeField]
	private GameObject _asList, _asOne;

	Dictionary<string, DialogueAsList> _asListDict = new Dictionary<string, DialogueAsList>();

	private void Awake()
	{
		Instance = this;
	}
	private void Start()
	{
		_gameManager.OnPauseGame += OnPause;
	}
	private void OnPause(bool pause, bool force)
	{
		OnForce = force;
		GamePause = pause;
	}
	public void PlayNewList(List<TalkObject> talkObject, string tableRef, List<AudioSource> src, string talkName)
	{

		if (_asListDict.ContainsKey(talkName))
			_asListDict[talkName].Setup(talkObject, tableRef, src, this, talkName);
		else
		{
			GameObject obj = Instantiate(_asList);
			_asListDict.Add(talkName, obj.GetComponent<DialogueAsList>().Setup(talkObject, tableRef, src, this, talkName));
		}
	}
	public void PlayNewOne(AudioObject audioObj)
	{
		GameObject obj = Instantiate(_asOne);
		obj.GetComponent<DialogueAsOne>().Setup(audioObj, this, _source);
	}
	public void DestroyDialogue(string name)
	{
		bool temp = _asListDict.ContainsKey(name);
		if (temp == false)
			return;

		GameObject obj = _asListDict[name].gameObject;
		_asListDict.Remove(name);
		Destroy(obj);
	}
}

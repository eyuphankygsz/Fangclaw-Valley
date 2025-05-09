using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

/// <summary>
/// Manages dialogue audio and subtitles in the game
/// </summary>
public class DialogueManager : MonoBehaviour
{
	// Singleton instance
	private static DialogueManager _instance;
	public static DialogueManager Instance
	{
		get
		{
			if (_instance == null)
			{
				Debug.LogWarning("DialogueManager instance was accessed before it was initialized");
			}
			return _instance;
		}
		private set { _instance = value; }
	}

	public TextMeshProUGUI Subtitles { get => _subtitles; }
	[SerializeField] private TextMeshProUGUI _subtitles;
	[SerializeField] private AudioSource _source;
	[SerializeField] private GameObject _asList, _asOne;

	[Inject] private GameManager _gameManager;

	// State tracking
	public bool GamePause { get; private set; }
	public bool OnForce { get; private set; }

	// Dictionary to track active dialogue lists
	private Dictionary<string, DialogueAsList> _asListDict = new Dictionary<string, DialogueAsList>();

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		if (_gameManager != null)
		{
			_gameManager.OnPauseGame += OnPause;
		}
		else
		{
			Debug.LogError("GameManager not injected into DialogueManager");
		}
	}

	private void OnDestroy()
	{
		if (_gameManager != null)
		{
			_gameManager.OnPauseGame -= OnPause;
		}
		
		// Clear any remaining dialogues
		foreach (var dialoguePair in _asListDict)
		{
			if (dialoguePair.Value != null && dialoguePair.Value.gameObject != null)
			{
				Destroy(dialoguePair.Value.gameObject);
			}
		}
		_asListDict.Clear();
	}

	private void OnPause(bool pause, bool force)
	{
		OnForce = force;
		GamePause = pause;
	}

	/// <summary>
	/// Plays a sequence of dialogues from a list
	/// </summary>
	/// <param name="talkObject">List of talk objects to play</param>
	/// <param name="tableRef">Reference to the localization table</param>
	/// <param name="src">List of audio sources to use</param>
	/// <param name="talkName">Unique name for this dialogue sequence</param>
	public void PlayNewList(List<TalkObject> talkObject, string tableRef, List<AudioSource> src, string talkName)
	{
		if (string.IsNullOrEmpty(talkName))
		{
			Debug.LogWarning("Attempted to play dialogue with null or empty name");
			return;
		}

		if (talkObject == null || talkObject.Count == 0)
		{
			Debug.LogWarning($"Attempted to play dialogue '{talkName}' with null or empty talk objects");
			return;
		}

		DialogueAsList dialogue;
		
		if (_asListDict.TryGetValue(talkName, out dialogue))
		{
			dialogue.Setup(talkObject, tableRef, src, this, talkName);
		}
		else
		{
			GameObject obj = Instantiate(_asList);
			dialogue = obj.GetComponent<DialogueAsList>();
			
			if (dialogue != null)
			{
				dialogue.Setup(talkObject, tableRef, src, this, talkName);
				_asListDict.Add(talkName, dialogue);
			}
			else
			{
				Debug.LogError($"Failed to get DialogueAsList component from prefab for '{talkName}'");
				Destroy(obj);
			}
		}
	}

	/// <summary>
	/// Plays a single dialogue
	/// </summary>
	/// <param name="audioObj">The audio object to play</param>
	public void PlayNewOne(AudioObject audioObj)
	{
		if (audioObj == null)
		{
			Debug.LogWarning("Attempted to play null AudioObject");
			return;
		}

		GameObject obj = Instantiate(_asOne);
		DialogueAsOne dialogue = obj.GetComponent<DialogueAsOne>();
		
		if (dialogue != null)
		{
			dialogue.Setup(audioObj, this, _source);
		}
		else
		{
			Debug.LogError("Failed to get DialogueAsOne component from prefab");
			Destroy(obj);
		}
	}

	/// <summary>
	/// Destroys a dialogue by name
	/// </summary>
	/// <param name="name">The name of the dialogue to destroy</param>
	public void DestroyDialogue(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			Debug.LogWarning("Attempted to destroy dialogue with null or empty name");
			return;
		}

		if (!_asListDict.TryGetValue(name, out DialogueAsList dialogue))
		{
			return;
		}

		if (dialogue != null && dialogue.gameObject != null)
		{
			GameObject obj = dialogue.gameObject;
			_asListDict.Remove(name);
			Destroy(obj);
		}
		else
		{
			_asListDict.Remove(name);
		}
	}
}

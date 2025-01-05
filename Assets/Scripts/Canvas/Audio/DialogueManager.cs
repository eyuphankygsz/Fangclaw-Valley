using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class DialogueManager : MonoBehaviour
{
	public static DialogueManager Instance;

	[SerializeField]
	private TextMeshProUGUI _text;
	[SerializeField]
	private AudioSource _source;

	private int _index = 0;
	private List<AudioObject> _audioObjects = new List<AudioObject>();
	private AudioObject _audioObject;

	Coroutine _routine, _tempRoutine;

	private AudioSource _tempSource;


	private void Awake()
	{
		Instance = this;
	}

	public void PlayList(List<AudioObject> aObjects)
	{
		_index = 0;
		_audioObjects = aObjects;
		PlayAudio();
	}
	public void PlayOne(AudioObject aObjects)
	{
		_index = 0;
		_audioObjects.Clear();
		_audioObjects.Add(aObjects);
		PlayAudio();
	}
	public void PlayOne(TalkObject talkObject, string tableRef, AudioSource src)
	{
		_index = 0;
		_audioObjects.Clear();

		if (_tempSource != null)
		{
			_tempSource.Stop();
			_tempSource = null;
		}

		_tempSource = src;
		PlayAudioInstant(talkObject, tableRef);

	}

	private void PlayAudioInstant(TalkObject talkObject, string tableRef)
	{
		if (_routine != null)
		{
			_source.Stop();
			StopCoroutine(_routine);
		}
		if (_tempRoutine != null)
			StopCoroutine(_tempRoutine);

		_tempRoutine = StartCoroutine(PlayAudioAsync(talkObject, tableRef));
	}

	private IEnumerator PlayAudioAsync(TalkObject talkObject, string tableRef)
	{
		LocalizedAsset<AudioClip> _audio = new LocalizedAsset<AudioClip>() { TableReference = tableRef + "Sounds", TableEntryReference = talkObject.TalkText };
		var loadOperation = _audio.LoadAssetAsync();

		yield return loadOperation;

		if (loadOperation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
		{
			_tempSource.clip = loadOperation.Result;
			_tempSource.Play();
		}
		else
			Debug.LogError("Audio clip could not be loaded.");

		LocalizedString _skipLocal = new LocalizedString() { TableReference = tableRef, TableEntryReference = talkObject.TalkText };
		_text.text = _skipLocal.GetLocalizedString();

		_tempRoutine = StartCoroutine(CheckEndTemp(talkObject));
	}
	private void PlayAudio()
	{
		if (_routine != null)
			StopCoroutine(_routine);

		if (_tempSource == null)
		{
			_source.clip = _audioObjects[_index].Clip;
			if (_source.clip != null)
				_source.Play();

		}
		else
		{
			_tempSource.clip = _audioObjects[_index].Clip;
			_tempSource.Play();
		}

		_text.text = _audioObjects[_index].Text.GetLocalizedString();
		_routine = StartCoroutine(CheckEnd());

	}

	IEnumerator CheckEnd()
	{
		yield return new WaitForSeconds(_audioObjects[_index].Delay);
		while (true)
		{

			if (!_source.isPlaying)
			{
				_text.text = "";
				NextAudio();
				break;
			}
			yield return null;
		}

	}
	IEnumerator CheckEndTemp(TalkObject obj)
	{
		yield return new WaitForSeconds(obj.TalkDelay);
		while (true)
		{
			if (!_tempSource.isPlaying)
			{
				_text.text = "";
				break;
			}
			yield return null;
		}

	}
	private void NextAudio()
	{
		_index++;
		if (_index >= _audioObjects.Count)
		{
			_index = 0;
			return;
		}
		PlayAudio();
	}

}

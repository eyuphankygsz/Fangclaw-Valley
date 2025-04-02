using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class DialogueAsList : MonoBehaviour
{
	private Coroutine _tempRoutine;
	private AudioSource _source, _tempSource;
	private int _index = 0;

	private TalkEvents _talkEvents;
	private DialogueManager _manager;
	public DialogueAsList Setup(TalkObject talkObject, string tableRef, AudioSource src, TalkEvents talkEvents, DialogueManager mng)
	{
		_manager = mng;
		PlayOneFromList(talkObject, tableRef, src, talkEvents);
		return this;
	}
	public void PlayOneFromList(TalkObject talkObject, string tableRef, AudioSource src, TalkEvents talkEvents)
	{
		if (_talkEvents == null)
			_talkEvents = talkEvents;

		_index = 0;

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

			while (_manager.GamePause)
				yield return null;


			_tempSource.Play();
		}
		else
			Debug.LogError("Audio clip could not be loaded.");

		if (PlayerPrefs.GetInt("Subtitles") == 1)
		{
			LocalizedString _skipLocal = new LocalizedString() { TableReference = tableRef, TableEntryReference = talkObject.TalkText };
			_manager.Subtitles.text = _skipLocal.GetLocalizedString();
		}
		else
		{
			_manager.Subtitles.text = "";
		}

		_tempRoutine = StartCoroutine(CheckEndTemp(talkObject));
	}
	IEnumerator CheckEndTemp(TalkObject obj)
	{
		while (true)
		{
			if (!_tempSource.isPlaying && !_manager.GamePause)
			{
				yield return new WaitForSeconds(obj.TalkDelay);
				_manager.Subtitles.text = "";
				_talkEvents.PlayNext();
				break;
			}
			yield return null;
		}

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class DialogueAsList : MonoBehaviour
{
	private Coroutine _tempRoutine;
	private AudioSource _source, _tempSource;

	private DialogueManager _manager;

	private int _id;
	private TalkObject[] _talkObjects;
	private AudioSource[] _audioSources;
	private string _tableRef;
	private string _talkName;
	public DialogueAsList Setup(List<TalkObject> talkObject, string tableRef, List<AudioSource> src, DialogueManager mng, string talkName)
	{
        _manager = mng;
		_talkName = talkName;
		_tableRef = tableRef;

		_talkObjects = talkObject.ToArray();
		_audioSources = src.ToArray();

        for (int i = 0; i < talkObject.Count; i++)
        {
            string voiceLang = PlayerPrefs.GetString("voice_lang", "en");
            var locale = LocalizationSettings.AvailableLocales.GetLocale(voiceLang);
            LocalizedAsset<AudioClip> _audio = new LocalizedAsset<AudioClip>() 
            { 
                TableReference = tableRef + "Sounds", 
                TableEntryReference = talkObject[i].TalkText,
                LocaleOverride = locale
            };
            var loadOperation = _audio.LoadAssetAsync();
        }

		PlayNext();
		return this;
	}

	private void PlayNext()
	{
		if (_id >= _talkObjects.Length)
		{
			DialogueManager.Instance.DestroyDialogue(_talkName);
			_id = 0;
			return;
		}
		PlayOneFromList(_talkObjects[_id], _tableRef, _audioSources[_talkObjects[_id].SourceIndex]);
		_id++;
	}
	public void PlayOneFromList(TalkObject talkObject, string tableRef, AudioSource src)
	{
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
		string voiceLang = PlayerPrefs.GetString("voice_lang", "en");
		var locale = LocalizationSettings.AvailableLocales.GetLocale(voiceLang);
		LocalizedAsset<AudioClip> _audio = new LocalizedAsset<AudioClip>() 
		{ 
			TableReference = tableRef + "Sounds", 
			TableEntryReference = talkObject.TalkText,
			LocaleOverride = locale
		};
		var loadOperation = _audio.LoadAssetAsync();

		yield return loadOperation;
		if (loadOperation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
		{
			_tempSource.clip = loadOperation.Result;

			yield return new WaitUntil(() => !_manager.GamePause);
			_tempSource.Play();
		}

		if (PlayerPrefs.GetInt("Subtitles") == 1)
		{
			LocalizedString _skipLocal = new LocalizedString() { TableReference = tableRef, TableEntryReference = talkObject.TalkText };
			_manager.Subtitles.text = _skipLocal.GetLocalizedString();
		}
		else
			_manager.Subtitles.text = "";

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
				PlayNext();
				break;
			}
			yield return null;
		}

	}
}

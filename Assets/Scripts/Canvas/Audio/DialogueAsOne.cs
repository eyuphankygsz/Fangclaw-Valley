using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAsOne : MonoBehaviour
{

	private Coroutine _routine;
	private AudioSource _source;
	private int _index = 0;
	private List<AudioObject> _audioObjects = new List<AudioObject>();

	private DialogueManager _manager;
	public void Setup(AudioObject audioObject, DialogueManager mng, AudioSource src)
	{
		_manager = mng;
		_source = src;

		PlayOne(audioObject);
	}
	public void PlayOne(AudioObject aObjects)
	{
		if (_manager.GamePause && _manager.OnForce)
			return;
		_index = 0;
		_audioObjects.Clear();
		_audioObjects.Add(aObjects);
		PlayAudio();
	}
	private void PlayAudio()
	{
		if (_routine != null)
			StopCoroutine(_routine);

		_source.clip = _audioObjects[_index].Clip;
		if (_source.clip != null)
			_source.Play();


		_manager.Subtitles.text = _audioObjects[_index].Text.GetLocalizedString();
		_routine = StartCoroutine(CheckEnd());

	}

	IEnumerator CheckEnd()
	{
		yield return new WaitForSeconds(_audioObjects[_index].Delay);
		while (true)
		{

			if (!_source.isPlaying)
			{
				_manager.Subtitles.text = "";
				break;
			}
			yield return null;
		}

	}
}
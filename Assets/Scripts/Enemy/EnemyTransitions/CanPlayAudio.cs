using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CanPlayAudio : MonoBehaviour
{
	[SerializeField]
	Dictionary<AudioClip, bool> _audios = new Dictionary<AudioClip, bool>();

	public bool CanPlay(AudioClip clip)
	{
		bool canPlay = _audios[clip];
		_audios[clip] = false;

		return canPlay;
	}
	public void EnablePlay(AudioClip clip)
	{
		if (_audios.ContainsKey(clip))
			_audios[clip] = true;
	}
	public void AddAudio(AudioClip clip)
	{
		if (_audios.ContainsKey(clip)) return;
		_audios.Add(clip, true);
	}
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.UI;
using Zenject;

public class AudioSetting : Setting
{
	[SerializeField]
	private AudioMixer _mixer;
	[SerializeField]
	private Slider _slider;

	private float _currentValue;
	private float _tempValue;

	[SerializeField]
	private List<AudioSource> _sources;
	private List<AudioSource> _stoppedSrc = new List<AudioSource>();

	[SerializeField]
	private List<PlayableDirector> _directors;
	private List<PlayableDirector> _stoppedDrct = new List<PlayableDirector>();

	[Inject]
	private GameManager _pauseMenu;

	private void Start()
	{
		_pauseMenu.OnPauseGame += OnPause;
		SetSFXParam();
	}
	public void OnPause(bool pause, bool force)
	{
		AudioListener.pause = pause;
	}
	public void SetSFX(Slider slider) =>
		_tempValue = slider.value;

	private void SetSFXParam() =>
		_mixer.SetFloat("SFXParam", _currentValue == 0 ? -80 : Mathf.Log10(_currentValue) * 20);

	public override void Restore()
	{
		_tempValue = _currentValue;
		_slider.value = _currentValue;
	}

	public override void Save()
	{
		_currentValue = _tempValue;
		PlayerPrefs.SetFloat("SFX", _currentValue);
		SetSFXParam();

	}

	public override void Load()
	{
		if (!PlayerPrefs.HasKey("SFX"))
			PlayerPrefs.SetFloat("SFX", 1);

		_currentValue = PlayerPrefs.GetFloat("SFX");
		_tempValue = _currentValue;
		_slider.value = _currentValue;
		Start();
	}
}

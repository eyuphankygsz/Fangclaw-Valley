using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MusicSetting : Setting
{
	[SerializeField]
	private AudioMixer _mixer;
	[SerializeField]
	private Slider _slider;

	private float _currentValue;
	private float _tempValue;

	private void Start() =>
		SetMusicParam();
	public void SetMusic(Slider slider) =>
		_tempValue = slider.value;
	private void SetMusicParam() =>
		_mixer.SetFloat("MusicParam", _currentValue == 0 ? -80 : Mathf.Log10(_currentValue) * 20);

	public override void Restore()
	{
		_tempValue = _currentValue;
		_slider.value = _currentValue;
	}

	public override void Save()
	{
		_currentValue = _tempValue;
		PlayerPrefs.SetFloat("Music", _currentValue);
		SetMusicParam();
	}

	public override void Load()
	{
		if (!PlayerPrefs.HasKey("Music"))
			PlayerPrefs.SetFloat("Music", 1);

		_currentValue = PlayerPrefs.GetFloat("Music");
		_tempValue = _currentValue;
		SetMusicParam();

		_slider.value = _currentValue;
	}
}

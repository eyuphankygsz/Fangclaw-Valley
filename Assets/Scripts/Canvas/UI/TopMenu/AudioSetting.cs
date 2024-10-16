using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSetting : Setting
{
	[SerializeField]
	private AudioMixer _mixer;
	[SerializeField]
	private Slider _slider;

	private float _currentValue;
	private float _tempValue;

	private void Awake()
	{
		if (!PlayerPrefs.HasKey("SFX"))
			PlayerPrefs.SetFloat("SFX", 1);

		_currentValue = PlayerPrefs.GetFloat("SFX");
		_tempValue = _currentValue;
		_slider.value = _currentValue;
	}
	private void Start() =>
		SetSFXParam();
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
}

using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class SubtitlesSetting : Setting
{
	private bool _isOn;
	private bool _tempIsOn;

	[SerializeField]
	private TextMeshProUGUI _text;
	[SerializeField]
	private LocalizedString _on, _off;

	private void Awake()
	{
		if (!PlayerPrefs.HasKey("Subtitles"))
			PlayerPrefs.SetInt("Subtitles", 1);

		_isOn = PlayerPrefs.GetInt("Subtitles") == 1 ? true : false;
		_tempIsOn = _isOn;
		_text.text = _isOn ? _on.GetLocalizedString() : _off.GetLocalizedString();
	}
	public void Change()
	{
		_tempIsOn = !_tempIsOn;
		_text.text = _isOn ? _on.GetLocalizedString() : _off.GetLocalizedString();
	}
	public override void Restore()
	{
		_tempIsOn = _isOn;
		_text.text = _isOn ? _on.GetLocalizedString() : _off.GetLocalizedString();
	}

	public override void Save()
	{
		_isOn = _tempIsOn;
		PlayerPrefs.SetInt("Subtitles", _isOn ? 1 : 0);
	}
}

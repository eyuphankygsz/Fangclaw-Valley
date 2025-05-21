using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class VoiceOverSetting : Setting
{
	[SerializeField]
	private TextMeshProUGUI _voiceOver;

	private Dictionary<string, string> _languageDict = new Dictionary<string, string>
	{
		{ "English", "English" },
		{ "Turkish", "Türkçe" }
	};
	private string _voiceOverString = "selected_voiceover", _selectedVoiceOver;
	private string[]  _shorts = new string[]
	{
		"English",
		"Turkish",
	};

	private int _localeID, _tempLocaleID;


	public override void Load()
	{
		PlayerPrefs.DeleteKey(_voiceOverString);
		PlayerPrefs.DeleteKey("selected_locale");
		
		if (!PlayerPrefs.HasKey(_voiceOverString))
		{
			bool found = false;

			foreach (var item in _languageDict)
			{
				if (Application.systemLanguage.Equals(item.Key))
				{
					found = true;
					_selectedVoiceOver = item.Key;
					break;
				}
			}

			if(!found)
				_selectedVoiceOver = "English";

			PlayerPrefs.SetString(_voiceOverString, _selectedVoiceOver);
			return;
		}
		else
			_selectedVoiceOver = PlayerPrefs.GetString(_voiceOverString);
		
		Debug.Log("SelectedLocale = " + _selectedVoiceOver);


		int i = _localeID = 0;
		foreach (var item in _languageDict)
		{
			if (item.Key.Equals(_selectedVoiceOver))
				break;
			i++;
			_localeID = i;
		}
		Debug.Log("LocaleID = " + i);

		_tempLocaleID = _localeID;

		if (_voiceOver != null)
			_voiceOver.text = _languageDict[_selectedVoiceOver];

	}

	public void NextLanguage()
	{
		_tempLocaleID = (_tempLocaleID + 1) % _shorts.Length;
		_voiceOver.text = _languageDict[_shorts[_tempLocaleID]];
	}

	public override void Restore()
	{
		_tempLocaleID = _localeID;
		_voiceOver.text = _languageDict[_shorts[_localeID]];
	}

	public override void Save()
	{
		_localeID = _tempLocaleID;
		_selectedVoiceOver = _shorts[_localeID];
		PlayerPrefs.SetString(_voiceOverString, _selectedVoiceOver);
		LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
	}
}

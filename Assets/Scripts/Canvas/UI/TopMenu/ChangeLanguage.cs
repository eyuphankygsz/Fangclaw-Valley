using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ChangeLanguage : Setting
{
	[SerializeField]
	private TextMeshProUGUI _language;
	private Dictionary<string, string> _languageDict = new Dictionary<string, string>
	{
		{ "bg", "Български" },
		{ "da", "Dansk" },
		{ "nl", "Nederlands" },
		{ "en", "English" },
		{ "fr", "Français" },
		{ "de", "Deutsch" },
		{ "el", "Ελληνικά" },
		{ "it", "Italiano" },
		{ "ja", "日本語" },
		{ "ru", "Русский" },
		{ "es", "Español" },
		{ "sv", "Svenska" },
		{ "tr", "Türkçe" },
		{ "uk", "Українська" },
	};

	private string[] _shorts = new string[]
	{
		"bg",
		"da",
		"nl",
		"en",
		"fr",
		"de",
		"el",
		"it",
		"ja",
		"ru",
		"es",
		"sv",
		"tr",
		"uk"
	};
	private string _selectedLocale;
	private int _localeID;
	private int _tempLocaleID;
	private string _localeString = "selected_locale";




	public override void Load()
	{
		if (!PlayerPrefs.HasKey(_localeString))
		{
			_selectedLocale = PlayerPrefs.GetString(_localeString);

			foreach (var item in _languageDict)
			{
				if (Application.systemLanguage.Equals(item.Key))
					break;
				_localeID++;
			}

			if (_localeID >= _languageDict.Count)
				_localeID = 0;

			_tempLocaleID = _localeID;
			_language.text = _languageDict[_shorts[_localeID]];
			return;
		}

		_selectedLocale = PlayerPrefs.GetString(_localeString);

		foreach (var item in _languageDict)
		{
			if (item.Key.Equals(_selectedLocale))
				break;
			_localeID++;

		}

		_tempLocaleID = _localeID;
		_language.text = _languageDict[_shorts[_tempLocaleID]];
	}

	public void NextLanguage()
	{
		_tempLocaleID = (_tempLocaleID + 1) % _shorts.Length;
		_language.text = _languageDict[_shorts[_tempLocaleID]];
	}

	public override void Restore()
	{
		_tempLocaleID = _localeID;
		_language.text = _languageDict[_shorts[_localeID]];
	}

	public override void Save()
	{
		_localeID = _tempLocaleID;
		_selectedLocale = _shorts[_tempLocaleID];
		PlayerPrefs.SetString(_localeString, _selectedLocale);
		LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
	}
}

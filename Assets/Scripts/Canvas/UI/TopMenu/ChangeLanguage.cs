using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ChangeLanguage : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _language;
	private Dictionary<string, string> _languageDict = new Dictionary<string, string>
	{
		{ "en", "English" },
		{ "fr", "Français" },
		{ "es", "Espanol" },
		{ "tr", "Türkçe" },
	};

	private string[] _shorts = new string[]
	{
		"en",
		"fr",
		"es",
		"tr"
	};
	private string _selectedLocale;
	private int _localeID;
	string _localeString = "selected_locale";

	public void Initialize()
	{
		if (!PlayerPrefs.HasKey(_localeString)) return;

		_selectedLocale = PlayerPrefs.GetString(_localeString);

		foreach (var item in _languageDict)
		{
			if (item.Key.Equals(_localeString))
				break;

			_localeID++;
		}
	}
	public void NextLanguage()
	{
		_localeID = (_localeID + 1) % _shorts.Length;
		_selectedLocale = _shorts[_localeID];
		PlayerPrefs.SetString(_localeString, _selectedLocale);
		LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
	}
}

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ChangeLanguage : Setting
{
	[SerializeField]
	private TextMeshProUGUI _language;
	[SerializeField]
	private TMP_FontAsset _standartFontAsset;
	[SerializeField]
	private Setting[] _changeSettingStrings;

	private Dictionary<string, string> _languageDict = new Dictionary<string, string>
	{
		{ "bg", "Български" },
		{ "zh-Hans", "中文（简体）" },
		{ "da", "Dansk" },
		{ "nl", "Nederlands" },
		{ "en", "English" },
		{ "fi", "Suomalainen" },
		{ "fr", "Français" },
		{ "de", "Deutsch" },
		{ "el", "Ελληνικά" },
		{ "hu", "Magyar" },
		{ "id", "Indonesia" },
		{ "it", "Italiano" },
		{ "ja", "日本語" },
		{ "ko", "한국인" },
		{ "no", "Norsk" },
		{ "ru", "Русский" },
		{ "es", "Español" },
		{ "sv", "Svenska" },
		{ "tr", "Türkçe" },
		{ "uk", "Українська" },
	};

	private string[] _shorts = new string[]
	{
		"bg",
		"zh-Hans",
		"da",
		"nl",
		"en",
		"fi",
		"fr",
		"de",
		"el",
		"hu",
		"id",
		"it",
		"ja",
		"ko",
		"no",
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

	[SerializeField]
	private LocaleFont[] _localeFonts;



	private string[] TableNames = new string[]
	{
		"albion",
		"AudioObjects",
		"events",
		"Hints",
		"InventoryItems",
		"InteractableNames",
		"ItemFunctions",
		"Notes",
		"Quests",
		"RoomNames",
		"Settings",
	};

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
		if (_language != null)
			_language.text = _languageDict[_shorts[_tempLocaleID]];

		ReattachFonts();

		//string[] langs = new string[]
		//{
		//	"ja",
		//	"zh-Hans",
		//	"ko",
		//};
		//for (int i = 0; i < 3; i++)
		//{

		//	string str = "";
		//	foreach (var tableName in TableNames)
		//	{

		//		var tableCollection = LocalizationEditorSettings.GetStringTableCollection(tableName); // Table adını yaz
		//		if (tableCollection == null)
		//		{
		//			Debug.LogError("Table bulunamadı!");
		//			return;
		//		}

		//		foreach (var table in tableCollection.StringTables)
		//		{
		//			if (table.LocaleIdentifier.Code == langs[i]) // Sadece Basitleştirilmiş Çince
		//			{
		//				var stringTable = table as StringTable;
		//				Debug.Log($"=== {langs[i]} Table: {table.LocaleIdentifier} ===");

		//				foreach (var entry in stringTable.Values)
		//				{
		//					str += entry.LocalizedValue;
		//					Debug.Log($"Key: {entry.KeyId}, Value: {entry.LocalizedValue}");
		//				}
		//			}
		//		}
		//	}

		//	File.WriteAllText(Path.Combine(Application.persistentDataPath, langs[i] + "-Table.txt"), str);
		//}
	}

	private void ReattachFonts()
	{
		LocaleFont newFont = _localeFonts.FirstOrDefault(x => x.LocaleName == _shorts[_tempLocaleID]);
		if (newFont == null)
			newFont = new LocaleFont() { font = _standartFontAsset, LocaleName = "" };

		TextMeshProUGUI[] tmps = FindObjectsOfType<TextMeshProUGUI>(true);

		foreach (var tmp in tmps)
			tmp.font = newFont.font;

	}
	public LocaleFont GetFont()
	{
		LocaleFont newFont = _localeFonts.FirstOrDefault(x => x.LocaleName == _shorts[_tempLocaleID]);
		if (newFont == null)
			newFont = new LocaleFont() { font = _standartFontAsset, LocaleName = "" };

		return newFont;
	}

	public void NextLanguage()
	{
		_tempLocaleID = (_tempLocaleID + 1) % _shorts.Length;
		_language.text = _languageDict[_shorts[_tempLocaleID]];

		LocaleFont localeFont = _localeFonts.FirstOrDefault(x => x.LocaleName == _shorts[_tempLocaleID]);

		_language.font = (localeFont != null) ? localeFont.font : _standartFontAsset;
	}

	public override void Restore()
	{
		_tempLocaleID = _localeID;
		_language.text = _languageDict[_shorts[_localeID]];
	}

	public override void Save()
	{
		_localeID = _tempLocaleID;
		_selectedLocale = _shorts[_localeID];
		PlayerPrefs.SetString(_localeString, _selectedLocale);
		ReattachFonts();
		LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];

		foreach (var setting in _changeSettingStrings)
			setting.UpdateString();

	}
}

[System.Serializable]
public class LocaleFont
{
	public string LocaleName;
	public TMP_FontAsset font;
}
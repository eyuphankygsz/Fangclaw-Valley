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
		{ "Bulgarian", "Български" },
		{ "ChineseSimplified", "中文（简体）" },
		{ "Danish", "Dansk" },
		{ "Dutch", "Nederlands" },
		{ "English", "English" },
		{ "Finnish", "Suomalainen" },
		{ "French", "Français" },
		{ "German", "Deutsch" },
		{ "Greek", "Ελληνικά" },
		{ "Hungarian", "Magyar" },
		{ "Indonesian", "Indonesia" },
		{ "Italian", "Italiano" },
		{ "Japanese", "日本語" },
		{ "Korean", "한국인" },
		{ "Norwegian", "Norsk" },
		{ "Russian", "Русский" },
		{ "Spanish", "Español" },
		{ "Swedish", "Svenska" },
		{ "Turkish", "Türkçe" },
		{ "Ukrainian", "Українська" },
	};
	private string _selectedLocale;
	private string _localeString = "selected_locale";
	private string[]  _shorts = new string[]
	{
		"Bulgarian",
		"ChineseSimplified",
		"Danish",
		"Dutch",
		"English",
		"Finnish",
		"French",
		"German",
		"Greek",
		"Hungarian",
		"Indonesian",
		"Italian",
		"Japanese",
		"Korean",
		"Norwegian",
		"Russian",
		"Spanish",
		"Swedish",
		"Turkish",
		"Ukrainian"
	};

	private int _localeID, _tempLocaleID;

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
			bool found = false;

			foreach (var item in _languageDict)
			{
				if (Application.systemLanguage.Equals(item.Key))
				{
					found = true;
					_selectedLocale = item.Key;
					break;
				}
			}

			if(!found)
				_selectedLocale = "English";

			PlayerPrefs.SetString(_localeString, _selectedLocale);
			return;
		}
		else
			_selectedLocale = PlayerPrefs.GetString(_localeString);
		
		Debug.Log("SelectedLocale = " + _selectedLocale);

		foreach (var item in _languageDict)
		{
			if (item.Key.Equals(_selectedLocale))
				break;
			_localeID++;

		}
		Debug.Log("LocaleID = " + _localeID);

		_tempLocaleID = _localeID;

		if (_language != null)
			_language.text = _languageDict[_selectedLocale];

		LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
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
		{
			Debug.Log("NewFont is  NULL");
			newFont = new LocaleFont() { font = _standartFontAsset, LocaleName = "" };
		}

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
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

public class TalkEventsHolder : MonoBehaviour
{
	[SerializeField]
	private string _tableName;
	[SerializeField]
	private List<TalkList> _talksList;
	private JsonSerializerSettings _jsonSettings;


	private void Awake()
	{
		_jsonSettings = new JsonSerializerSettings
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			TypeNameHandling = TypeNameHandling.All,
			Formatting = Formatting.Indented
		};
		Load();
	}
	private void Load()
	{
		string json = Resources.Load<TextAsset>("texts/" + _tableName).ToString();
		JsonConvert.DeserializeObject<TalkWrapper>(json);

		TalkWrapper dataWrapper = JsonConvert.DeserializeObject<TalkWrapper>(json, _jsonSettings);
		if (dataWrapper?.Talks == null)
			return;

		_talksList = new List<TalkList>();

		//int i = 0;
		foreach (var entry in dataWrapper.Talks)
		{
			_talksList.Add(new TalkList() { TalkName = entry.TalkName, TalkObject = entry.TalkObject });
			//for (int j = 0; j < _talksList[i].TalkObject.Count; j++)
			//{
			//	LocalizedString _skipLocal = new LocalizedString() { TableReference = _talkName, TableEntryReference = _talksList[i].TalkObject[j].TalkText };
			//	Debug.Log(_skipLocal.GetLocalizedString());
			//}
			//i++;
			//Debug.Log("========");

		}
	}
	public TalkList GetList(string listName)
	{
		return _talksList.First(t => t.TalkName == listName);
	}
	public string GetTableRef()
	{
		return _tableName;
	}

}
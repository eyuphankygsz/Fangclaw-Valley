using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;

public class TalkEvents : MonoBehaviour
{
	[SerializeField]
	private string _talkName;
	[SerializeField]
	private List<TalkList> _talksList;



	private AudioSource _src;
	private JsonSerializerSettings _jsonSettings;
	private int _id;
	private TalkList _currentTalkList;


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
		_src =	GetComponent<AudioSource>();
		string json = Resources.Load<TextAsset>("texts/" + _talkName).ToString();
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

	public void SelectTalkList(string talkList)
	{
		_currentTalkList = _talksList.First(t => t.TalkName == talkList);
		PlayNext();
	}

	public void PlayNext()
	{
		if (_id >= _currentTalkList.TalkObject.Count)
			return;

		TalkObject tObj = _currentTalkList.TalkObject[_id];
		DialogueManager.Instance.PlayOne(tObj, _talkName, _src);

		_id++;
	}





}


public class TalkObject
{
	public string TalkText;
	public string TalkAudio;
	public float TalkDelay;
}

public class TalkList
{
	public string TalkName;
	public List<TalkObject> TalkObject;
}

[System.Serializable]
public class TalkWrapper
{
	public List<TalkList> Talks;

	public TalkWrapper(List<TalkList> talkList)
	{
		if (talkList == null)
		{
			Talks = new List<TalkList>();
			return;
		}

		Talks = talkList
			.Select(entry => new TalkList
			{
				TalkName = entry.TalkName,
				TalkObject = entry.TalkObject?.Select(obj => new TalkObject
				{
					TalkText = obj.TalkText,
					TalkAudio = obj.TalkAudio,
					TalkDelay = obj.TalkDelay
				}).ToList()
			}).ToList();
	}
}

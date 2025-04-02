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

	[SerializeField]
	private List<AudioSource> _sources;
	private JsonSerializerSettings _jsonSettings;
	private int _id;
	private TalkList _currentTalkList;

	[SerializeField]
	private TalkEventsHolder _holder;

	public void SelectTalkList()
	{
		_currentTalkList = _holder.GetList(_talkName);
		PlayNext();
	}

	public void PlayNext()
	{
		if (_id >= _currentTalkList.TalkObject.Count)
		{
			DialogueManager.Instance.DestroyDialogue(_currentTalkList.TalkName);
			_id = 0;
			return;
		}

		TalkObject tObj = _currentTalkList.TalkObject[_id];
		DialogueManager.Instance.PlayNewList(tObj, _holder.GetTableRef(), _sources[tObj.SourceIndex], this, _currentTalkList.TalkName);

		_id++;
	}

}


public class TalkObject
{
	public string TalkText;
	public float TalkDelay;
	public int SourceIndex;
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
					TalkDelay = obj.TalkDelay,
					SourceIndex = obj.SourceIndex
				}).ToList()
			}).ToList();
	}
}

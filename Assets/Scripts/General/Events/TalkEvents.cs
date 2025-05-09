using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TalkEvents : MonoBehaviour
{
	[SerializeField]
	private string _talkName;

	[SerializeField]
	private List<AudioSource> _sources;
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
		DialogueManager.Instance.PlayNewList(_currentTalkList.TalkObject, _holder.GetTableRef(), _sources, _currentTalkList.TalkName);
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

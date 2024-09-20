using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
	[SerializeField]
	private int _eventsTracker;

	[SerializeField]
	private List<CustomEventsList> _customEvents;

	public void IncreaseTracker()
	{
		StopEvents();

		if (_eventsTracker + 1 == _customEvents.Count)
			return;

		_eventsTracker++;
		SetupEvents();
	}

	public void StopEvents()
	{
		foreach (var item in _customEvents[_eventsTracker].Events)
			item.gameObject.SetActive(false);
	}
	public void StopAll()
	{
		foreach (var list in _customEvents)
			foreach (var item in list.Events)
				item.gameObject.SetActive(false);
	}
	public void SetupEvents()
	{
		foreach (var item in _customEvents[_eventsTracker].Events)
			item.Setup();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.M))
			IncreaseTracker();
	}

}

[System.Serializable]
public class CustomEventsList
{
	public List<CustomEvents> Events;
}

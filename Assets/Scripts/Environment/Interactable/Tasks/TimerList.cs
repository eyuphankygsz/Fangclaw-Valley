using UnityEngine;

public class TimerList : MonoBehaviour
{
	[SerializeField]
	private TimerEvents[] _events;

	private TimerEvents _currentEvent;

	public void CurrentEvent(int id)
	{
		_currentEvent = _events[id];
	}
	public void Setup(float time)
	{
		_currentEvent.Setup(time);
	}

}

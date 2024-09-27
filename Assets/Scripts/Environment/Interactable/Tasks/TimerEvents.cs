using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimerEvents : MonoBehaviour
{
	[SerializeField]
	private float _time;
	[SerializeField]
	private UnityEvent _events;

	private Coroutine _routine;

	private bool _atStartSetup;
	private bool _playing;
	public void Setup(float time)
	{
		_time = time;
		if (_playing) 
			return;

		_playing = true;
		if (_atStartSetup)
		{
			InstantActivate();
			return;
		}

		if (_routine != null)
			StopCoroutine(_routine);

		Debug.Log("TimerStart");
		_routine = StartCoroutine(Timer());
	}
	public void Restart() => _playing = false;

	public void SetAtStartSetup() => _atStartSetup = true;
	public void InstantActivate()
	{
		_playing = true;
		_events.Invoke();
	}
	private IEnumerator Timer()
	{
		yield return new WaitForSeconds(_time);
		Debug.Log("TimerDone");
		InstantActivate();
	}
}

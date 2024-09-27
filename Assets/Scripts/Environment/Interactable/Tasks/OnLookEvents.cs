using UnityEngine;
using UnityEngine.Events;

public class OnLookEvents : MonoBehaviour
{
	[SerializeField]
	private UnityEvent _events;

	private Coroutine _routine;
	private bool _playing;

	private void OnBecameVisible()
	{
		Debug.Log("VISIBLE");
		if (_playing)
			return;
		_playing = true;
		_events.Invoke();
	}

	private void OnBecameInvisible()
	{
		Debug.Log("INVISIBLE");
	}

	public void Restart() => _playing = false;
}

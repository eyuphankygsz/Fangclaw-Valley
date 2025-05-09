using UnityEngine;
using UnityEngine.Events;

public class HitterHelpers : MonoBehaviour
{
	[SerializeField]
	private UnityEvent _events;

	private bool _canInvoke;
	public void InvokeEvents()
	{
		if (_canInvoke)
			return;

		_events?.Invoke();
	}
	public void ReEnable()
	{
		_canInvoke = false;
	}
}

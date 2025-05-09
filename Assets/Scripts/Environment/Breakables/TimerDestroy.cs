using UnityEngine;

public class TimerDestroy : MonoBehaviour
{
	[SerializeField]
	private float _time;
	public void StartDestroy()
	{
		Destroy(gameObject, _time);
	}
}

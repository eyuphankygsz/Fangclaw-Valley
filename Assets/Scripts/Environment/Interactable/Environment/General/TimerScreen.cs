using TMPro;
using UnityEngine;

public class TimerScreen : MonoBehaviour
{
	[SerializeField]
	private int _timeLeft, _timeMax;

	[SerializeField]
	private TextMeshProUGUI _tmp;

	public void ResetTime()
	{
		_timeLeft = _timeMax;
		_tmp.text = "";
	}
	public void WriteDown()
	{
		_tmp.text = _timeLeft.ToString();
	}
	public void Decrease()
	{
		_timeLeft -= 1;
		WriteDown();
	}
}

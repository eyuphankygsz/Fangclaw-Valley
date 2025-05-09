using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class ClockTurn : MonoBehaviour, ISaveable
{
	[SerializeField]
	private Transform _hourHand, _minuteHand; // Saat ve dakika ibreleri
	[SerializeField]
	private float _speed;

	private int _hour, _minute;
	private bool _isTouched;

	private float _minuteRotationPerStep = 6f; // Her bir dakika i�in 6 derece
	private float _hourRotationPerStep = 0.5f; // Her bir dakika i�in saat ibresinin d�n�� a��s�

	private float _currentMinuteRotation = 0f;
	private float _currentHourRotation = 0f;

	private bool _stopTurn, _clockWise;

	private Coroutine _routine;

	[SerializeField]
	private ClockHand _clockHand;
	[SerializeField]
	private TheTimes[] _times;

	private TheTimes _currentTime;

	private int _tempHour;


	[SerializeField]
	private AudioSource _clockSource;
	[SerializeField]
	private UnityEvent _touchEvents;

	[SerializeField]
	private RunningTimeEvents[] _runningTimeEvents;

	[Inject]
	private SaveManager _saveManager;

	private void Start()
	{
		ResetClock();
	}

	public void StartTurn(bool right)
	{
		if (!_isTouched)
			_touchEvents?.Invoke();
		_isTouched = true;

		_clockWise = right;
		if (_routine != null)
			StopCoroutine(_routine);

		_stopTurn = false;
		_routine = StartCoroutine(RotateClock(right));
	}

	private IEnumerator RotateClock(bool right)
	{

		_clockSource.Play();
		if (_currentTime != null)
		{
			_currentTime.OnFalseEvents?.Invoke();
			_currentTime = null;
		}
		while (!_stopTurn)
		{
			_currentMinuteRotation += _minuteRotationPerStep * Time.deltaTime * _speed * (right ? 1 : -1);
			_minuteHand.localRotation = Quaternion.Euler(0, _currentMinuteRotation, transform.rotation.z);

			_currentHourRotation += _hourRotationPerStep * Time.deltaTime * _speed * (right ? 1 : -1);
			_hourHand.localRotation = Quaternion.Euler(0, _currentHourRotation, transform.rotation.z);

			// E�er dakika 360 dereceyi ge�erse
			if (_currentMinuteRotation >= 360f)
			{
				_currentMinuteRotation = 0f;
				_minuteHand.localRotation = Quaternion.Euler(0, 0, 0);

				// Saat ibresini 30 derece d�nd�r
				_hour++;
				if (_hour == 12) // 12 saatin ge�ilmesi durumunda
					_hour = 0;

				_currentHourRotation = _hour * 30f;
				_hourHand.localRotation = Quaternion.Euler(0, _currentHourRotation, transform.rotation.z);
			}
			// E�er dakika negatifse
			else if (_currentMinuteRotation < 0)
			{
				_currentMinuteRotation = 360f;
				_minuteHand.localRotation = Quaternion.Euler(0, 0, 360f);

				_hour--;
				if (_hour == -1)
					_hour = 11;
				_currentHourRotation = _hour * 30f;
				_hourHand.localRotation = Quaternion.Euler(0, _currentHourRotation, transform.rotation.z);
			}

			CheckRunningTimes(_currentMinuteRotation / 6f % 60f);

			yield return null;
		}
	}

	private void CheckRunningTimes(float eM)
	{
		float hour = _hour;
		float minute = Mathf.RoundToInt(eM / 5f) * 5;

		for (int i = 0; i < _runningTimeEvents.Length; i++)
			if (!_runningTimeEvents[i].IsDone &&
			  hour >= _runningTimeEvents[i].MinHour && hour <= _runningTimeEvents[i].MaxHour &&
				minute >= _runningTimeEvents[i].MinMinute && minute <= _runningTimeEvents[i].MaxMinute)
			{
				Debug.Log("INVOKED!");
				_runningTimeEvents[i].Events?.Invoke();
				bool conditionDone = true;
				foreach (var condition in _runningTimeEvents[i].Conditions)
				{
					if (!condition.CheckCondition())
						conditionDone = false;
				}

				if (conditionDone)
					_runningTimeEvents[i].IsDone = true;
			}
	}

	public void StopTurn(bool changeTurn)
	{
		_clockSource.Stop();
		_stopTurn = true;

		if (_routine != null)
			StopCoroutine(_routine);

		if (!changeTurn)
			CheckTime();
	}

	private void CheckTime()
	{

		// Dakika hesaplamas� (0-59 aras�nda olacak �ekilde)
		float exactMinutes = (_currentMinuteRotation / 6f) % 60f;
		_minute = Mathf.RoundToInt(exactMinutes / 5f) * 5;
		_minuteHand.localRotation = Quaternion.Euler(0, (_minute / 5) * (360f / 12), 0);
		// E�er dakika 60 olduysa, saati artt�r ve dakika s�f�rla
		if (_minute == 60 && _clockWise)
		{
			_minute = 0;
			_hour++;
			if (_hour == 12)
				_hour = 0;

			_currentHourRotation = _hour * 30f;
			_hourHand.localRotation = Quaternion.Euler(0, _currentHourRotation, transform.rotation.z);

			_currentMinuteRotation = 0f;
			_minuteHand.localRotation = Quaternion.Euler(0, 0, 0);
		}
		else if (_minute == 0 && !_clockWise)
		{
			_minute = 0;
			_hour--;
			if (_hour == -1)
				_hour = 11;

			_currentHourRotation = _hour * 30f;
			_hourHand.localRotation = Quaternion.Euler(0, _currentHourRotation, transform.rotation.z);

			_currentMinuteRotation = 360f;
			_minuteHand.localRotation = Quaternion.Euler(0, 0, 360f);

		}
		for (int i = 0; i < _times.Length; i++)
		{
			TheTimes time = _times[i];

			if (time.Hour == _hour && time.Minute == _minute)
			{
				_currentTime = time;
				time.OnTrueEvents?.Invoke();
			}
		}
	}

	private void ResetClock()
	{
		// �brelerin ba�lang�� pozisyonlar�n� s�f�rla
		_currentMinuteRotation = 0f;
		_currentHourRotation = 0f;
		_minuteHand.localRotation = Quaternion.Euler(0, 0, 0);
		_hourHand.localRotation = Quaternion.Euler(0, 0, 0);
	}

	public GameData GetSaveFile()
	{
		bool[] runningtimes = new bool[_runningTimeEvents.Length];

		int i = 0;
		foreach (var item in _runningTimeEvents)
		{
			runningtimes[i] = item.IsDone;
			i++;
		}

		return new ClockTurnData()
		{
			Name = "ClockTurn",
			IsTouched = _isTouched,
			Hour = _hour,
			Minute = _minute,
			RunningTimesIsDone = runningtimes
		};
	}

	public void SetLoadFile()
	{
		ClockTurnData data = _saveManager.GetData<ClockTurnData>("ClockTurn");
		if (data == null)
			return;
		_isTouched = data.IsTouched;


		_hour = data.Hour;
		_currentHourRotation = _hour * 30f;
		_hourHand.localRotation = Quaternion.Euler(0, _currentHourRotation, transform.rotation.z);

		_minute = data.Minute;
		_minuteHand.localRotation = Quaternion.Euler(0, (_minute / 5) * (360f / 12), 0);

		bool[] runningtimes = data.RunningTimesIsDone;

		int i = 0;
		foreach (var item in _runningTimeEvents)
		{
			item.IsDone = runningtimes[i];
			i++;
		}
	}
}

[System.Serializable]
public class TheTimes
{
	public int Hour;
	public int Minute;
	public UnityEvent OnTrueEvents;
	public UnityEvent OnFalseEvents;
}

[System.Serializable]
public class RunningTimeEvents
{
	public int MinHour;
	public int MaxHour;
	public int MinMinute;
	public int MaxMinute;
	public bool IsDone;
	public UnityEvent Events;
	public AbstractCondition[] Conditions;
}
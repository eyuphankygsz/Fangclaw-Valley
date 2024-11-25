using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class GameTime : MonoBehaviour, ISaveable
{
	private const float SECONDS = 1.6f;

	[Inject]
	private SaveManager _saveManager;
	[Inject]
	private GameManager _gameManager;

	private TimeSpan _inGameTime;
	private int _dayCount;
	private WaitForSeconds _waitseconds = new WaitForSeconds(SECONDS);

	[SerializeField]
	private TextMeshProUGUI _timeText;
	private Coroutine _timeRoutine;

	void Awake()
	{

	}
	private void Start()
	{
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
		_gameManager.OnPauseGame += ManageTime;
		ManageTime(false);
		SetLoadFile();
	}

	private void ManageTime(bool stop)
	{
		if (_timeRoutine != null)
			StopCoroutine(_timeRoutine);
		if (!stop)
		{
			_timeRoutine = StartCoroutine(UpdateGameTime());
		}


	}
	private IEnumerator UpdateGameTime()
	{
		while (true)
		{
			yield return _waitseconds;
			_inGameTime = _inGameTime.Add(TimeSpan.FromMinutes(1));

			_timeText.text = _inGameTime.ToString(@"hh\:mm");
		}
	}

	public TimeSpan GetTime()
	{
		return _inGameTime;
	}

	public GameData GetSaveFile()
	{
		return new GameTimeData()
		{
			Name = "GameTime",
			GameTime = _inGameTime,
			DayCount = _dayCount
		};
	}

	public void SetLoadFile()
	{
		GameTimeData data = _saveManager.GetData<GameTimeData>("GameTime");
		_inGameTime = data == null ? new TimeSpan(6, 0, 0) : data.GameTime;
		_dayCount = data == null ? 0 : data.DayCount;

	}
}

[Serializable]
public class GameTimeData : GameData
{
	public TimeSpan GameTime;
	public int DayCount;
}

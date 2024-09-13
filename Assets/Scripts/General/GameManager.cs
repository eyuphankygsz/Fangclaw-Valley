using System;
using System.Diagnostics;
using UnityEngine;

public class GameManager
{
	public event Action<bool> OnPauseGame;

	private bool _pauseGame;
	public bool PauseGame
	{
		get => _pauseGame;
		set
		{
			if (_pauseGame != value)
			{
				Time.timeScale = value ? 0 : 1;
				_pauseGame = value;
				OnPauseGame?.Invoke(_pauseGame);
			}
		}
	}

	public event Action<bool> OnInspecting;
	private bool _inspecting;
	public bool Inspecting
	{
		get => _inspecting;
		set 
		{
			_inspecting = value;
			OnInspecting?.Invoke(_inspecting);
		}
	}


	public event Action<bool> OnSaveGame;

	private bool _saveGame;
	public bool SaveGame
	{
		get => _saveGame;
		set
		{
			if (_saveGame != value)
			{
				_saveGame = value;
				PauseGame = value;
				OnSaveGame?.Invoke(_saveGame);
			}
		}
	}



	public void SetPauseGame(bool pauseGame)
	{
		PauseGame = pauseGame;
	}
	public void SetSaveGame(bool saveGame)
	{
		SaveGame = saveGame;
	}
}

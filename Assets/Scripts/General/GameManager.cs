using System;
using System.Diagnostics;

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
				_pauseGame = _inspecting ? true : value;
				Debug.WriteLine(_pauseGame);
				OnPauseGame?.Invoke(_pauseGame);
			}
		}
	}

	private bool _inspecting;
	public bool Inspecting
	{
		get => _inspecting;
		set => _inspecting = value;
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

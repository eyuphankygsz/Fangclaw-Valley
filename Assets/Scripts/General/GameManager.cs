using System;
using UnityEngine;

public class GameManager
{
	public event Action<bool, bool> OnPauseGame;

	private bool _pauseGame;
	public bool PauseGame
	{
		get => _pauseGame;
		set
		{
			if (_pauseGame != value)
			{
				if(!IsDied)
				{
					Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
					Cursor.visible = value;

					Time.timeScale = value ? 0 : 1;
					_pauseGame = value;
					OnPauseGame?.Invoke(_pauseGame, _force);
				}

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
	public bool IsDied;
	public event Action<bool> OnSaveGame;

	private bool _saveGame;
	public bool SaveGame
	{
		get => _saveGame;
		set
		{
			if (_saveGame != value && !_force)
			{
				_saveGame = value;
				PauseGame = value;
				OnSaveGame?.Invoke(_saveGame);
			}
		}
	}



	public event Action<bool> OnForce;

	private bool _force;
	public bool Force
	{
		get => _force;
		set
		{
			if (_force != value)
			{
				_force = value;
				OnForce?.Invoke(_force);
			}
		}
	}

	public event Action<bool> OnWeaponChanging;
	private bool _wChanging;
	public bool WeaponChanging
	{
		get => _wChanging;
		set
		{
			_wChanging = value;
			OnInspecting?.Invoke(_wChanging);
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

	public event Action<bool> OnChase;
	private int _onChase;
	public int IsOnChase
	{
		get => _onChase;
		set
		{
			if (value != _onChase)
			{
				_onChase = value;
				OnChase?.Invoke(_onChase > 0);
			}
		}
	}



	public GameObject Player;
}

using System.Collections;
using UnityEngine;
using Zenject;

public class Interactable_Save : Interactable
{
	private Coroutine _saveRoutine;
	private AudioSource _src;

	[Inject]
	private GameManager _manager;
	[SerializeField]
	private Collider _collider;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	private void Awake()
	{
		base.Awake();
		_src = GetComponent<AudioSource>();
	}
	private void Start()
	{
		_manager.OnChase += OnChase;
	}
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

	private void OnChase(bool onChase)
	{
		if(onChase)
			_collider.enabled = false;
		else
			_collider.enabled = true;
	}

	public override void LoadData()
	{
		return;
	}

	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		if (_saveRoutine != null) return;

		_src.Play();
		_saveRoutine = StartCoroutine(SaveGameRoutine());
	}

	private IEnumerator SaveGameRoutine()
	{
		yield return _saveManager.SaveGame(OnSaveComplete);
	}
	private void OnSaveComplete()
	{
		_saveRoutine = null;
	}
	public override GameData GetGameData()
	{
		return null;
	}
}

using UnityEngine;
using Zenject;

public class Interactable_KeyReader : Interactable
{
	[SerializeField]
	private AudioClip[] _clips;
	private AudioSource _source;

	private bool _isOn;
	private bool _isOneTimeDone;


	[Inject]
	private GameManager _manager;
	private PlayerWeaponController _weaponController;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	private void Awake()
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
	{
		base.Awake();
		_source = GetComponentInParent<AudioSource>();
	}

	private KReaderData _data = new KReaderData();
	public override void OnInteract(Enum_Weapons weapon)
	{

		base.OnInteract(weapon);
		if (_weaponController == null)
			_weaponController = _manager.Player.GetComponent<PlayerWeaponController>();

		if (IsWeaponInclude(_weaponController.GetCurrentWeaponEnum()))
			HandleReader();
	}

	private void HandleReader()
	{
		if (!_isOneTimeDone)
		{
			_isOneTimeDone = true;
			_oneTimeEvents?.Invoke();
		}

		if (_clips.Length > 0)
			_source.PlayOneShot(_clips[Random.Range(0, _clips.Length)]);
		if (_isOn)
			_falseEvents?.Invoke();
		else
			_trueEvents?.Invoke();

		_isOn = !_isOn;
	}
	public override GameData GetGameData()
	{
		_data = new KReaderData
		{
			Name = InteractableName,
			IsOn = _isOn,
			IsOneTimeDone = _isOneTimeDone
		};

		return _data;
	}

	public override void LoadData()
	{
		KReaderData data = _saveManager.GetData<KReaderData>(InteractableName);
		if (data == null) return;

		if (data.IsOn)
			_trueDoneEvents?.Invoke();
		else
			_falseDoneEvents?.Invoke();

		_data = data;
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
	}
}
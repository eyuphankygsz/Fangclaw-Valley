using UnityEngine;
using UnityEngine.Events;
using Zenject;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public abstract class Interactable : MonoBehaviour, ISaveable
{
	[field: SerializeField]
	public string ObjectName { get; protected set; }

	[field: SerializeField]
	public string InteractableName { get; protected set; }
	public bool IsActive;

	protected bool _used;

	[SerializeField] protected Enum_Weapons[] _includedWeapons;


	[SerializeField]
	protected UnityEvent _oneTimeEvents;
	[SerializeField]
	protected UnityEvent _doneEvents;
	[SerializeField]
	protected UnityEvent _trueEvents;
	[SerializeField]
	protected UnityEvent _falseEvents;
	[SerializeField]
	protected UnityEvent _interactEvents;

	[Inject]
	protected SaveManager _saveManager;
	public virtual void OnInteract(Enum_Weapons weapon)
	{
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());

	}
	public virtual void SetStatusManually(bool on)
	{

	}
	public void ChangeObjectName()
	{

	}
	public abstract GameData GetGameData();
	public abstract void LoadData();
	protected void Awake()
	{
		var x = GetSaveFile();
		if (x == null)
			return;

		x.Name = InteractableName;
	}
	protected void Start()
	{
		LoadData();
	}

	protected bool IsWeaponInclude(Enum_Weapons e)
	{
		for (int i = 0; i < _includedWeapons.Length; i++)
			if (_includedWeapons[i] == e)
				return true;
		return false;
	}


	protected void OneTimeEvent()
	{
		if (!_used)
		{
			_used = true;
			_oneTimeEvents.Invoke();
		}

	}
	protected void DoneEvent()
	{
		_doneEvents.Invoke();
	}


	public GameData GetSaveFile() => GetGameData();

	public void SetLoadFile()
	{
		return;
	}
}

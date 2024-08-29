using UnityEngine;
using Zenject;

public abstract class Interactable : MonoBehaviour, ISaveable
{
	[field: SerializeField]
	public string ObjectName { get; protected set; }

	[field: SerializeField]
	public string InteractableName { get; protected set; }
	public bool IsActive;


	[SerializeField] protected Enum_Weapons[] _includedWeapons;

	[Inject]
	protected SaveManager _saveManager;
	public virtual void OnInteract(Enum_Weapons weapon)
	{
		if (!_saveManager.HasItem(gameObject, GetSaveFile()))
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

	public GameData GetSaveFile() => GetGameData();

	public void SetLoadFile()
	{
		return;
	}
}

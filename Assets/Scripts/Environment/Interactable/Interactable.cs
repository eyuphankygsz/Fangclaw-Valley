using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
	[field: SerializeField]
	public string ObjectName { get; protected set; }

	[field: SerializeField]
    public string InteractableName { get; protected set; }
    public bool IsActive;
    
    [SerializeField] protected Enum_Weapons[] _includedWeapons;
    
    public abstract void OnInteract(Enum_Weapons weapon);
	public abstract InteractableData SaveData();
	public abstract void LoadData();

	protected void Awake()
	{
		SaveManager.Instance.AddInteractable(this);
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
}

using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected Enum_Weapons[] _includedWeapons;
    public abstract void OnInteract(Enum_Weapons weapon);

    protected bool IsWeaponInclude(Enum_Weapons e)
    {
        for (int i = 0; i < _includedWeapons.Length; i++)
            if (_includedWeapons[i] == e)
                return true;
        return false;
    }
}

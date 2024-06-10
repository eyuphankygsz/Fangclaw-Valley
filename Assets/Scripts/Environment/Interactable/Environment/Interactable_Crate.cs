using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Crate : Interactable
{

    [SerializeField] private GameObject _originalObj, _shatterObj;
    public override void OnInteract(Enum_Weapons weapon)
    {
        if (IsWeaponInclude(weapon))
            Shatter();
    }

    private void Shatter()
    {

        for (int i = 0; i < _shatterObj.transform.childCount; i++)
        {
            Destroy(_shatterObj.transform.GetChild(i), 2);
        }
        _originalObj.SetActive(false);
        _shatterObj.SetActive(true);
    }
}

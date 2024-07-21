using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakMode : IWeaponModes
{
    public Weapons Weapon { get; set; }

    public void ExecuteMode()
    {
        if (Weapon.GetHitObject().TryGetComponent(out Interactable i)) 
            i.OnInteract(Weapon.GetWeaponEnum());
    }

    public void Setup(Weapons weapon)
    {
        Weapon = weapon;
    }
}

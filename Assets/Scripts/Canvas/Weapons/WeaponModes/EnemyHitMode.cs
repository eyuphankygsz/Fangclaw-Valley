using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitMode : IWeaponModes
{
    public Weapons Weapon { get; set; }

    public void ExecuteMode()
    {
        //CHANGE IT AFTER MAKING ENEMIES (Damage enemy?)
        //if (Weapon.GetHitObject().TryGetComponent<Interactable>(out Interactable i))
        //    i.OnInteract(Weapon.GetWeaponEnum());
    }

    public void Setup(Weapons weapon)
    {
        Weapon = weapon;
    }
}

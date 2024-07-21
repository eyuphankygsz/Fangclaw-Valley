using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponModes
{
    void Setup(Weapons weapon);
    void ExecuteMode();
    Weapons Weapon { get; set; }
}

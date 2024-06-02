using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool StopMove { get; private set; }
    private PlayerMovement _playerMovement;
    private PlayerCamera _playerCamera;
    private PlayerWeaponController _playerWeapon;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCamera = GetComponent<PlayerCamera>();
        _playerWeapon = GetComponent<PlayerWeaponController>();
    }

    void Update()
    {
        if (!StopMove)
        {
            _playerMovement.ManageMove();
            _playerCamera.ManageRotate();
            _playerWeapon.ManageGun();
        }
    }
}

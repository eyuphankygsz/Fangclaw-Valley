using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool StopMove { get; private set; }
    private PlayerMovement _playerMovement;
    private PlayerCamera _playerCamera;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerCamera = GetComponent<PlayerCamera>();
    }

    void Update()
    {
        if (!StopMove)
        {
            _playerMovement.TryMove();
            _playerCamera.TryRotate();
        }
    }
}

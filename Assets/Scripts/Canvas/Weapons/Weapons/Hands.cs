using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hands : Weapons
{
    private Dictionary<int, IWeaponModes> _modes = new Dictionary<int, IWeaponModes>();
    private IWeaponModes _defaultMode;

    private WeaponData _data = new WeaponData();

    [SerializeField]
    private AudioClip[] _punchClips;
    private int _punchCounter;

    public override void Move()
    {
        MoveNormal();
        MoveByCamera();
        ClampMove();
    }
    private void MoveNormal()
    {
        Y_Movement();
    }
    private void MoveByCamera()
    {
        X_Movement();
    }
    private void ClampMove()
    {
        ClampTransform();
    }


    private void OnLeftTrigger(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !_onAction && !_isFreeze)
        {
            _onAction = true;
            _weaponHelpers.StopChange = true;
            _weaponHelpers.CooldownGun(.8f, StopCooldown);
            PunchAnimation();
        }

    }
    private void StopCooldown()
    {
        _weaponHelpers.StopChange = false;
        _onAction = false;
    }

    private void PunchAnimation()
    {
        _animator.SetTrigger("Punch");
    }
    private void TryHit()
    {
        if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _rayLength, _interactableLayers))
        {
            _hitObject = hit.collider.gameObject;
            if (_modes.TryGetValue(_hitObject.layer, out IWeaponModes wMode))
                wMode.ExecuteMode();
        }
        _defaultMode.ExecuteMode();
    }
    public override void OnSelected(ControlSchema schema)
    {
        _controls = schema;
        _controls.Player.PrimaryShoot.performed += OnLeftTrigger;
    }

    public override void OnChanged()
    {
        if (_controls != null)
            _controls.Player.PrimaryShoot.performed -= OnLeftTrigger;
    }

    public override void SetWeapon()
    {
        _defaultMode = new DefaultMode();

        _modes.Add(9, new EnemyHitMode()); //EnemyHits
        _modes.Add(10, new BreakMode()); //Breakables

        foreach (var wMode in _modes)
            wMode.Value.Setup(this);


        _actionSleep = new WaitForSeconds(1);
    }

    public override GameData GetSave()
    {
        _data = new WeaponData()
        {
            Name = gameObject.name,
            IsSelected = gameObject.activeSelf,
            IsPicked = IsPicked
        };
        return _data;
    }

    public override void LoadSave()
    {
        _data = _saveManager.GetData<WeaponData>(gameObject.name);
        if (_data == null)
        {
            gameObject.SetActive(true);
            IsPicked = true;
            return;
        }
        gameObject.SetActive(_data.IsSelected);
        return;
    }
}

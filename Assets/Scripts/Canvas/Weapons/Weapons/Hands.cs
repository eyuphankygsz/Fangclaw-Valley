using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : Weapons
{
    private Dictionary<int, IWeaponModes> _modes = new Dictionary<int, IWeaponModes>();
    private IWeaponModes _defaultMode;
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

    public override void OnAction()
    {
        if (Input.GetMouseButtonDown(0) && !_onAction)
        {
            _onAction = true;
            PunchAnimation();
        }
    }

    private void PunchAnimation()
    {
        _animator.SetTrigger("Punch");
    }
    private void TryHit()
    {
        if (Physics.Raycast(transform.position, _camera.forward, out RaycastHit hit, _rayLength, _interactableLayers))
        {
            _hitObject = hit.collider.gameObject;
            if (_modes.TryGetValue(_hitObject.layer, out IWeaponModes wMode))
                wMode.ExecuteMode();
        }
        _defaultMode.ExecuteMode();
    }
    public override void OnSelected()
    {

    }

    public override void OnChanged()
    {
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
}

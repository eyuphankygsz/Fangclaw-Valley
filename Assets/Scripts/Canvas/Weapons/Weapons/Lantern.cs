using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lantern : Weapons
{
    [SerializeField] private GameObject _lightSource;

    private bool _active;


    public override void Move()
    {
        MoveNormal();
        MoveByCamera();
        ClampMove();
    }

    private void MoveNormal()
    {
        Y_Movement();

        float y = Mathf.InverseLerp(_yLimit.x, _yLimit.y, _pivot.transform.localPosition.y);
        _lightSource.transform.localPosition = (Vector3.up * y) + new Vector3(_lightSource.transform.localPosition.x, 3f, 0);
    }
    private void MoveByCamera()
    {
        X_Movement(); 
        _lightSource.transform.localPosition = new Vector3((-_pivot.transform.localPosition.x - 1) * 1.5f, _lightSource.transform.localPosition.y, 0);
    }
    private void ClampMove()
    {
        ClampTransform();
    }

    public override void OnAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _active = !_active;
            _lightSource.SetActive(_active);
        }
    }

    public override void OnSelected()
    {
    }

    public override void OnChanged()
    {
        _active = false;
        _lightSource.SetActive(false);
    }
}

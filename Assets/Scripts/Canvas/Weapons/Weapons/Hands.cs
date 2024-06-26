using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : Weapons
{
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
        if (Input.GetMouseButtonDown(0))
        {
            //Do Something
        }
    }

    public override void OnSelected()
    {

    }

    public override void OnChanged()
    {
    }
}

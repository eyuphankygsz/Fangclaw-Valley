using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : Weapons
{

    public override void Move()
    {
        MoveNormal();
        MoveByCamera();
        ClampTransform();
    }
    private void MoveNormal()
    {

    }
    private void MoveByCamera()
    {

    }
    private void ClampTransform()
    {
        float x = Mathf.Clamp(_rectTransform.anchoredPosition.x, _xAxis.x, _xAxis.y);
        float y = Mathf.Clamp(_rectTransform.anchoredPosition.y, _yAxis.x, _yAxis.y);
        _rectTransform.anchoredPosition = new Vector2(x, y);
    }

    public override void OnAction()
    {
        throw new System.NotImplementedException();
    }

    public override void OnSelected()
    {

    }
}

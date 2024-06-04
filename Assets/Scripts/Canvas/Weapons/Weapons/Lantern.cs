using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lantern : Weapons
{
    [SerializeField] private GameObject _lightSource;
    [SerializeField] private Color _deactiveColor, _activeColor;

    private Image _image;
    private bool _active;
    [SerializeField] private float _yOffset, _yDivide = 1;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    public override void Move()
    {
        MoveNormal();
        MoveByCamera();
        ClampMove();
    }
    private float m = 0.005f;
    private float c = 1f;

    private void MoveNormal()
    {
        Y_Movement();

        Vector2 lightPos = _rectTransform.anchoredPosition - new Vector2(0, (_startPos.y + _yOffset) / _yDivide);
        float B = m * lightPos.y + c;

        _lightSource.transform.localPosition = new Vector3(0, B, 0);

        Vector3 clampedPosition = new Vector3(
            _lightSource.transform.localPosition.x,
            Mathf.Clamp(_lightSource.transform.localPosition.y, -0.6f, 0.5f),
            _lightSource.transform.localPosition.z
        );

        _lightSource.transform.localPosition = clampedPosition;

        Debug.Log("Clamped Position: " + clampedPosition);
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
        _active = !_active;
        if (_active)
        {
            _lightSource.SetActive(true);
            _image.color = _activeColor;
        }
        else
        {
            _lightSource.SetActive(false);
            _image.color = _deactiveColor;
        }
    }

    public override void OnSelected()
    {
        if (_image == null)
            _image = GetComponent<Image>();
    }

    public override void OnChanged()
    {
        Debug.Log("ONCHANGED1");
        _active = false;
        _lightSource.SetActive(false);
        Debug.Log("ONCHANGED2");
    }
}

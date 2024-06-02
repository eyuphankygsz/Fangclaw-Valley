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
    private void MoveNormal()
    {
        Y_Movement();

        Vector2 lightPos = _rectTransform.anchoredPosition - new Vector2(_startPos.x ,_startPos.y + 40);

        Debug.Log("Light Pos: " + lightPos);

        Vector3 movement = ((transform.up * lightPos.y / 100) + (transform.right * lightPos.x / 100)) * Time.deltaTime;

        Debug.Log("Movement: " + movement);

        _lightSource.transform.position += movement * 4;

        Vector3 clampedPosition = new Vector3(
            Mathf.Clamp(_lightSource.transform.localPosition.x, -0.4f, 0.4f),
            Mathf.Clamp(_lightSource.transform.localPosition.y, -0.6f, 0.6f),
            0.7f
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
        if(_active)
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
        if(_image == null)
            _image = GetComponent<Image>();
    }
}

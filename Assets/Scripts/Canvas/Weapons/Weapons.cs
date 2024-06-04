using System.Threading;
using UnityEngine;

public abstract class Weapons : MonoBehaviour
{
    [SerializeField] protected float Damage { get; set; }
    [SerializeField] protected float _moveSpeed;


    [SerializeField] protected Vector2 _xAxis;
    [SerializeField] protected Vector2 _yAxis;
    [SerializeField] protected float _xPolynomial;
    [SerializeField] protected float _ySpeed;

    protected Vector2 _startPos;
    protected float _minY;
    protected float _xPolyStart;
    protected RectTransform _rectTransform;

    private float _xPos;
    private void Awake()
    {
        Setup();
    }
    private void OnEnable()
    {
        if (_rectTransform != null) return;
        Setup();
    }
    private void Setup()
    {
        _rectTransform = GetComponent<RectTransform>();
        _startPos = _rectTransform.anchoredPosition;
        _xPolyStart = _xPolynomial;
        _minY = GetYPos();
        _xPos = _startPos.x;
    }
    public abstract void OnAction();
    public abstract void OnSelected();
    public abstract void OnChanged();
    public abstract void Move();
    protected void Y_Movement()
    {
        float y = GetYPos();

        _rectTransform.anchoredPosition = new Vector2(_xPos, _startPos.y) + (Vector2.up * y);
        _xPolynomial += _ySpeed * Time.deltaTime;

        if (y < _minY)
            _xPolynomial = _xPolyStart;
    }
    protected void X_Movement()
    {
        float x = PlayerInputs.Instance.GetCameraDirection().x;

        if (x != 0)
        {
            Vector2 moveDirection = new Vector2(-x, 0);
            _rectTransform.anchoredPosition += moveDirection * 400 * Time.deltaTime;
        }
        else if (_rectTransform.anchoredPosition.x != _startPos.x)
        {
            _rectTransform.anchoredPosition = Vector3.Lerp(_rectTransform.anchoredPosition, new Vector2(_startPos.x, _rectTransform.anchoredPosition.y), 10 * Time.deltaTime);
        }
        _xPos = _rectTransform.anchoredPosition.x;
    }
    protected void ClampTransform()
    {
        float x = Mathf.Clamp(_rectTransform.anchoredPosition.x, _xAxis.x, _xAxis.y);
        float y = Mathf.Clamp(_rectTransform.anchoredPosition.y, _yAxis.x, _yAxis.y);
        _rectTransform.anchoredPosition = new Vector2(x, y);
    }
    private float GetYPos()
    {
        return (-Mathf.Pow(_xPolynomial, 2) / 20) - (3 * _xPolynomial / 10) + 80;
    }
}

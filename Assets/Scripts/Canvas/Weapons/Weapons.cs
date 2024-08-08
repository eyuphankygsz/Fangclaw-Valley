using System.Collections;
using System.Threading;
using UnityEngine;

public abstract class Weapons : MonoBehaviour
{
    [SerializeField] protected float Damage { get; set; }
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected Enum_Weapons _weaponEnum; // Daha sonra weapon class ekle (Attackable)
    [SerializeField] protected float _rayLength;
    [SerializeField] protected float _rayRotation;
    [SerializeField] protected float _ySpeed;
    [SerializeField] protected LayerMask _interactableLayers;
    [SerializeField] protected Sprite _weaponCross;

	protected  Vector2 _xLimit = new Vector2(-1.2f, 1.8f);
    protected Vector2 _yLimit = new Vector2(-0.2f, 1.2f);
    protected Transform _pivot, _camera;
    protected GameObject _hitObject;
    protected Animator _animator;
    protected WaitForSeconds _actionSleep;
    protected bool _onAction;

    private float _xPolynomial = -1.376f;
    private Vector2 _startPos;
    private float _xPolyStart;
    private float _xPos;
    private void Awake()
    {
        Setup();
        SetWeapon();
    }
    private void OnEnable()
    {
        if (_startPos != null) return;
        Setup();
    }
    private void Setup()
    {
        _pivot = transform.parent;
        _startPos = _pivot.transform.localPosition;
        _xPolyStart = _xPolynomial;
        _xPos = _startPos.x;
        _camera = Camera.main.transform;
        _animator = GetComponent<Animator>();
    }
    public abstract void OnAction();
    public abstract void OnSelected();
    public abstract void OnChanged();
    public abstract void SetWeapon();
    public abstract void Move();
    public GameObject GetHitObject()
    {
        GameObject tempObj = _hitObject;
        _hitObject = null;
        return tempObj;
    }
    protected void Y_Movement()
    {
        float y = GetYPos();

        _pivot.transform.localPosition = new Vector3(_xPos, _startPos.y, 0) + (Vector3.up * y);
        _xPolynomial += _ySpeed * Time.deltaTime;

        if (y < _yLimit.x)
        {
            _xPolynomial = _xPolyStart;
        }
    }
    protected void X_Movement()
    {
        float x = MouseDirection.Instance.GetCameraDirection().x;

        if (x != 0)
        {
            Vector3 moveDirection = new Vector3(-x, 0, 0);
            _pivot.transform.localPosition += moveDirection * _moveSpeed * Time.deltaTime;
        }
        else if (_pivot.transform.localPosition.x != _startPos.x)
        {
            _pivot.transform.localPosition = Vector3.Lerp(_pivot.transform.localPosition, new Vector2(_startPos.x, _pivot.transform.localPosition.y), 10 * Time.deltaTime);
        }
        _xPos = _pivot.transform.localPosition.x;
    }
    protected void ClampTransform()
    {
        float x = Mathf.Clamp(_pivot.transform.localPosition.x, _xLimit.x, _xLimit.y);
        float y = Mathf.Clamp(_pivot.transform.localPosition.y, _yLimit.x, _yLimit.y);
        _pivot.transform.localPosition = new Vector3(x, y, 0);
    }
    public Enum_Weapons GetWeaponEnum()
    {
        return _weaponEnum;
    }
    protected IEnumerator EndAction() { yield return _actionSleep; _onAction = false; }
    private float GetYPos()
    {
        return (-Mathf.Pow(_xPolynomial, 2) / 1.5f) - (_xPolynomial / 10) + 1.2f;
    }

    //private void OnDrawGizmos()
    //{
    //    if (_camera == null) _camera = Camera.main.transform;
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawRay(_camera.position, _camera.forward * _rayLength);
    //}
    public Sprite GetCross()
    {
        return _weaponCross;
    }
}

using DG.Tweening;
using System.Collections;
using UnityEngine;
using Zenject;

public abstract class Weapons : MonoBehaviour, ISaveable
{

	[SerializeField] protected float Damage { get; set; }
	[SerializeField] protected float _moveSpeed;
	[SerializeField] protected Enum_Weapons _weaponEnum; // Daha sonra weapon class ekle (Attackable)
	[SerializeField] protected float _rayLength;
	[SerializeField] protected float _rayRotation;
	[SerializeField] protected float _yMoveDuration;
	[SerializeField] protected LayerMask _interactableLayers;
	[SerializeField] protected Sprite _weaponCross;
	[SerializeField] protected AudioSource _source;

	public bool IsPicked;
	public bool CanChange = true;

	[SerializeField] protected Vector2 _xLimit = new Vector2(-1.2f, 1.8f);
	[SerializeField] protected Vector2 _yLimit = new Vector2(-0.2f, 1.2f);
	protected Transform _pivot, _camera;
	protected GameObject _hitObject;
	protected Animator _animator;
	protected WaitForSeconds _actionSleep;
	protected bool _onAction;
	protected bool _isFreeze;
	protected ControlSchema _controls;
	protected Vector2 _startPos;

	private float _xPos;
	private bool _moveUp, _waitMove;

	[Inject]
	protected WeaponHelpers _weaponHelpers;
	[Inject]
	protected SaveManager _saveManager;

	private void Awake()
	{
		Setup();
		SetWeapon();
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
	}
	private void Start()
	{
		SetLoadFile();
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
		_xPos = _startPos.x;
		_camera = Camera.main.transform;
		_animator = GetComponent<Animator>();
		if (_source == null)
			_source = GetComponent<AudioSource>();
	}
	public abstract void OnSelected(ControlSchema schema);
	public abstract void OnChanged();
	public abstract void SetWeapon();
	public abstract void Move();
	public abstract GameData GetSave();
	public abstract void LoadSave();
	public void SetFreeze(bool freeze)
	{
		_isFreeze = freeze;
	}

	public GameObject GetHitObject()
	{
		GameObject tempObj = _hitObject;
		_hitObject = null;
		return tempObj;
	}
	protected void Y_Movement()
	{
		if (_forceSaved)
		{
			float distance = _lastPos.y;
			_pivot.transform.localPosition += new Vector3(0, 45 * Time.deltaTime, 0);
			if (distance - _pivot.transform.localPosition.y <= 0)
				_forceSaved = false;
			return;
		}

		if (!_waitMove)
			TryMoveTo();
	}
	private void TryMoveTo()
	{
		Debug.Log("TryMoveTo");
		_waitMove = true;
		_moveUp = !_moveUp;

		_pivot.transform.DOLocalMoveY(_moveUp ? _yLimit.y : _yLimit.x, _yMoveDuration).OnComplete(() => _waitMove = false).SetEase(Ease.InOutSine);
	}
	protected void X_Movement()
	{
		float x = MouseDirection.Instance.GetCameraDirection().x;
		x = Mathf.Clamp(x, -1, 1);
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

	private bool _forceSaved;
	private Vector3 _lastPos;
	public void OnForce()
	{
		if (!_forceSaved)
		{
			_forceSaved = true;
			_lastPos = _pivot.transform.localPosition;
		}

		if (_pivot.transform.localPosition.y > -10)
			_pivot.transform.localPosition -= new Vector3(_xPos, 45 * Time.deltaTime, 0);
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

	public abstract void SetWeaponControls(bool setEnable);
	private void OnDrawGizmos()
	{
		if (_camera == null)
			_camera = Camera.main.transform;

		Gizmos.color = Color.red;
		Gizmos.DrawRay(_camera.position, _camera.forward * _rayLength);
	}
	public Sprite GetCross()
	{
		return _weaponCross;
	}

	public GameData GetSaveFile()
	{
		return GetSave();
	}

	public void SetLoadFile()
	{
		LoadSave();
	}
}

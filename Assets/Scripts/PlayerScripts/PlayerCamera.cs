using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using Zenject;

public class PlayerCamera : MonoBehaviour
{
	public float MouseSensitivity { get => _mouseSensitivity; set => _mouseSensitivity = value; }
	private float _pitch = 0f;
	private float _yaw = 0f;

	[SerializeField] private float _mouseSensitivity = 5f;
	[SerializeField] private float _rotationSpeed;
	[SerializeField] private Transform _camera;
	[SerializeField] private Transform _cameraHolderBase;
	[Inject]
	private GameManager _gameManager;

	private bool _force, _inspecting;

	private bool _randomMoveBool, _lockHelper;

	public bool LockRandom;

	private TweenerCore<Quaternion, Quaternion, NoOptions> _tw1, _tw2;
	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		// Mevcut kameran�n rotasyonunu al ve pitch/yaw hesapla
		Vector3 cameraEulerAngles = _camera.localRotation.eulerAngles;
		_pitch = cameraEulerAngles.x;
		_yaw = transform.eulerAngles.y;
	}
	private void Start()
	{
		_gameManager.OnForce += Force;
		_gameManager.OnInspecting += Inspecting;
	}
	private void Force(bool force)
	{
		if (!force)
			SetCameraRotation();

		_force = force;
	}
	private void Inspecting(bool inspect)
	{
		_inspecting = inspect;
		
		if (inspect)
		{
			if (_tw1 != null)
			{
				_tw1.Kill();
				_tw2.Kill();
			}

			_randomMoveBool = false;
		}
	}
	public void ManageRotate()
	{
		if (_force || _inspecting) return;

		if (!_randomMoveBool && !LockRandom)
		{
			float randomX = Random.Range(-1f, 1f); // Daha k���k ve kontroll� yatay sallant�
			float randomY = Random.Range(-1f, 1f); // Daha k���k ve kontroll� dikey sallant�
			_lockHelper = false;

			_tw1 = _cameraHolderBase
				.DOLocalRotateQuaternion(
					Quaternion.Euler(randomX, randomY, 0),
					Random.Range(1.2f, 1.5f) // H�zl� ama do�al bir s�re
				)
				.SetEase(Ease.InOutSine) // Daha yumu�ak bir ge�i�
				.OnComplete(() =>
				{
					// Kameray� ba�lang�� pozisyonuna d�nd�r
					randomX = Random.Range(-1f, 1f);
					randomY = Random.Range(-1f, 1f); ;

					_tw2 = _cameraHolderBase
						.DOLocalRotateQuaternion(Quaternion.Euler(randomX, randomY, 0), Random.Range(1.2f, 1.5f))
						.SetEase(Ease.InOutSine)
						.OnComplete(RandomComplete); // D�ng�y� devam ettir
				});

			_randomMoveBool = true;
		}
		else if (LockRandom && !_lockHelper)
		{
			_cameraHolderBase.DOPause();
			_lockHelper = true;
		}
		else if (!LockRandom)
			_cameraHolderBase.DOPlay();

		Vector2 cameraDirection = MouseDirection.Instance.GetCameraDirection();

		float gamepadMultiplier = InputDeviceManager.Instance.CurrentDevice == InputDeviceManager.InputDeviceType.Gamepad ? 12 : 1;
		float mouseX = cameraDirection.x * _rotationSpeed * Time.deltaTime * gamepadMultiplier;
		float mouseY = cameraDirection.y * _rotationSpeed * Time.deltaTime * gamepadMultiplier;

		_yaw += mouseX;
		_pitch -= mouseY;

		_pitch = Mathf.Clamp(_pitch, -90f, 90f);

		_camera.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
		transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
	}
	public void RandomLock(bool locked)
	{
		LockRandom = locked;
	}
	private void RandomComplete()
	{
		_randomMoveBool = false;
	}
	public void SetCameraRotation()
	{
		Vector3 newEulerAngles = _camera.eulerAngles;
		_pitch = newEulerAngles.x;
		if (_pitch > 180f) _pitch -= 360f;
		_yaw = newEulerAngles.y;
	}

	public void SetSensitivity(float value)
	{
		_mouseSensitivity = value;
		_rotationSpeed = _mouseSensitivity * 20f;
	}
}

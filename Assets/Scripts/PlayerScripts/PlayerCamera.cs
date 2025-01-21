using DG.Tweening;
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

	private bool _force;

	private bool _randomMoveBool;
	private void Awake()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		// Mevcut kameranýn rotasyonunu al ve pitch/yaw hesapla
		Vector3 cameraEulerAngles = _camera.localRotation.eulerAngles;
		_pitch = cameraEulerAngles.x;
		_yaw = transform.eulerAngles.y;
	}
	private void Start()
	{
		_gameManager.OnForce += Force;
	}
	private void Force(bool force)
	{
		if (!force)
			SetCameraRotation();

		_force = force;
	}
	public void ManageRotate()
	{
		if (_force) return;

		if (!_randomMoveBool)
		{
			float randomX = Random.Range(-2f, 2f); // Daha küçük ve kontrollü yatay sallantý
			float randomY = Random.Range(-2f, 2f); // Daha küçük ve kontrollü dikey sallantý
			// Kamerayý rastgele bir açýya döndür
			_cameraHolderBase
				.DOLocalRotateQuaternion(
					Quaternion.Euler(randomX, randomY, 0),
					Random.Range(1.2f, 1.5f) // Hýzlý ama doðal bir süre
				)
				.SetEase(Ease.InOutSine) // Daha yumuþak bir geçiþ
				.OnComplete(() =>
				{
					// Kamerayý baþlangýç pozisyonuna döndür
					randomX = Random.Range(-2f, 2f);
					randomY = Random.Range(-2f, 2f);;
					
					_cameraHolderBase
						.DOLocalRotateQuaternion(Quaternion.Euler(randomX, randomY, 0), Random.Range(1.2f,1.5f))
						.SetEase(Ease.InOutSine)
						.OnComplete(RandomComplete); // Döngüyü devam ettir
				});

			_randomMoveBool = true;
		}
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

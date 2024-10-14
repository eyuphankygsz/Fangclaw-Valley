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
	[Inject]
	private GameManager _gameManager;

	private bool _force;
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

		Vector2 cameraDirection = MouseDirection.Instance.GetCameraDirection();

		float mouseX = cameraDirection.x * _rotationSpeed * Time.deltaTime;
		float mouseY = cameraDirection.y * _rotationSpeed * Time.deltaTime;

		_yaw += mouseX;
		_pitch -= mouseY;

		_pitch = Mathf.Clamp(_pitch, -90f, 90f); 

		_camera.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
		transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
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
		Debug.Log(_mouseSensitivity);
		_mouseSensitivity = value;
		_rotationSpeed = _mouseSensitivity * 100f;
	}
}

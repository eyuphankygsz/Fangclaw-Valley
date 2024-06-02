using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private float _pitch = 0f;

    [SerializeField] private float _rotateSpeed;
    [SerializeField] private Camera _camera;

    public void TryRotate()
    {
        Vector2 cameraDirection = PlayerInputs.Instance.GetCameraDirection();

        float mouseX = cameraDirection.x * _rotateSpeed * Time.deltaTime;
        float mouseY = cameraDirection.y * _rotateSpeed * Time.deltaTime;

        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, -90f, 90f); // Clamp the pitch to prevent flipping

        _camera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}

using UnityEngine;

public class MouseDirection : MonoBehaviour
{
    public static MouseDirection Instance { get; private set; }

    private void Awake()
    {
            Instance = this;
    }
    public Vector2 GetCameraDirection()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        return new Vector2(mouseX, mouseY);
    }
}

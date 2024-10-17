using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class ScrollControls : MonoBehaviour
{
    [SerializeField]
    private ScrollRect _scrollRect;
    [SerializeField]
    private float scrollSpeed = 0.1f;

    [Inject]
    private InputManager _inputManager;

    private bool _isInitialized;
    private void Start()
    {
        if (!_isInitialized)
        {
            _inputManager.Controls.Player.InventoryScroll.performed += Scroll;
            _isInitialized = true;
        }
    }
    void OnEnable()
    {
        if (_inputManager != null && !_isInitialized && _inputManager.Controls != null)
        {
            _inputManager.Controls.Player.InventoryScroll.performed += Scroll;
            _isInitialized = true;
        }
    }
    void OnDisable()
    {
        if (_inputManager.Controls != null)
        {
            _inputManager.Controls.Player.InventoryScroll.performed -= Scroll;
            _isInitialized = false;
        }
    }

    private void Scroll(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Vector2 move = ctx.ReadValue<Vector2>();
            _scrollRect.verticalNormalizedPosition += move.y * scrollSpeed * Time.deltaTime;
        }
    }

}

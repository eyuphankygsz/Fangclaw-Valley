using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _controller;

    [SerializeField] private float _speed;
    
    // Start is called before the first frame update
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void TryMove()
    {
        Vector3 direction = PlayerInputs.Instance.GetMovementInput();
        Vector3 forward = transform.forward * direction.z;
        Vector3 right = transform.right * direction.x;
        _controller.Move((forward + right)  * _speed * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool GravityOff { get; set; }

    private CharacterController _controller;
    private float _gravity = -9.8f;


    [SerializeField] private float _gravityMultiplier = 1;
    [SerializeField] private float _speed;
    
    // Start is called before the first frame update
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void ManageMove()
    {
        ApplyMovement();
    }
    private float CalculateGravity()
    {
        return GravityOff ? 0 :_gravity * _gravityMultiplier;
    }
    private void ApplyMovement()
    {
        Vector3 direction = PlayerInputs.Instance.GetMovementInput();
        Vector3 movement = (transform.forward * direction.z) + (transform.right * direction.x);
        movement *= _speed;
        movement.y = CalculateGravity();
        _controller.Move(movement * Time.deltaTime);
    }
}

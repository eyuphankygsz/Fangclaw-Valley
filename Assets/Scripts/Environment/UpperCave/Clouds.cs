using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed;
    private void LateUpdate()
    {
        transform.Rotate(Vector3.up * _rotateSpeed * Time.deltaTime);
    }
}

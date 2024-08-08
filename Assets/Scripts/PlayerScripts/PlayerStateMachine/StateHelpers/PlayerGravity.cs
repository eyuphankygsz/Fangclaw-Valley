using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
	private bool _gravityOff; 
    private float _gravity = -9.8f;
	[SerializeField] private float _gravityMultiplier = 1;

	public float CalculateGravity()
	{
		return _gravityOff ? 0 : _gravity * _gravityMultiplier;
	}
}

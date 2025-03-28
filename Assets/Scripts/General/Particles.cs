using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem[] _particles;

	[SerializeField]
	private bool _isActive;


	private void Awake()
	{
		if(_isActive)
			StartParticles();
		else
			StopParticles();
	}
	public void StopParticles()
	{
		foreach (var item in _particles)
		{
			var emission = item.emission;
			emission.enabled = false;
		}
	}
	public void StartParticles()
	{
		foreach (var item in _particles)
		{
			var emission = item.emission;
			emission.enabled = true;
		}
	}
}

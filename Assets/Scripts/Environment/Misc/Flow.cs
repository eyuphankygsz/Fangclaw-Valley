using UnityEngine;

public class Flow : MonoBehaviour
{
	[SerializeField]
	private float _speed;
	[SerializeField]
	private float _min, _max;

	private float _currentSpeed;
	private float _target;
	private Light _light;

	private void Awake()
	{
		_light = GetComponentInChildren<Light>();
		SetNewTarget();
	}
	private void SetNewTarget()
	{
		_target = Random.Range(_min, _max);
		_currentSpeed = _target > _light.intensity ? _speed : -_speed;
	}

	public void GoToTarget()
	{
		if (_light == null)
			_light = GetComponentInChildren<Light>();

		_light.intensity += _currentSpeed * Time.deltaTime;
		if ((_currentSpeed > 0 && _light.intensity > _target)
			 || (_currentSpeed < 0 && _light.intensity < _target))
			_light.intensity = _target;


		if (_light.intensity == _target)
		{
			SetNewTarget();
		}
	}
}

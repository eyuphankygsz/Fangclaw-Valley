using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MouseSensitivitySetting : Setting
{
	[SerializeField]
	private Slider _slider;
	[SerializeField]
	private PlayerCamera _camera;

	private LiftGammaGain _liftGammaGain;

	private float _currentValue;
	private float _tempValue;

	private void Start() =>
		SetSensitivityParam();
	public void SetSensitivityTemp(Slider slider) =>
		_tempValue = slider.value;

	private void SetSensitivityParam()
	{
		if (_camera != null)
			_camera.SetSensitivity(_currentValue);
	}

	public override void Restore()
	{
		_tempValue = _currentValue;
		_slider.value = _currentValue;
		SetSensitivityParam();
	}

	public override void Save()
	{
		_currentValue = _tempValue;
		PlayerPrefs.SetFloat("Sensitivity", _currentValue);
		SetSensitivityParam();

	}

	public override void Load()
	{
		if (!PlayerPrefs.HasKey("Sensitivity"))
			PlayerPrefs.SetFloat("Sensitivity", .4f);

		_currentValue = PlayerPrefs.GetFloat("Sensitivity");
		_tempValue = _currentValue;
		_slider.value = _currentValue;

		SetSensitivityParam();
	}
}

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GammaSetting : Setting
{
	[SerializeField]
	private Volume _postProcessing;
	[SerializeField]
	private Slider _slider;


    private LiftGammaGain _liftGammaGain;

    private float _currentValue;
	private float _tempValue;


    private void Start() =>
		SetGammaParam();
	public void SetGammaTemp(Slider slider)
	{
		_tempValue = slider.value;

        if (_postProcessing != null && _postProcessing.profile.TryGet<LiftGammaGain>(out _liftGammaGain))
		{
            _liftGammaGain.gamma.value = new Vector4(0, 0, 0, _tempValue);
            _liftGammaGain.gamma.overrideState = true;
        }
    }

	private void SetGammaParam()
	{
        if (_postProcessing != null && _postProcessing.profile.TryGet<LiftGammaGain>(out _liftGammaGain))
            _liftGammaGain.gamma.value = new Vector4(0, 0, 0, _currentValue);

		PlayerPrefs.SetFloat("Gamma", _currentValue);
    }

    public override void Restore()
	{
		_tempValue = _currentValue;
		_slider.value = _currentValue;
		SetGammaParam();
	}

	public override void Save()
	{
		_currentValue = _tempValue;
		SetGammaParam();

	}

	public override void Load()
	{
		if (!PlayerPrefs.HasKey("Gamma"))
			PlayerPrefs.SetFloat("Gamma", 0);

		_currentValue = PlayerPrefs.GetFloat("Gamma");
		_tempValue = _currentValue;
		_slider.value = _currentValue;


		if (_postProcessing != null && _postProcessing.profile.TryGet<LiftGammaGain>(out _liftGammaGain))
			_liftGammaGain.gamma.value = new Vector4(0, 0, 0, _currentValue);
	}
}

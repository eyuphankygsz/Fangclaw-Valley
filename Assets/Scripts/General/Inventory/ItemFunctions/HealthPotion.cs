using UnityEngine;

public class HealthPotion : UseFunction
{
	[SerializeField]
	private PlayerHealth _health; 
	
	private float _targetLensDistortion = -0.5f;
	private float _targetFocusDistance = 0f;
	private float _targetChromaticAberration = 1.1f;
	private Color _targetColor = new Color(210f / 255f, 0f, 0f);

	[SerializeField]
	private PostProcessingChanger _ppc;
	public override bool Use()
	{
		if (_health.Health == 100)
		{
			DialogueManager.Instance.PlayNewOne(_cantUse);
			return false;
		}

        _health.AddHealth(35);
		_ppc.StartProcessChange(_targetLensDistortion, _targetFocusDistance, _targetChromaticAberration, _targetColor, null, null);
		return true;
	}
}

using UnityEngine;

public class StaminaPotion : UseFunction
{
	[SerializeField]
	private PlayerStamina _playerStamina;

	private float _targetLensDistortion = -0.5f;
	private float _targetFocusDistance = 0f;
	private float _targetChromaticAberration = 1.1f;
	private Color _targetColor = new Color(0f, 80f / 255f, 1f);

	[SerializeField]
	private PostProcessingChanger _ppc;

	public override bool Use()
	{
		if (_playerStamina.Stamina == 100)
		{
			DialogueManager.Instance.PlayNewOne(_cantUse);
			return false;
		}

		_playerStamina.AddStamina(35);
		_ppc.StartProcessChange(_targetLensDistortion, _targetFocusDistance, _targetChromaticAberration, _targetColor, null, null);
		return true;
	}
}

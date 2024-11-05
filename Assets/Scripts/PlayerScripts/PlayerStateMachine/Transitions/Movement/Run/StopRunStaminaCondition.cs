using UnityEngine;

public class StopRunStaminaCondition : AbstractCondition
{
	[SerializeField]
	private PlayerStamina _playerStamina;
	[SerializeField]
	private AudioSource _source;
	[SerializeField]
	private AudioClip _clip;


	private bool _playing;

	public override bool CheckCondition()
	{
		if (_playerStamina.Stamina == 0)
		{
			if (!_playing)
			{
				_playing = true;
				_source.clip = _clip;
				_source.Play();
			}
			return true;
		}
		_playing = false;
		return false;
	}

    public override void ResetFrameFreeze()
    {

    }
}

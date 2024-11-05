using UnityEngine;
using Zenject;

public class JumpStaminaCondition : AbstractCondition
{
    [SerializeField]
    private PlayerStamina _playerStamina;
    [SerializeField]
    private float _stamina;

    public override bool CheckCondition()
    {
        if (_playerStamina.Stamina > _stamina)
            return true;
        return false;
    }

    public override void ResetFrameFreeze()
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
	private bool _attacking;
	private bool _attackWait;

	[SerializeField]
	private Transform _attackPos;
	[SerializeField]
	private float _attackRadius;
	[SerializeField]
	private LayerMask _layer;


	[SerializeField]
	private PlayerHealth _pHealth;


	private float _targetLensDistortion = -0.5f;
	private float _targetFocusDistance = 0f;
	private float _targetChromaticAberration = 1.3f;
	private Color _targetColor = new Color(121f / 255, 0, 0);

	[SerializeField]
	private PostProcessingChanger _ppc;




	public void SetAttackTrue() =>
		_attacking = true;
	public void SetAttackFalse()
	{
		_attacking = false;
		_attackWait = false;
	}

	public void TryAttack()
	{
		if (!_attacking || _attackWait) return;

		Collider[] cols = Physics.OverlapSphere(_attackPos.position, _attackRadius, _layer);
		if (cols.Length == 0) return;
		_ppc.StartProcessChange(_targetLensDistortion, _targetFocusDistance, _targetChromaticAberration, _targetColor, null, null, 0.6f);
		_attackWait = true;
		_pHealth.AddHealth(-10);

	}


	private void OnDrawGizmos()
	{
		if (!_attacking) return;

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(_attackPos.position, _attackRadius);
	}
}

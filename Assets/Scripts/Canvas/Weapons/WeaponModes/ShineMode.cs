using UnityEngine;

public class ShineMode : MonoBehaviour
{
	public Weapons Weapon { get; set; }
	[SerializeField]
	private Transform _center;
	[SerializeField]
	private Vector3 _halfExtents;
	[SerializeField]
	private float _range;
	[SerializeField]
	private LayerMask _enemyLayer, _seeThrough;

	private bool _hitBool;
	RaycastHit _hit;

	[SerializeField]
	private AchievementCheck _eyesWideShut;
	[SerializeField]
	private SteamAchievements _achievements;

	private IEnemyController _lastHit;

	public void ExecuteModeUpdate()
	{
		RaycastHit hit;
		if (Physics.SphereCast(_center.position, _halfExtents.x, _center.forward, out hit, _range, _enemyLayer))
		{
			RaycastHit hitOthers;
			if (Physics.SphereCast(_center.position, _halfExtents.x, _center.forward, out hitOthers))
			{
				int layer = hitOthers.collider.gameObject.layer;
				if ((_seeThrough.value & (1 << layer)) != 0)
				{
					if (hit.transform.TryGetComponent<IEnemyController>(out IEnemyController controller))
					{
						_hit = hit;
						if (!_hitBool)
							_achievements.TryEnableAchievement(_eyesWideShut);

						if (_lastHit != controller)
						{
							StopShine();
						}

						_lastHit = controller;
						controller.Shined();
						_hitBool = true;
					}
					else
					{
						StopShine();
						_hitBool = false;
					}
				}
				else
				{
					StopShine();
				}
			}
			else
			{
				StopShine();
			}

		}
		else
		{
			_hitBool = false;
			StopShine();
		}
	}

	public void StopMode()
	{
		StopShine();
	}
	private void StopShine()
	{
		if (_lastHit != null)
		{
			_lastHit.StopShined();
			_lastHit = null;
		}
	}



	private void OnDrawGizmos()
	{
		if (!gameObject.activeSelf) return;

		Gizmos.color = _hitBool ? Color.yellow : Color.red;

		Vector3 endPoint = _hitBool ? _hit.point : _center.position + (_center.forward * _range);

		Gizmos.DrawLine(_center.position, endPoint);
		Gizmos.DrawWireSphere(endPoint, _halfExtents.x);

	}
}

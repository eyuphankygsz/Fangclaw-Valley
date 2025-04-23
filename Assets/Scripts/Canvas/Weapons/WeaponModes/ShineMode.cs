using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
	private Interactable_LightMe _lastLightMe;

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

						if (hit.transform.TryGetComponent<Interactable_LightMe>(out Interactable_LightMe lightme))
						{
							_hit = hit;

							if (_lastLightMe != lightme && _lastLightMe != null)
								_lastLightMe.OnStopInteract(Enum_Weapons.Lantern);

							_lastLightMe = lightme;
							lightme.OnInteract(Enum_Weapons.Lantern);
						}
						else if (_lastLightMe != null)
						{
							_lastLightMe.OnStopInteract(Enum_Weapons.Lantern);
							_lastLightMe = null;
						}
					}


				}
				else
				{
					StopShine();
				}
			}
		}
		else
		{
			if (_lastLightMe != null)
			{
				_lastLightMe.OnStopInteract(Enum_Weapons.Lantern);
				_lastLightMe = null;
			}
			_hitBool = false;
			StopShine();
		}

	}

	public void StopMode()
	{
		if (_lastLightMe != null)
		{
			_lastLightMe.OnStopInteract(Enum_Weapons.Lantern);
			_lastLightMe = null;
		}

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



		//RaycastHit hit;
		//if (Physics.SphereCast(_center.position, _halfExtents.x, _center.forward, out hit, _range, _enemyLayer))
		//{
		//	RaycastHit hitOthers;
		//	_hit = hit;
		//			Debug.Log("mal1");
		//	_hitBool = true;
		//	if (Physics.SphereCast(_center.position, _halfExtents.x, _center.forward, out hitOthers))
		//	{
		//		int layer = hitOthers.collider.gameObject.layer;
		//		if ((_seeThrough.value & (1 << layer)) == 0)
		//		{
		//			_hit = hitOthers;
		//			Debug.Log("mal2");
		//		}
		//	}
		//}
		//else if (Physics.SphereCast(_center.position, _halfExtents.x, _center.forward, out hit, _range))
		//{
		//	int layer = hit.collider.gameObject.layer;
		//	if ((_seeThrough.value & (1 << layer)) == 0)
		//	{
		//		_hitBool = true;
		//		Debug.Log("mal:::" + layer);
		//		_hit = hit;
		//	}
		//	else
		//	{
		//		_hitBool = false;
		//	}
		//}


		Gizmos.color = _hitBool ? Color.yellow : Color.red;

		Vector3 endPoint = _hitBool ? _hit.point : _center.position + (_center.forward * _range);

		Gizmos.DrawLine(_center.position, endPoint);
		Gizmos.DrawWireSphere(endPoint, _halfExtents.x);

	}
}

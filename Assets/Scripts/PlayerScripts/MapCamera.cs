using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
	[SerializeField]
	private Transform _camera;
	[SerializeField]
	private Transform _player;

	[SerializeField]
	private float _playerPosY;
	public void SetMap(Vector3 pos, Vector3 rot)
	{
		pos.y = _camera.position.y;
		_camera.position = pos;

		pos.y = _playerPosY;
		_player.position = pos;

		rot.x = 0;
		rot.z = 0;
		rot.y += 180;
		_player.rotation = Quaternion.Euler(rot);
	}
}

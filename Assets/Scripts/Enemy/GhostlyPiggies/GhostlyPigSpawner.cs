using System.Collections;
using System.Linq;
using UnityEngine;

public class GhostlyPigSpawner : MonoBehaviour
{
	[SerializeField]
	public AvailablePiggy[] _availablePiggies;

	[SerializeField]
	private Transform[] _spawnPoints;
	private bool[] _spawnPointsOccupied;
	private int _selectedSpawnPoint;

	private Vector3 _rot;
	[SerializeField]
	private Transform _player;

	private Coroutine _spawnRoutine;
	private WaitForSeconds _wait = new WaitForSeconds(5);
	private bool _spawning;

	[SerializeField]
	private DidPigSpawned _didPigSpawned;

	private void Awake()
	{
		_spawnPointsOccupied = new bool[_spawnPoints.Length];
	}
	public void SetSpawnPoints(Transform tf)
	{
		_spawnPoints = new Transform[tf.childCount];
		for (int i = 0; i < tf.childCount; i++)
			_spawnPoints[i] = tf.GetChild(0);


		_spawnPointsOccupied = new bool[_spawnPoints.Length];
	}

	private Transform GetNewPosition()
	{
		_selectedSpawnPoint = Random.Range(0, _spawnPoints.Length);
		if (_spawnPointsOccupied[_selectedSpawnPoint])
			return GetNewPosition();
		else
		{
			_spawnPointsOccupied[_selectedSpawnPoint] = true;
			return _spawnPoints[_selectedSpawnPoint];
		}
	}
	private Quaternion GetNewRotation(Transform piggy)
	{
		_rot = _player.position - piggy.position;
		_rot.y = 0;

		return Quaternion.LookRotation(_rot);
	}

	public void StartPiggySpawner()
	{
		Debug.Log("START PIG SPAWNER");
		if (_spawnRoutine != null)
			StopCoroutine(_spawnRoutine);

		_spawnRoutine = StartCoroutine(PiggySpawnTimer());
	}

	public void StopPiggySpawner()
	{
		Debug.Log("STOP PIG SPAWNER");
		_spawning = false;
		StopCoroutine(_spawnRoutine);
	}
	private IEnumerator PiggySpawnTimer()
	{
		_spawning = true;

		while (_spawning)
		{
			_wait = new WaitForSeconds(Random.Range(20, 40));
			yield return _wait;
			if (_spawning)
				SpawnPiggyByTimer();
		}
	}


	public void SpawnPiggyByTimer()
	{
		AvailablePiggy piggy = GetAvailablePiggy();
		if (piggy != null)
		{
			piggy.Piggy.GetComponent<GhostlyPiggyController>().SetPosition(GetNewPosition());
			piggy.Piggy.transform.rotation = GetNewRotation(piggy.Piggy.transform);

			piggy.SpawnPoint = _selectedSpawnPoint;
			piggy.IsOccupied = true;

			piggy.Piggy.SetActive(true);
			piggy.Piggy.GetComponent<EnemyStateMachine>().SetCurrentState("Follow");
		}
	}

	public void SpawnPiggy()
	{
		AvailablePiggy piggy = GetAvailablePiggy();
		if (piggy != null)
		{
			piggy.Piggy.GetComponent<GhostlyPiggyController>().SetPosition(GetNewPosition());
			piggy.Piggy.transform.rotation = GetNewRotation(piggy.Piggy.transform);

			piggy.SpawnPoint = _selectedSpawnPoint;
			piggy.IsOccupied = true;

			piggy.Piggy.SetActive(true);
			piggy.Piggy.GetComponent<EnemyStateMachine>().SetCurrentState("Follow");

			_didPigSpawned.SetPigSpawnedTrue();
		}
	}
	private AvailablePiggy GetAvailablePiggy()
	{
		foreach (AvailablePiggy piggy in _availablePiggies)
		{
			if (!piggy.IsOccupied)
			{
				piggy.IsOccupied = true;
				return piggy;
			}
		}
		return null;
	}
	public void SetOccupied(GameObject piggy)
	{
		AvailablePiggy thepiggy = _availablePiggies.Where(p => p.Piggy == piggy).First();
		thepiggy.IsOccupied = false;
		_spawnPointsOccupied[thepiggy.SpawnPoint] = false;
	}
}


[System.Serializable]
public class AvailablePiggy
{
	public GameObject Piggy;
	public bool IsOccupied;
	public int SpawnPoint;
}

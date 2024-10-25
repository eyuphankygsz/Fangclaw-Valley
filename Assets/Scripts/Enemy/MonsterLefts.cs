using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class MonsterLefts : MonoBehaviour
{
	[SerializeField]
	private Transform[] _allTransforms;
	[SerializeField]
	private bool[] _isRoomOpen;
	[SerializeField]
	private GameObject[] _leftItems;

	private List<Transform> _currentTransforms;

	[SerializeField]
	private List<MonsterTransforms> _monsterTransforms = new List<MonsterTransforms>()
	{
		new MonsterTransforms()
		{
			MonsterName = "Whisperer",
			Transforms = new List<Transform>()
		},
		new MonsterTransforms()
		{
			MonsterName = "ShutDaddy",
			Transforms = new List<Transform>()
		},
		new MonsterTransforms()
		{
			MonsterName = "XYZ",
			Transforms = new List<Transform>()
		}
	};

	public void PlaceLefts()
	{
		_currentTransforms = _allTransforms.ToList();
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				int rand = Random.Range(0, _currentTransforms.Count);
				_monsterTransforms[i].Transforms.Add(_currentTransforms[rand]);
				PlaceItem(_currentTransforms[rand], i);
				_currentTransforms.RemoveAt(rand);
			}

		}
	}

	private void PlaceItem(Transform tf, int monsterID)
	{
		Transform objTransform = tf.GetChild(Random.Range(0, tf.childCount));
		Instantiate(_leftItems[monsterID], objTransform.position, Quaternion.identity);
	}

}

[Serializable]
public class MonsterTransforms
{
	public string MonsterName;
	public List<Transform> Transforms;
}

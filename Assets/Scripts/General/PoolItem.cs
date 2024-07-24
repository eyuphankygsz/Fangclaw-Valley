using UnityEngine;

[CreateAssetMenu(fileName = "NewPoolItem", menuName = "Random Item Drop/New Pool Item")]
public class PoolItem : ScriptableObject
{
	public int INITIAL_COUNT = 10;
	public GameObject Item;
}

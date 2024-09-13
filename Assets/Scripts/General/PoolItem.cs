using UnityEngine;

[CreateAssetMenu(fileName = "NewPoolItem", menuName = "Random Item Drop/New Pool Item")]
public class PoolItem : ScriptableObject
{
	public int INITIAL_COUNT = 10;
	public int INITIAL_QUANTITY = 1;
	public GameObject Item;
}

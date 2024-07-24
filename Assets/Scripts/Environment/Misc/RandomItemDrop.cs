using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRandomItem", menuName = "Random Item Drop/New Random Item")]

public class RandomItemDrop : ScriptableObject
{
	[SerializeField]
	public List<ItemWithChance> Items = new List<ItemWithChance>();
}

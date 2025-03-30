using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AudioObject List", menuName = "AudioObject/New AudioObject List")]
public class AudioObjectList : ScriptableObject
{
	public List<AudioObject> AudioObjects;
}

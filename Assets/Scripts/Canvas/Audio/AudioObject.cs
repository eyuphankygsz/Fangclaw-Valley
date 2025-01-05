using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New AudioObject", menuName = "AudioObject/New AudioObject")]
public class AudioObject : ScriptableObject
{
	public AudioClip Clip;
	public LocalizedString Text;
	public float Delay;
}

using UnityEngine;

[CreateAssetMenu(fileName = "New Control Input", menuName = "Control Input/New Control Input")]
public class TheInput : ScriptableObject
{
	public string ActionName;
	public int InputIndex;
	public string DefaultValue;
}
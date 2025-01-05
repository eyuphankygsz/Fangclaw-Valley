using UnityEngine;

public abstract class UseFunction : MonoBehaviour
{
	public abstract bool Use();
	[SerializeField]
	protected AudioObject _cantUse;
}

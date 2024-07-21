using UnityEngine;

public class BrokeParts : MonoBehaviour
{
    private void OnEnable() => Destroy(gameObject, 2);
}

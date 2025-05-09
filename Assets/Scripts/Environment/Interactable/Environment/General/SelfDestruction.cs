using UnityEngine;

public class SelfDestruction : MonoBehaviour
{
    [SerializeField]
    private float _timer;
    
    public void Destruction()
    {
        Destroy(gameObject);
    }
}

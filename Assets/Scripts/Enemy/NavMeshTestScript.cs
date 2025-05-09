using UnityEngine;
using UnityEngine.AI;

public class NavMeshTestScript : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    private NavMeshAgent _agent;
    
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        _agent.SetDestination(_target.position);
    }
}

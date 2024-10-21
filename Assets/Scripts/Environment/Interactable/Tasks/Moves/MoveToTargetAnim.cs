using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveToTargetAnim : MonoBehaviour
{
    [SerializeField]
    private Transform _moveObj;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private UnityEvent _events;
    public void StartMove()
    {
        StartCoroutine(Move());
    }
    IEnumerator Move()
    {
        while (Vector3.Distance(_moveObj.position, _target.position) > 0.1f)
        {
            Vector3 direction = _target.position - _moveObj.position;
            _moveObj.position += direction * _speed * Time.deltaTime;
            yield return null;
        }
        _events.Invoke();
    }
}

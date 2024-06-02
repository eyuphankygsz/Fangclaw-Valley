using UnityEngine;

public abstract class Weapons : MonoBehaviour
{
    [SerializeField] protected float Damage { get; set; }
    [SerializeField] protected float _moveSpeed;


    [SerializeField] protected Vector2 _xAxis;
    [SerializeField] protected Vector2 _yAxis;
    [SerializeField] protected RectTransform _rectTransform;

    public abstract void OnAction();
    public abstract void OnSelected();
    public abstract void Move();
}

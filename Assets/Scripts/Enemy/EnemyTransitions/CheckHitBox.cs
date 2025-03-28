
using UnityEngine;

public class CheckHitBox : AbstractCondition
{
    [SerializeField]
    private LayerMask _hitLayer;
    [SerializeField]
    private Transform _transform;
    [SerializeField]
    private Vector3 _extends, _offset;
	
    public override bool CheckCondition()
    { 
        var overlap = Physics.OverlapBox(_transform.position + _offset, _extends / 2, _transform.rotation, _hitLayer);

        foreach (var item in overlap)
            item.GetComponent<HitterHelpers>().InvokeEvents();

        return overlap.Length > 0;
    }

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(_transform.position + _offset,_extends / 2);
	}

	public override void ResetFrameFreeze()
    {

    }
}

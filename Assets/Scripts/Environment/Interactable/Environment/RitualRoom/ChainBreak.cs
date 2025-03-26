using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChainBreak : MonoBehaviour
{
	[SerializeField]
	private List<MeshRenderer> _renderers;
	private float _start = 2, _end = -1.86f, _duration = 1;

	private List<MaterialPropertyBlock> _materialBlocks;

	[SerializeField]
	private UnityEvent _events, _eventsInstant;
	public void StartBreak()
	{
		_materialBlocks = new List<MaterialPropertyBlock>();
		foreach (var renderer in _renderers)
		{
			var block = new MaterialPropertyBlock();
			renderer.GetPropertyBlock(block);
			_materialBlocks.Add(block);
			renderer.SetPropertyBlock(block);
		}
		StartCoroutine(BreakChain());
	}

	public IEnumerator BreakChain()
	{
		Vector3 vector = Vector3.zero;

		float elapsedTime = 0;
		float z = _start;
		while (_materialBlocks[0].GetVector("_DissolveOffest").z != _end)
		{
			if (elapsedTime < _duration)
			{
				elapsedTime += Time.deltaTime;
				float t = elapsedTime / _duration;
			    z = Mathf.Lerp(_start, _end, t);
				Debug.Log($"z: {z}");
			}
			int i = 0;
			foreach (var item in _materialBlocks)
			{
				vector = item.GetVector("_DissolveOffest");
				vector.z = z;
				item.SetVector("_DissolveOffest", vector);
				_renderers[i].SetPropertyBlock(item);
				i++;
			}
			yield return null;
		}
		StopBreak();
	}
	public void StartInstantly() =>
		_eventsInstant?.Invoke();

	public void StopBreak() =>
		_events?.Invoke();

}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlowMachine : MonoBehaviour
{
	[SerializeField]
	private List<Flow> _flows;

	private float _deltaTime = 0.0f;
	[SerializeField]
	private TextMeshProUGUI _tmp;

	private float _timer, _updateInterval = 0.5f, _fps;
	void Update()
	{
		for (int i = 0; i < _flows.Count; i++)
			_flows[i].GoToTarget();

		if (_tmp != null)
		{
			_timer += Time.deltaTime;
			if (_timer >= _updateInterval)
			{
				_fps = 1.0f / Time.deltaTime;
				_tmp.text = "FPS: " + Mathf.RoundToInt(_fps).ToString();
				_timer = 0; // Zamaný sýfýrla
			}

		}


	}
}

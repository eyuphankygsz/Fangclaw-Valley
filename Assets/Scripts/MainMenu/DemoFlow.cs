using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoFlow : MonoBehaviour
{
	[SerializeField]
	private float _speed;
	[SerializeField]
	private Image _image;

	private bool _skip;
	void Update()
	{
		_image.rectTransform.localPosition += new Vector3(0, _speed * Time.deltaTime, 0);
		if (!_skip && _image.rectTransform.localPosition.y > 3400)
		{
			_skip = true;
			PlayerPrefs.SetString("SceneToLoad", "MainMenu");
			SceneManager.LoadScene("LoadingScreen");
		}
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoSkip : MonoBehaviour
{
	[SerializeField]
	private VideoPlayer _player;
	
	private bool _playing;
	
	void Update()
	{
		if (!_playing && _player.isPlaying)
			_playing = true;


		if (_playing && !_player.isPlaying)
		{
			_playing = false;
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
		}
	}
}

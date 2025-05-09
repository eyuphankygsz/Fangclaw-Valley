using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
	public void GoTo(string sceneName)
	{
		PlayerPrefs.SetString("SceneToLoad", sceneName);
		SceneManager.LoadScene("LoadingScreen");
	}
}

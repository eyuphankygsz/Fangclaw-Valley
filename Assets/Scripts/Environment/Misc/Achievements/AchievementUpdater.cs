using UnityEngine;

public class AchievementUpdater : MonoBehaviour
{
	public string PPKey;

	public void SetInt(int value)
	{
		PlayerPrefs.SetInt(PPKey, value);
	}
	public void SetFloat(float value)
	{
		PlayerPrefs.SetFloat(PPKey, value);
	}
	public void SetString(string value)
	{
		PlayerPrefs.SetString(PPKey, value);
	}
	public void IncreaseInt(int value)
	{
		PlayerPrefs.SetInt(PPKey, PlayerPrefs.GetInt(PPKey, 0) + value);
	}
	public void IncreaseFloat(float value)
	{
		PlayerPrefs.SetFloat(PPKey, PlayerPrefs.GetFloat(PPKey, 0) + value);
	}




}